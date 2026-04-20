using Application.Interfaces.Services.Users;
using Domain.Entities;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Web.Features.EquipeConversations.Requests;
using Web.Hubs;

namespace Web.Features.EquipeConversations;

public class SendEquipeMessageEndpoint : Endpoint<SendEquipeMessageRequest, object>
{
    private readonly IEquipeConversationRepository _repository;
    private readonly IAuthenticatedUserService _authenticatedUserService;
    private readonly IHubContext<ChatHub> _hubContext;

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp", "application/pdf"
    };

    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB

    public SendEquipeMessageEndpoint(
        IEquipeConversationRepository repository,
        IAuthenticatedUserService authenticatedUserService,
        IHubContext<ChatHub> hubContext)
    {
        _repository = repository;
        _authenticatedUserService = authenticatedUserService;
        _hubContext = hubContext;
    }

    public override void Configure()
    {
        AllowFileUploads();
        Post("equipe-conversations/{ConversationId}/messages");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(SendEquipeMessageRequest req, CancellationToken ct)
    {
        var user = _authenticatedUserService.GetAuthenticatedUser();
        if (user == null)
        {
            HttpContext.Response.StatusCode = 401;
            return;
        }

        var userId = user.Id;
        var isAdmin = User.IsInRole(Domain.Constants.User.Roles.ADMINISTRATOR);

        var conversation = await _repository.GetByIdAsync(req.ConversationId);
        if (conversation == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var authorized = await _repository.IsUserInEquipeAsync(userId, conversation.EquipeId, isAdmin);
        if (!authorized)
        {
            HttpContext.Response.StatusCode = 403;
            return;
        }

        var hasText = !string.IsNullOrWhiteSpace(req.Text);
        var hasAttachment = req.Attachment is not null;

        if (!hasText && !hasAttachment)
        {
            HttpContext.Response.StatusCode = 400;
            return;
        }

        string? attachmentUrl = null;
        string? attachmentFileName = null;
        string? attachmentContentType = null;

        if (hasAttachment)
        {
            if (!AllowedContentTypes.Contains(req.Attachment!.ContentType))
            {
                HttpContext.Response.StatusCode = 400;
                return;
            }

            if (req.Attachment.Length > MaxFileSize)
            {
                HttpContext.Response.StatusCode = 400;
                return;
            }

            attachmentUrl = await SaveFile(req.Attachment);
            attachmentFileName = req.Attachment.FileName;
            attachmentContentType = req.Attachment.ContentType;
        }

        var message = new EquipeMessage
        {
            Texte = hasText ? req.Text!.Trim() : null,
            ExpediteurId = userId,
            Date = DateTime.UtcNow,
            EquipeConversationId = req.ConversationId,
            AttachmentUrl = attachmentUrl,
            AttachmentFileName = attachmentFileName,
            AttachmentContentType = attachmentContentType
        };

        var saved = await _repository.AddMessageAsync(message);
        await _repository.UpdateLastMessageAtAsync(req.ConversationId, saved.Date);

        var senderNames = await _repository.GetSenderNamesByUserIdsAsync(new[] { userId });
        var senderName = senderNames.TryGetValue(userId, out var n) ? n : (user.UserName ?? "User");

        var payload = new
        {
            saved.Id,
            Text = saved.Texte,
            SenderId = saved.ExpediteurId,
            SenderName = senderName,
            saved.Date,
            EquipeConversationId = saved.EquipeConversationId,
            EquipeId = conversation.EquipeId,
            saved.AttachmentUrl,
            saved.AttachmentFileName,
            saved.AttachmentContentType
        };

        // Broadcast to all users in the equipe group (including admin). Sender dedupes on the client
        // based on SenderId, since the sender also received the payload via the HTTP response.
        await _hubContext.Clients
            .Group($"equipe_{conversation.EquipeId}")
            .SendAsync("ReceiveEquipeMessage", payload, ct);

        await Send.OkAsync(payload, cancellation: ct);
    }

    private static async Task<string> SaveFile(IFormFile file, string folder = "uploads/chat")
    {
        var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folder);

        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        var fileExtension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{fileExtension}";
        var fullPath = Path.Combine(savePath, fileName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/{folder}/{fileName}";
    }
}

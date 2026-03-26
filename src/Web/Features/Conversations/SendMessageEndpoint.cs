using Application.Interfaces.Services.Users;
using Domain.Entities;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Web.Features.Conversations.Requests;
using Web.Hubs;

namespace Web.Features.Conversations;

public class SendMessageEndpoint : Endpoint<SendMessageRequest, object>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IAuthenticatedUserService _authenticatedUserService;
    private readonly IHubContext<ChatHub> _hubContext;

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp", "application/pdf"
    };

    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB

    public SendMessageEndpoint(
        IConversationRepository conversationRepository,
        IAuthenticatedUserService authenticatedUserService,
        IHubContext<ChatHub> hubContext)
    {
        _conversationRepository = conversationRepository;
        _authenticatedUserService = authenticatedUserService;
        _hubContext = hubContext;
    }

    public override void Configure()
    {
        AllowFileUploads();
        Post("conversations/{ConversationId}/messages");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(SendMessageRequest req, CancellationToken ct)
    {
        var user = _authenticatedUserService.GetAuthenticatedUser();
        if (user == null)
        {
            HttpContext.Response.StatusCode = 401;
            return;
        }

        var userId = user.Id;

        var conversation = await _conversationRepository.GetByIdAsync(req.ConversationId);
        if (conversation == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        if (conversation.AdminId != userId && conversation.MemberId != userId)
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

        var receiverId = conversation.AdminId == userId
            ? conversation.MemberId
            : conversation.AdminId;

        var message = new Message
        {
            Texte = hasText ? req.Text!.Trim() : null,
            ExpediteurId = userId,
            ReceveurId = receiverId,
            Date = DateTime.UtcNow,
            ConversationId = req.ConversationId,
            AttachmentUrl = attachmentUrl,
            AttachmentFileName = attachmentFileName,
            AttachmentContentType = attachmentContentType
        };

        var saved = await _conversationRepository.AddMessageAsync(message);
        await _conversationRepository.UpdateLastMessageAtAsync(req.ConversationId, saved.Date);

        var payload = new
        {
            saved.Id,
            Text = saved.Texte,
            SenderId = saved.ExpediteurId,
            saved.Date,
            saved.ConversationId,
            saved.AttachmentUrl,
            saved.AttachmentFileName,
            saved.AttachmentContentType,
            Type = (int)saved.Type,
            AppointmentId = (Guid?)null,
            AppointmentDate = (DateTime?)null,
            AppointmentStatus = (int?)null,
            AppointmentMotif = (string?)null
        };

        await _hubContext.Clients.Group($"user_{receiverId}").SendAsync("ReceiveMessage", payload, ct);

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

using Application.Interfaces.Services.Members;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.Interfaces.Services.Module.Dto;

namespace Web.Features.Members.Modules.GetMyModuleDetail;

public class GetMyModuleDetailEndpoint : EndpointWithoutRequest<ModuleDto>
{
    private readonly IAuthenticatedMemberService _authenticatedMemberService;
    private readonly IMemberModuleRepository _memberModuleRepository;
    private readonly GarneauTemplateDbContext _context;

    public GetMyModuleDetailEndpoint(
        IAuthenticatedMemberService authenticatedMemberService,
        IMemberModuleRepository memberModuleRepository,
        GarneauTemplateDbContext context)
    {
        _authenticatedMemberService = authenticatedMemberService;
        _memberModuleRepository = memberModuleRepository;
        _context = context;
    }

    public override void Configure()
    {
        Get("members/me/modules/{moduleId}");
        Roles(Domain.Constants.User.Roles.MEMBER);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var moduleIdString = Route<string>("moduleId");
        if (!Guid.TryParse(moduleIdString, out var moduleId))
        {
            HttpContext.Response.StatusCode = 400;
            return;
        }

        var member = _authenticatedMemberService.GetAuthenticatedMember();
        var isAssigned = await _memberModuleRepository.IsAssignedAsync(member.Id, moduleId);
        if (!isAssigned)
        {
            HttpContext.Response.StatusCode = 403;
            return;
        }

        var module = await _context.Modules
            .Include(m => m.Sections.Where(s => s.Deleted == null).OrderBy(s => s.SortOrder))
            .FirstOrDefaultAsync(m => m.Id == moduleId, ct);

        if (module is null)
        {
            HttpContext.Response.StatusCode = 404;
            return;
        }

        Response = new ModuleDto
        {
            Id = module.Id.ToString(),
            Name = module.Name,
            Subject = module.Subject,
            Content = module.Content,
            CardImageUrl = module.CardImageUrl,
            Sections = module.Sections.Select(s => new ModuleSectionDto
            {
                Id = s.Id.ToString(),
                Title = s.Title,
                Content = s.Content,
                SortOrder = s.SortOrder
            }).ToList()
        };
    }
}

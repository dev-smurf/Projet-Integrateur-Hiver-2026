using Application.Exceptions.Users;
using Domain.Common;
using Domain.Entities;
using Domain.Entities.Identity;
using Domain.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Features.Common;
using IMapper = AutoMapper.IMapper;

namespace Web.Features.Admins.Members.UpdateMember;

public class UpdateMemberEndpoint : EndpointWithSanitizedRequest<UpdateMemberRequest, SucceededOrNotResponse>
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IEquipeRepository _equipeRepository;

    public UpdateMemberEndpoint(
        IMapper mapper,
        IUserRepository userRepository,
        IMemberRepository memberRepository,
        IEquipeRepository equipeRepository)
    {
        _mapper = mapper;
        _userRepository = userRepository;
        _memberRepository = memberRepository;
        _equipeRepository = equipeRepository;
    }

    public override void Configure()
    {
        DontCatchExceptions();

        Put("members/{id}");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(UpdateMemberRequest req, CancellationToken ct)
    {
        var existingMember = _memberRepository.FindById(req.Id);
        var existingUser = _userRepository.FindByEmail(req.Email, true);
        if (existingMember.Email == req.Email || existingUser == null)
        {
            await UpdateMember(req, existingMember, existingMember.User);
            await Send.OkAsync(new SucceededOrNotResponse(true), cancellation: ct);
            return;
        }

        if (existingUser.IsActive())
            throw new UserWithEmailAlreadyExistsException("A user with this email already exists.");

        var previousUserId = existingMember.User.Id;
        await UpdateMember(req, existingMember, existingUser);
        await _userRepository.DeleteUserWithId(previousUserId);

        await Send.OkAsync(new SucceededOrNotResponse(true), cancellation: ct);
    }

    private async Task UpdateMember(UpdateMemberRequest req, Member existingMember, User user)
    {
        existingMember = _mapper.Map(req, existingMember);
        existingMember.SetUser(_mapper.Map(req, user));

        await _memberRepository.Update(existingMember);
        await _equipeRepository.ReplaceUserEquipes(existingMember.User, req.EquipeIds);
    }
}

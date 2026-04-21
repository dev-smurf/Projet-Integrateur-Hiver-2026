using Application.Interfaces.Services.Members;
using System;
using Application.Interfaces.Services.Users;
using Application.Services.Members.Dtos;
using Application.Services.Users.Dtos;
using AutoMapper;
using Application.Extensions;
using Domain.Entities;
using Domain.Entities.Identity;
using Domain.Repositories;
using Application.Interfaces.Services.Notifications;
using Application.Settings;
using Microsoft.Extensions.Options;

namespace Application.Services.Members;

public class MemberRegistrationService : IMemberRegistrationService
{
    private readonly IMapper _mapper;
    private readonly IMemberRepository _memberRepository;
    private readonly IEquipeRepository _equipeRepository;
    private readonly IUserCreationService _userCreationService;
    private readonly IUserRepository _userRepository;
    private readonly INotificationService _notificationService;
    private readonly string _baseUrl;

    public MemberRegistrationService(
        IMapper mapper,
        IMemberRepository memberRepository,
        IEquipeRepository equipeRepository,
        IUserCreationService userCreationService,
        IUserRepository userRepository,
        INotificationService notificationService,
        IOptions<ApplicationSettings> applicationSettings)
    {
        _mapper = mapper;
        _memberRepository = memberRepository;
        _equipeRepository = equipeRepository;
        _userCreationService = userCreationService;
        _userRepository = userRepository;
        _notificationService = notificationService;
        _baseUrl = applicationSettings.Value.BaseUrl;
    }

    public async Task<Member> RegisterMember(MemberRegistrationDto memberRegistrationDto)
    {
        var userCreationDto = _mapper.Map<UserCreationDto>(memberRegistrationDto);
        var user = await _userCreationService.CreateUserOrUpdateDeletedUserWithSameEmail(userCreationDto);

        var member = _mapper.Map<Member>(memberRegistrationDto);
        member.SetUser(user);

        await _memberRepository.Create(member);
        await AssignMemberToEquipes(user, memberRegistrationDto.EquipeIds);

        var baseUrlNormalized = (_baseUrl ?? string.Empty).TrimEnd('/');
        var rawToken = await _userRepository.GetResetPasswordTokenForUser(user);
        var encodedToken = rawToken.Base64UrlEncode();
        var link = $"{baseUrlNormalized}/reset-password?userId={user.Id}&token={encodedToken}";
        try
        {
            var destination = user.Email ?? memberRegistrationDto.Email;
            if (!string.IsNullOrWhiteSpace(destination))
            {
                if (string.IsNullOrWhiteSpace(user.Email))
                    user.Email = destination;

                await _notificationService.SendAccountCreatedNotification(memberRegistrationDto.FirstName, user, link);
            }
        }
        catch
        {
        }

        return member;
    }

    private async Task AssignMemberToEquipes(User user, IEnumerable<Guid>? equipeIds)
    {
        var ids = equipeIds?
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList() ?? [];

        if (ids.Count == 0)
        {
            return;
        }

        await _equipeRepository.AssignUserToEquipes(user, ids);
    }
}

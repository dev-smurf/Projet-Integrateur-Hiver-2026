using Application.Interfaces.Services.Members;
using Application.Interfaces.Services.Users;
using Application.Services.Members.Dtos;
using Application.Services.Users.Dtos;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using Application.Interfaces.Services.Notifications;
using Application.Settings;
using Microsoft.Extensions.Options;

namespace Application.Services.Members;

public class MemberRegistrationService : IMemberRegistrationService
{
    private readonly IMapper _mapper;
    private readonly IMemberRepository _memberRepository;
    private readonly IUserCreationService _userCreationService;
    private readonly INotificationService _notificationService;
    private readonly string _baseUrl;

    public MemberRegistrationService(
        IMapper mapper,
        IMemberRepository memberRepository,
        IUserCreationService userCreationService,
        INotificationService notificationService,
        IOptions<ApplicationSettings> applicationSettings)
    {
        _mapper = mapper;
        _memberRepository = memberRepository;
        _userCreationService = userCreationService;
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

        var link = $"{_baseUrl}/login";
        try
        {
            var destination = user.Email ?? memberRegistrationDto.Email;
            if (!string.IsNullOrWhiteSpace(destination))
            {
                if (string.IsNullOrWhiteSpace(user.Email))
                    user.Email = destination;

                await _notificationService.SendAccountCreatedNotification(user, link);
            }
        }
        catch
        {

        }

        return member;
    }
}
namespace Application.Interfaces.Services.Equipe.Dto;

public class MemberEquipeDto
{
    public string Id { get; set; } = null!;
    public string MemberId { get; set; } = null!;
    public string EquipeId { get; set; } = null!;
    public string Firstname { get; set; } = null!;
    public string Lastname { get; set; } = null!;
    public string Email { get; set; } = null!;
}

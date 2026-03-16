using Domain.Extensions;
using Web.Features.Common;

public class CreateEquipeRequest : ISanitizable
{
    public string NameFr { get; set; } = null!;
    public string NameEn { get; set; } = null!;

    public void Sanitize()
    {
        NameFr = NameFr.Trim().CapitalizeFirstLetterOfEachWord()!;
        NameEn = NameEn.Trim().CapitalizeFirstLetterOfEachWord()!;
    }

}


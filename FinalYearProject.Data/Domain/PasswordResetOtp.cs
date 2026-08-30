using FinalYearProject.Data.Domain.Entities;
using FinalYearProject.Data.Domain.Entities.Shared;

namespace FinalYearProject.Data.Domain;

public class PasswordResetOtp : BaseEntity
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; }

    public string OtpHash { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }

    public bool IsUsed { get; set; }
}
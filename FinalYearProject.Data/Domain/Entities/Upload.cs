using FinalYearProject.Data.Domain.Entities.Shared;


namespace FinalYearProject.Data.Domain.Entities;

public class Upload: BaseEntity
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    public string Filename { get; set; }

    // Serialized CP-ABE ciphertext
    public string AbeCipherText { get; set; } = default!;

    // Base64 AES ciphertext
    public string SymmetricCipherText { get; set; } = default!;
    public Guid PolicyId { get; set; }

    public virtual Policy Policy { get; set; }
}

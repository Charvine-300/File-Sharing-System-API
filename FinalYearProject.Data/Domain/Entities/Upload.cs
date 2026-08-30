using FinalYearProject.Data.Domain.Entities.Shared;


namespace FinalYearProject.Data.Domain.Entities;

public class Upload: BaseEntity
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    public string Filename { get; set; }
    public string ContentType { get; set; }
    public string CloudinaryPublicId { get; set; }
    public string CloudinaryUrl { get; set; }
    public string FileNonce { get; set; }

    public string FileTag { get; set; }

    // Serialized CP-ABE ciphertext
    public string AbeCipherText { get; set; } = default!;

    // Wrapped symmetric key encrypted with CP-ABE
    public string WrappedKey { get; set; } = default!;
    public Guid PolicyId { get; set; }

    public virtual Policy Policy { get; set; }
}

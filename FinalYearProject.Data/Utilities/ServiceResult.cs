namespace FinalYearProject.Data.Utilities;

public record class ServiceResult
{
    public required bool IsSuccessful { get; set; }
    public string? Message { get; set; }
    public required int StatusCode { get; set; }
}

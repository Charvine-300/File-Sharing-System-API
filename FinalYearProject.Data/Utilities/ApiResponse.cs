namespace FinalYearProject.Data.Utilities;


public record ApiRecordResponse<T>(bool IsSuccessful, int StatusCode, string? Message, T? Data);

public record ApiResponse<T>(bool IsSuccessful, int StatusCode, string? Message, T? Data) where T : class;

public record ApiResponse(bool IsSuccessful, int StatusCode, string? Message);

namespace FinalYearProject.Data.Utilities;

public record PaginationResponse<T>(int PageSize, int CurrentPage, int TotalPages, int TotalRecords, IEnumerable<T> Records);

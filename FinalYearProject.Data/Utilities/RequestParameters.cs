namespace FinalYearProject.Data.Utilities;


/// <summary>
/// Pagination and filtering query parameters object
/// </summary>
public class RequestParameters
{
    /// <summary>
    /// The maximum page size to return.
    /// </summary>
    const int maxPageSize = 100;


    /// <summary>
    /// Default keyword if non was passed from the request
    /// </summary>
    private string _keyword = string.Empty;

    /// <summary>
    /// Search by keyword or filtering result by search query string.
    /// </summary>
    public string Search
    {
        get
        {
            return _keyword;
        }
        set
        {
            _keyword = string.IsNullOrWhiteSpace(value) ? _keyword : value;
        }
    }

    /// <summary>
    /// Default page size if non was passed from the request
    /// </summary>
    private int _pageSize = 10;

    /// <summary>
    /// Default page number if non was passed from the request
    /// </summary>
    private int _pageNumber = 1;

    /// <summary>
    /// Get and Set the pageSize to return. Return MaxPageSize <see cref="maxPageSize"/> if the page size from the request is greater than 10 but defaults to ten if it is less than or equal to ten
    /// </summary>
    public int PageSize
    {
        get
        {
            if (_pageSize <= 0)
            {
                _pageSize = 10;
            }
            else if (_pageSize > maxPageSize)
            {
                _pageSize = maxPageSize;
            }
            return _pageSize;
        }
        set
        {
            _pageSize = value > maxPageSize ? maxPageSize : value;
        }
    }


    /// <summary>
    /// Page number 1-2, 2-3
    /// </summary>
    public int PageNumber
    {
        get
        {
            if (_pageNumber <= 0)
            {
                _pageNumber = 1;
            }
            return _pageNumber;
        }
        set
        {
            _pageNumber = value <= 0 ? 1 : value;
        }
    }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

}

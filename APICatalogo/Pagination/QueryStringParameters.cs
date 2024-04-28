namespace APICatalogo.Pagination;

public abstract class QueryStringParameters
{
    const int maxPageSize = 50;

    private int _pageSize = maxPageSize;

    public int PageNumber { get; set; } = 1;
    public int PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
    }
}

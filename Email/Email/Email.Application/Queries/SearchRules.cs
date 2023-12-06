namespace Email.Application.Queries;

/// <summary>
/// The values used to constrain search parameters.
/// </summary>
public static class SearchRules
{
    /// <summary>
    /// The minimum valid search time in unix epoch seconds format.
    /// </summary>
    public const long MinimumTimeUnixSeconds = 1672531200; // 2023-01-01 00:00:00

    /// <summary>
    /// The maximum valid search time in unix epoch seconds format.
    /// </summary>
    public const long MaximumTimeUnixSeconds = 4102444799; // 2099-12-31 23:59:59

    /// <summary>
    /// The maximum valid page size.
    /// </summary>
    public const int MaximumPageSize = 500;
}

namespace DapperSample.Infrastructure;

public interface IRequestFilter
{
    /// <summary>
    /// Sorting for the query
    /// </summary>
    string? Sorting { get; set; }
    /// <summary>
    /// The generic text to search if any of the properties contains a value
    /// </summary>
    string? FilterText { get; set; }
    /// <summary>
    /// Used for pagination to limit the maximum query count
    /// </summary>
    int MaxResultCount { get; set; }
    /// <summary>
    /// Used for pagination to skip specific query sequence
    /// </summary>
    int SkipCount { get; set; }
}
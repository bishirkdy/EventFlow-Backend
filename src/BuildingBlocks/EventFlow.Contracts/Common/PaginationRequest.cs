namespace EventFlow.Contracts.Common;

public sealed record PaginationRequest
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;

    public int NormalizedPageNumber => Math.Max(1, PageNumber);
    public int NormalizedPageSize => Math.Clamp(PageSize, 1, 100);
}

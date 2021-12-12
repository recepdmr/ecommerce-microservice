namespace CatalogService.Api.Application.ViewModels
{
    public class PaginationItemsViewModel<TEntity> where TEntity : class
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public long Count { get; set; }
        public IEnumerable<TEntity> Data { get; set; }

        public PaginationItemsViewModel(int pageSize, int pageIndex, long count, IEnumerable<TEntity> data)
        {
            PageSize = pageSize;
            PageIndex = pageIndex;
            Count = count;
            Data = data;
        }
    }
}
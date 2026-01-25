using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.DTOs
{
    public class PaginatedResultDto<T> where T : class
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public List<T> Itmes { get; set; }

        public PaginatedResultDto(int PageNumber, int PageSize, int TotalCount, List<T> Itmes)
        {
            this.PageNumber = PageNumber;
            this.PageSize = PageSize;
            this.TotalCount = TotalCount;
            this.TotalPages = TotalPages;
            this.Itmes = Itmes;
            this.TotalPages = (int)Math.Ceiling(TotalCount / (Double)PageSize);
        }
    }
}
using ERP_version1.Models.HR;

namespace ERP_version1.ViewModels
{
    public class EmployeeIndexViewModel
    {
        public List<Employee> Employees { get; set; } = new List<Employee>();

        public string? SearchTerm { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int PageSize { get; set; }

        public int TotalEmployees { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;

        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
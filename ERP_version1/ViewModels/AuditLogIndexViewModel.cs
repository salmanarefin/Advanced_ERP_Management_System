using ERP_version1.Models.Common;

namespace ERP_version1.ViewModels
{
    public class AuditLogIndexViewModel
    {
        public List<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

        public string? SearchTerm { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int PageSize { get; set; }

        public int TotalAuditLogs { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;

        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
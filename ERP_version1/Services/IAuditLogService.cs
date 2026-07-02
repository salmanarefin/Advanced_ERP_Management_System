using ERP_version1.ViewModels;

namespace ERP_version1.Services
{
    public interface IAuditLogService
    {
        Task LogAsync(
            string action,
            string moduleName,
            string tableName,
            int? recordId,
            string description);

        Task<AuditLogIndexViewModel> GetAuditLogsAsync(string? searchTerm, int page);
    }
}
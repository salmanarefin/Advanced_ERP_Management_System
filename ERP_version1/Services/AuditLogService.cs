using ERP_version1.Data;
using ERP_version1.Models.Common;
using ERP_version1.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ERP_version1.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditLogService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(
            string action,
            string moduleName,
            string tableName,
            int? recordId,
            string description)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            string? userId = httpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            string? userEmail = httpContext?.User?.Identity?.Name;
            string? ipAddress = httpContext?.Connection?.RemoteIpAddress?.ToString();

            AuditLog auditLog = new AuditLog
            {
                UserId = userId,
                UserEmail = userEmail,
                Action = action,
                ModuleName = moduleName,
                TableName = tableName,
                RecordId = recordId,
                Description = description,
                IpAddress = ipAddress,
                CreatedAt = DateTime.Now
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        public async Task<AuditLogIndexViewModel> GetAuditLogsAsync(string? searchTerm, int page)
        {
            int pageSize = 10;

            var query = _context.AuditLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(log =>
                    (log.UserEmail != null && log.UserEmail.Contains(searchTerm)) ||
                    log.Action.Contains(searchTerm) ||
                    (log.ModuleName != null && log.ModuleName.Contains(searchTerm)) ||
                    (log.TableName != null && log.TableName.Contains(searchTerm)) ||
                    (log.Description != null && log.Description.Contains(searchTerm)) ||
                    (log.IpAddress != null && log.IpAddress.Contains(searchTerm))
                );
            }

            int totalAuditLogs = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalAuditLogs / (double)pageSize);

            if (page < 1)
            {
                page = 1;
            }

            if (totalPages > 0 && page > totalPages)
            {
                page = totalPages;
            }

            var auditLogs = await query
                .OrderByDescending(log => log.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new AuditLogIndexViewModel
            {
                AuditLogs = auditLogs,
                SearchTerm = searchTerm,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalAuditLogs = totalAuditLogs
            };
        }
    }
}
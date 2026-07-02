using ERP_version1.Models.PayrollModule;

namespace ERP_version1.Services
{
    public interface IPayrollService
    {
        Task<List<Payroll>> GetAllPayrollsAsync();

        Task<Payroll?> GetPayrollDetailsAsync(int id);

        Task<string?> GeneratePayrollAsync(Payroll payroll);

        Task<Payroll?> GetPayrollForDeleteAsync(int id);

        Task<bool> DeletePayrollAsync(int id);
    }
}
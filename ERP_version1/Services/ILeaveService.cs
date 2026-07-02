using ERP_version1.Models.LeaveModule;

namespace ERP_version1.Services
{
    public interface ILeaveService
    {
        Task<List<LeaveApplication>> GetAllLeaveApplicationsAsync();

        Task<LeaveApplication?> GetLeaveApplicationDetailsAsync(int id);

        Task<LeaveApplication?> GetLeaveApplicationForActionAsync(int id);

        Task<string?> ApplyLeaveAsync(LeaveApplication leaveApplication);

        Task<string?> ApproveLeaveAsync(int id);

        Task<string?> RejectLeaveAsync(int id, string rejectionReason);

        Task<LeaveApplication?> GetLeaveApplicationForDeleteAsync(int id);

        Task<bool> DeleteLeaveApplicationAsync(int id);
    }
}
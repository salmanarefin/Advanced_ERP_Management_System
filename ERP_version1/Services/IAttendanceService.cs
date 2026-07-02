using ERP_version1.Models.AttendanceModule;

namespace ERP_version1.Services
{
    public interface IAttendanceService
    {
        Task<List<Attendance>> GetAllAttendancesAsync();

        Task<Attendance?> GetAttendanceDetailsAsync(int id);

        Task<Attendance?> GetAttendanceForDeleteAsync(int id);

        Task<string?> CheckInAsync(int employeeId);

        Task<string?> CheckOutAsync(int attendanceId);

        Task<bool> DeleteAttendanceAsync(int id);
    }
}
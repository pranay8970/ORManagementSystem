using ORManagement.Application.DTOs.Dashboard;

namespace ORManagement.Application.Interfaces.Repositories;

public interface IDashboardRepository
{
    Task<SurgeonDashboardDto> GetSurgeonDashboardAsync(int hospitalId, int surgeonId);

    Task<SchedulerDashboardDto> GetSchedulerDashboardAsync(int hospitalId);

    Task<List<DashboardTodayScheduleDto>> GetTodayScheduleAsync(int hospitalId, DateTime date);
}
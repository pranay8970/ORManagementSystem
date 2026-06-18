using ORManagement.Application.DTOs.Dashboard;
using ORManagement.Application.DTOs.Shared;

namespace ORManagement.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<ServiceResultDto<SurgeonDashboardDto>> GetSurgeonDashboardAsync(
        int hospitalId,
        int surgeonId);

    Task<ServiceResultDto<SchedulerDashboardDto>> GetSchedulerDashboardAsync(int hospitalId);

    Task<ServiceResultDto<List<DashboardTodayScheduleDto>>> GetTodayScheduleAsync(
        int hospitalId,
        DateTime? date);
}
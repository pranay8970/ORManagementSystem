using Microsoft.Extensions.Logging;
using ORManagement.Application.DTOs.Dashboard;
using ORManagement.Application.DTOs.Shared;
using ORManagement.Application.Interfaces.Repositories;
using ORManagement.Application.Interfaces.Services;

namespace ORManagement.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(
        IDashboardRepository dashboardRepository,
        ILogger<DashboardService> logger)
    {
        _dashboardRepository = dashboardRepository;
        _logger = logger;
    }

    public async Task<ServiceResultDto<SurgeonDashboardDto>> GetSurgeonDashboardAsync(
        int hospitalId,
        int surgeonId)
    {
        var dashboard = await _dashboardRepository.GetSurgeonDashboardAsync(
            hospitalId,
            surgeonId);

        return ServiceResultDto<SurgeonDashboardDto>.Ok(dashboard);
    }

    public async Task<ServiceResultDto<SchedulerDashboardDto>> GetSchedulerDashboardAsync(int hospitalId)
    {
        var dashboard = await _dashboardRepository.GetSchedulerDashboardAsync(hospitalId);

        return ServiceResultDto<SchedulerDashboardDto>.Ok(dashboard);
    }

    public async Task<ServiceResultDto<List<DashboardTodayScheduleDto>>> GetTodayScheduleAsync(
        int hospitalId,
        DateTime? date)
    {
        var selectedDate = date?.Date ?? DateTime.UtcNow.Date;

        var schedule = await _dashboardRepository.GetTodayScheduleAsync(
            hospitalId,
            selectedDate);

        return ServiceResultDto<List<DashboardTodayScheduleDto>>.Ok(schedule);
    }
}
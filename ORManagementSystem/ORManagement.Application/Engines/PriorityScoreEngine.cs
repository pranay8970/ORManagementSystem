namespace ORManagement.Application.Engines;

public class PriorityScoreEngine
{
    public decimal CalculateScore(
        string priority,
        string patientReadiness,
        DateTime createdAt,
        int cyclesWaited,
        int? durationFitScore = null)
    {
        var waitingDays = Math.Min((DateTime.UtcNow - createdAt).Days, 30);

        var priorityWeight = priority switch
        {
            "Emergency" => 3,
            "Urgent" => 2,
            "Elective" => 1,
            _ => 1
        };

        var readinessWeight = patientReadiness switch
        {
            "Ready" => 1.0m,
            "PendingClearance" => 0.5m,
            "NotReady" => 0m,
            _ => 0m
        };

        var fitScore = durationFitScore ?? 0;

        return priorityWeight * 50
               + waitingDays * 2
               + readinessWeight * 20
               + fitScore
               + cyclesWaited * 10;
    }
}
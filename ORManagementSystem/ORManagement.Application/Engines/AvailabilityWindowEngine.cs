namespace ORManagement.Application.Engines;

public class AvailabilityWindowEngine
{
    public bool IsValidMask(int availableDaysMask)
    {
        return availableDaysMask >= 1 && availableDaysMask <= 31;
    }

    public string ToDisplayText(int availableDaysMask)
    {
        if (availableDaysMask == 31)
        {
            return "Mon-Fri";
        }

        if (availableDaysMask == 3)
        {
            return "Mon-Tue";
        }

        if (availableDaysMask == 1)
        {
            return "Mon";
        }

        var days = new List<string>();

        if ((availableDaysMask & 1) == 1)
        {
            days.Add("Mon");
        }

        if ((availableDaysMask & 2) == 2)
        {
            days.Add("Tue");
        }

        if ((availableDaysMask & 4) == 4)
        {
            days.Add("Wed");
        }

        if ((availableDaysMask & 8) == 8)
        {
            days.Add("Thu");
        }

        if ((availableDaysMask & 16) == 16)
        {
            days.Add("Fri");
        }

        return string.Join(", ", days);
    }

    public bool IsDateAllowed(DateTime date, int availableDaysMask)
    {
        var dayMask = GetDayMask(date);

        return dayMask != 0 && (availableDaysMask & dayMask) > 0;
    }

    private static int GetDayMask(DateTime date)
    {
        return date.DayOfWeek switch
        {
            DayOfWeek.Monday => 1,
            DayOfWeek.Tuesday => 2,
            DayOfWeek.Wednesday => 4,
            DayOfWeek.Thursday => 8,
            DayOfWeek.Friday => 16,
            _ => 0
        };
    }
}
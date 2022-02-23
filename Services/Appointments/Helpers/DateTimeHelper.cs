namespace Services.Appointments.Helpers;

public class DateTimeHelper
{
    public static DateTime RoundTo30Minutes(DateTime date)
    {
        var minutes = date.Minute;

        if (minutes == 0) return date;

        if (minutes < 30) return date.AddMinutes(-minutes).AddMinutes(0).ToUniversalTime();

        return date.AddMinutes(-minutes).AddMinutes(30).ToUniversalTime();
    }
    public static bool ValidateDate(DateTime date)
    {
        return !date.Equals(default) && date.Minute % 30 == 0;
    }
}

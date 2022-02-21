namespace Services.Appointments.Helpers;

public class DateTimeHelper
{
    public static DateTime Round(DateTime date, TimeSpan interval) {
        return new DateTime(
            (long)Math.Round(date.Ticks / (double)interval.Ticks) * interval.Ticks
        ).ToUniversalTime();
    } // https://docs.microsoft.com/en-us/answers/questions/590682/how-can-i-format-and-round-the-datetimenow-to-the.html

    public static DateTime RoundTo30Minutes(DateTime date)
    {
        return Round(date, TimeSpan.FromMinutes(30));
    }

    public static bool ValidateDate(DateTime date)
    {
        return !date.Equals(default) && date.Minute % 30 == 0;
    }
}

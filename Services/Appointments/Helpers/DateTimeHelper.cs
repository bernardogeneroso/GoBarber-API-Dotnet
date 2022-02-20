namespace Services.Appointments.Helpers;

public class DateTimeHelper
{
    public static DateTime Round(DateTime date, TimeSpan interval) {
        return new DateTime(
            (long)Math.Round(date.Ticks / (double)interval.Ticks) * interval.Ticks
        ).ToUniversalTime();
    } // https://docs.microsoft.com/en-us/answers/questions/590682/how-can-i-format-and-round-the-datetimenow-to-the.html
}

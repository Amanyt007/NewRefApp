namespace NewRefApp.Data
{
    public static class Helper
    {
        public static DateTime GetIndianTime()
        {
            DateTime utcNow = Helper.GetIndianTime();

            // Use "India Standard Time" for Windows
            TimeZoneInfo indianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

            DateTime istTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, indianZone);

            return istTime;
        }

    }
}

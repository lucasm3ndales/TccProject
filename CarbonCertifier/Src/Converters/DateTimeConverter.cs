namespace CarbonCertifier.Converters;

public static class DateTimeConverter
{
    public static DateTime ConvertStringToDateTime(string value)
    { 
        return DateTime.Parse(value).ToUniversalTime();
    }
}
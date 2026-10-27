using System.Globalization;

namespace CvAnalyzer.Api.Services;

public static class DateParsingHelper
{
    private static readonly string[] CustomFormats = new[]
    {
        "yyyy-MM-dd",
        "yyyy/MM/dd",
        "dd/MM/yyyy",
        "dd-MM-yyyy",
        "MM/yyyy",
        "M/yyyy",
        "yyyy/MM",
        "yyyy-MM",
        "MMM yyyy",
        "MMMM yyyy",
        "MMM-yyyy",
        "yyyy"
    };

    public static DateOnly? TryParseFlexibleDate(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        var cleaned = input.Trim();

        var presentWords = new[] { "present", "présent", "current", "actuel", "en cours", "now", "ongoing" };
        if (presentWords.Any(w => cleaned.Contains(w, StringComparison.OrdinalIgnoreCase)))
            return null;

        // Try strict custom formats first
        foreach (var format in CustomFormats)
        {
            if (DateOnly.TryParseExact(cleaned, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var exactDate))
            {
                return exactDate;
            }
        }

        // Try general date parsing as a fallback
        if (DateOnly.TryParse(cleaned, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fullDate))
            return fullDate;

        // Try "just a year" range
        if (int.TryParse(cleaned, out var year) && year > 1950 && year < 2100)
            return new DateOnly(year, 1, 1);

        return null;
    }
}
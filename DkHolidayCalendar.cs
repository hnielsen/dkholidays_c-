using System;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

internal class DkHolidayCalendar
{
	#region Constants

    private static Dictionary<string, int> _holidaysRelativeToEaster 
        = new Dictionary<string, int>
            {
                {"Fastelavn", -49},
                {"Palmesøndag", -7},
                {"Skærtorsdag", -3},
                {"Langfredag", -2},
                {"Påskedag", 0},
                {"2. påskedag", 1},
                {"Store Bededag", 26},
                {"Kr. Himmelfart", 39},
                {"Pinsedag", 49},
                {"2. pinsedag", 50}
            };

    private static Dictionary<string, System.DateTime> _fixedHolidays 
        = new Dictionary<string, System.DateTime>
            {
                {"Juleaften", new System.DateTime(1, 12, 24)},
                {"1. juledag", new System.DateTime(1, 12, 25)},
                {"2. juledag", new System.DateTime(1, 12, 26)},
				{"Nytårsaften", new System.DateTime(1, 12, 31)},
                {"Nytårsdag", new System.DateTime(1, 1, 1)},
                {"Grundlovsdag", new System.DateTime(1, 6, 5)},
            };

    #endregion
		
    /// <summary>
    /// Gets the date that Easter Sunday occurs on in a given year
    /// </summary>
    /// <remarks>
    /// See http://www.dayweekyear.com/faq_holidays_calculation.jsp
    /// See http://www.wordiq.com/definition/Computus#Meeus.2FJones.2FButcher_Gregorian_Algorithm
    /// </remarks>
    /// <param name="year"></param>
    /// <returns></returns>
    private static System.DateTime GetEaster(int year)
    {
        int Y = year;
        int a = Y % 19;
        int b = Y / 100;
        int c = Y % 100;
        int d = b / 4;
        int e = b % 4;
        int f = (b + 8) / 25;
        int g = (b - f + 1) / 3;
        int h = (19 * a + b - d - g + 15) % 30;
        int i = c / 4;
        int k = c % 4;
        int L = (32 + 2 * e + 2 * i - h - k) % 7;
        int m = (a + 11 * h + 22 * L) / 451;
        int month = (h + L - 7 * m + 114) / 31;
        int day = ((h + L - 7 * m + 114) % 31) + 1;
        return new System.DateTime(year, month, day);
    }

    public static IDictionary<string, System.DateTime> GetDanishHolidays(int year)
    {
        var easterDependentHolidays = GetEasterDependentHolidays(year);
        var fixedHolidays = GetFixedHolidays(year);

        var allHolidays = easterDependentHolidays
			.Union(fixedHolidays)
			.OrderBy(h => h.Value)
            .ToDictionary(pair => pair.Key, pair => pair.Value);
        
        return allHolidays;
    }

    public static IDictionary<string, System.DateTime> GetDanishHolidays()
    {
        return GetDanishHolidays(System.DateTime.Now.Year);
    }

    public static string GetDanishHolidaysString(int year)
    {
        var easterDependentHolidays = GetEasterDependentHolidays(year);
        var fixedHolidays = GetFixedHolidays(year);

        var dadk = CultureInfo.CreateSpecificCulture("da-DK").DateTimeFormat;
        var allHolidays = easterDependentHolidays
			.Union(fixedHolidays)
			.OrderBy(h => h.Value)
            .ToDictionary(pair => pair.Key, pair => pair.Value.ToString("d",dadk));
        var output = allHolidays.Select(d => string.Format("{0}\t{1}", d.Key, d.Value));
        return string.Join("\n", output);
    }

    public static string GetDanishHolidaysString()
    {
        return GetDanishHolidaysString(System.DateTime.Now.Year);
    }

    private static IEnumerable<KeyValuePair<string, System.DateTime>> GetEasterDependentHolidays(int year)
    {
        var easter = GetEaster(year);
        return _holidaysRelativeToEaster.ToDictionary(h => h.Key, h => easter.AddDays(h.Value));
    }

    private static IEnumerable<KeyValuePair<string, System.DateTime>> GetFixedHolidays(int year)
    {
        return _fixedHolidays.ToDictionary(f => f.Key, f => f.Value.AddYears(year - f.Value.Year));
    }
}
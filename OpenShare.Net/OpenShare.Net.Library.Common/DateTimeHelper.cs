﻿using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace OpenShare.Net.Library.Common
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// Takes a string in mm/dd/yyyy format to try and convert to a date.
        /// </summary>
        /// <param name="value">String to try and convert to local date.</param>
        /// <returns>A DateTime object initialized to the date passed in as a string in mm/dd/yyyy format.</returns>
        public static DateTime GetDateTimeFromShortDateString(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new Exception("Invalid DateTime value.");

            var dateMatch = Regex.Match(value, @"^([0-3]?[0-9])/([0-3]?[0-9])/([0-9]{2}?[0-9]{2})$");
            if (!dateMatch.Success)
                throw new Exception("Invalid DateTime value.");

            return new DateTime(
                Convert.ToInt32(dateMatch.Groups[3].Value),
                Convert.ToInt32(dateMatch.Groups[1].Value),
                Convert.ToInt32(dateMatch.Groups[2].Value));
        }

        /// <summary>
        /// Tries to convert the <paramref name="value"/> parameter into a four digit year.
        /// If this method fails to produce a viable value, it will return null.
        /// If year is less than zero or greater than the DateTime.MaxValue.Year property, it will also return null.
        /// <remarks>
        /// Currently the CultureInfo.CurrentCulture.Calendar.TwoDigitYearMax is 2029.
        /// This method is subject to change behavior based on operating system regional and language settings, and the
        /// .NET Framework implementation of CultureInfo.CurrentCulture.Calendar.ToFourDigitYear(year);
        /// e.g. GetFourDigitYear(0) : 2000,
        /// GetFourDigitYear(29) : 2029,
        /// GetFourDigitYear(30) : 1930,
        /// GetFourDigitYear(99) : 1999,
        /// GetFourDigitYear(999) : 999
        /// </remarks>
        /// </summary>
        /// <param name="value">The value to try and convert to a four digit year.</param>
        /// <returns>An integer value that is a four digit year value or null.</returns>
        public static int? GetFourDigitYear(int? value)
        {
            if (value == null)
                return null;

            var year = Convert.ToInt32(value);
            if (year < 0
                || year > DateTime.MaxValue.Year)
                return null;

            return year >= 100
                ? year
                : CultureInfo.CurrentCulture.Calendar.ToFourDigitYear(year);
        }

        /// <summary>
        /// Tries to convert the <paramref name="value"/> parameter into a four digit year.
        /// If this method fails to produce a viable value, it will return null.
        /// If year is less than zero or greater than the DateTime.MaxValue.Year property, it will also return null.
        /// <remarks>
        /// Currently the CultureInfo.CurrentCulture.Calendar.TwoDigitYearMax is 2029.
        /// This method is subject to change behavior based on operating system regional and language settings, and the
        /// .NET Framework implementation of CultureInfo.CurrentCulture.Calendar.ToFourDigitYear(year);
        /// e.g. GetFourDigitYear("0") : 2000,
        /// GetFourDigitYear("29") : 2029,
        /// GetFourDigitYear("30") : 1930,
        /// GetFourDigitYear("99") : 1999,
        /// GetFourDigitYear("999") : 999
        /// </remarks>
        /// </summary>
        /// <param name="value">The value to try and convert to a four digit year.</param>
        /// <returns>An integer value that is a four digit year value or null.</returns>
        public static int? GetFourDigitYear(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            int year;
            if (!int.TryParse(value.Trim(), out year)
                || year < 0
                || year > DateTime.MaxValue.Year)
                return null;
            
            return year >= 100
                ? year
                : CultureInfo.CurrentCulture.Calendar.ToFourDigitYear(year);
        }

        /// <summary>
        /// Tries to convert the <paramref name="value"/> parameter into a four digit year.
        /// If this method fails to produce a viable value, it will return null.
        /// If year is less than zero or greater than the DateTime.MaxValue.Year property, it will also return null.
        /// <remarks>
        /// Currently the CultureInfo.CurrentCulture.Calendar.TwoDigitYearMax is 2029.
        /// This method is subject to change behavior based on operating system regional and language settings, and the
        /// .NET Framework implementation of CultureInfo.CurrentCulture.Calendar.ToFourDigitYear(year);
        /// e.g. GetFourDigitYear(0) : "2000",
        /// GetFourDigitYear(29) : "2029",
        /// GetFourDigitYear(30) : "1930",
        /// GetFourDigitYear(99) : "1999",
        /// GetFourDigitYear(999) : "999"
        /// </remarks>
        /// </summary>
        /// <param name="value">The value to try and convert to a four digit year.</param>
        /// <returns>A string value that is a four digit year or null.</returns>
        public static string GetFourDigitYearAsString(int? value)
        {
            if (value == null)
                return null;

            var year = Convert.ToInt32(value);
            if (year < 0
                || year > DateTime.MaxValue.Year)
                return null;

            return year >= 100
                ? $"{year:d4}"
                : $"{CultureInfo.CurrentCulture.Calendar.ToFourDigitYear(year):d4}";
        }

        /// <summary>
        /// Tries to convert the <paramref name="value"/> parameter into a four digit year.
        /// If this method fails to produce a viable value, it will return null.
        /// If year is less than zero or greater than the DateTime.MaxValue.Year property, it will also return null.
        /// <remarks>
        /// Currently the CultureInfo.CurrentCulture.Calendar.TwoDigitYearMax is 2029.
        /// This method is subject to change behavior based on operating system regional and language settings, and the
        /// .NET Framework implementation of CultureInfo.CurrentCulture.Calendar.ToFourDigitYear(year);
        /// e.g. GetFourDigitYear("0") : "2000",
        /// GetFourDigitYear("29") : "2029",
        /// GetFourDigitYear("30") : "1930",
        /// GetFourDigitYear("99") : "1999",
        /// GetFourDigitYear("999") : "999"
        /// </remarks>
        /// </summary>
        /// <param name="value">The value to try and convert to a four digit year.</param>
        /// <returns>A string value that is a four digit year or null.</returns>
        public static string GetFourDigitYearAsString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            int year;
            if (!int.TryParse(value.Trim(), out year)
                || year < 0
                || year > DateTime.MaxValue.Year)
                return null;

            return year >= 100
                ? $"{year:d4}"
                : $"{CultureInfo.CurrentCulture.Calendar.ToFourDigitYear(year):d4}";
        }
        
        /// <summary>
        /// Gets the time an application would have run, or should run today in UTC, relative to the UTC time zone.
        /// The <paramref name="runStart"/> parameter should be an Hour, Minute, Second, based specification of when to run, represented
        /// by the current UTC system time. The <paramref name="runStart"/> paramater operates purely in UTC time zone.
        /// </summary>
        /// <param name="runStart">The timespan to know the hour, minute, second of the UTC timezone to run an application.</param>
        /// <returns>A DateTime of the time an application would have run today in UTC, relative to the specified time zone.</returns>
        public static DateTime GetPreferredUtcRunDate(
            TimeSpan runStart)
        {
            return GetPreferredUtcRunDate(DateTime.UtcNow, runStart, TimeZoneInfo.Utc);
        }

        /// <summary>
        /// Gets the time an application would have run, or should run today in UTC, relative to the specified time zone.
        /// The <paramref name="runStart"/> parameter should be an Hour, Minute, Second, based specification of when to run, represented
        /// by the current UTC system time, but treated as if the <paramref name="runStart"/> had been related to the specified time zone.
        /// </summary>
        /// <param name="runStart">The timespan to know the hour, minute, second of the specified timezone to run an application.</param>
        /// <param name="timeZoneId">The .NET Time Zone specification, e.g. "Eastern Standard Time".</param>
        /// <returns>A DateTime of the time an application would have run today in UTC, relative to the specified time zone.</returns>
        public static DateTime GetPreferredUtcRunDate(
            TimeSpan runStart,
            string timeZoneId)
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return GetPreferredUtcRunDate(DateTime.UtcNow, runStart, timeZoneInfo);
        }

        /// <summary>
        /// Gets the time an application would have run, or should run today in UTC, relative to the specified time zone.
        /// The <paramref name="runStart"/> parameter should be an Hour, Minute, Second, based specification of when to run, represented
        /// by the current UTC system time, but treated as if the <paramref name="runStart"/> had been related to the specified time zone.
        /// </summary>
        /// <param name="runStart">The timespan to know the hour, minute, second of the specified timezone to run an application.</param>
        /// <param name="timeZoneInfo">The .NET Time Zone specification, e.g. "Eastern Standard Time" represented as a TimeZoneInfo object.</param>
        /// <returns>A DateTime of the time an application would have run today in UTC, relative to the specified time zone.</returns>
        public static DateTime GetPreferredUtcRunDate(
            TimeSpan runStart,
            TimeZoneInfo timeZoneInfo)
        {
            return GetPreferredUtcRunDate(DateTime.UtcNow, runStart, timeZoneInfo);
        }

        /// <summary>
        /// Gets the time an application would have run, or should run today in UTC, relative to the specified time zone.
        /// The <paramref name="utcNow"/> parameter should be typically called with DateTime.UtcNow from external calling code
        /// and is available as a parameter for more flexibility.
        /// The <paramref name="runStart"/> parameter should be an Hour, Minute, Second, based specification of when on the specified date,
        /// represented by <paramref name="utcNow"/> parameter, but treated as if the <paramref name="runStart"/> had been related to the specified time zone.
        /// </summary>
        /// <param name="utcNow">Relative time to do comparisons and conversions.</param>
        /// <param name="runStart">The timespan to know the hour, minute, second of the specified timezone to run an application.</param>
        /// <param name="timeZoneId">The .NET Time Zone specification, e.g. "Eastern Standard Time".</param>
        /// <returns>A DateTime of the time an application would have run today in UTC, relative to the specified time zone.</returns>
        public static DateTime GetPreferredUtcRunDate(
            DateTime utcNow,
            TimeSpan runStart,
            string timeZoneId)
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return GetPreferredUtcRunDate(utcNow, runStart, timeZoneInfo);
        }

        /// <summary>
        /// Gets the time an application would have run, or should run today in UTC, relative to the specified time zone.
        /// The <paramref name="utcNow"/> parameter should be typically called with DateTime.UtcNow from external calling code
        /// and is available as a parameter for more flexibility.
        /// The <paramref name="runStart"/> parameter should be an Hour, Minute, Second, based specification of when on the specified date,
        /// represented by <paramref name="utcNow"/> parameter, but treated as if the <paramref name="runStart"/> had been related to the specified time zone.
        /// </summary>
        /// <param name="utcNow">Relative time to do comparisons and conversions.</param>
        /// <param name="runStart">The timespan to know the hour, minute, second of the specified timezone to run an application.</param>
        /// <param name="timeZoneInfo">The .NET Time Zone specification, e.g. "Eastern Standard Time" represented as a TimeZoneInfo object.</param>
        /// <returns>A DateTime of the time an application would have run today in UTC, relative to the specified time zone.</returns>
        public static DateTime GetPreferredUtcRunDate(
            DateTime utcNow,
            TimeSpan runStart,
            TimeZoneInfo timeZoneInfo)
        {
            // Convert utcNow to passed in TimeZoneInfo ... should be like DateTime.UtcNow passed in, but was left as a variable to pass in intentionally.
            var timeZoneDate = TimeZoneInfo.ConvertTimeFromUtc(utcNow, timeZoneInfo);
            var timeZoneRunDate = timeZoneDate.StartOfDay().Add(runStart);

            // Convert back to UTC.
            return TimeZoneInfo.ConvertTimeToUtc(timeZoneRunDate, timeZoneInfo);
        }

        /// <summary>
        /// Gets a reminder timespan of when to trigger a reminder based on the current UTC system time,
        /// a <paramref name="runStart"/> parameter, <paramref name="period"/> paramater.
        /// The <paramref name="runStart"/> parameter should be an Hour, Minute, Second, based specification of when to run, represented
        /// by the current UTC system time. The <paramref name="runStart"/> paramater operates purely in UTC time zone.
        /// <remarks>
        /// This method could be useful in job scheduling, and service fabric or other microservice systems spread across disperate systems.
        /// </remarks>
        /// </summary>
        /// <param name="runStart">The timespan to know the hour, minute, second of the UTC timezone to run an application.</param>
        /// <param name="period">The period of time between reminders.</param>
        /// <returns>A timespan of when the next reminder should be invoked.</returns>
        public static TimeSpan GetTimeTillNextReminder(
            TimeSpan runStart,
            TimeSpan period)
        {
            return GetTimeTillNextReminder(DateTime.UtcNow, runStart, period, TimeZoneInfo.Utc);
        }

        /// <summary>
        /// Gets a reminder timespan of when to trigger a reminder based on the current UTC system time,
        /// a <paramref name="runStart"/> parameter, <paramref name="period"/> paramater, and the timezone information.
        /// The <paramref name="runStart"/> parameter should be an Hour, Minute, Second, based specification of when to run, represented
        /// by the current UTC system time, but treated as if the <paramref name="runStart"/> had been related to the specified time zone.
        /// <remarks>
        /// This method could be useful in job scheduling, and service fabric or other microservice systems spread across disperate systems.
        /// </remarks>
        /// </summary>
        /// <param name="runStart">The timespan to know the hour, minute, second of the specified timezone to run an application.</param>
        /// <param name="period">The period of time between reminders.</param>
        /// <param name="timeZoneInfo">The .NET Time Zone specification, e.g. "Eastern Standard Time" represented as a TimeZoneInfo object.</param>
        /// <returns>A timespan of when the next reminder should be invoked.</returns>
        public static TimeSpan GetTimeTillNextReminder(
            TimeSpan runStart,
            TimeSpan period,
            TimeZoneInfo timeZoneInfo)
        {
            return GetTimeTillNextReminder(DateTime.UtcNow, runStart, period, timeZoneInfo);
        }

        /// <summary>
        /// Gets a reminder timespan of when to trigger a reminder based on the current UTC system time,
        /// a <paramref name="runStart"/> parameter, <paramref name="period"/> paramater, and the timezone information.
        /// The <paramref name="runStart"/> parameter should be an Hour, Minute, Second, based specification of when to run, represented
        /// by the current UTC system time, but treated as if the <paramref name="runStart"/> had been related to the specified time zone.
        /// <remarks>
        /// This method could be useful in job scheduling, and service fabric or other microservice systems spread across disperate systems.
        /// </remarks>
        /// </summary>
        /// <param name="runStart">The timespan to know the hour, minute, second of the specified timezone to run an application.</param>
        /// <param name="period">The period of time between reminders.</param>
        /// <param name="timeZoneId">The .NET Time Zone specification, e.g. "Eastern Standard Time".</param>
        /// <returns>A timespan of when the next reminder should be invoked.</returns>
        public static TimeSpan GetTimeTillNextReminder(
            TimeSpan runStart,
            TimeSpan period,
            string timeZoneId)
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return GetTimeTillNextReminder(DateTime.UtcNow, runStart, period, timeZoneInfo);
        }

        /// <summary>
        /// Gets a reminder timespan of when to trigger a reminder based on the current time passed into <paramref name="utcNow"/> paramater,
        /// a <paramref name="runStart"/> parameter, <paramref name="period"/> paramater, and the timezone information.
        /// The <paramref name="utcNow"/> parameter should be typically called with DateTime.UtcNow from external calling code
        /// and is available as a parameter for more flexibility.
        /// The <paramref name="runStart"/> parameter should be an Hour, Minute, Second, based specification of when on the specified date,
        /// represented by <paramref name="utcNow"/> parameter, but treated as if the <paramref name="runStart"/> had been related to the specified time zone.
        /// <remarks>
        /// This method could be useful in job scheduling, and service fabric or other microservice systems spread across disperate systems.
        /// </remarks>
        /// </summary>
        /// <param name="utcNow">Relative time to do comparisons and conversions.</param>
        /// <param name="runStart">The timespan to know the hour, minute, second of the specified timezone to run an application.</param>
        /// <param name="period">The period of time between reminders.</param>
        /// <param name="timeZoneId">The .NET Time Zone specification, e.g. "Eastern Standard Time".</param>
        /// <returns>A timespan of when the next reminder should be invoked.</returns>
        public static TimeSpan GetTimeTillNextReminder(
            DateTime utcNow,
            TimeSpan runStart,
            TimeSpan period,
            string timeZoneId)
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return GetTimeTillNextReminder(utcNow, runStart, period, timeZoneInfo);
        }

        /// <summary>
        /// Gets a reminder timespan of when to trigger a reminder based on the current time passed into <paramref name="utcNow"/> paramater,
        /// a <paramref name="runStart"/> parameter, <paramref name="period"/> paramater, and the timezone information.
        /// The <paramref name="utcNow"/> parameter should be typically called with DateTime.UtcNow from external calling code
        /// and is available as a parameter for more flexibility.
        /// The <paramref name="runStart"/> parameter should be an Hour, Minute, Second, based specification of when on the specified date,
        /// represented by <paramref name="utcNow"/> parameter, but treated as if the <paramref name="runStart"/> had been related to the specified time zone.
        /// <remarks>
        /// This method could be useful in job scheduling, and service fabric or other microservice systems spread across disperate systems.
        /// </remarks>
        /// </summary>
        /// <param name="utcNow">Relative time to do comparisons and conversions.</param>
        /// <param name="runStart">The timespan to know the hour, minute, second of the specified timezone to run an application.</param>
        /// <param name="period">The period of time between reminders.</param>
        /// <param name="timeZoneInfo">The .NET Time Zone specification, e.g. "Eastern Standard Time" represented as a TimeZoneInfo object.</param>
        /// <returns>A timespan of when the next reminder should be invoked.</returns>
        public static TimeSpan GetTimeTillNextReminder(
            DateTime utcNow,
            TimeSpan runStart,
            TimeSpan period,
            TimeZoneInfo timeZoneInfo)
        {
            var preferredUtcRunDate = GetPreferredUtcRunDate(utcNow, runStart, timeZoneInfo);
            var nextPreferredUtcRunDate = preferredUtcRunDate.Add(period);
            return preferredUtcRunDate == utcNow
                ? TimeSpan.Zero
                : preferredUtcRunDate < utcNow
                    ? nextPreferredUtcRunDate.Subtract(utcNow)
                    : preferredUtcRunDate.Subtract(utcNow);
        }
    }
}

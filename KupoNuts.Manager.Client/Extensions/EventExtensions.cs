﻿// This document is intended for use by Kupo Nut Brigade developers.

namespace KupoNuts.Events
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Text;
	using KupoNuts.Utils;
	using NodaTime;
	using NodaTime.Text;

	public static class EventExtensions
	{
		public static Duration GetDuration(this Event self)
		{
			if (string.IsNullOrEmpty(self.Duration))
				return Duration.FromSeconds(0);

			try
			{
				return DurationPattern.Roundtrip.Parse(self.Duration).Value;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Failed to deserialize duration string: \"" + self.Duration + "\" " + ex.Message);
				return Duration.FromSeconds(0);
			}
		}

		public static void GetDuration(this Event self, out string duration)
		{
			Duration dur = self.GetDuration();
			duration = dur.Hours.ToString("D2") + ":" + dur.Minutes.ToString("D2");
		}

		public static void SetDuration(this Event self, string duration)
		{
			string[] parts = duration.Split(':');

			if (parts.Length != 2)
				throw new Exception("Invalid duration format: \"" + duration + "\"");

			int hour = int.Parse(parts[0]);
			int minute = int.Parse(parts[1]);

			Duration dur = Duration.FromMinutes((hour * 60) + minute);
			self.Duration = DurationPattern.Roundtrip.Format(dur);
		}

		public static Instant GetDateTime(this Event self)
		{
			if (string.IsNullOrEmpty(self.DateTime))
				return Instant.FromJulianDate(0);

			return InstantPattern.ExtendedIso.Parse(self.DateTime).Value;
		}

		public static void SetDateTime(this Event self, string date, string time)
		{
			string[] parts = date.Split('-');

			if (parts.Length != 3)
				throw new Exception("Invalid date format: \"" + date + "\"");

			int year = int.Parse(parts[0]);
			int month = int.Parse(parts[1]);
			int day = int.Parse(parts[2]);

			parts = time.Split(':');

			if (parts.Length != 2)
				throw new Exception("Invalid time format: \"" + time + "\"");

			int hour = int.Parse(parts[0]);
			int minute = int.Parse(parts[1]);

			LocalDateTime ldt = new LocalDateTime(year, month, day, hour, minute);
			ZonedDateTime zdt = ldt.InZoneLeniently(DateTimeZoneProviders.Tzdb.GetSystemDefault());
			Instant instant = zdt.ToInstant();

			self.SetDateTime(instant);
		}

		public static void GetDateTime(this Event self, out string date, out string time)
		{
			Instant instant = self.GetDateTime();

			ZonedDateTime zdt = instant.InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault());
			LocalDateTime ldt = zdt.LocalDateTime;

			date = ldt.Year.ToString("D4") + "-" + ldt.Month.ToString("D2") + "-" + ldt.Day.ToString("D2");
			time = ldt.Hour.ToString("D2") + ":" + ldt.Minute.ToString("D2");
		}

		public static void SetDateTime(this Event self, Instant instant)
		{
			self.DateTime = InstantPattern.ExtendedIso.Format(instant);
		}

		public static void GetRepeats(this Event self, out bool mon, out bool tue, out bool wed, out bool thu, out bool fri, out bool sat, out bool sun)
		{
			mon = FlagsUtils.IsSet(Event.Days.Monday, self.Repeats);
			tue = FlagsUtils.IsSet(Event.Days.Tuesday, self.Repeats);
			wed = FlagsUtils.IsSet(Event.Days.Wednesday, self.Repeats);
			thu = FlagsUtils.IsSet(Event.Days.Thursday, self.Repeats);
			fri = FlagsUtils.IsSet(Event.Days.Friday, self.Repeats);
			sat = FlagsUtils.IsSet(Event.Days.Saturday, self.Repeats);
			sun = FlagsUtils.IsSet(Event.Days.Sunday, self.Repeats);
		}

		public static void SetRepeats(this Event self, bool mon, bool tue, bool wed, bool thu, bool fri, bool sat, bool sun)
		{
			Event.Days repeats = self.Repeats;
			FlagsUtils.Set(ref repeats, Event.Days.Monday, mon);
			FlagsUtils.Set(ref repeats, Event.Days.Tuesday, tue);
			FlagsUtils.Set(ref repeats, Event.Days.Wednesday, wed);
			FlagsUtils.Set(ref repeats, Event.Days.Thursday, thu);
			FlagsUtils.Set(ref repeats, Event.Days.Friday, fri);
			FlagsUtils.Set(ref repeats, Event.Days.Saturday, sat);
			FlagsUtils.Set(ref repeats, Event.Days.Sunday, sun);
			self.Repeats = repeats;
		}

		public static string GetNextOccurance(this Event self)
		{
			DateTimeZone zone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
			Instant? next = self.GetNextOccurance(zone);

			if (next == null)
				return "Never";

			ZonedDateTime zdt = ((Instant)next).InZone(zone);
			StringBuilder builder = new StringBuilder();
			builder.Append(zdt.ToString("hh:mm ", CultureInfo.InvariantCulture));
			builder.Append(zdt.ToString("tt", CultureInfo.InvariantCulture).ToLower());
			builder.Append(zdt.ToString(" dd/MM/yyyy", CultureInfo.InvariantCulture).ToLower());
			return builder.ToString();
		}

		public static Instant? GetNextOccurance(this Event self, DateTimeZone zone)
		{
			Instant eventDateTime = self.GetDateTime();

			Instant now = SystemClock.Instance.GetCurrentInstant();
			if (eventDateTime < now)
			{
				if (self.Repeats == 0)
					return null;

				LocalDate nextDate;
				LocalDateTime dateTime = eventDateTime.InZone(zone).LocalDateTime;

				LocalDate date = dateTime.Date;
				LocalDate todaysDate = TimeUtils.Now.InZone(zone).LocalDateTime.Date;

				List<LocalDate> dates = new List<LocalDate>();
				foreach (Event.Days day in Enum.GetValues(typeof(Event.Days)))
				{
					if (!FlagsUtils.IsSet(self.Repeats, day))
						continue;

					nextDate = todaysDate.Next(TimeUtils.ToIsoDay(day));
					dates.Add(nextDate);
				}

				if (dates.Count <= 0)
					return eventDateTime;

				dates.Sort();
				nextDate = dates[0];

				Period dateOffset = nextDate - date;
				dateTime = dateTime + dateOffset;

				Instant nextInstant = dateTime.InZoneLeniently(zone).ToInstant();
				return nextInstant;
			}

			return eventDateTime;
		}
	}
}
﻿// This document is intended for use by Kupo Nut Brigade developers.

namespace KupoNuts.Bot.Characters
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using Discord;
	using XIVAPI;

	public static class CharacterAPIExtensions
	{
		public static Embed BuildEmbed(this CharacterAPI.Character self)
		{
			EmbedBuilder builder = new EmbedBuilder();
			builder.Color = Color.Blue;

			StringBuilder titleBuilder = new StringBuilder();

			if (self.Title != null && self.TitleTop)
			{
				titleBuilder.Append(self.Title.Name);
				titleBuilder.Append(" - ");
			}

			titleBuilder.Append(self.Name);

			if (self.Title != null && !self.TitleTop)
			{
				titleBuilder.Append(" - ");
				titleBuilder.Append(self.Title.Name);
			}

			builder.Title = titleBuilder.ToString();
			builder.ImageUrl = self.Portrait;

			StringBuilder descriptionBuilder = new StringBuilder();

			if (self.Race != null && self.Tribe != null)
			{
				descriptionBuilder.Append(self.Tribe.Name);
				descriptionBuilder.Append(", ");
				descriptionBuilder.AppendLine(self.Race.Name);
			}

			builder.Description = descriptionBuilder.ToString();

			builder.AddField("Bio", self.Bio, true);

			if (self.ClassJobs != null)
			{
				StringBuilder jobsBuilder = new StringBuilder();

				jobsBuilder.Append(self.GetJobString(Jobs.Paladin));
				jobsBuilder.Append(self.GetJobString(Jobs.Warrior));
				jobsBuilder.Append(self.GetJobString(Jobs.Darkknight));
				jobsBuilder.Append(self.GetJobString(Jobs.Gunbreaker));
				jobsBuilder.AppendLine();
				jobsBuilder.Append(self.GetJobString(Jobs.Whitemage));
				jobsBuilder.Append(self.GetJobString(Jobs.Scholar));
				jobsBuilder.Append(self.GetJobString(Jobs.Astrologian));
				jobsBuilder.AppendLine();
				jobsBuilder.Append(self.GetJobString(Jobs.Monk));
				jobsBuilder.Append(self.GetJobString(Jobs.Dragoon));
				jobsBuilder.Append(self.GetJobString(Jobs.Ninja));
				jobsBuilder.Append(self.GetJobString(Jobs.Samurai));
				jobsBuilder.AppendLine();
				jobsBuilder.Append(self.GetJobString(Jobs.Bard));
				jobsBuilder.Append(self.GetJobString(Jobs.Machinist));
				jobsBuilder.Append(self.GetJobString(Jobs.Dancer));
				jobsBuilder.AppendLine();
				jobsBuilder.Append(self.GetJobString(Jobs.Blackmage));
				jobsBuilder.Append(self.GetJobString(Jobs.Summoner));
				jobsBuilder.Append(self.GetJobString(Jobs.Redmage));
				jobsBuilder.Append(self.GetJobString(Jobs.Bluemage));

				builder.AddField("Jobs", jobsBuilder.ToString());

				StringBuilder craftersBuilder = new StringBuilder();
				craftersBuilder.Append(self.GetJobString(Jobs.Carpenter));
				craftersBuilder.Append(self.GetJobString(Jobs.Blacksmith));
				craftersBuilder.Append(self.GetJobString(Jobs.Armorer));
				craftersBuilder.Append(self.GetJobString(Jobs.Goldsmith));
				craftersBuilder.AppendLine();
				craftersBuilder.Append(self.GetJobString(Jobs.Leatherworker));
				craftersBuilder.Append(self.GetJobString(Jobs.Weaver));
				craftersBuilder.Append(self.GetJobString(Jobs.Alchemist));
				craftersBuilder.Append(self.GetJobString(Jobs.Culinarian));
				craftersBuilder.AppendLine();
				craftersBuilder.Append(self.GetJobString(Jobs.Botanist));
				craftersBuilder.Append(self.GetJobString(Jobs.Miner));
				craftersBuilder.Append(self.GetJobString(Jobs.Fisher));

				builder.AddField("Gatherers / Crafters", craftersBuilder.ToString());
			}

			return builder.Build();
		}

		public static string GetJobString(this CharacterAPI.Character self, Jobs id)
		{
			CharacterAPI.ClassJob? classJob = self.GetClassJob(id);

			if (classJob == null)
				return string.Empty;

			return classJob.GetDisplayString() + Utils.Characters.Tab;
		}

		public static CharacterAPI.ClassJob? GetClassJob(this CharacterAPI.Character self, Jobs id)
		{
			if (self.ClassJobs == null)
				return null;

			foreach (CharacterAPI.ClassJob job in self.ClassJobs)
			{
				if (job.Job == null || job.Job.ID == null)
					return null;

				if (job.Job.ID != (uint)id)
					continue;

				return job;
			}

			return null;
		}

		public static string? GetDisplayString(this CharacterAPI.ClassJob self)
		{
			StringBuilder jobsBuilder = new StringBuilder();
			if (self.Job == null || self.Job.ID == null)
				return null;

			Jobs job = (Jobs)(int)self.Job.ID;

			jobsBuilder.Append(job.GetEmote());
			jobsBuilder.Append(" ");

			if (self.Level > 0)
				jobsBuilder.Append("**");

			if (self.Level >= 80)
				jobsBuilder.Append("__");

			jobsBuilder.Append(self.Level.ToString());

			if (self.Level >= 80)
				jobsBuilder.Append("__");

			if (self.Level <= 0)
			{
				jobsBuilder.Append(Utils.Characters.Space);
			}
			else
			{
				jobsBuilder.Append("**");
			}

			return jobsBuilder.ToString();
		}
	}
}

﻿// This document is intended for use by Kupo Nut Brigade developers.

namespace KupoNutsBot.Status
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Threading.Tasks;
	using Discord;
	using Discord.Rest;
	using Discord.WebSocket;
	using KupoNutsBot.Services;

	public class StatusService : ServiceBase
	{
		private bool online;

		public override async Task Initialize()
		{
			this.online = true;

			_ = Task.Factory.StartNew(() => this.UpdateStatus(), TaskCreationOptions.LongRunning);

			await this.PostStatus();
		}

		public override async Task Shutdown()
		{
			this.online = false;
			await this.PostStatus();
		}

		private async Task PostStatus()
		{
			if (Database.Instance.LogChannel <= 0)
			{
				Console.WriteLine("No Log Channel set. Kupo Nuts will not post logs to discord");
				return;
			}

			EmbedBuilder builder = new EmbedBuilder();
			builder.Color = Color.DarkRed;
			builder.Title = "Kupo Nuts Bot Status";

			builder.AddField("Status", this.online ? "Online" : "Offline", true);
			string dateTime = DateTime.UtcNow.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
			builder.AddField("Last Online", dateTime, true);

			SocketTextChannel channel = (SocketTextChannel)Program.DiscordClient.GetChannel(Database.Instance.LogChannel);

			RestUserMessage message;
			if (Database.Instance.StatusMessage == 0)
			{
				message = await channel.SendMessageAsync(null, false, builder.Build());
				Database.Instance.StatusMessage = message.Id;
				Database.Instance.Save();
			}
			else
			{
				message = (RestUserMessage)await channel.GetMessageAsync(Database.Instance.StatusMessage);
				await message.ModifyAsync(x =>
				{
					x.Embed = builder.Build();
				});
			}
		}

		private async Task UpdateStatus()
		{
			int minutes = DateTime.UtcNow.Minute;
			int delay = minutes % 15;
			Console.WriteLine("Waiting " + delay + " minutes before status update");

			await Task.Delay(new TimeSpan(0, delay, 0));

			while (this.online)
			{
				await Task.Delay(new TimeSpan(0, 15, 0));
				await this.PostStatus();
			}
		}
	}
}

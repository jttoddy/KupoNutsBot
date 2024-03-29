﻿// This document is intended for use by Kupo Nut Brigade developers.

namespace KupoNuts.Bot.Commands
{
	using Discord;
	using Discord.WebSocket;

	public class CommandMessage
	{
		public readonly string CommandPrefix;
		public readonly string Command;
		public readonly SocketMessage Message;

		public CommandMessage(string commandPrefix, string command, SocketMessage message)
		{
			this.CommandPrefix = commandPrefix;
			this.Command = command;
			this.Message = message;
		}

		public ulong Id
		{
			get
			{
				return this.Message.Id;
			}
		}

		public ISocketMessageChannel Channel
		{
			get
			{
				return this.Message.Channel;
			}
		}

		public IGuildUser Author
		{
			get
			{
				return this.Message.GetAuthor();
			}
		}

		public IGuild Guild
		{
			get
			{
				return this.Message.GetGuild();
			}
		}
	}
}

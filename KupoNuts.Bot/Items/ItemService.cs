﻿// This document is intended for use by Kupo Nut Brigade developers.

namespace KupoNuts.Bot.Items
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Threading.Tasks;
	using Discord;
	using KupoNuts.Bot.Commands;
	using KupoNuts.Bot.Services;
	using XIVAPI;

	public class ItemService : ServiceBase
	{
		public static string DyeEmote = "<:Dye:624532865404633088> ";
		public static string SalvageEmote = "<:Desynth:624533057634041876> ";
		public static string ConvertToMateriaEmote = "<:ConvertToMateria:624532395261165569> ";
		public static string UntradableEmote = "<:Untradable:624532956710699008> ";
		public static string UniqueEmote = "<:Unique:624532714506158091> ";
		public static string ArmoireEmote = "<:Armoire:624513915765784584> ";
		public static string GlamourDresserEmote = "<:GlamourDresser:624513915719778305> ";
		public static string CrestEmote = "<:CompanyCrests:624513915325382678> ";
		public static string AdvancedMeldingForbiddenEmote = "<:AdvancedMeldingForbidden:624534020906156032> ";

		[Command("ISearch", Permissions.Everyone, "Gets information on an item")]
		[Command("ItemSearch", Permissions.Everyone, "Gets information on an item")]
		public async Task<Embed> GetItem(string search)
		{
			List<SearchAPI.Result> results = await SearchAPI.Search(search, "Item");

			if (results.Count <= 0)
				throw new UserException("I couldn't find any items that match that search.");

			if (results.Count > 1)
			{
				EmbedBuilder embed = new EmbedBuilder();

				StringBuilder description = new StringBuilder();
				for (int i = 0; i < Math.Min(results.Count, 10); i++)
				{
					description.AppendLine(results[i].ID + " - " + results[i].Name);
				}

				embed.Title = results.Count + " results found";
				embed.Description = description.ToString();
				return embed.Build();
			}

			ulong? id = results[0].ID;

			if (id == null)
				throw new Exception("No Id in item");

			return await this.GetItem((ulong)id);
		}

		[Command("ISearch", Permissions.Everyone, "Gets information on an item")]
		[Command("ItemSearch", Permissions.Everyone, "Gets information on an item")]
		public async Task<Embed> GetItem(ulong itemID)
		{
			ItemAPI.Item item = await ItemAPI.Get(itemID);
			return item.ToEmbed();
		}
	}
}

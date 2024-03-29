﻿using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VoidJailerMod.Initialization;
using VoidJailerMod.Skills.Spike;
using Xan.ROR2VoidPlayerCharacterCommon;

namespace VoidJailerMod
{
    public static class Localization {

		public const string UNIQUE_SURVIVOR_PREFIX = "VOID_JAILER_PLAYER";

		public const string SURVIVOR_NAME = $"{UNIQUE_SURVIVOR_PREFIX}_NAME";
		public const string SURVIVOR_DESC = $"{UNIQUE_SURVIVOR_PREFIX}_DESCRIPTION";
		public const string SURVIVOR_LORE = $"{UNIQUE_SURVIVOR_PREFIX}_LORE";
		public const string SURVIVOR_OUTRO = $"{UNIQUE_SURVIVOR_PREFIX}_OUTRO_FLAVOR";
		public const string SURVIVOR_OUTRO_FAILED = $"{UNIQUE_SURVIVOR_PREFIX}_OUTRO_FLAVOR_FAIL"; // As far as this one goes, I am not actually sure if it gets used. I'll add it anyway.
		public const string SURVIVOR_UMBRA = $"{UNIQUE_SURVIVOR_PREFIX}_UMBRA";

		public const string DEFAULT_SKIN = $"{UNIQUE_SURVIVOR_PREFIX}_DEFAULT_SKIN";
		public const string GHOST_SKIN = $"{UNIQUE_SURVIVOR_PREFIX}_GHOST_SKIN";

		public const string PASSIVE_KEYWORD = $"{UNIQUE_SURVIVOR_PREFIX}_PASSIVE_KEYWORD";
		public const string PASSIVE_NAME = $"{UNIQUE_SURVIVOR_PREFIX}_PASSIVE_NAME";
		public const string PASSIVE_DESC = $"{UNIQUE_SURVIVOR_PREFIX}_PASSIVE_DESC";

		public const string SKILL_PRIMARY_SHOTGUN_NAME = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_PRIMARY_SHOTGUN_NAME";
		public const string SKILL_PRIMARY_MINIGUN_NAME = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_PRIMARY_MINIGUN_NAME";
		public const string SKILL_SECONDARY_NAME = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_SECONDARY_NAME";
		public const string SKILL_UTILITY_NAME = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_UTILITY_NAME";
		public const string SKILL_SPECIAL_FURY_NAME = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_SPECIAL_FURY_NAME";
		public const string SKILL_SPECIAL_MORTAR_NAME = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_SPECIAL_MORTAR_NAME";

		public const string SKILL_PRIMARY_SHOTGUN_DESC = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_PRIMARY_SHOTGUN_DESC";
		public const string SKILL_PRIMARY_MINIGUN_DESC = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_PRIMARY_MINIGUN_DESC";
		public const string SKILL_SECONDARY_DESC = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_SECONDARY_DESC";
		public const string SKILL_UTILITY_DESC = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_UTILITY_DESC";
		public const string SKILL_SPECIAL_FURY_DESC = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_SPECIAL_FURY_DESC";
		public const string SKILL_SPECIAL_MORTAR_DESC = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_SPECIAL_MORTAR_DESC";
		public const string SKILL_SPECIAL_FURY_KEYWORD = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_SPECIAL_FURY_KEYWORD";

		public const string BUFF_PLAYERVOIDJAILER_FURY_NAME = "BUFF_PLAYERVOIDJAILER_FURY_NAME";
		public const string BUFF_PLAYERVOIDJAILER_FURY_DESC = "BUFF_PLAYERVOIDJAILER_FURY_DESC";

		public const string VOID_COMMON_API_MESSAGE_NAME = "VOID_COMMON_API_MESSAGE_NAME";
		public const string VOID_COMMON_API_MESSAGE_DESC = "VOID_COMMON_API_MESSAGE_DESC";
		public const string VOID_COMMON_API_MESSAGE_CONTENT = "VOID_COMMON_API_MESSAGE_CONTENT";

		private const string SKULL = "<sprite name=\"Skull\" tint=1>";


		/// <summary>
		/// Takes a floating point value in the range of 0 to 1 and converts it to a rounded percentage in the range of 0 to 100 as a string.
		/// </summary>
		/// <param name="fpPercent"></param>
		/// <returns></returns>
		private static string Percentage(float fpPercent) => Mathf.RoundToInt(fpPercent * 100).ToString() + "%";

		/// <summary>
		/// Ronds a floating point value to the nearest integer, returning it as a string.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private static string Round(float value) => Mathf.RoundToInt(value).ToString();

		/// <summary>
		/// Ronds a floating point value to the nearest tenths place, returning it as a string.
		/// If the value is a whole number, it returns that number verbatim.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		// also lmao
		private static string RoundTen(float value) => value == Mathf.Floor(value) ? value.ToString() : (Mathf.Round(value * 10) / 10).ToString("0.0");

		/// <summary>
		/// Lazily "pluralize" a word by adding "s" to the end if the input value is not exactly 1.
		/// </summary>
		/// <param name="word"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		private static string LazyPluralize(string word, float value) => value == 1 ? word : word + "s";

		/// <summary>
		/// Converts a set of lines (each as one parameter) into RoR2's standard character details format.
		/// </summary>
		/// <param name="lines"></param>
		/// <returns></returns>
		private static string LinesToSurvivorDetails(string intro, params string[] lines) {
			StringBuilder resultBuilder = new StringBuilder(intro);
			resultBuilder.AppendLine();
			resultBuilder.AppendLine();
			foreach (string line in lines) {
				resultBuilder.Append("<style=cSub>< ! > ");
				resultBuilder.Append(line);
				resultBuilder.AppendLine("</style>");
				resultBuilder.AppendLine();
			}
			return resultBuilder.ToString();
		}

		private static void Bind(string key, string value) {
			Log.LogTrace($"Registering language key \"{key}\"...");
			LanguageAPI.Add(key, value);
		}

		private static void ReplaceBind(string key, string value) {
			Log.LogTrace($"Overwriting language key \"{key}\"...");
			XanVoidAPI.AddOrReplaceLang(key, value);
		}

		internal static void Init() {
			#region Name and Lore
			Bind(SURVIVOR_NAME, "Void Jailer");
			Bind(SURVIVOR_DESC, LinesToSurvivorDetails(
				"The Void Jailer specializes in short to medium range one-on-one combat. It has an exceptionally high burst damage output, but struggles with attack speed, making it vulnerable to swarms of enemies.",
				"<style=cIsVoid>Spike</style> is a shotgun-like ability that can deal devastating blows to larger targets. It does a significant amount of extra damage against rooted enemies, so consider picking up a <style=cIsVoid>Tentabauble</style> if you find one, and make good use of your secondary!",
				"<style=cIsVoid>Bind</style> roots and damages a single target, pulling them directly in front of you.",
				"<style=cIsVoid>Dive</style> is a powerful escape tool. Alongside healing you, it makes enemies lose track of you, and protects you from incoming damage.",
				"<style=cIsVoid>Fury</style> gives you armor and increases your damage output. It is useful against bosses. Be careful though, you aren't invincible, and reckless use can get you killed!"
			));
			Bind(SURVIVOR_LORE, THE_LORE);
			Bind(SURVIVOR_OUTRO, "..and so it left, intrigued to become what it once sought to capture");
			Bind(SURVIVOR_OUTRO_FAILED, "..and so it was detained, awaiting its sentence at the end of Time");
			Bind(SURVIVOR_UMBRA, "Rogue Warden");
			#endregion

			#region Palettes
			Bind(DEFAULT_SKIN, "Default");
			Bind(GHOST_SKIN, "Newly Hatched Zoea");
			#endregion

			#region Passive
			string voidBornIntro = "The Void Jailer inherits all of the benefits and drawbacks of its kin.";
			Bind(PASSIVE_NAME, "<style=cIsVoid>Void Entity</style>");
			Bind(PASSIVE_DESC, voidBornIntro);
			#endregion

			#region Primary
			Bind(SKILL_PRIMARY_SHOTGUN_NAME, "Spike");
			Bind(SKILL_PRIMARY_MINIGUN_NAME, "Perforate");
			Bind(SKILL_PRIMARY_MINIGUN_DESC, $"The firerate of your claw has increased dramatically, but less Darts are shot per burst.");
			#endregion

			#region Secondary
			Bind(SKILL_SECONDARY_NAME, "Bind");
			
			#endregion

			#region Utility
			Bind(SKILL_UTILITY_NAME, "Dive");
			Bind(SKILL_UTILITY_DESC, $"Propel yourself through the void at <style=cIsUtility><style=cUserSetting>{Percentage(Configuration.UtilitySpeed)}</style> movement speed</style> for <style=cIsUtility><style=cUserSetting>{RoundTen(Configuration.UtilityDuration)}</style> {LazyPluralize("second", Configuration.UtilityDuration)}</style>, healing <style=cIsHealing><style=cUserSetting>{Percentage(Configuration.UtilityRegeneration)}</style> maximum health</style>. Gain <style=cIsUtility>Immunity</style> and <style=cIsUtility>Invisibility</style> while away.");
			#endregion

			#region Special
			Bind(SKILL_SPECIAL_FURY_NAME, "Fury of the Warden");
			Bind(SKILL_SPECIAL_MORTAR_NAME, "Cavitation Mortar");
			#endregion

			#region Buffs
			Bind(BUFF_PLAYERVOIDJAILER_FURY_NAME, "Fury");
			#endregion

			Bind(VOID_COMMON_API_MESSAGE_NAME, "<style=cDeath>Mod Update Message</style>");
			Bind(VOID_COMMON_API_MESSAGE_DESC, "There's important information for both new and old users. Hover for more.");
			Bind(VOID_COMMON_API_MESSAGE_CONTENT, "A huge rewrite has just occurred to the core systems for this mod.\n\nFirstly, <style=cIsVoid>the ability to edit stats and configs in realtime</style> has been added. Additionally, <style=cIsVoid>these settings automatically synchronize over the network from the host!</style>\n\nSecondly, some stats and mechanics (especially with respect to the black hole) have changed. <style=cIsDamage>In general, if something isn't working like it used to, the option to change it back is <i>probably</i> in the configs.</style> Try checking there if anything seems off.\n\nFinally, it is <style=cDeath>strongly recommended to use the in-game configuration screen</style> (from Risk of Options) instead of the r2modman/Thunderstore menu. It will be a <i>lot</i> easier.\n\n<style=cIsHealing>Try it now! You can turn this message off in the mod's settings.</style>");

			Log.LogTrace("Localization initialized.");
		}

		public static void ReloadStattedTexts(BodyIndex reaver) {
			ReplaceBind(SKILL_PRIMARY_SHOTGUN_DESC, $"Fire a burst of Void Darts from your claw, dealing <style=cIsDamage><style=cUserSetting>{Configuration.BasePrimaryProjectileCount}</style>x<style=cUserSetting>{Percentage(Configuration.BasePrimaryDamage)}</style> damage</style>. Damage is boosted by <style=cUserSetting>{Percentage(Configuration.NullifiedDamageBoost)}</style> on targets that are <style=cIsVoid>Nullified</style> or <style=cIsVoid>Tethered</style>.");
			ReplaceBind(SKILL_SECONDARY_DESC, $"Latch onto an enemy from a distance, applying a force that tries to pull them directly in front of you. Deals <style=cIsDamage><style=cUserSetting>{Percentage(Configuration.BaseSecondaryDamage)}</style> damage</style> and inflicts <style=cIsVoid>Nullify</style>. Heals <style=cIsHealing><style=cUserSetting>{Percentage(Configuration.SecondaryHealAmountOnHit)}</style> maximum health</style>.");

			string commonSpecialStatDesc = $"Gain <style=cIsUtility><style=cUserSetting>{RoundTen(Configuration.SpecialRageArmorBoost)}</style> Armor</style> and replace <style=cIsVoid>Spike</style> with <style=cIsVoid>Perforate</style>.";
			string perforateDesc = $"<style=cIsUtility>Can be fired while sprinting.</style> Void Darts are augmented, firing much faster and causing them to inflict <style=cIsVoid>Instant Collapse</style>, causing them to do a total of <style=cIsDamage><style=cUserSetting>{Configuration.SpecialRageProjectileCount}</style>x<style=cUserSetting>{Percentage(Configuration.SpecialRageDamageBoost * Configuration.BasePrimaryDamage)}</style> damage</style>.";

			ReplaceBind(SKILL_SPECIAL_FURY_DESC, $"Become enraged, gaining <style=cIsDamage>Fury</style> for <style=cUserSetting>{RoundTen(Configuration.SpecialRageDuration)}</style> seconds. Replaces <style=cIsVoid>Spike</style> with <style=cIsVoid>Perforate</style>.");
			ReplaceBind(SKILL_SPECIAL_FURY_KEYWORD, $"[ Fury ]\n{commonSpecialStatDesc}\n\n[ Perforate ]\n{perforateDesc}");
			ReplaceBind(BUFF_PLAYERVOIDJAILER_FURY_DESC, commonSpecialStatDesc);

			LateInit(reaver);
		}

		private static void LateInitInstakillMortar(BodyIndex bodyIndex) {
			bool monsters = XanVoidAPI.CanCharacterInstakillMonsters(bodyIndex);
			bool bosses = XanVoidAPI.CanCharacterInstakillBosses(bodyIndex);
			bool players = XanVoidAPI.CanCharacterFriendlyFire(bodyIndex);
			float fallback = XanVoidAPI.GetFallbackDamage(bodyIndex);

			StringBuilder result = new StringBuilder();
			int listSize = 0;
			if (monsters || bosses || players) {
				result.Append("<style=cDeath>instantly kills</style> all");

				if (monsters) {
					result.Append(" <style=cIsDamage>monsters</style>");
					listSize++;
				}
				if (bosses) {
					if (listSize > 0) result.Append(',');
					result.Append(" <style=cIsDamage>bosses</style>");
					listSize++;
				}
				if (players) {
					if (listSize == 0) {
						result.Append(" players");
					} else if (listSize == 1) {
						result.Append(" and <style=cIsDamage>players</style>");
					} else {
						result.Append(", and <style=cIsDamage>players</style>");
					}
					listSize++;
				}
				result.Append(" caught within its radius. ");
			}
			if (!monsters || !bosses) {
				if (listSize > 0) {
					result.Append("It instead ");
				}
				result.Append($"deals <style=cIsDamage>{((int)Math.Floor(fallback)) * 100}% base damage</style> to all");
				bool and = false;
				if (!monsters) {
					result.Append(" <style=cIsDamage>monsters</style>");
					and = true;
				}
				if (!bosses) {
					if (and) result.Append(" and");
					result.Append(" <style=cIsDamage>bosses</style>");
				}
				result.Append(" caught within its radius.");
			}


			if (Configuration.MortarCanInstakill) {
				ReplaceBind(SKILL_SPECIAL_MORTAR_DESC, $"Fire a {(Configuration.MortarBombCount > 1 ? $"cluster of {Configuration.MortarBombCount} small, mortar-like void bubbles." : "small, mortar-like void bubble.")} On impact, the mortar will explode, which {result}");
			} else {
				ReplaceBind(SKILL_SPECIAL_MORTAR_DESC, $"Fire a {(Configuration.MortarBombCount > 1 ? $"cluster of {Configuration.MortarBombCount} small, mortar-like void bubbles that explode on impact, doing" : "small, mortar-like void bubble that explodes on impact, doing")} up to <style=cIsDamage>{(Configuration.MortarBombCount > 1 ? $"<style=cUserSetting>{Configuration.MortarBombCount}</style>x" : string.Empty)}<style=cUserSetting>{Percentage(Configuration.MortarDamage)}</style> damage</style> on impact.");
			}
		}

		internal static void LateInit(BodyIndex jailer) {
			string blackHoleDesc = XanVoidAPI.BuildBlackHoleDescription(jailer, true);
			string desc = $"[ Reave ]\n<style=cSub>Upon death, {blackHoleDesc}";
			desc += "\n\n[ Void Entity ]\n<style=cSub>The Void Jailer is <style=cIsUtility>immune</style> to <style=cIsVoid>Void Fog</style>, and will not take damage within <style=cIsVoid>Void Seeds</style> or whilst outside the range of a protective bubble in Void environments.</style>";

			ReplaceBind(PASSIVE_KEYWORD, desc);
			LateInitInstakillMortar(jailer);
		}
		// TO SELF: The idea was that Void figured out survivors killed Providence, they want to take Mithrix, suspect the survivors are strong enough to do that too and so they try to work with the players
		private const string THE_LORE = @"""Yes, that's correct. I did indeed fry that rice.""
		
- Void Jailer, End of Time";

	}
}

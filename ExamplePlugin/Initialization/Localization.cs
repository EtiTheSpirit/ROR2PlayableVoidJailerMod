using R2API;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VoidJailerMod.Skills.Spike;

namespace VoidJailerMod
{
    public static class Localization {

		public const string UNIQUE_SURVIVOR_PREFIX = "VOID_JAILER_PLAYER";

		public const string SURVIVOR_NAME = $"{UNIQUE_SURVIVOR_PREFIX}_NAME";
		public const string SURVIVOR_DESC = $"{UNIQUE_SURVIVOR_PREFIX}_DESCRIPTION";
		public const string SURVIVOR_LORE = $"{UNIQUE_SURVIVOR_PREFIX}_LORE";
		public const string SURVIVOR_OUTRO = $"{UNIQUE_SURVIVOR_PREFIX}_OUTRO_FLAVOR";
		public const string SURVIVOR_OUTRO_FAILED = $"{UNIQUE_SURVIVOR_PREFIX}_OUTRO_FLAVOR_FAIL"; // As far as this one goes, I am not actually sure if it gets used. I'll add it anyway.

		public const string DEFAULT_SKIN = $"{UNIQUE_SURVIVOR_PREFIX}_DEFAULT_SKIN";
		public const string GHOST_SKIN = $"{UNIQUE_SURVIVOR_PREFIX}_GHOST_SKIN";

		public const string PASSIVE_KEYWORD = $"{UNIQUE_SURVIVOR_PREFIX}_PASSIVE_KEYWORD";
		public const string PASSIVE_NAME = $"{UNIQUE_SURVIVOR_PREFIX}_PASSIVE_NAME";
		public const string PASSIVE_DESC = $"{UNIQUE_SURVIVOR_PREFIX}_PASSIVE_DESC";

		public const string SKILL_PRIMARY_NAME = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_PRIMARY_NAME";
		public const string SKILL_SECONDARY_NAME = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_SECONDARY_NAME";
		public const string SKILL_UTILITY_NAME = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_UTILITY_NAME";
		public const string SKILL_SPECIAL_NAME = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_SPECIAL_NAME";

		public const string SKILL_PRIMARY_DESC = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_PRIMARY_DESC";
		public const string SKILL_SECONDARY_DESC = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_SECONDARY_DESC";
		public const string SKILL_UTILITY_DESC = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_UTILITY_DESC";
		public const string SKILL_SPECIAL_DESC = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_SPECIAL_DESC";
		public const string SKILL_SPECIAL_KEYWORD = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_SPECIAL_KEYWORD";

		public const string BUFF_PLAYERVOIDJAILER_FURY_NAME = "BUFF_PLAYERVOIDJAILER_FURY_NAME";
		public const string BUFF_PLAYERVOIDJAILER_FURY_DESC = "BUFF_PLAYERVOIDJAILER_FURY_DESC";

		public const string VOID_RIFT_SHOCK_NAME = "VOID_RIFT_SHOCK_NAME";
		public const string VOID_RIFT_SHOCK_DESC = "VOID_RIFT_SHOCK_DESC";

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

		internal static void Init() {
			#region Name and Lore
			Bind(SURVIVOR_NAME, "Void Jailer");
			Bind(SURVIVOR_DESC, LinesToSurvivorDetails(
				"The Void Jailer specializes in short range, one-on-one combat. It has an exceptionally high burst damage output, but struggles with attack speed, making it vulnerable to swarms of enemies.",
				"<style=cIsVoid>Spike</style> is a shotgun-like ability that can deal devastating blows to larger targets. It does extra damage against rooted enemies, so consider picking up a <style=cIsVoid>Tentabauble</style> if you find one.",
				"<style=cIsVoid>Bind</style> roots and damages a single target, pulling them directly in front of you.",
				"<style=cIsVoid>Dive</style> is a powerful escape tool. Alongside healing you, it makes enemies lose track of you, and protects you from incoming damage.",
				"<style=cIsVoid>Fury</style> gives you armor and increases your damage output. It is useful against bosses. Be careful though, you aren't invincible, and reckless use can get you killed!"
			));
			Bind(SURVIVOR_LORE, THE_LORE);
			Bind(SURVIVOR_OUTRO, "..and so it left, intrigued to become what it once sought to capture");
			Bind(SURVIVOR_OUTRO_FAILED, "..and so it was detained, awaiting its sentence at the end of Time");
			#endregion

			#region Palettes
			Bind(DEFAULT_SKIN, "Default");
			Bind(GHOST_SKIN, "Friendly");
			#endregion

			#region Passive
			string voidBornIntro = "The Void Jailer inherits all of the benefits and drawbacks of its kin.";
			string desc = $"[ Reave ]\n<style=cSub>Upon death, <style=cIsVoid>Reave</style> is triggered, firing a small black hole that <style=cIsDamage>instantly kills</style> all monsters <style=cIsDamage>and players</style> caught within.";
			if (Configuration.VoidImmunity) {
				desc += "\n\n[ Void Entity ]\n<style=cSub>Grants <style=cIsUtility>immunity</style> to the Void's <style=cIsVoid>passive environmental damage</style> and <style=cIsVoid>fog</style>.</style>";
			}

			Bind(PASSIVE_NAME, "<style=cIsVoid>Void Entity</style>");
			Bind(PASSIVE_KEYWORD, desc);
			Bind(PASSIVE_DESC, voidBornIntro);
			#endregion

			#region Primary
			Bind(SKILL_PRIMARY_NAME, "Spike");
			Bind(SKILL_PRIMARY_DESC, $"Fire spikes from your claw, dealing <style=cIsDamage><style=cUserSetting>{Configuration.BasePrimaryProjectileCount}</style>x<style=cUserSetting>{Percentage(Configuration.BasePrimaryDamage)}</style> damage</style>, plus an additional <style=cIsDamage>75% damage</style> on targets that are <style=cIsVoid>Nullified</style> or <style=cIsVoid>Tethered</style>.");
			#endregion

			#region Secondary
			Bind(SKILL_SECONDARY_NAME, "Bind");
			Bind(SKILL_SECONDARY_DESC, $"Grab an enemy through the void, applying a force that tries to pull them directly in front of you. Deals <style=cIsDamage><style=cUserSetting>{Percentage(Configuration.BaseSecondaryDamage)}</style> damage</style> and inflicts <style=cIsVoid>Nullify</style>.");
			#endregion

			#region Utility
			Bind(SKILL_UTILITY_NAME, "Dive");
			Bind(SKILL_UTILITY_DESC, $"Propel yourself through the void at <style=cUserSetting>{Percentage(Configuration.UtilitySpeed)} movement speed</style> for <style=cUserSetting>{RoundTen(Configuration.UtilityDuration)} {LazyPluralize("second", Configuration.UtilityDuration)}</style>, healing <style=cIsHealing>{Percentage(Configuration.UtilityRegeneration)} maximum health</style>. Gain <style=cIsUtility>Immunity</style> and <style=cIsUtility>Invisibility</style> while away.");
			#endregion

			string commonSpecialStatDesc = $"Gain <style=cIsUtility><style=cUserSetting>{RoundTen(Configuration.SpecialArmorBoost)}</style> Armor</style>. <style=cIsVoid>Spike</style> projectiles cause <style=cIsVoid>Instant Collapse</style>, causing them to do a total of <style=cIsDamage><style=cUserSetting>{Configuration.BasePrimaryProjectileCount}</style>x<style=cUserSetting>{Percentage(Configuration.SpecialDamageBoost * Configuration.BasePrimaryDamage)}</style> damage</style>.";

			#region Special
			Bind(SKILL_SPECIAL_NAME, "Fury of the Warden");
			Bind(SKILL_SPECIAL_DESC, $"Become enraged, gaining <style=cIsDamage>Fury</style> for <style=cUserSetting>{RoundTen(Configuration.SpecialDuration)}</style> seconds.");
			Bind(SKILL_SPECIAL_KEYWORD, $"[ Fury ]\n{commonSpecialStatDesc}");
			#endregion

			#region Buffs
			Bind(BUFF_PLAYERVOIDJAILER_FURY_NAME, "Fury");
			Bind(BUFF_PLAYERVOIDJAILER_FURY_DESC, commonSpecialStatDesc);
			#endregion

			Log.LogTrace("Localization initialized.");
		}

		// TO SELF: The idea was that Void figured out survivors killed Providence, they want to take Mithrix, suspect the survivors are strong enough to do that too and so they try to work with the players
		private const string THE_LORE = @"I'm sorry Gordon, you'll just have to wait until after the test. Then I can spend time writing a story.";

	}
}

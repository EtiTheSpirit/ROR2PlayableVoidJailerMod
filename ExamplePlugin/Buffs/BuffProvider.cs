using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VoidJailerMod.Buffs.Interop;

namespace VoidJailerMod.Buffs {
	public static class BuffProvider {

		public static BuffDef Fury { get; private set; }

		internal static void Init() {

			BetterUIInteroperability.Init();

			Log.LogTrace("Creating \"Fury\" effect.");
			Fury = ScriptableObject.CreateInstance<BuffDef>();
			Fury.isDebuff = false;
			Fury.buffColor = new Color32(255, 127, 127, 255);
			Fury.canStack = false;
			ContentAddition.AddBuffDef(Fury);

			On.RoR2.CharacterBody.RecalculateStats += OnRecalculateStats;
			BetterUIInteroperability.RegisterBuffInfo(Fury, Localization.BUFF_PLAYERVOIDJAILER_FURY_NAME, Localization.BUFF_PLAYERVOIDJAILER_FURY_DESC);
			Log.LogTrace("Buffs initialized.");
		}

		private static void OnRecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats originalMethod, CharacterBody @this) {
			originalMethod(@this);
			if (@this.HasBuff(Fury)) {
				@this.armor += Configuration.SpecialArmorBoost;
			}
		}
	}
}

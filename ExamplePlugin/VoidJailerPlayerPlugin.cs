using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VoidJailerMod.Buffs;
using VoidJailerMod.Damage;
using VoidJailerMod.Effects;
using VoidJailerMod.Survivor;

namespace VoidJailerMod {
	[BepInDependency(R2API.R2API.PluginGUID)]
	[BepInDependency("Xan.HPBarAPI")]
	[BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
	[R2APISubmoduleDependency(nameof(ItemAPI), nameof(LanguageAPI), nameof(LoadoutAPI), nameof(PrefabAPI))]
	[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
	public class VoidJailerPlayerPlugin : BaseUnityPlugin {
		public const string PLUGIN_GUID = PLUGIN_AUTHOR + "." + PLUGIN_NAME;
		public const string PLUGIN_AUTHOR = "Xan";
		public const string PLUGIN_NAME = "VoidJailerPlayerCharacter";
		public const string PLUGIN_VERSION = "1.2.2";
		
		public void Awake() {
			Log.Init(Logger);
			Configuration.Init(Config);
			Localization.Init();
			BuffProvider.Init();
			DamageTypeProvider.Init();
			ProjectileProvider.Init();
			EffectProvider.Init();
			VoidJailerSurvivor.Init(this);
			Log.LogTrace("Mod initialization cycle complete.");
		}

	}
}

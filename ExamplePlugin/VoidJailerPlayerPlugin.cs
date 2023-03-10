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
using VRAPI;

namespace VoidJailerMod {
	[BepInDependency(R2API.R2API.PluginGUID)]
	[BepInDependency("Xan.HPBarAPI")]
	[BepInDependency("com.DrBibop.VRAPI")]
	[BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
	[R2APISubmoduleDependency(nameof(ItemAPI), nameof(LanguageAPI), nameof(LoadoutAPI), nameof(PrefabAPI))]
	[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
	public class VoidJailerPlayerPlugin : BaseUnityPlugin {
		public const string PLUGIN_GUID = PLUGIN_AUTHOR + "." + PLUGIN_NAME;
		public const string PLUGIN_AUTHOR = "Xan";
		public const string PLUGIN_NAME = "VoidJailerPlayerCharacter";
		public const string PLUGIN_VERSION = "1.3.0";

		public static bool IsVR => VR.enabled && MotionControls.enabled;
		
		public void Awake() {
			Log.Init(Logger);
			Configuration.Init(Config);
			Localization.Init();
			BuffProvider.Init();
			DamageTypeProvider.Init();
			ProjectileProvider.Init();
			EffectProvider.Init();
			VoidJailerSurvivor.Init(this);
			RoR2Application.onLoad += LoadCustomHands;
			Log.LogTrace("Mod initialization cycle complete.");
		}

		private void LoadCustomHands() {
			if (IsVR) {
				AssetBundle jailerHands = AssetBundle.LoadFromMemory(Properties.Resources.voidjailerhandsbundle);
				if (jailerHands) {
					GameObject claw = jailerHands.LoadAsset<GameObject>("JailerClawHand");
					GameObject squid = jailerHands.LoadAsset<GameObject>("JailerSquidHand");
					if (claw && squid) {
						MotionControls.AddHandPrefab(claw);
						MotionControls.AddHandPrefab(squid);
					}
				}
			}
		}

	}
}

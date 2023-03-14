#pragma warning disable Publicizer001
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace VoidJailerMod.Effects {
	public static class EffectProvider {
#if !USE_VOID_CHARACTER_API
		/// <summary>
		/// A variation of the crit goggles kill effect that does not emit a sound. This is not immediately set and may be null if referenced in a mod's init cycle.
		/// </summary>
		public static GameObject SilentVoidCritDeathEffect { get; private set; }
#endif
		/// <summary>
		/// The crunchy explode effect on Collapse.
		/// </summary>
		public static GameObject CollapseExplode { get; private set; }


		public static GameObject SpawnEffect { get; private set; }

		/// <summary>
		/// The muzzle flash for the primary attack.
		/// </summary>
		public static GameObject SpikeMuzzleFlash { get; private set; }


		internal static void Init() {
#if !USE_VOID_CHARACTER_API
			On.RoR2.HealthComponent.AssetReferences.Resolve += InterceptHealthCmpAssetReferences;
#endif
			Log.LogTrace("Creating Collapse explode effect...");
			CollapseExplode = CreateNetworkedCloneFromPath("RoR2/DLC1/BleedOnHitVoid/FractureImpactEffect.prefab", "FastFractureImpactEffect");

			Log.LogTrace("Creating muzzle flash effect for Spike...");
			SpikeMuzzleFlash = CreateNetworkedCloneFromPath("RoR2/DLC1/VoidJailer/VoidJailerDartMuzzleFlash.prefab", "VoidJailerSurvivorDartMuzzleFlash");

			Log.LogTrace("Creating spawn effect...");
			SpawnEffect = CreateNetworkedCloneFromPath("RoR2/DLC1/VoidJailer/VoidJailerSpawnEffect.prefab", "VoidJailerSpawn");
			/*
			Log.LogTrace("Creating tether charge effect...");
			ChargeTetherEffect = CreateNetworkedCloneFromPath("RoR2/DLC1/VoidJailer/VoidJailerCaptureCharge.prefab", "VoidJailerTetherCharge");

			Log.LogTrace("Creating tether attack indicator effect...");
			TetherAttackIndicatorEffect = CreateNetworkedCloneFromPath("RoR2/DLC1/VoidJailer/VoidJailerCaptureAttackIndicator.prefab", "VoidJailerTetherIndicator");
			*/
			Log.LogTrace("Effect init complete.");
		}

		private static GameObject CreateNetworkedCloneFromPath(string path, string newName) {
			Log.LogTrace($"Duplicating {path} as {newName}...");
			GameObject o = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(path).WaitForCompletion(), newName);
			Log.LogTrace("Adding a network identity...");
			o.AddComponent<NetworkIdentity>();
			Log.LogTrace("Registering...");
			ContentAddition.AddEffect(o);
			Log.LogTrace("Done.");
			return o;
		}

#if !USE_VOID_CHARACTER_API
		private static void InterceptHealthCmpAssetReferences(On.RoR2.HealthComponent.AssetReferences.orig_Resolve originalMethod) {
			originalMethod();
			SilentVoidCritDeathEffect = PrefabAPI.InstantiateClone(HealthComponent.AssetReferences.critGlassesVoidExecuteEffectPrefab, "SilentVoidCritDeathJailer");
			SilentVoidCritDeathEffect.AddComponent<NetworkIdentity>();
			EffectComponent fx = SilentVoidCritDeathEffect.GetComponentInChildren<EffectComponent>();
			fx.soundName = null;
			ContentAddition.AddEffect(SilentVoidCritDeathEffect);
			On.RoR2.HealthComponent.AssetReferences.Resolve -= InterceptHealthCmpAssetReferences; // Clean up!
			Log.LogTrace("Instantiated prefab for silent void crit death effect.");
		}
#endif

	}
}

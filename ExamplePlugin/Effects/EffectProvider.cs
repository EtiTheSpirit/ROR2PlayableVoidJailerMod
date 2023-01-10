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

		/// <summary>
		/// A variation of the crit goggles kill effect that does not emit a sound. This is not immediately set and may be null if referenced in a mod's init cycle.
		/// </summary>
		public static GameObject SilentVoidCritDeathEffect { get; private set; }

		public static GameObject CollapseExplode { get; private set; }

		internal static void Init() {
			On.RoR2.HealthComponent.AssetReferences.Resolve += InterceptHealthCmpAssetReferences;
			CollapseExplode = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/BleedOnHitVoid/FractureImpactEffect.prefab").WaitForCompletion(), "FastFractureImpactEffect");
			ContentAddition.AddEffect(CollapseExplode);
		}

		private static void InterceptHealthCmpAssetReferences(On.RoR2.HealthComponent.AssetReferences.orig_Resolve originalMethod) {
			originalMethod();

			SilentVoidCritDeathEffect = PrefabAPI.InstantiateClone(HealthComponent.AssetReferences.critGlassesVoidExecuteEffectPrefab, "SilentVoidCritDeath");
			SilentVoidCritDeathEffect.AddComponent<NetworkIdentity>();
			EffectComponent fx = SilentVoidCritDeathEffect.GetComponentInChildren<EffectComponent>();
			fx.soundName = null;
			ContentAddition.AddEffect(SilentVoidCritDeathEffect);
			On.RoR2.HealthComponent.AssetReferences.Resolve -= InterceptHealthCmpAssetReferences; // Clean up!
			Log.LogTrace("Instantiated prefab for silent void crit death effect.");
		}

	}
}

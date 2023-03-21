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
		/// The crunchy explode effect on Collapse.
		/// </summary>
		public static GameObject CollapseExplode { get; private set; }

		/// <summary>
		/// The spawn effect from when the jailer teleports in.
		/// </summary>
		public static GameObject SpawnEffect { get; private set; }

		/// <summary>
		/// The muzzle flash for the primary attack.
		/// </summary>
		public static GameObject SpikeMuzzleFlash { get; private set; }


		internal static void Init() {
			Log.LogTrace("Creating Collapse explode effect...");
			CollapseExplode = CreateNetworkedCloneFromPath("RoR2/DLC1/BleedOnHitVoid/FractureImpactEffect.prefab", "FastFractureImpactEffect");

			Log.LogTrace("Creating muzzle flash effect for Spike...");
			SpikeMuzzleFlash = CreateNetworkedCloneFromPath("RoR2/DLC1/VoidJailer/VoidJailerDartMuzzleFlash.prefab", "VoidJailerSurvivorDartMuzzleFlash");

			Log.LogTrace("Creating spawn effect...");
			SpawnEffect = CreateNetworkedCloneFromPath("RoR2/DLC1/VoidJailer/VoidJailerSpawnEffect.prefab", "VoidJailerSpawnEffectResizable");
			EffectComponent effect = SpawnEffect.GetComponent<EffectComponent>();
			effect.applyScale = true;
			effect.disregardZScale = false;

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

	}
}

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine;
using R2API;
using RoR2.Projectile;
using RoR2;
using UnityEngine.Networking;
using static R2API.DamageAPI;

namespace VoidJailerMod.Damage {
	public static class ProjectileProvider {

		public static GameObject SpikeDart { get; private set; }

		public static GameObject ExplosiveSpikeDart { get; private set; }

		public static GameObject ExaggeratedDeathBombProjectile { get; private set; }

		/// <summary>
		/// A helper value to return the maximum distance of the projectile.
		/// </summary>
		public static float SpikeMaxDistance { get; private set; }

		internal static void Init() {
			Log.LogTrace("Creating Dart projectile for \"Spike\" primary ability...");
			SpikeDart = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidJailer/VoidJailerDart.prefab").WaitForCompletion(), "PlayerVoidJailerDart");
			SpikeDart.AddComponent<NetworkIdentity>();

			Log.LogTrace("Adjusting parameters of Spike Dart...");
			float rotationSpeedIfUsed = 22.5f;
			ProjectileSimple projectile = SpikeDart.GetComponent<ProjectileSimple>();
			if (Configuration.FasterPrimaryProjectiles) {
				Log.LogTrace("User wants to use faster projectiles. Doubling the speed and halving the lifetime...");
				rotationSpeedIfUsed *= 2f;
				projectile.desiredForwardSpeed *= 2f;
				projectile.lifetime /= 2f;
			}
			if (Configuration.HomingPrimaryProjectiles) {
				Log.LogTrace("User wants slight aim assist. Adding steering...");
				ProjectileSteerTowardTarget autoAimer = SpikeDart.AddComponent<ProjectileSteerTowardTarget>();
				autoAimer.rotationSpeed = rotationSpeedIfUsed;
			}

			Log.LogTrace("Adding NullBoosted damage type to dart...");
			DamageAPI.ModdedDamageTypeHolderComponent cmp = SpikeDart.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
			cmp.Add(DamageTypeProvider.NullBoosted);


			Log.LogTrace("Creating Fury Dart Extension (this type is fired with the Fury status effect)...");
			ExplosiveSpikeDart = PrefabAPI.InstantiateClone(SpikeDart, "PlayerVoidJailerExplosiveDart");
			ExplosiveSpikeDart.AddComponent<NetworkIdentity>();
			ExplosiveSpikeDart.GetComponent<DamageAPI.ModdedDamageTypeHolderComponent>().Add(DamageTypeProvider.PerformsFastFracture);


			Log.LogTrace("Creating death bomb projectile...");
			ExaggeratedDeathBombProjectile = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidJailer/VoidJailerDeathBombProjectile.prefab").WaitForCompletion(), "ExaggeratedVoidJailerDeathBomb");
			ModdedDamageTypeHolderComponent holder = ExaggeratedDeathBombProjectile.AddComponent<ModdedDamageTypeHolderComponent>();
			holder.Add(DamageTypeProvider.PerformsFakeVoidDeath);

			Log.LogTrace("Registering projectiles...");
			ContentAddition.AddProjectile(SpikeDart);
			ContentAddition.AddProjectile(ExplosiveSpikeDart);
			ContentAddition.AddProjectile(ExaggeratedDeathBombProjectile);
			SpikeMaxDistance = projectile.desiredForwardSpeed * projectile.lifetime;

			Log.LogTrace("Projectile init complete.");
		}
	}
}

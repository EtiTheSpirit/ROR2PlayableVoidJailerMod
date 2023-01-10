using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine;
using R2API;
using RoR2.Projectile;
using RoR2;

namespace VoidJailerMod.Damage {
	public static class ProjectileProvider {

		public static GameObject SpikeDart { get; private set; }

		public static GameObject ExplosiveSpikeDart { get; private set; }

		/// <summary>
		/// A helper value to return the maximum distance of the projectile.
		/// </summary>
		public static float SpikeMaxDistance { get; private set; }

		internal static void Init() {
			SpikeDart = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidJailer/VoidJailerDart.prefab").WaitForCompletion(), "PlayerVoidJailerDart");
			GameObject dartImpact = PrefabAPI.InstantiateClone(SpikeDart.GetComponent<ProjectileSingleTargetImpact>().impactEffect, "PlayerVoidJailerDartExplosionFX");
			dartImpact.transform.localScale *= 2.8f;

			float rotationSpeedIfUsed = 20f;
			ProjectileSimple projectile = SpikeDart.GetComponent<ProjectileSimple>();
			if (Configuration.FasterPrimaryProjectiles) {
				rotationSpeedIfUsed *= 2f;
				projectile.desiredForwardSpeed *= 2f;
				projectile.lifetime /= 2f;
			}
			if (Configuration.HomingPrimaryProjectiles) {
				ProjectileSteerTowardTarget autoAimer = SpikeDart.AddComponent<ProjectileSteerTowardTarget>();
				autoAimer.rotationSpeed = rotationSpeedIfUsed;
			}
			DamageAPI.ModdedDamageTypeHolderComponent cmp = SpikeDart.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
			cmp.Add(DamageTypeProvider.NullBoosted);


			ExplosiveSpikeDart = PrefabAPI.InstantiateClone(SpikeDart, "PlayerVoidJailerExplosiveDart");
			ExplosiveSpikeDart.GetComponent<DamageAPI.ModdedDamageTypeHolderComponent>().Add(DamageTypeProvider.PerformsFastFracture);

			ContentAddition.AddProjectile(SpikeDart);
			ContentAddition.AddProjectile(ExplosiveSpikeDart);
			SpikeMaxDistance = projectile.desiredForwardSpeed * projectile.lifetime;
		}
	}
}

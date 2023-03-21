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
using VoidJailerMod.Initialization;
using Xan.ROR2VoidPlayerCharacterCommon;
using Xan.ROR2VoidPlayerCharacterCommon.ProjectileHelper;
using Xan.ROR2VoidPlayerCharacterCommon.DamageBehavior;

namespace VoidJailerMod.Damage {
	public static class ProjectileProvider {

		public static GameObject SpikeDart { get; private set; }

		public static GameObject ExplosiveSpikeDart { get; private set; }

		public static GameObject CavitationMortarInstakill { get; private set; }

		public static GameObject CavitationMortarNoInstakill { get; private set; }

		/// <summary>
		/// A helper value to return the maximum distance of a Void Dart.
		/// </summary>
		public static float SpikeMaxDistance => SpikeBaseSpeed * SpikeBaseLifetime;

		/// <summary>
		/// The base lifetime of a Void Dart
		/// </summary>
		public static float SpikeBaseLifetime { get; private set; }

		/// <summary>
		/// The base speed of a Void Dart
		/// </summary>
		public static float SpikeBaseSpeed { get; private set; }

		internal static void Init() {
			Log.LogTrace("Creating Dart projectile for \"Spike\" primary ability...");
			SpikeDart = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidJailer/VoidJailerDart.prefab").WaitForCompletion(), "PlayerVoidJailerDart");
			SpikeDart.AddComponent<NetworkIdentity>();

			Log.LogTrace("Storing parameters of Spike Dart...");
			ProjectileSimple projectile = SpikeDart.GetComponent<ProjectileSimple>();
			SpikeBaseLifetime = projectile.lifetime;
			SpikeBaseSpeed = projectile.desiredForwardSpeed;

			Log.LogTrace("Adding NullBoosted damage type to dart...");
			ModdedDamageTypeHolderComponent cmp = SpikeDart.AddComponent<ModdedDamageTypeHolderComponent>();
			cmp.Add(DamageTypeProvider.NullBoosted);

			Log.LogTrace("Creating Fury Dart Extension (this type is fired with the Fury status effect)...");
			ExplosiveSpikeDart = PrefabAPI.InstantiateClone(SpikeDart, "PlayerVoidJailerExplosiveDart");
			ExplosiveSpikeDart.AddComponent<NetworkIdentity>();
			ExplosiveSpikeDart.GetComponent<ModdedDamageTypeHolderComponent>().Add(DamageTypeProvider.PerformsFastFracture);

			CavitationMortarInstakill = PrefabAPI.InstantiateClone(VoidImplosionObjects.DevastatorBomblet, "JailerCavitationMortarBomblet");
			CavitationMortarInstakill.GetComponent<Rigidbody>().useGravity = false;
			CavitationMortarInstakill.GetComponent<ProjectileImpactExplosion>().blastRadius = 6;
			CavitationMortarInstakill.AddComponent<FakeGravity>();
			CavitationMortarInstakill.AddComponent<CavitationMortarIdentifierTag>();
			CavitationMortarInstakill.GetComponent<ModdedDamageTypeHolderComponent>().Add(VoidDamageTypes.ExaggeratedVoidDeathRequiresNoise);
			CavitationMortarNoInstakill = PrefabAPI.InstantiateClone(VoidImplosionObjects.NoInstakillDevastatorBomblet, "JailerCavitationMortarBombletNoInstakill");
			CavitationMortarNoInstakill.GetComponent<Rigidbody>().useGravity = false;
			CavitationMortarNoInstakill.GetComponent<ProjectileImpactExplosion>().blastRadius = 6;
			CavitationMortarNoInstakill.AddComponent<FakeGravity>();
			CavitationMortarNoInstakill.AddComponent<CavitationMortarIdentifierTag>();
			CavitationMortarNoInstakill.GetComponent<ModdedDamageTypeHolderComponent>().Add(VoidDamageTypes.ExaggeratedVoidDeathRequiresNoise);

			On.RoR2.Projectile.ProjectileController.DispatchOnInitialized += OnProjectileDispatched;

			Log.LogTrace("Registering projectiles...");
			ContentAddition.AddProjectile(SpikeDart);
			ContentAddition.AddProjectile(ExplosiveSpikeDart);
			ContentAddition.AddProjectile(CavitationMortarInstakill);
			ContentAddition.AddProjectile(CavitationMortarNoInstakill);

			Log.LogTrace("Mutating prefabs...");
			UpdateProjectilePrefabs();

			Log.LogTrace("Projectile init complete.");
		}

		private static void OnProjectileDispatched(On.RoR2.Projectile.ProjectileController.orig_DispatchOnInitialized originalMethod, ProjectileController @this) {
			originalMethod(@this);
			if (@this.GetComponent<CavitationMortarIdentifierTag>() != null) {
				Log.LogTrace("Intercepted a projectile that needs to use gravity and blast radius configs.");
				FakeGravity fakeGravity = @this.GetComponent<FakeGravity>();
				fakeGravity.gravity = Configuration.MortarGravity;
				@this.GetComponent<ProjectileExplosion>().blastRadius = Configuration.MortarBlastRadius;
			}
		}

		internal static void UpdateProjectilePrefabs() {
			Log.LogTrace("Configuration changed! Mutating prefabs...");
			bool faster = Configuration.FasterPrimaryProjectiles;
			bool homing = Configuration.HomingPrimaryProjectiles;
			UpdateProjectilePrefab(SpikeDart, faster, homing);
			UpdateProjectilePrefab(ExplosiveSpikeDart, faster, homing);
		}

		internal static void UpdateProjectilePrefab(GameObject prefab, bool faster, bool homing) {
			ProjectileSimple projectile = prefab.GetComponent<ProjectileSimple>();
			projectile.desiredForwardSpeed = SpikeBaseSpeed * (faster ? 2f : 1f);
			projectile.lifetime = SpikeBaseLifetime * (faster ? 0.5f : 1f);

			if (homing) {
				ProjectileSteerTowardTarget autoAimer = prefab.GetComponent<ProjectileSteerTowardTarget>();
				if (!autoAimer) autoAimer = prefab.AddComponent<ProjectileSteerTowardTarget>();
				autoAimer.rotationSpeed = 22.5f * (faster ? 2f : 1f);
			} else {
				ProjectileSteerTowardTarget autoAimer = prefab.GetComponent<ProjectileSteerTowardTarget>();
				if (autoAimer) UnityEngine.Object.DestroyImmediate(autoAimer);
			}
		}

		private class CavitationMortarIdentifierTag : MonoBehaviour { }
	}
}

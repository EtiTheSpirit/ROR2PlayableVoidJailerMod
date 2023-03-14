using EntityStates;
using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using VoidJailerMod.Buffs;
using VoidJailerMod.Damage;
using VoidJailerMod.Effects;
using VoidJailerMod.XansTools;

namespace VoidJailerMod.Skills.Spike {
	public class SpikeShotgunFireSequence : GenericProjectileBaseState, VRInterop.IAimRayProvider {

		public static float GetMaxShotgunAdjustmentAngle() {
			if (VRInterop.DoVRAimCompensation) {
				return 7f;
			}
			return 2.5f;
		}

		public Ray PublicAimRay => GetAimRay();


		public SpikeShotgunFireSequence() {
			// projectilePrefab = ProjectileProvider.SpikeDart;
			effectPrefab = EffectProvider.SpikeMuzzleFlash;
			attackSoundString = "Play_voidJailer_m1_shoot";
			damageCoefficient = Configuration.BasePrimaryDamage;
			bloom = 1;
			minSpread = 0;
			maxSpread = 3;
			recoilAmplitude = 1;
			force = 400f;
			duration = 1f;
		}

		public override void FireProjectile() {
			if (isAuthority) {
				Ray aimRay = ModifyProjectileAimRay(VRInterop.GetDominantHandRay(this));
				aimRay.direction = Util.ApplySpread(aimRay.direction, minSpread, maxSpread, 1f, 1f, 0f, projectilePitchBonus);
				GameObject target = null;
				Vector3 newDirection = aimRay.direction;
				if (Configuration.HomingPrimaryProjectiles) {
					BullseyeSearch bullseyeSearch = new BullseyeSearch {
						teamMaskFilter = TeamMask.allButNeutral,
						maxAngleFilter = GetMaxShotgunAdjustmentAngle(),
						minDistanceFilter = 0f,
						maxDistanceFilter = ProjectileProvider.SpikeMaxDistance,
						searchOrigin = aimRay.origin,
						searchDirection = aimRay.direction,
						sortMode = BullseyeSearch.SortMode.Angle,
						filterByLoS = true
					};
					bullseyeSearch.teamMaskFilter.RemoveTeam(TeamComponent.GetObjectTeam(gameObject));
					bullseyeSearch.RefreshCandidates();
					bullseyeSearch.FilterOutGameObject(gameObject);

					IEnumerable<HurtBox> results = bullseyeSearch.GetResults();
					results = results.SkipWhile(result => result && !FriendlyFireManager.ShouldSeekingProceed(result.healthComponent, TeamComponent.GetObjectTeam(gameObject)));
					HurtBox victim = results.FirstOrDefault();
					target = (victim && victim.healthComponent && victim.healthComponent.body) ? victim.healthComponent.body.gameObject : null;
					if (victim) {
						newDirection = (victim.transform.position - aimRay.origin).normalized;
					}
				}
				GameObject projectile;
				if (HasBuff(BuffProvider.Fury)) {
					projectile = ProjectileProvider.ExplosiveSpikeDart;
				} else {
					projectile = ProjectileProvider.SpikeDart;
				}
				FireProjectileInfo info = new FireProjectileInfo {
					projectilePrefab = projectile,
					position = aimRay.origin,
					rotation = Util.QuaternionSafeLookRotation(newDirection),
					owner = gameObject,
					damage = damageStat * damageCoefficient,
					force = force,
					crit = Util.CheckRoll(critStat, characterBody.master),
					damageColorIndex = DamageColorIndex.Void,
					target = target,
					useSpeedOverride = false
				};
				ProjectileManager.instance.FireProjectile(info);
			}
		}

		public override void OnEnter() {
			targetMuzzle = "ClawMuzzle";
			base.OnEnter();
			if (HasBuff(BuffProvider.Fury)) {
				damageCoefficient *= Configuration.SpecialDamageBoost;
			}
			if (Configuration.ScaleDamageNotSpeed) {
				duration = (1f / Configuration.BaseAttackSpeed);
				float boost = (attackSpeedStat - Configuration.BaseAttackSpeed) + 1f;
				if (boost >= 1) {
					damageCoefficient *= boost;
				}

				attackSpeedStat = Configuration.BaseAttackSpeed;
			}
			characterBody.SetAimTimer(duration + 3f);
			// int bullets = GetNumberOfBullets(characterBody);
			for (int i = 1; i < Configuration.BasePrimaryProjectileCount; i++) {
				FireProjectile();
			}
			priorityReductionDuration = BasePriorityReductionDuration / attackSpeedStat;
		}

		public override Ray ModifyProjectileAimRay(Ray aimRay) {
			aimRay.origin += UnityEngine.Random.insideUnitSphere * (MaxRandomDistance * 0.8f) / (Configuration.UseFullSizeCharacter ? 1f : 2f);
			return aimRay;
		}

		public override void PlayAnimation(float duration) {
			if (Configuration.ScaleDamageNotSpeed) {
				duration = (1f / Configuration.BaseAttackSpeed);
				GetModelAnimator().SetFloat(FireAnimationPlaybackRateName, duration);
			}
			PlayAnimation(FireAnimationLayerName, FireAnimationStateName, FireAnimationPlaybackRateName, duration);
		}

		public override InterruptPriority GetMinimumInterruptPriority() {
			return InterruptPriority.Frozen;
		}

		public static string FireAnimationLayerName => EntityStates.VoidJailer.Weapon.Fire.animationLayerName;

		public static string FireAnimationStateName => EntityStates.VoidJailer.Weapon.Fire.animationStateName;

		public static string FireAnimationPlaybackRateName => EntityStates.VoidJailer.Weapon.Fire.animationPlaybackRateName;

		public static float MaxRandomDistance => EntityStates.VoidJailer.Weapon.Fire.maxRandomDistance;

		public static float BasePriorityReductionDuration => EntityStates.VoidJailer.Weapon.Fire.basePriorityReductionDuration;

		private float priorityReductionDuration;

		// private Transform muzzleTransform;
	}
}

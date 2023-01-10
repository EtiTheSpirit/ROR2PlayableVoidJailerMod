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

namespace VoidJailerMod.Skills.Spike {
	public class SpikeCommonFireSequence : GenericProjectileBaseState {
		
		public SpikeCommonFireSequence() {
			// projectilePrefab = ProjectileProvider.SpikeDart;
			effectPrefab = EffectProvider.SpikeMuzzleFlash;
			attackSoundString = "Play_voidJailer_m1_shoot";
			damageCoefficient = Configuration.BasePrimaryDamage;
			bloom = 1;
			minSpread = 0;
			maxSpread = 3;
			recoilAmplitude = 1;
			force = 700f;
			duration = 1f;
		}

		public override void FireProjectile() {
			if (isAuthority) {
				Ray aimRay = ModifyProjectileAimRay(GetAimRay());
				aimRay.direction = Util.ApplySpread(aimRay.direction, minSpread, maxSpread, 1f, 1f, 0f, projectilePitchBonus);
				GameObject target = null;
				Vector3 newDirection = aimRay.direction;
				if (Configuration.HomingPrimaryProjectiles) {
					BullseyeSearch bullseyeSearch = new BullseyeSearch {
						teamMaskFilter = TeamMask.AllExcept(characterBody.teamComponent.teamIndex),
						maxAngleFilter = 2.5f,
						minDistanceFilter = 0f,
						maxDistanceFilter = ProjectileProvider.SpikeMaxDistance,
						searchOrigin = aimRay.origin,
						searchDirection = aimRay.direction,
						sortMode = BullseyeSearch.SortMode.Angle,
						filterByLoS = true
					};
					bullseyeSearch.RefreshCandidates();
					bullseyeSearch.FilterOutGameObject(gameObject);
					HurtBox firstVictimHurtBox = bullseyeSearch.GetResults().FirstOrDefault();
					target = (firstVictimHurtBox && firstVictimHurtBox.healthComponent && firstVictimHurtBox.healthComponent.body) ? firstVictimHurtBox.healthComponent.body.gameObject : null;
					if (firstVictimHurtBox) {
						newDirection = (firstVictimHurtBox.transform.position - aimRay.origin).normalized;
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
			characterBody.SetAimTimer(duration + 3f);
			// int bullets = GetNumberOfBullets(characterBody);
			for (int i = 1; i < Configuration.BasePrimaryProjectileCount; i++) {
				FireProjectile();
			}
			priorityReductionDuration = BasePriorityReductionDuration / attackSpeedStat;
		}

		public override Ray ModifyProjectileAimRay(Ray aimRay) {
			aimRay.origin += UnityEngine.Random.insideUnitSphere * (MaxRandomDistance * 0.8f);
			return aimRay;
		}

		public override void PlayAnimation(float duration) {
			PlayAnimation(FireAnimationLayerName, FireAnimationStateName, FireAnimationPlaybackRateName, duration);
		}

		public override InterruptPriority GetMinimumInterruptPriority() {
			if (fixedAge <= priorityReductionDuration) {
				return InterruptPriority.PrioritySkill;
			}
			return InterruptPriority.Skill;
		}

		// private new GameObject projectilePrefab => HasBuff(BuffProvider.Fury) ? ProjectileProvider.ExplosiveSpikeDart : ProjectileProvider.SpikeDart;

		public static string FireAnimationLayerName => EntityStates.VoidJailer.Weapon.Fire.animationLayerName;

		public static string FireAnimationStateName => EntityStates.VoidJailer.Weapon.Fire.animationStateName;

		public static string FireAnimationPlaybackRateName => EntityStates.VoidJailer.Weapon.Fire.animationPlaybackRateName;

		public static float MaxRandomDistance => EntityStates.VoidJailer.Weapon.Fire.maxRandomDistance;

		public static float BasePriorityReductionDuration => EntityStates.VoidJailer.Weapon.Fire.basePriorityReductionDuration;

		private float priorityReductionDuration;

		// private Transform muzzleTransform;
	}
}

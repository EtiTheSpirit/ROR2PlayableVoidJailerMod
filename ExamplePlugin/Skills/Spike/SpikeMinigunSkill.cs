using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using VoidJailerMod.Buffs;
using VoidJailerMod.Damage;
using VoidJailerMod.Effects;

namespace VoidJailerMod.Skills.Spike {
	public class SpikeMinigunSkill : GenericProjectileBaseState {

		public SpikeMinigunSkill() {
			// projectilePrefab = ProjectileProvider.SpikeDart;
			effectPrefab = EffectProvider.SpikeMuzzleFlash;
			attackSoundString = "Play_voidJailer_m1_shoot";
			damageCoefficient = Configuration.BasePrimaryDamage;
			bloom = 0.8f;
			minSpread = 0;
			maxSpread = 2;
			recoilAmplitude = 1;
			force = 300f;
			duration = float.PositiveInfinity;
		}

		public override void FireProjectile() {
			PlayAnimation(DelayBetweenShots * 3);
			if (isAuthority) {
				Ray aimRay = ModifyProjectileAimRay(GetAimRay());
				aimRay.direction = Util.ApplySpread(aimRay.direction, minSpread, maxSpread, 1f, 1f, 0f, projectilePitchBonus);
				GameObject target = null;
				Vector3 newDirection = aimRay.direction;
				if (Configuration.HomingPrimaryProjectiles) {
					BullseyeSearch bullseyeSearch = new BullseyeSearch {
						teamMaskFilter = TeamMask.allButNeutral,
						maxAngleFilter = 2.5f,
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

			// BaseState.OnEnter is as follows:
			if (characterBody) {
				attackSpeedStat = characterBody.attackSpeed;
				damageStat = characterBody.damage;
				critStat = characterBody.crit;
				moveSpeedStat = characterBody.moveSpeed;
			}
			// Done

			if (HasBuff(BuffProvider.Fury)) {
				damageCoefficient *= Configuration.SpecialDamageBoost;
			}
			remainingTime = 0;
			Transform modelTransform = GetModelTransform();
			if (modelTransform) {
				ChildLocator childLocator = modelTransform.GetComponent<ChildLocator>();
				if (childLocator) {
					Transform clawMuzzle = childLocator.FindChild("ClawMuzzle");
					if (clawMuzzle && chargeVfxPrefab) {
						_chargeVfxInstance = UnityEngine.Object.Instantiate(chargeVfxPrefab, clawMuzzle.position, clawMuzzle.rotation, clawMuzzle);
						ScaleParticleSystemDuration particleSystemDurationMgr = _chargeVfxInstance.GetComponent<ScaleParticleSystemDuration>();
						if (particleSystemDurationMgr) {
							particleSystemDurationMgr.newDuration = 0.5f;
						}
					}
				}
			}
		}

		public override Ray ModifyProjectileAimRay(Ray aimRay) {
			aimRay.origin += UnityEngine.Random.insideUnitSphere * (MaxRandomDistance * 0.15f) / (Configuration.UseFullSizeCharacter ? 1f : 2f);
			return aimRay;
		}

		public override void PlayAnimation(float duration) {
			PlayAnimation(FireAnimationLayerName, FireAnimationStateName, FireAnimationPlaybackRateName, duration);
		}

		public override void Update() {
			base.Update();
			if (_chargeVfxInstance) {
				Ray aimRay = GetAimRay();
				_chargeVfxInstance.transform.forward = aimRay.direction;
			}
		}

		public override void FixedUpdate() {
			fixedAge += Time.fixedDeltaTime;
			characterBody.SetAimTimer(3f);
			if (isAuthority) {
				remainingTime -= Time.fixedDeltaTime;
				if (remainingTime <= 0) {
					remainingTime = DelayBetweenShots;
					int numBullets = Mathf.CeilToInt(Configuration.BasePrimaryProjectileCount * BASE_BULLETS_PER_BURST_AS_FRAC);
					for (int i = 0; i < numBullets; i++) {
						FireProjectile();
					}
				}
				if (!inputBank.skill1.down || !characterBody.HasBuff(BuffProvider.Fury)) {
					outer.SetNextStateToMain();
				}
			}
		}

		public override void OnExit() {
			base.OnExit();
			if (_chargeVfxInstance) {
				Destroy(_chargeVfxInstance);
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority() {
			return InterruptPriority.PrioritySkill;
		}

		public static string FireAnimationLayerName => EntityStates.VoidJailer.Weapon.Fire.animationLayerName;

		public static string FireAnimationStateName => EntityStates.VoidJailer.Weapon.Fire.animationStateName;

		public static string FireAnimationPlaybackRateName => EntityStates.VoidJailer.Weapon.Fire.animationPlaybackRateName;

		public static float MaxRandomDistance => EntityStates.VoidJailer.Weapon.Fire.maxRandomDistance;

		public static float BasePriorityReductionDuration => EntityStates.VoidJailer.Weapon.Fire.basePriorityReductionDuration;

		public static string SpikeAttackSoundEffect => EntityStates.VoidJailer.Weapon.ChargeFire.attackSoundEffect;

		public static string SpikeAnimationLayerName => EntityStates.VoidJailer.Weapon.ChargeFire.animationLayerName;

		public static string SpikeAnimationStateName => EntityStates.VoidJailer.Weapon.ChargeFire.animationStateName;

		public static string SpikeAnimationPlaybackRateName => EntityStates.VoidJailer.Weapon.ChargeFire.animationPlaybackRateName;

		public static float SpikeBaseDuration => EntityStates.VoidJailer.Weapon.ChargeFire.baseDuration;

		public static GameObject chargeVfxPrefab;

		private GameObject _chargeVfxInstance;

		private float remainingTime;

		public float DelayBetweenShots => INV_BASE_BULLETS_PER_ATTACK_SPEED / attackSpeedStat;

		/// <summary>
		/// How many bullets are fired per unit of Attack Speed? Note that this is lower because the survivor's default attack speed is 1.45f
		/// </summary>
		public const float BASE_BULLETS_PER_ATTACK_SPEED = 2.5f;
		public const float INV_BASE_BULLETS_PER_ATTACK_SPEED = 1f / BASE_BULLETS_PER_ATTACK_SPEED;

		/// <summary>
		/// This is multiplied with the user's settings (see <see cref="Configuration.BasePrimaryProjectileCount"/>) to figure out how many shots to fire in the burst.
		/// </summary>
		public const float BASE_BULLETS_PER_BURST_AS_FRAC = 0.25f;

	}
}

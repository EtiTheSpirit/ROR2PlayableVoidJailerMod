using EntityStates;
using HG;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VoidJailerMod.Damage;
using VoidJailerMod.Initialization;
using Xan.ROR2VoidPlayerCharacterCommon;
using Xan.ROR2VoidPlayerCharacterCommon.VRMod;

namespace VoidJailerMod.Skills.Special {
	public class MortarSkill : BaseState, VRInterop.IAimRayProvider {

		private bool _fired = false;

		private static System.Random rng = new System.Random();

		public override void OnEnter() {
			base.OnEnter();
			StartAimMode(2f);
			Duration = BaseDuration / attackSpeedStat;
			PlayCrossfade(AnimationLayerName, AnimationStateName, AnimationPlaybackRateName, 1f / attackSpeedStat, 0.15f);
		}

		public void FireProjectile(bool perfectAccuracy) {
			if (isAuthority) {
				Ray aimRay = VRInterop.GetNonDominantHandRay(this);
				Vector2 spread = Configuration.MortarSpread;
				// string muzzleTransformName = EntityStates.VoidJailer.Weapon.ChargeCapture.muzzleString;

				if (!perfectAccuracy) aimRay.direction = Util.ApplySpread(aimRay.direction, spread.x, spread.y, 1, 0.25f);
				FireProjectileInfo shot = new FireProjectileInfo {
					projectilePrefab = Configuration.MortarCanInstakill ? ProjectileProvider.CavitationMortarInstakill : ProjectileProvider.CavitationMortarNoInstakill,
					owner = characterBody.gameObject,
					position = aimRay.origin,//GetModelChildLocator().FindChild(muzzleTransformName).transform.position,
					rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
					damage = Configuration.MortarDamage * damageStat,
					damageColorIndex = DamageColorIndex.Void,
					crit = Util.CheckRoll(critStat, characterBody.master),
					speedOverride = Configuration.MortarSpeed
				};
				ProjectileManager.instance.FireProjectile(shot);
			}
		}

		private static readonly uint[] audioIds = new uint[] {
			1007663829,
			420940519,
			509108194,
			873009051,
		};

		public override void FixedUpdate() {
			base.FixedUpdate();
			if (!_fired && isAuthority) {
				_fired = true;
				Util.PlayAttackSpeedSound(EnterSoundString, gameObject, attackSpeedStat);

				for (int i = 0; i < Configuration.MortarBombCount; i++) {
					FireProjectile(i == 0 && Configuration.FirstMortarIsAlwaysAccurate);
				}
			}
			if (fixedAge >= Duration) {
				outer.SetNextStateToMain();
			}
		}

		public override void OnExit() {
			base.OnExit();

			PlayAnimation(
				EntityStates.VoidJailer.Weapon.ExitCapture.animationLayerName,
				EntityStates.VoidJailer.Weapon.ExitCapture.animationStateName,
				EntityStates.VoidJailer.Weapon.ExitCapture.animationPlaybackRateName, 
				1f / attackSpeedStat
			);
		}

		public override InterruptPriority GetMinimumInterruptPriority() {
			return InterruptPriority.Frozen;
		}

		public string AnimationLayerName { get; } = "Gesture, Additive";

		public string AnimationStateName { get; } = "FireTentacle";

		public string AnimationPlaybackRateName { get; } = "Tentacle.playbackRate";

		public float Duration { get; set; }

		public float BaseDuration { get; } = 1;

		public string EnterSoundString { get; } = "Play_item_proc_laserTurbine_explode";//"Play_artifactBoss_attack1_shoot";

		public Ray PublicAimRay => GetAimRay();
	}
}

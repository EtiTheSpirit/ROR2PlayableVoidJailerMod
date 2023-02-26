using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VoidJailerMod.Damage;

namespace VoidJailerMod.Skills.Spike {
	public class SpikeShotgunSkill : BaseState {

		public override void OnEnter() {
			base.OnEnter();
			_totalDuration = SpikeBaseDuration / attackSpeedStat;
			_crossFadeDuration = _totalDuration * 0.25f;
			_chargingDuration = _totalDuration - _crossFadeDuration;
			Transform modelTransform = GetModelTransform();
			Util.PlayAttackSpeedSound(SpikeAttackSoundEffect, gameObject, attackSpeedStat);
			if (modelTransform) {
				ChildLocator childLocator = modelTransform.GetComponent<ChildLocator>();
				if (childLocator) {
					Transform clawMuzzle = childLocator.FindChild("ClawMuzzle");
					if (clawMuzzle && chargeVfxPrefab) {
						_chargeVfxInstance = UnityEngine.Object.Instantiate(chargeVfxPrefab, clawMuzzle.position, clawMuzzle.rotation, clawMuzzle);
						ScaleParticleSystemDuration particleSystemDurationMgr = _chargeVfxInstance.GetComponent<ScaleParticleSystemDuration>();
						if (particleSystemDurationMgr) {
							particleSystemDurationMgr.newDuration = _totalDuration;
						}
					}
				}
			}
			PlayCrossfade(SpikeAnimationLayerName, SpikeAnimationStateName, SpikeAnimationPlaybackRateName, _chargingDuration, _crossFadeDuration);
			characterBody.SetAimTimer(_totalDuration + 3f);
		}

		public override void Update() {
			base.Update();
			if (_chargeVfxInstance) {
				Ray aimRay = GetAimRay();
				_chargeVfxInstance.transform.forward = aimRay.direction;
			}
		}

		public override void FixedUpdate() {
			base.FixedUpdate();
			if (fixedAge >= _totalDuration && isAuthority) {
				Log.LogTrace($"Transitioning into a new instance of {nameof(SpikeShotgunFireSequence)}!");
				outer.SetNextState(new SpikeShotgunFireSequence());
				return;
			}
		}

		public override void OnExit() {
			base.OnExit();
			if (_chargeVfxInstance) {
				Destroy(_chargeVfxInstance);
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority() {
			return InterruptPriority.Skill;
		}

		public static string SpikeAttackSoundEffect => EntityStates.VoidJailer.Weapon.ChargeFire.attackSoundEffect;

		public static string SpikeAnimationLayerName => EntityStates.VoidJailer.Weapon.ChargeFire.animationLayerName;

		public static string SpikeAnimationStateName => EntityStates.VoidJailer.Weapon.ChargeFire.animationStateName;

		public static string SpikeAnimationPlaybackRateName => EntityStates.VoidJailer.Weapon.ChargeFire.animationPlaybackRateName;

		public static float SpikeBaseDuration => EntityStates.VoidJailer.Weapon.ChargeFire.baseDuration;

		public static GameObject chargeVfxPrefab;

		private float _totalDuration;

		private float _crossFadeDuration;

		private float _chargingDuration;

		private GameObject _chargeVfxInstance;

	}
}

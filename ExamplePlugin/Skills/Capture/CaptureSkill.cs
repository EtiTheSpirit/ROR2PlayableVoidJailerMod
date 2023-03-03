using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VoidJailerMod.Effects;
using VoidJailerMod.Skills.Spike;

namespace VoidJailerMod.Skills.Capture {
	public class CaptureSkill : BaseState {
		public override void OnEnter() {
			base.OnEnter();
			duration = 1;
			duration /= attackSpeedStat;
			_crossFadeDuration = duration * 0.25f;
			PlayCrossfade(AnimationLayerName, AnimationStateName, AnimationPlaybackRateName, duration, _crossFadeDuration);
			soundID = Util.PlayAttackSpeedSound(EnterSoundString, gameObject, attackSpeedStat);
			if (ChargeEffectPrefab) {
				EffectManager.SimpleMuzzleFlash(ChargeEffectPrefab, gameObject, MuzzleString, false);
			}
			if (AttackIndicatorPrefab) {
				Transform coreTransform = characterBody.coreTransform;
				if (coreTransform) {
					attackIndicatorInstance = UnityEngine.Object.Instantiate(AttackIndicatorPrefab, coreTransform);
				}
			}
		}

		public override void FixedUpdate() {
			base.FixedUpdate();
			if (isAuthority && fixedAge >= duration && isAuthority) {
				Log.LogTrace($"Transitioning into a new instance of {nameof(CaptureCommonPullSequence)}!");
				outer.SetNextState(new CaptureCommonPullSequence());
			}
		}

		public override void Update() {
			if (attackIndicatorInstance) {
				attackIndicatorInstance.transform.forward = GetAimRay().direction;
			}
			base.Update();
		}

		public override void OnExit() {
			AkSoundEngine.StopPlayingID(soundID);
			if (attackIndicatorInstance) {
				Destroy(attackIndicatorInstance);
			}
			base.OnExit();
		}

		public override InterruptPriority GetMinimumInterruptPriority() {
			return InterruptPriority.Frozen;
		}

		public static string AnimationLayerName => EntityStates.VoidJailer.Weapon.ChargeCapture.animationLayerName;

		public static string AnimationStateName => EntityStates.VoidJailer.Weapon.ChargeCapture.animationStateName;

		public static string AnimationPlaybackRateName => EntityStates.VoidJailer.Weapon.ChargeCapture.animationPlaybackRateName;

		public static float duration;

		public static string EnterSoundString => EntityStates.VoidJailer.Weapon.ChargeCapture.enterSoundString;

		public static GameObject ChargeEffectPrefab => EntityStates.VoidJailer.Weapon.ChargeCapture.chargeEffectPrefab;

		public static GameObject AttackIndicatorPrefab => EntityStates.VoidJailer.Weapon.ChargeCapture.attackIndicatorPrefab;

		public static string MuzzleString => EntityStates.VoidJailer.Weapon.ChargeCapture.muzzleString;

		private float _crossFadeDuration;

		private uint soundID;

		private GameObject attackIndicatorInstance;
	}
}

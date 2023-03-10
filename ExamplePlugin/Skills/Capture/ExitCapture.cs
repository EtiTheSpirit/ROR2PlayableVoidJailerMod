using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidJailerMod.Skills.Capture {
	public class ExitCapture : BaseState {
		public override void OnEnter() {
			base.OnEnter();
			duration = 0.2f / attackSpeedStat;
			Util.PlaySound(EnterSoundString, gameObject);
			PlayAnimation(AnimationLayerName, AnimationStateName, AnimationPlaybackRateName, duration);
		}

		public override void FixedUpdate() {
			base.FixedUpdate();
			if (isAuthority && fixedAge >= duration) {
				outer.SetNextStateToMain();
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority() {
			return InterruptPriority.Skill;
		}

		public static string AnimationLayerName => EntityStates.VoidJailer.Weapon.ExitCapture.animationLayerName;

		public static string AnimationStateName => EntityStates.VoidJailer.Weapon.ExitCapture.animationStateName;

		public static string AnimationPlaybackRateName => EntityStates.VoidJailer.Weapon.ExitCapture.animationPlaybackRateName;

		public float duration;

		public static string EnterSoundString => EntityStates.VoidJailer.Weapon.ExitCapture.enterSoundString;
	}
}

using EntityStates;
using UnityEngine.Networking;
using UnityEngine.PlayerLoop;
using VoidJailerMod.Buffs;
using VoidJailerMod.Initialization;

namespace VoidJailerMod.Skills.Special {
	public class FurySkill : BaseState {

		public override void OnEnter() {
			base.OnEnter();
			if (NetworkServer.active) {
				characterBody.AddTimedBuff(BuffProvider.Fury, Configuration.SpecialRageDuration);
			}

			// Below: No good!
			// This allows people to accidentally spend multiple stocks as it spends one every frame.
			// Give people buffer room, such as the game's minimum 0.5 second cooldown.
			/*
			if (isAuthority) {
				outer.SetNextStateToMain();
			}
			*/
		}

		public override void FixedUpdate() {
			base.FixedUpdate();
			if (fixedAge >= 0.5f && isAuthority) {
				outer.SetNextStateToMain();
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority() {
			return InterruptPriority.Death;
		}

	}
}

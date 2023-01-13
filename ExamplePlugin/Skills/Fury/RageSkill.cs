using EntityStates;
using UnityEngine.Networking;
using VoidJailerMod.Buffs;

namespace VoidJailerMod.Skills.Fury {
	public class FurySkill : BaseState {

		public override void OnEnter() {
			base.OnEnter();
			if (NetworkServer.active) {
				characterBody.AddTimedBuff(BuffProvider.Fury, Configuration.SpecialDuration);
			}
			if (isAuthority) {
				outer.SetNextStateToMain();
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority() {
			return InterruptPriority.Death;
		}

	}
}

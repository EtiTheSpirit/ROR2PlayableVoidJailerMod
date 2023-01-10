using EntityStates;
using VoidJailerMod.Buffs;

namespace VoidJailerMod.Skills.Fury {
	public class FurySkill : BaseState {

		public override void OnEnter() {
			base.OnEnter();
			if (isAuthority) {
				characterBody.AddTimedBuff(BuffProvider.Fury, Configuration.SpecialDuration);
			}
			outer.SetNextStateToMain();
		}

		public override InterruptPriority GetMinimumInterruptPriority() {
			return InterruptPriority.Death;
		}

	}
}

using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using static R2API.DamageAPI;

namespace VoidJailerMod.Damage {
	public static class DamageTypeProvider {

		public static ModdedDamageType NullBoosted { get; private set; }

		public static ModdedDamageType PerformsFakeVoidDeath { get; private set; }

		public static ModdedDamageType PerformsFastFracture { get; private set; }

		internal static void Init() {
			NullBoosted = ReserveDamageType();
			PerformsFakeVoidDeath = ReserveDamageType();
			PerformsFastFracture = ReserveDamageType();
		}

	}
}

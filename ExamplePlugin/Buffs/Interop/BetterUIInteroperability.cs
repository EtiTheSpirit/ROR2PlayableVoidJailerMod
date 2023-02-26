using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace VoidJailerMod.Buffs.Interop {
	public static class BetterUIInteroperability {

		private delegate void RegisterBuffInfoDelegate(BuffDef buff, string name, string description);

		private static RegisterBuffInfoDelegate RegisterBuffInfoMethod = null;

		internal static void Init() {
			Log.LogTrace("Trying to do shallow interoperability with BetterUI...");
			try {
				Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
				Type betterUIBuffs = null;

				for (int index = 0; index < asms.Length; index++) {
					try {
						Assembly asm = asms[index];
						betterUIBuffs = asm.GetTypes().FirstOrDefault(type => type.Namespace == "BetterUI" && type.Name == "Buffs");
						if (betterUIBuffs != null) break;
					} catch { } // Don't care about this one.
				}

				if (betterUIBuffs != null) {
					MethodInfo buffRegisterMtd = betterUIBuffs.GetMethod("RegisterBuffInfo", new Type[] { typeof(BuffDef), typeof(string), typeof(string) });
					if (buffRegisterMtd != null) {
						Log.LogTrace("Found the method for BetterUI.Buffs::RegisterBuffInfo");
						RegisterBuffInfoMethod = (RegisterBuffInfoDelegate)buffRegisterMtd.CreateDelegate(typeof(RegisterBuffInfoDelegate));
					}
				}
			} catch (Exception err) {
				Log.LogWarning("An error occurred while trying to find BetterUI via reflection. This is not a big problem (it just prevents the status effect from showing information when you hover over it).");
				Log.LogWarning(err.ToString());
			}
			if (RegisterBuffInfoMethod == null) {
				Log.LogTrace("Failed to find RegisterBuffInfo, either BetterUI is not installed or something else happened.");
			}
		}

		public static void RegisterBuffInfo(BuffDef def, string name, string description) {
			RegisterBuffInfoMethod?.Invoke(def, name, description);
		}

	}
}

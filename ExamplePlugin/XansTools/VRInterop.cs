using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VRAPI;

namespace VoidJailerMod.XansTools {
	public static class VRInterop {

		/// <summary>
		/// True if the VR mod is available and installed. VR API members <strong>must not</strong> be used if this is <see langword="false"/>.
		/// </summary>
		public static bool VRAvailable => VR.enabled && MotionControls.enabled;

		public static bool DoVRAimCompensation => VRAvailable && Configuration.VRExtendedAimCompensation;

		/// <summary>
		/// Returns true if, assuming this is called in the context of an effect that should not play in VR, the effect can show (either due to the player being remote, or due to VR being off)
		/// </summary>
		/// <returns></returns>
		public static bool CanShowNonVREffect(CharacterBody body) {
			if (!VRAvailable) return true; // Can show, no VR
			return !IsVRLocalPlayer(body); // Only show if it's *not* the local player in VR.
		}

		/// <summary>
		/// True if the given <see cref="CharacterBody"/> is associated with the client player, and this client is in VR.
		/// </summary>
		/// <param name="body"></param>
		/// <returns></returns>
		public static bool IsVRLocalPlayer(CharacterBody body) {
			if (!VRAvailable) return false;
			return body.IsUsingMotionControls(); // Checks IsVR()
		}

		public static Ray GetDominantHandRay(IAimRayProvider state) {
			if (!VRAvailable) return state.PublicAimRay;
			return MotionControls.dominantHand.aimRay;
		}

		public static Ray GetNonDominantHandRay(IAimRayProvider state) {
			if (!VRAvailable) return state.PublicAimRay;
			return MotionControls.nonDominantHand.aimRay;
		}


		public interface IAimRayProvider {
			
			public Ray PublicAimRay { get; }

		}
	}
}

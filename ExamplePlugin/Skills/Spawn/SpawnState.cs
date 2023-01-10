using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VoidJailerMod.Skills.Spawn {
	public class SpawnState : GenericCharacterSpawnState {
		public override void OnEnter() {
			base.OnEnter();
			PlaySpawnFX();
			GrantSpawnAnimationImmunity(3.5f);
		}

		private void PlaySpawnFX() {
			if (spawnFXPrefab != null && !string.IsNullOrEmpty(spawnFXTransformName)) {
				EffectManager.SimpleMuzzleFlash(spawnFXPrefab, gameObject, spawnFXTransformName, false);
			}
		}

		private void GrantSpawnAnimationImmunity(float duration) {
			if (isAuthority) characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, duration);
		}

		public static GameObject spawnFXPrefab => EntityStates.VoidJailer.SpawnState.spawnFXPrefab;

		public static string spawnFXTransformName => EntityStates.VoidJailer.SpawnState.spawnFXTransformName;
	}
}

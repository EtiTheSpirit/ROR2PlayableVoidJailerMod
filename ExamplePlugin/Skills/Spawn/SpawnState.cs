using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VoidJailerMod.Effects;

namespace VoidJailerMod.Skills.Spawn {
	public class SpawnState : GenericCharacterSpawnState {
		public override void OnEnter() {
			base.OnEnter();
			PlaySpawnFX();
			GrantSpawnAnimationImmunity(3.5f);
		}

		private void PlaySpawnFX() {
			if (!string.IsNullOrEmpty(spawnFXTransformName)) {
				EffectManager.SimpleMuzzleFlash(EffectProvider.SpawnEffect, gameObject, spawnFXTransformName, false);
			}
		}

		private void GrantSpawnAnimationImmunity(float duration) {
			if (isAuthority) characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, duration);
		}

		public static string spawnFXTransformName => EntityStates.VoidJailer.SpawnState.spawnFXTransformName;
	}
}

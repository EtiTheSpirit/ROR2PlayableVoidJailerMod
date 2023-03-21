using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using VoidJailerMod.Effects;

namespace VoidJailerMod.Skills.Spawn {
	public class SpawnState : EntityStates.VoidJailer.SpawnState {
		public override void OnEnter() {
			spawnSoundString = "Play_voidJailer_spawn";
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
			if (NetworkServer.active) {
				characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, duration);
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority() {
			return InterruptPriority.Death;
		}

		public static string spawnFXTransformName => EntityStates.VoidJailer.SpawnState.spawnFXTransformName;
	}
}

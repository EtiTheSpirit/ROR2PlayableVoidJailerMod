using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using EntityStates;
using R2API;
using static R2API.DamageAPI;
using VoidJailerMod.Damage;
using RoR2;
using Xan.ROR2VoidPlayerCharacterCommon.EntityStates;
using Xan.ROR2VoidPlayerCharacterCommon;

namespace VoidJailerMod.Skills.Death {
	public class DeathState : VoidDeathStateBase {

		public override void PlayDeathAnimation(float crossfadeDuration = 0.1f) {
			PlayCrossfade("Body", "Death", "Death.playbackRate", Duration, crossfadeDuration);
		}

		public override void OnEnter() {
			base.OnEnter();
			muzzleTransform = FindModelChild(MuzzleName);
			if (muzzleTransform && isAuthority) {
				FireProjectileInfo fireProjectileInfo = default;
				fireProjectileInfo.projectilePrefab = VoidImplosionObjects.JailerImplosion;
				fireProjectileInfo.position = muzzleTransform.position;
				fireProjectileInfo.rotation = Quaternion.LookRotation(characterDirection.forward, Vector3.up);
				fireProjectileInfo.owner = gameObject;
				fireProjectileInfo.damage = damageStat;
				fireProjectileInfo.crit = characterBody.RollCrit();
				ProjectileManager.instance.FireProjectile(fireProjectileInfo);
			}
		}

		public override void FixedUpdate() {
			base.FixedUpdate();
			if (fixedAge >= Duration) {
				DestroyModel();
				if (NetworkServer.active) {
					DestroyBodyAsapServer();
				}
			}
		}

		public override void OnExit() {
			DestroyModel();
			base.OnExit();
		}

		// Token: 0x040006FE RID: 1790
		public static float Duration => EntityStates.VoidJailer.DeathState.duration;

		// Token: 0x040006FF RID: 1791
		public static string MuzzleName => EntityStates.VoidJailer.DeathState.muzzleName;

		// Token: 0x04000700 RID: 1792
		private Transform muzzleTransform;
	}
}

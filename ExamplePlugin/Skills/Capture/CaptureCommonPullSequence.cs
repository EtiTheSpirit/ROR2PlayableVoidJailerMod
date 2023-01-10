using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using RoR2;
using System.Linq;
using VoidJailerMod.Skills.Spike;
using UnityEngine.AddressableAssets;

namespace VoidJailerMod.Skills.Capture {
	public class CaptureCommonPullSequence : BaseState {
		public override void OnEnter() {
			base.OnEnter();
			Duration = BaseDuration / attackSpeedStat;
			PlayAnimation(AnimationLayerName, AnimationStateName, AnimationPlaybackRateName, Duration);
			Util.PlaySound(EnterSoundString, gameObject);
			if (MuzzleflashEffectPrefab) {
				EffectManager.SimpleMuzzleFlash(MuzzleflashEffectPrefab, gameObject, MuzzleString, false);
			}
			Ray aimRay = GetAimRay();
			if (NetworkServer.active) {
				BullseyeSearch bullseyeSearch = new BullseyeSearch {
					teamMaskFilter = TeamMask.AllExcept(characterBody.teamComponent.teamIndex),
					maxAngleFilter = PullFieldOfView,
					minDistanceFilter = PullMinDistance,
					maxDistanceFilter = PullMaxDistance,
					searchOrigin = aimRay.origin,
					searchDirection = aimRay.direction,
					sortMode = BullseyeSearch.SortMode.Angle,
					filterByLoS = true
				};
				bullseyeSearch.RefreshCandidates();
				bullseyeSearch.FilterOutGameObject(gameObject);
				HurtBox firstVictimHurtBox = bullseyeSearch.GetResults().FirstOrDefault();
				if (firstVictimHurtBox) {
					Vector3 difference = firstVictimHurtBox.transform.position - aimRay.origin;
					float distance = difference.magnitude;
					Vector3 direction = difference / distance;
					float mass = 1f;
					CharacterBody body = firstVictimHurtBox.healthComponent.body;
					if (body.characterMotor) {
						mass = body.characterMotor.mass;
					} else {
						Rigidbody rb = firstVictimHurtBox.healthComponent.GetComponent<Rigidbody>();
						if (rb) {
							mass = rb.mass;
						}
					}
					if (RoR2Content.Buffs.Nullified) {
						body.AddTimedBuff(RoR2Content.Buffs.Nullified, DebuffDuration);
					}
					float speed = Trajectory.CalculateInitialYSpeedForHeight(Mathf.Abs(PullMinDistance - distance)) * Mathf.Sign(PullMinDistance - distance);
					direction *= speed;
					direction.y = PullLiftVelocity;
					DamageInfo damageInfo = new DamageInfo {
						attacker = gameObject,
						damage = damageStat * Configuration.BaseSecondaryDamage,
						damageColorIndex = DamageColorIndex.Void,
						position = firstVictimHurtBox.transform.position,
						procCoefficient = ProcCoefficient
					};
					firstVictimHurtBox.healthComponent.TakeDamageForce(direction * mass, true, true);
					firstVictimHurtBox.healthComponent.TakeDamage(damageInfo);
					GlobalEventManager.instance.OnHitEnemy(damageInfo, firstVictimHurtBox.healthComponent.gameObject);
					if (PullTracerPrefab) {
						Vector3 position = firstVictimHurtBox.transform.position;
						Vector3 start = characterBody.corePosition;
						Transform transform = FindModelChild(MuzzleString);
						if (transform) {
							start = transform.position;
						}
						EffectData effectData = new EffectData {
							origin = position,
							start = start
						};
						EffectManager.SpawnEffect(PullTracerPrefab, effectData, true);
					}
				}
			}
		}

		public override void FixedUpdate() {
			base.FixedUpdate();
			if (fixedAge > Duration && isAuthority) {
				Log.LogTrace($"Transitioning into a new instance of {nameof(ExitCapture)}!");
				outer.SetNextState(new ExitCapture());
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority() {
			return InterruptPriority.PrioritySkill;
		}

		public string AnimationLayerName { get; } = "Gesture, Additive";

		public string AnimationStateName { get; } = "FireTentacle";

		public string AnimationPlaybackRateName { get; } = "Tentacle.playbackRate";

		public float BaseDuration { get; } = 1;

		public string EnterSoundString { get; } = "Play_voidJailer_m2_shoot";

		public float PullFieldOfView { get; } = 10f;

		public float PullMinDistance { get; } = 1;

		public float PullMaxDistance { get; } = 50;

		public GameObject PullTracerPrefab { 
			get {
				if (_pullTracer == null) {
					_pullTracer = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidJailer/VoidJailerCaptureTracer.prefab").WaitForCompletion();
				}
				return _pullTracer;
			}
		}
		private static GameObject _pullTracer = null;

		public float PullLiftVelocity { get; } = 3;

		public float DebuffDuration { get; } = 6;

		public float ProcCoefficient { get; } = 0;

		public GameObject MuzzleflashEffectPrefab {
			get {
				if (_muzzleFlash == null) {
					_muzzleFlash = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidJailer/VoidJailerCaptureMuzzleflash.prefab").WaitForCompletion();
				}
				return _muzzleFlash;
			}
		}
		private static GameObject _muzzleFlash = null;

		public string MuzzleString { get; set; }

		// Token: 0x04000738 RID: 1848
		private float Duration { get; set; }
	}
}

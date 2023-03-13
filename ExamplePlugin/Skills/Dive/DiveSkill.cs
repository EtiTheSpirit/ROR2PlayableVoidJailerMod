using EntityStates;
using Mono.Security.Authenticode;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using RoR2;
using BlinkState = EntityStates.ImpMonster.BlinkState;
using VoidJailerMod.XansTools;

namespace VoidJailerMod.Skills.Dive {

	/// <summary>
	/// The "Dive" ability. This makes the player invisible, moves them in a predetermined direction based on either their movement or look direction if they are not moving.
	/// </summary>
	public class DiveSkill : BaseState {

		/// <summary>
		/// Keeps track of the amount of time the effect has gone on for, which is used to add the feel of acceleration to the movement.
		/// </summary>
		private float _currentTime;

		/// <summary>
		/// The direction that the player is moving in. This will never be zero in practice.
		/// </summary>
		private Vector3 _moveVector = Vector3.zero;

		/// <summary>
		/// The cached Y velocity of the player, which affects the direction of the ability.
		/// </summary>
		private float _yVelocity = 0f;

		private CharacterModel _characterModel;

		private HurtBoxGroup _hurtboxGroup;

		public override void OnEnter() {
			// Test: Start aim mode for a single frame and snap to that rotation before doing this ability.
			// Reason: When not moving, it falls back to the rotation of the character. A recent change made the rotation of the character
			// not necessarily match the direction that the player is attacking in, which may cause an unexpected move direction when using this utility.
			// By forcing the character to align with the camera just before activating this, they will now move in the direction of their camera, which
			// feels more natural to the average joe.
			StartAimMode(Time.fixedDeltaTime, true);

			// Bugfix: Holding down (but not releasing) RMB will still "fire" the secondary and put it on cooldown.
			if (inputBank.skill2.down) {
				skillLocator.secondary.stock++;
			}
			if (VRInterop.CanShowNonVREffect(characterBody)) {
				// If it's a remote player OR if it's the local player and the local player is not in VR, spawn the effect
				// Otherwise skip it because its blinding.
				EffectManager.SpawnEffect(GenericCharacterDeath.voidDeathEffect, new EffectData {
					origin = characterBody.corePosition,
					scale = characterBody.bestFitRadius
				}, false);
			}
			Util.PlayAttackSpeedSound(BlinkState.beginSoundString, gameObject, 0.6f);
			Util.PlayAttackSpeedSound(BlinkState.beginSoundString, gameObject, 0.7f);
			base.OnEnter();
			Transform modelTransform = GetModelTransform();
			if (modelTransform != null) {
				_characterModel = modelTransform.GetComponent<CharacterModel>();
				_hurtboxGroup = modelTransform.GetComponent<HurtBoxGroup>();
			}
			if (_characterModel != null) {
				_characterModel.invisibilityCount++;
			}
			if (_hurtboxGroup != null) {
				_hurtboxGroup.hurtBoxesDeactivatorCounter++;
			}
			if (NetworkServer.active) {
				characterBody.AddTimedBuff(RoR2Content.Buffs.Cloak, Configuration.UtilityDuration);
				characterBody.healthComponent.HealFraction(Configuration.UtilityRegeneration, default);
			}
			_moveVector = ((inputBank.moveVector == Vector3.zero) ? characterDirection.forward : inputBank.moveVector).normalized;
			_yVelocity = characterMotor.velocity.y;
		}

		public override void FixedUpdate() {
			base.FixedUpdate();
			_currentTime += Time.fixedDeltaTime;
			if (characterMotor != null && characterDirection != null) {
				characterMotor.velocity = Vector3.zero;
				characterMotor.rootMotion += _moveVector * (moveSpeedStat * Configuration.UtilitySpeed * Time.fixedDeltaTime) * Mathf.Sin(_currentTime / Configuration.UtilityDuration * 2.3561945f); // 135deg
				characterMotor.rootMotion += new Vector3(0f, _yVelocity * Time.fixedDeltaTime * Mathf.Cos(_currentTime / Configuration.UtilityDuration * 1.57079637f), 0f);
			}
			bool timeToExit = _currentTime >= Configuration.UtilityDuration && isAuthority;
			if (timeToExit) {
				outer.SetNextStateToMain();
			}
		}

		public override void OnExit() {
			if (VRInterop.CanShowNonVREffect(characterBody)) {
				// If it's a remote player OR if it's the local player and the local player is not in VR, spawn the effect
				// Otherwise skip it because its blinding.
				EffectManager.SpawnEffect(GenericCharacterDeath.voidDeathEffect, new EffectData {
					origin = characterBody.corePosition,
					scale = characterBody.bestFitRadius
				}, false);
			}
			Util.PlayAttackSpeedSound(BlinkState.endSoundString, gameObject, 1f);
			if (_characterModel != null) {
				_characterModel.invisibilityCount--;
			}
			if (_hurtboxGroup != null) {
				_hurtboxGroup.hurtBoxesDeactivatorCounter--;
			}
			base.OnExit();
		}

		public override InterruptPriority GetMinimumInterruptPriority() {
			return InterruptPriority.Frozen;
		}
	}

}

﻿using EntityStates;
using KinematicCharacterController;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.Skills;
using ROR2HPBarAPI.API;
using System;
using UnityEngine;
using UnityEngine.Networking;
using VoidJailerMod.Buffs;
using VoidJailerMod.Damage;
using VoidJailerMod.Effects;
using VoidJailerMod.Skills.Capture;
using VoidJailerMod.Skills.Dive;
using VoidJailerMod.Skills.Fury;
using VoidJailerMod.Skills.Spike;
using VoidJailerMod.Survivor.Interop;
using VoidJailerMod.XansTools;
using CharacterBody = RoR2.CharacterBody;
using Interactor = RoR2.Interactor;

namespace VoidJailerMod.Survivor {
	public class VoidJailerSurvivor {

		private static GameObject playerBodyPrefab;

		public static SurvivorDef Jailer { get; private set; }

		// TODO: Try to allow runtime edits by using one of the popular settings mods?
		// Why can't this be like MelonLoader where settings are just exposed out of the box
		public static void UpdateSettings(CharacterBody body) {
			Log.LogTrace("Updating body settings...");
			body.baseMaxHealth = Configuration.BaseMaxHealth;
			body.levelMaxHealth = Configuration.LevelMaxHealth;
			body.baseMaxShield = Configuration.BaseMaxShield;
			body.levelMaxShield = Configuration.LevelMaxShield;
			body.baseArmor = Configuration.BaseArmor;
			body.levelArmor = Configuration.LevelArmor;
			body.baseRegen = Configuration.BaseHPRegen;
			body.levelRegen = Configuration.LevelHPRegen;

			body.baseDamage = Configuration.BaseDamage;
			body.levelDamage = Configuration.LevelDamage;
			body.baseAttackSpeed = Configuration.BaseAttackSpeed;
			body.levelAttackSpeed = Configuration.LevelAttackSpeed;
			body.baseCrit = Configuration.BaseCritChance;
			body.levelCrit = Configuration.LevelCritChance;

			body.baseMoveSpeed = Configuration.BaseMoveSpeed;
			body.levelMoveSpeed = Configuration.LevelMoveSpeed;
			body.baseJumpCount = Configuration.BaseJumpCount;
			body.baseJumpPower = Configuration.BaseJumpPower;
			body.levelJumpPower = Configuration.LevelJumpPower;
			body.baseAcceleration = Configuration.BaseAcceleration;
		}

		internal static void Init(VoidJailerPlayerPlugin plugin) {
			Log.LogTrace("Initializing Survivor info!");

			playerBodyPrefab = ROR2ObjectCreator.CreateBody("VoidJailerSurvivor", "RoR2/DLC1/VoidJailer/VoidJailerBody.prefab");

			Log.LogTrace("Setting localPlayerAuthority=true on Jailer Body (this patches a bug)...");
			NetworkIdentity netID = playerBodyPrefab.GetComponent<NetworkIdentity>();
			netID.localPlayerAuthority = true;

			Log.LogTrace("Duplicating the body for display in the Survivor Selection screen...");
			GameObject playerBodyDisplayPrefab = PrefabAPI.InstantiateClone(playerBodyPrefab.GetComponent<ModelLocator>().modelBaseTransform.gameObject, "PlayerJailerBodyDisplay");
			netID = playerBodyDisplayPrefab.AddComponent<NetworkIdentity>();
			netID.localPlayerAuthority = true;

			#region Body / Stats
			Log.LogTrace("Preparing the Body's style and stats...");
			CharacterBody body = playerBodyPrefab.GetComponent<CharacterBody>();
			UpdateSettings(body);
			body.bodyColor = new Color(0.867f, 0.468f, 0.776f);
			body.baseNameToken = Localization.SURVIVOR_NAME;
			
			body.bodyFlags = CharacterBody.BodyFlags.ImmuneToExecutes | CharacterBody.BodyFlags.Void | CharacterBody.BodyFlags.ImmuneToVoidDeath;
			// Immunity to void death is intentional. This is because it deletes the character model, which we don't want.
			// Instead, we manually intercept the damage and apply it as a non-voiddeath source. This way it can kill the player and play the death animation without screwing up
			// the killcam / spectator cam by suddenly yoinking the model out of existence.

			playerBodyPrefab.GetComponent<SetStateOnHurt>().canBeHitStunned = false;
			ContentAddition.AddBody(playerBodyPrefab);

			Log.LogTrace("Setting up spawn animation...");
			EntityStateMachine initialStateCtr = playerBodyPrefab.GetComponent<EntityStateMachine>();
			initialStateCtr.initialStateType = UtilCreateSerializableAndNetRegister<Skills.Spawn.SpawnState>();

			Log.LogTrace("Setting up death animation...");
			CharacterDeathBehavior deathBehavior = playerBodyPrefab.GetComponent<CharacterDeathBehavior>();
			deathBehavior.deathState = UtilCreateSerializableAndNetRegister<Skills.Death.DeathState>();

			Log.LogTrace("Finished creating the base body prefab. Configuring all data...");
			#endregion

			#region Interactions, Camera, and Body Scale Properties
			Log.LogTrace("Setting up interactions and scale dependent properties...");
			Interactor interactor = playerBodyPrefab.GetComponent<Interactor>();
			if (Configuration.UseFullSizeCharacter) {
				interactor.maxInteractionDistance = 9f;
			} else {
				interactor.maxInteractionDistance = 6.5f;
				GameObject baseTransform = playerBodyPrefab.GetComponent<ModelLocator>().modelBaseTransform.gameObject;
				baseTransform.transform.localScale = Vector3.one * 0.5f;
				baseTransform.transform.Translate(new Vector3(0f, 4f, 0f));
				foreach (KinematicCharacterMotor kinematicCharacterMotor in playerBodyPrefab.GetComponentsInChildren<KinematicCharacterMotor>()) {
					// * 0.4f
					// + 1.25f
					// Up to *0.5f and +1.5f
					// This seems to fix the bungus issue!
					kinematicCharacterMotor.SetCapsuleDimensions(kinematicCharacterMotor.Capsule.radius * 0.5f, kinematicCharacterMotor.Capsule.height * 0.5f, 1.5f);
				}
			}
			CameraTargetParams camTargetParams = body.gameObject.GetComponent<CameraTargetParams>();
			if (!camTargetParams) {
				camTargetParams = body.gameObject.AddComponent<CameraTargetParams>();
			}
			camTargetParams.cameraParams ??= ScriptableObject.CreateInstance<CharacterCameraParams>();
			camTargetParams.cameraParams.data.idealLocalCameraPos = new Vector3(2.6f, 2.6f, -16f) / (Configuration.UseFullSizeCharacter ? 1f : 2f);
			camTargetParams.cameraParams.data.pivotVerticalOffset = -0.8f;
			Log.LogTrace("Done setting up interactions and scale dependent properties.");
			#endregion

			#region Skills
			Log.LogTrace("Setting up skills.");
			#region Passive
			// Passive stuff:
			Log.LogTrace("Passive skill...");
			SkillLocator skillLoc = playerBodyPrefab.GetComponent<SkillLocator>();
			skillLoc.passiveSkill = default;
			skillLoc.passiveSkill.enabled = true;
			skillLoc.passiveSkill.keywordToken = Localization.PASSIVE_KEYWORD;
			skillLoc.passiveSkill.skillNameToken = Localization.PASSIVE_NAME;
			skillLoc.passiveSkill.skillDescriptionToken = Localization.PASSIVE_DESC;
			// skillLoc.passiveSkill.icon = CommonImages.Passive;
			Log.LogTrace("Finished registering passive details...");
			#endregion

			#region Primary
			Log.LogTrace("Primary Skill (\"Spike\")...");
			SkillDef spike = ScriptableObject.CreateInstance<SkillDef>();
			spike.activationState = UtilCreateSerializableAndNetRegister<SpikeSkill>();
			spike.activationStateMachineName = "Weapon";
			spike.baseMaxStock = 1;
			spike.baseRechargeInterval = 0f;
			spike.beginSkillCooldownOnSkillEnd = true;
			spike.canceledFromSprinting = false;
			spike.cancelSprintingOnActivation = true;
			spike.dontAllowPastMaxStocks = true;
			spike.forceSprintDuringState = false;
			spike.fullRestockOnAssign = true;
			spike.interruptPriority = InterruptPriority.Any;
			spike.isCombatSkill = true;
			spike.mustKeyPress = false;
			spike.rechargeStock = spike.baseMaxStock;
			spike.requiredStock = 1;
			spike.stockToConsume = 1;
			spike.skillNameToken = Localization.SKILL_PRIMARY_NAME;
			spike.skillDescriptionToken = Localization.SKILL_PRIMARY_DESC;
			// icon
			ROR2ObjectCreator.AddSkill(playerBodyPrefab, spike, "primary", 0);

			Log.LogTrace($"Registering intermediary state {nameof(SpikeCommonFireSequence)}");
			ContentAddition.AddEntityState<SpikeCommonFireSequence>(out _);

			Log.LogTrace("Finished registering Spike.");
			#endregion

			#region Secondary
			Log.LogTrace("Secondary Skill (\"Bind\")...");
			SkillDef bind = ScriptableObject.CreateInstance<SkillDef>();//SurvivorCatalog.FindSurvivorDef("VoidJailer").bodyPrefab.GetComponent<SkillLocator>().secondary.skillDef;
			bind.activationState = UtilCreateSerializableAndNetRegister<CaptureSkill>();
			bind.activationStateMachineName = "Weapon";
			bind.baseMaxStock = 1;
			bind.baseRechargeInterval = 8;
			bind.beginSkillCooldownOnSkillEnd = true;
			bind.canceledFromSprinting = false;
			bind.cancelSprintingOnActivation = true;
			bind.dontAllowPastMaxStocks = true;
			bind.forceSprintDuringState = false;
			bind.fullRestockOnAssign = true;
			bind.interruptPriority = InterruptPriority.Any;
			bind.isCombatSkill = true;
			bind.mustKeyPress = true;
			bind.rechargeStock = bind.baseMaxStock;
			bind.requiredStock = 1;
			bind.stockToConsume = 1;
			bind.skillNameToken = Localization.SKILL_SECONDARY_NAME;
			bind.skillDescriptionToken = Localization.SKILL_SECONDARY_DESC;
			// icon
			ROR2ObjectCreator.AddSkill(playerBodyPrefab, bind, "secondary", 0);

			Log.LogTrace($"Registering intermediary states {nameof(SpikeCommonFireSequence)} and {nameof(ExitCapture)}");
			ContentAddition.AddEntityState<CaptureCommonPullSequence>(out _);
			ContentAddition.AddEntityState<ExitCapture>(out _);

			Log.LogTrace("Finished registering Bind.");
			#endregion

			#region Utility
			Log.LogTrace("Utility Skill (\"Dive\")...");
			SkillDef dive = ScriptableObject.CreateInstance<SkillDef>();
			dive.activationState = UtilCreateSerializableAndNetRegister<DiveSkill>();
			dive.activationStateMachineName = "Body";
			dive.baseMaxStock = 1;
			dive.baseRechargeInterval = 4f;
			dive.beginSkillCooldownOnSkillEnd = true;
			dive.canceledFromSprinting = false;
			dive.cancelSprintingOnActivation = false;
			dive.dontAllowPastMaxStocks = false;
			dive.forceSprintDuringState = true;
			dive.fullRestockOnAssign = true;
			dive.interruptPriority = InterruptPriority.PrioritySkill;
			dive.isCombatSkill = false;
			dive.mustKeyPress = false;
			dive.rechargeStock = dive.baseMaxStock;
			dive.requiredStock = 1;
			dive.stockToConsume = 1;
			dive.skillNameToken = Localization.SKILL_UTILITY_NAME;
			dive.skillDescriptionToken = Localization.SKILL_UTILITY_DESC;
			// icon
			ROR2ObjectCreator.AddSkill(playerBodyPrefab, dive, "utility", 0);
			Log.LogTrace("Finished registering Dive.");
			#endregion

			#region Special
			Log.LogTrace("Special Skill (\"Fury of the Warden\")...");
			SkillDef rage = ScriptableObject.CreateInstance<SkillDef>();
			rage.activationState = UtilCreateSerializableAndNetRegister<FurySkill>();
			rage.activationStateMachineName = "Body";
			rage.baseMaxStock = 1;
			rage.baseRechargeInterval = 45;
			rage.beginSkillCooldownOnSkillEnd = true;
			rage.canceledFromSprinting = false;
			rage.cancelSprintingOnActivation = false;
			rage.dontAllowPastMaxStocks = false;
			rage.forceSprintDuringState = false;
			rage.fullRestockOnAssign = true;
			rage.interruptPriority = InterruptPriority.Any;
			rage.isCombatSkill = false;
			rage.mustKeyPress = false;
			rage.rechargeStock = rage.baseMaxStock;
			rage.requiredStock = 1;
			rage.stockToConsume = 1;
			rage.skillNameToken = Localization.SKILL_SPECIAL_NAME;
			rage.skillDescriptionToken = Localization.SKILL_SPECIAL_DESC;
			rage.keywordTokens = new string[] { Localization.SKILL_SPECIAL_KEYWORD };
			// icon
			ROR2ObjectCreator.AddSkill(playerBodyPrefab, rage, "special", 0);
			Log.LogTrace("Finished registering Fury of the Warden.");
			#endregion

			#endregion

			#region Survivor
			Log.LogTrace("Creating Void Jailer Skins...");
			ROR2ObjectCreator.AddJailerSkins(playerBodyPrefab);

			Log.LogTrace("Creating SurvivorDef instance...");
			SurvivorDef survivorDef = ScriptableObject.CreateInstance<SurvivorDef>();
			survivorDef.bodyPrefab = playerBodyPrefab;
			survivorDef.descriptionToken = Localization.SURVIVOR_DESC;
			survivorDef.displayNameToken = Localization.SURVIVOR_NAME;
			survivorDef.outroFlavorToken = Localization.SURVIVOR_OUTRO;
			survivorDef.mainEndingEscapeFailureFlavorToken = Localization.SURVIVOR_OUTRO_FAILED;
			survivorDef.displayPrefab = playerBodyDisplayPrefab;
			survivorDef.displayPrefab.transform.localScale = Vector3.one * 0.25f;
			survivorDef.primaryColor = new Color(0.5f, 0.5f, 0.5f);
			survivorDef.desiredSortPosition = 44.45f;
			Jailer = survivorDef;
			ContentAddition.AddSurvivorDef(survivorDef);
			Log.LogTrace("Finished registering survivor...");

			ROR2ObjectCreator.FinalizeBody(playerBodyPrefab.GetComponent<SkillLocator>());
			Log.LogTrace("Survivor object initialization is complete.");
			#endregion

			#region Survivor Hooks
			Log.LogTrace("Creating method hooks...");
			On.RoR2.CharacterBody.SetBuffCount += InterceptBuffsEvent;
			On.RoR2.HealthComponent.TakeDamage += InterceptTakeDamageForVoidResist;
			On.RoR2.HealthComponent.TakeDamage += InterceptTakeDamageForNullBuff;
			On.RoR2.HealthComponent.TakeDamage += DoFakeVoidDeath;
			On.RoR2.HealthComponent.TakeDamage += DoFuryFracture;
			On.RoR2.CharacterBody.UpdateSingleTemporaryVisualEffect_refTemporaryVisualEffect_GameObject_float_bool_string += OnUpdatingVFX;
			// On.RoR2.CharacterMaster.SpawnBody += OnSpawnBody;
			

			BodyCatalog.availability.CallWhenAvailable(() => {
				// Register to HPBarAPI.
				Registry.RegisterColorProvider(plugin, body.bodyIndex, new HPBarColorMarshaller());
				Log.LogTrace("Custom Void-Style HP Bar colors registered.");
			});
			Log.LogTrace("Survivor init complete.");
			#endregion
		}

#pragma warning disable Publicizer001
		private static void OnUpdatingVFX(On.RoR2.CharacterBody.orig_UpdateSingleTemporaryVisualEffect_refTemporaryVisualEffect_GameObject_float_bool_string originalMethod, CharacterBody @this, ref TemporaryVisualEffect tempEffect, GameObject tempEffectPrefab, float effectRadius, bool active, string childLocatorOverride) {
			if (tempEffectPrefab == CharacterBody.AssetReferences.noCooldownEffectPrefab) {
				active = active || @this.HasBuff(BuffProvider.Fury);
			}
			originalMethod(@this, ref tempEffect, tempEffectPrefab, effectRadius, active, childLocatorOverride);
		}

		/// <summary>
		/// Utility: Use <see cref="ContentAddition.AddEntityState{T}(out bool)"/> to register the provided entity state type, then return a new instance of <see cref="SerializableEntityStateType"/> using the same type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		private static SerializableEntityStateType UtilCreateSerializableAndNetRegister<T>() where T : EntityState {
			Log.LogTrace($"Registering EntityState {typeof(T).FullName} and returning a new instance of {nameof(SerializableEntityStateType)} of that type...");
			ContentAddition.AddEntityState<T>(out _);
			return new SerializableEntityStateType(typeof(T));
		}

		/// <summary>
		/// Intent: Render brainstalks when the player has the "Fury" effect. This does not work properly. An IL edit to the line in vanilla code for brainstalks itself may be necessary. I would like to avoid this.
		/// </summary>
		/// <param name="originalMethod"></param>
		/// <param name="this"></param>
		private static void UpdateAllTempVFX(On.RoR2.CharacterBody.orig_UpdateAllTemporaryVisualEffects originalMethod, CharacterBody @this) {
			originalMethod(@this);
			@this.UpdateSingleTemporaryVisualEffect(ref @this.noCooldownEffectInstance, CharacterBody.AssetReferences.noCooldownEffectPrefab, @this.radius, @this.HasBuff(BuffProvider.Fury) || @this.HasBuff(RoR2Content.Buffs.NoCooldowns), "Head");
		}

		/// <summary>
		/// Intent: Intercept damage to see if it has the custom "Null Boosted" type. This applies to the primary fire only. It increases the damage dealt if the victim is tethered or nullified.
		/// </summary>
		/// <param name="originalMethod"></param>
		/// <param name="this"></param>
		/// <param name="damageInfo"></param>
		private static void InterceptTakeDamageForNullBuff(On.RoR2.HealthComponent.orig_TakeDamage originalMethod, HealthComponent @this, DamageInfo damageInfo) {
			if (damageInfo.HasModdedDamageType(DamageTypeProvider.NullBoosted) && (@this.body.HasBuff(DLC1Content.Buffs.JailerTether) || @this.body.HasBuff(RoR2Content.Buffs.Nullified))) {
				damageInfo.damageColorIndex = DamageColorIndex.WeakPoint;
				damageInfo.damage *= 1.75f;
			}
			originalMethod(@this, damageInfo);
		}

		/// <summary>
		/// Intent: Do our best to see if damage from a Void Seed / Atmosphere is being dealt, and cancel it.
		/// </summary>
		/// <param name="originalMethod"></param>
		/// <param name="this"></param>
		/// <param name="damageInfo"></param>
		private static void InterceptTakeDamageForVoidResist(On.RoR2.HealthComponent.orig_TakeDamage originalMethod, HealthComponent @this, DamageInfo damageInfo) {
			if (damageInfo.rejected) {
				originalMethod(@this, damageInfo);
				return;
			}

			if (@this.body != null && @this.body.baseNameToken == Localization.SURVIVOR_NAME) {
				if (damageInfo.attacker == null && damageInfo.inflictor == null && damageInfo.damageType == (DamageType.BypassBlock | DamageType.BypassArmor)) {
					Log.LogTrace("Rejecting damage for what I believe to be Void atmosphere damage (it has no source/attacker, and the damage type bypasses blocks and armor only).");
					damageInfo.rejected = true;
				}
			}

			originalMethod(@this, damageInfo);
		}

		/// <summary>
		/// Intent: Cancel void fog.
		/// </summary>
		/// <param name="originalMethod"></param>
		/// <param name="this"></param>
		/// <param name="buffType"></param>
		/// <param name="newCount"></param>
		private static void InterceptBuffsEvent(On.RoR2.CharacterBody.orig_SetBuffCount originalMethod, CharacterBody @this, BuffIndex buffType, int newCount) {
			if (@this.baseNameToken == Localization.SURVIVOR_NAME) {
				if (buffType == MegaVoidFog || buffType == NormVoidFog || buffType == WeakVoidFog) {
					Log.LogTrace("Rejecting attempt to add fog to player's status effects.");
					originalMethod(@this, buffType, 0); // Always 0
					return;
				}
			}

			originalMethod(@this, buffType, newCount);
		}

		/// <summary>
		/// Detects when a character dies, and spawns a fake void crit glasses effect. This is used to dramatize the existing void death effect when enemies get killed by the black hole thing. 
		/// Looks really cool, purely cosmetic.
		/// </summary>
		/// <param name="originalMethod"></param>
		/// <param name="this"></param>
		/// <param name="damageInfo"></param>
		private static void DoFakeVoidDeath(On.RoR2.HealthComponent.orig_TakeDamage originalMethod, HealthComponent @this, DamageInfo damageInfo) {
			if (damageInfo.rejected) {
				originalMethod(@this, damageInfo);
				return;
			}

			originalMethod(@this, damageInfo);
			if (damageInfo.HasModdedDamageType(DamageTypeProvider.PerformsFakeVoidDeath)) {
				if (!@this.alive && @this.wasAlive && @this.body) {
					Vector3 pos = @this.body.corePosition;
					float radius = @this.body.bestFitRadius;

					if (damageInfo.attacker) {
						CharacterBody attacker = damageInfo.attacker.GetComponent<CharacterBody>();
						if (attacker != null) {
							EffectManager.SpawnEffect(
								EffectProvider.SilentVoidCritDeathEffect,
								new EffectData {
									origin = pos,
									scale = radius
								},
								true
							);
						}
					}
				}
			}
		}

		/// <summary>
		/// Similar to <see cref="DoFakeVoidDeath(On.RoR2.HealthComponent.orig_TakeDamage, HealthComponent, DamageInfo)"/>, this spawns the cromch effect from Collapse.
		/// </summary>
		/// <param name="originalMethod"></param>
		/// <param name="this"></param>
		/// <param name="damageInfo"></param>
		private static void DoFuryFracture(On.RoR2.HealthComponent.orig_TakeDamage originalMethod, HealthComponent @this, DamageInfo damageInfo) {
			if (damageInfo.rejected) {
				originalMethod(@this, damageInfo);
				return;
			}

			originalMethod(@this, damageInfo);
			if (damageInfo.HasModdedDamageType(DamageTypeProvider.PerformsFastFracture)) {
				EffectManager.SpawnEffect(
					EffectProvider.CollapseExplode,
					new EffectData {
						origin = @this.body.corePosition,
						scale = 0.6f
					},
					true
				);
			}
			
		}

#pragma warning restore Publicizer001

		#region Values To Remember

		/// <summary>
		/// A reference to the highest power void fog.
		/// </summary>
		public static BuffIndex MegaVoidFog {
			get {
				if (_megaVoidFog == BuffIndex.None) {
					_megaVoidFog = DLC1Content.Buffs.VoidRaidCrabWardWipeFog.buffIndex;
				}
				return _megaVoidFog;
			}
		}

		/// <summary>
		/// A reference to ordinary void fog.
		/// </summary>
		public static BuffIndex NormVoidFog {
			get {
				if (_normVoidFog == BuffIndex.None) {
					_normVoidFog = RoR2Content.Buffs.VoidFogStrong.buffIndex;
				}
				return _normVoidFog;
			}
		}

		/// <summary>
		/// A reference to weak void fog.
		/// </summary>
		public static BuffIndex WeakVoidFog {
			get {
				if (_weakVoidFog == BuffIndex.None) {
					_weakVoidFog = RoR2Content.Buffs.VoidFogMild.buffIndex;
				}
				return _weakVoidFog;
			}
		}

		private static BuffIndex _megaVoidFog = BuffIndex.None;
		private static BuffIndex _normVoidFog = BuffIndex.None;
		private static BuffIndex _weakVoidFog = BuffIndex.None;

		#endregion

	}
}

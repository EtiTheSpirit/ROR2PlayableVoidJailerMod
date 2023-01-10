using EntityStates;
using KinematicCharacterController;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.Skills;
using ROR2HPBarAPI.API;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR;
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

		// TODO: Leave this public?
		public static CharacterBody BodyTemplate { get; private set; }

		public static SurvivorDef Jailer { get; private set; }

		// TODO: Try to allow runtime edits by using one of the popular settings mods?
		// Why can't this be like MelonLoader where settings are just exposed out of the box
		public static void UpdateSettings() {
			Log.LogTrace("Updating body settings...");
			BodyTemplate.baseMaxHealth = Configuration.BaseMaxHealth;
			BodyTemplate.levelMaxHealth = Configuration.LevelMaxHealth;
			BodyTemplate.baseMaxShield = Configuration.BaseMaxShield;
			BodyTemplate.levelMaxShield = Configuration.LevelMaxShield;
			BodyTemplate.baseArmor = Configuration.BaseArmor;
			BodyTemplate.levelArmor = Configuration.LevelArmor;
			BodyTemplate.baseRegen = Configuration.BaseHPRegen;
			BodyTemplate.levelRegen = Configuration.LevelHPRegen;

			BodyTemplate.baseDamage = Configuration.BaseDamage;
			BodyTemplate.levelDamage = Configuration.LevelDamage;
			BodyTemplate.baseAttackSpeed = Configuration.BaseAttackSpeed;
			BodyTemplate.levelAttackSpeed = Configuration.LevelAttackSpeed;
			BodyTemplate.baseCrit = Configuration.BaseCritChance;
			BodyTemplate.levelCrit = Configuration.LevelCritChance;

			BodyTemplate.baseMoveSpeed = Configuration.BaseMoveSpeed;
			BodyTemplate.levelMoveSpeed = Configuration.LevelMoveSpeed;
			BodyTemplate.baseJumpCount = Configuration.BaseJumpCount;
			BodyTemplate.baseJumpPower = Configuration.BaseJumpPower;
			BodyTemplate.levelJumpPower = Configuration.LevelJumpPower;
			BodyTemplate.baseAcceleration = Configuration.BaseAcceleration;
		}

		internal static void Init(VoidJailerPlayerPlugin plugin) {
			GameObject playerBodyPrefab = ROR2ObjectCreator.CreateBody("VoidJailerSurvivor", "RoR2/DLC1/VoidJailer/VoidJailerBody.prefab");
			GameObject playerBodyLocator = PrefabAPI.InstantiateClone(playerBodyPrefab.GetComponent<ModelLocator>().modelBaseTransform.gameObject, "PlayerJailerBodyDisplay");
			playerBodyLocator.AddComponent<NetworkIdentity>();

			#region Body / Stats
			CharacterBody body = playerBodyPrefab.GetComponent<CharacterBody>();
			BodyTemplate = body;
			body.bodyColor = new Color(0.867f, 0.468f, 0.776f);
			body.baseNameToken = Localization.SURVIVOR_NAME;
			
			body.bodyFlags = CharacterBody.BodyFlags.ImmuneToExecutes | CharacterBody.BodyFlags.Void | CharacterBody.BodyFlags.ImmuneToVoidDeath;
			// Immunity to void death is intentional. This is because it deletes the character model, which we don't want.
			// Instead, we manually intercept the damage and apply it as a non-voiddeath source. This way it can kill the player and play the death animation without screwing up
			// the killcam / spectator cam by suddenly yoinking the model out of existence.
			UpdateSettings();
			playerBodyPrefab.GetComponent<SetStateOnHurt>().canBeHitStunned = false;
			ContentAddition.AddBody(playerBodyPrefab);

			ContentAddition.AddEntityState<Skills.Spawn.SpawnState>(out _);
			playerBodyPrefab.GetComponent<EntityStateMachine>().initialStateType = new SerializableEntityStateType(typeof(Skills.Spawn.SpawnState));

			ContentAddition.AddEntityState<Skills.Death.DeathState>(out _);
			CharacterDeathBehavior deathBehavior = playerBodyPrefab.GetComponent<CharacterDeathBehavior>();
			deathBehavior.deathState = new SerializableEntityStateType(typeof(Skills.Death.DeathState));
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
			ContentAddition.AddEntityState<SpikeSkill>(out _);
			SkillDef spike = ScriptableObject.CreateInstance<SkillDef>();
			spike.activationState = new SerializableEntityStateType(typeof(SpikeSkill));
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
			Log.LogTrace("Finished registering Spike.");
			#endregion

			#region Secondary
			ContentAddition.AddEntityState<CaptureSkill>(out _);
			SkillDef bind = ScriptableObject.CreateInstance<SkillDef>();//SurvivorCatalog.FindSurvivorDef("VoidJailer").bodyPrefab.GetComponent<SkillLocator>().secondary.skillDef;
			bind.activationState = new SerializableEntityStateType(typeof(CaptureSkill));
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
			Log.LogTrace("Finished registering Bind.");
			#endregion

			#region Utility
			ContentAddition.AddEntityState<DiveSkill>(out _);
			SkillDef dive = ScriptableObject.CreateInstance<SkillDef>();
			dive.activationState = new SerializableEntityStateType(typeof(DiveSkill));
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
			ContentAddition.AddEntityState<FurySkill>(out _);
			SkillDef rage = ScriptableObject.CreateInstance<SkillDef>();
			rage.activationState = new SerializableEntityStateType(typeof(FurySkill));
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
			Log.LogTrace("Finished registering Rage.");
			#endregion

			#endregion

			#region Survivor
			ROR2ObjectCreator.AddJailerSkins(playerBodyPrefab);
			Log.LogTrace("Finished registering skins...");

			SurvivorDef survivorDef = ScriptableObject.CreateInstance<SurvivorDef>();
			survivorDef.bodyPrefab = playerBodyPrefab;
			survivorDef.descriptionToken = Localization.SURVIVOR_DESC;
			survivorDef.displayNameToken = Localization.SURVIVOR_NAME;
			survivorDef.outroFlavorToken = Localization.SURVIVOR_OUTRO;
			survivorDef.mainEndingEscapeFailureFlavorToken = Localization.SURVIVOR_OUTRO_FAILED;
			survivorDef.displayPrefab = playerBodyLocator;
			survivorDef.displayPrefab.transform.localScale = Vector3.one * 0.25f;
			survivorDef.primaryColor = new Color(0.5f, 0.5f, 0.5f);
			survivorDef.desiredSortPosition = 44.45f;
			Jailer = survivorDef;
			ContentAddition.AddSurvivorDef(survivorDef);
			Log.LogTrace("Finished registering survivor...");

			ROR2ObjectCreator.FinalizeBody(playerBodyPrefab.GetComponent<SkillLocator>());
			#endregion

			#region Survivor Hooks
			On.RoR2.CharacterBody.SetBuffCount += InterceptBuffsEvent;
			On.RoR2.HealthComponent.TakeDamage += InterceptTakeDamageForVoidResist;
			On.RoR2.HealthComponent.TakeDamage += InterceptTakeDamageForNullBuff;
			On.RoR2.HealthComponent.TakeDamage += DoFakeVoidDeath;
			On.RoR2.HealthComponent.TakeDamage += DoFuryFracture;
			On.RoR2.CharacterBody.UpdateAllTemporaryVisualEffects += UpdateAllTempVFX;

			BodyCatalog.availability.CallWhenAvailable(() => {
				Registry.RegisterColorProvider(plugin, body.bodyIndex, new HPBarColorMarshaller());
				Log.LogTrace("Custom Void-Style HP Bar colors registered.");
			});

			On.RoR2.SurvivorMannequins.SurvivorMannequinSlotController.RebuildMannequinInstance += OnRebuildMannequinInstance;
			#endregion
		}

		private static void UpdateAllTempVFX(On.RoR2.CharacterBody.orig_UpdateAllTemporaryVisualEffects orig, CharacterBody @this) {
			@this.UpdateSingleTemporaryVisualEffect(ref @this.noCooldownEffectInstance, CharacterBody.AssetReferences.noCooldownEffectPrefab, @this.radius, @this.HasBuff(BuffProvider.Fury) || @this.HasBuff(RoR2Content.Buffs.NoCooldowns), "Head");
		}

		private static void InterceptTakeDamageForNullBuff(On.RoR2.HealthComponent.orig_TakeDamage originalMethod, HealthComponent @this, DamageInfo damageInfo) {
			
			if (damageInfo.HasModdedDamageType(DamageTypeProvider.NullBoosted) && (@this.body.HasBuff(DLC1Content.Buffs.JailerTether) || @this.body.HasBuff(RoR2Content.Buffs.Nullified))) {
				damageInfo.damageColorIndex = DamageColorIndex.WeakPoint;
				damageInfo.damage *= 1.75f;
			}
			originalMethod(@this, damageInfo);
		}

#pragma warning disable Publicizer001

		private static void OnRebuildMannequinInstance(On.RoR2.SurvivorMannequins.SurvivorMannequinSlotController.orig_RebuildMannequinInstance originalMethod, RoR2.SurvivorMannequins.SurvivorMannequinSlotController @this) {
			originalMethod(@this);
			if (@this.mannequinInstanceTransform != null && @this.currentSurvivorDef == Jailer) {
				Animator previewAnimator = @this.mannequinInstanceTransform.transform.Find("mdlVoidJailer").GetComponent<Animator>();
				previewAnimator.SetBool("isGrounded", true); // Fix an animation bug
				previewAnimator.SetFloat("Spawn.playbackRate", 1f);
				previewAnimator.Play("Spawn");
				// Util.PlaySound(spawnSoundString, @this.mannequinInstanceTransform.gameObject);
				// EffectManager.SimpleMuzzleFlash(NullifierSpawnState.spawnEffectPrefab, @this.mannequinInstanceTransform.gameObject, "PortalEffect", false);
			}
		}


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

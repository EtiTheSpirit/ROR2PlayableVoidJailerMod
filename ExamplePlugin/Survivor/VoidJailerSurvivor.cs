using EntityStates;
using KinematicCharacterController;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.Skills;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using VoidJailerMod.Buffs;
using VoidJailerMod.Damage;
using VoidJailerMod.Effects;
using VoidJailerMod.Initialization.Sprites;
using VoidJailerMod.Skills.Capture;
using VoidJailerMod.Skills.Dive;
using VoidJailerMod.Skills.Special;
using VoidJailerMod.Skills.Spike;
using VoidJailerMod.XansTools;
using Xan.ROR2VoidPlayerCharacterCommon;
using Xan.ROR2VoidPlayerCharacterCommon.DamageBehavior;
using Xan.ROR2VoidPlayerCharacterCommon.Registration;
using CharacterBody = RoR2.CharacterBody;
using Interactor = RoR2.Interactor;
using Configuration = VoidJailerMod.Initialization.Configuration;
using Xan.ROR2VoidPlayerCharacterCommon.SurvivorHelper;
using Xan.ROR2VoidPlayerCharacterCommon.VRMod;
using static UnityEngine.CullingGroup;
using System.ComponentModel;

namespace VoidJailerMod.Survivor {
	public static class VoidJailerSurvivor {


		public static SurvivorDef Jailer { get; private set; }

		public static BodyIndex JailerIndex { get; private set; }

		public static SkillDef MinigunPrimary { get; private set; }

		private static GameObject _playerBodyPrefab;
		private static ExtraPassiveSkill _modNotice;
		private static SkillLocator _prefabSkillLocator;
		private static bool _survivorReady = false;

		public static void UpdateSettings(CharacterBody body) {
			Log.LogTrace("Updating body settings...");
			body.baseMaxHealth = Configuration.CommonVoidEnemyConfigs.BaseMaxHealth;
			body.levelMaxHealth = Configuration.CommonVoidEnemyConfigs.LevelMaxHealth;
			body.baseMaxShield = Configuration.CommonVoidEnemyConfigs.BaseMaxShield;
			body.levelMaxShield = Configuration.CommonVoidEnemyConfigs.LevelMaxShield;
			body.baseArmor = Configuration.CommonVoidEnemyConfigs.BaseArmor;
			body.levelArmor = Configuration.CommonVoidEnemyConfigs.LevelArmor;
			body.baseRegen = Configuration.CommonVoidEnemyConfigs.BaseHPRegen;
			body.levelRegen = Configuration.CommonVoidEnemyConfigs.LevelHPRegen;

			body.baseDamage = Configuration.CommonVoidEnemyConfigs.BaseDamage;
			body.levelDamage = Configuration.CommonVoidEnemyConfigs.LevelDamage;
			body.baseAttackSpeed = Configuration.CommonVoidEnemyConfigs.BaseAttackSpeed;
			body.levelAttackSpeed = Configuration.CommonVoidEnemyConfigs.LevelAttackSpeed;
			body.baseCrit = Configuration.CommonVoidEnemyConfigs.BaseCritChance;
			body.levelCrit = Configuration.CommonVoidEnemyConfigs.LevelCritChance;

			body.baseMoveSpeed = Configuration.CommonVoidEnemyConfigs.BaseMoveSpeed;
			body.levelMoveSpeed = Configuration.CommonVoidEnemyConfigs.LevelMoveSpeed;
			body.baseJumpCount = Configuration.CommonVoidEnemyConfigs.BaseJumpCount;
			body.baseJumpPower = Configuration.CommonVoidEnemyConfigs.BaseJumpPower;
			body.levelJumpPower = Configuration.CommonVoidEnemyConfigs.LevelJumpPower;
			body.baseAcceleration = Configuration.CommonVoidEnemyConfigs.BaseAcceleration;

			if (_survivorReady) {
				SkillLocator locator = body.skillLocator;
				UpdateCooldowns(_prefabSkillLocator);
				if (locator) UpdateCooldowns(locator);
				Localization.ReloadStattedTexts(body.bodyIndex);
			}
		}

		private static void UpdateCooldowns(SkillLocator locator) {

			locator.secondary.skillFamily.variants[0].skillDef.baseRechargeInterval = Configuration.SecondaryCooldown;

			locator.utility.skillFamily.variants[0].skillDef.baseRechargeInterval = Configuration.UtilityCooldown;

			locator.special.skillFamily.variants[0].skillDef.baseRechargeInterval = Configuration.SpecialRageCooldown;
			locator.special.skillFamily.variants[1].skillDef.baseRechargeInterval = Configuration.MortarCooldown;
			locator.special.skillFamily.variants[1].skillDef.baseMaxStock = Configuration.MortarStocks;
		}

		internal static void Init(VoidJailerPlayerPlugin plugin) {
			Log.LogTrace("Initializing Survivor info!");

			_playerBodyPrefab = SurvivorHollower.CreateBodyWithSkins("VoidJailerSurvivor", "RoR2/DLC1/VoidJailer/VoidJailerBody.prefab", "RoR2/DLC1/VoidJailer/VoidJailerAllyBody.prefab");

			Log.LogTrace("Setting localPlayerAuthority=true on Jailer Body (this patches a bug)...");
			NetworkIdentity netID = _playerBodyPrefab.GetComponent<NetworkIdentity>();
			netID.localPlayerAuthority = true;

			Log.LogTrace("Duplicating the body for display in the Survivor Selection screen...");
			GameObject playerBodyDisplayPrefab = PrefabAPI.InstantiateClone(_playerBodyPrefab.GetComponent<ModelLocator>().modelBaseTransform.gameObject, "PlayerJailerBodyDisplay");
			netID = playerBodyDisplayPrefab.AddComponent<NetworkIdentity>();
			netID.localPlayerAuthority = true;

			#region Body / Stats
			Log.LogTrace("Preparing the Body's style and stats...");
			CharacterBody body = _playerBodyPrefab.GetComponent<CharacterBody>();
			UpdateSettings(body);
			body.bodyColor = new Color(0.867f, 0.468f, 0.776f);
			body.baseNameToken = Localization.SURVIVOR_NAME;
			body.subtitleNameToken = Localization.SURVIVOR_UMBRA;
			body.portraitIcon = Images.Portrait.texture;

			body.bodyFlags = CharacterBody.BodyFlags.ImmuneToExecutes | CharacterBody.BodyFlags.Void;

			_playerBodyPrefab.GetComponent<SetStateOnHurt>().canBeHitStunned = false;
			ContentAddition.AddBody(_playerBodyPrefab);

			Log.LogTrace("Setting up spawn animation...");
			EntityStateMachine initialStateCtr = _playerBodyPrefab.GetComponent<EntityStateMachine>();
			initialStateCtr.initialStateType = UtilCreateSerializableAndNetRegister<Skills.Spawn.SpawnState>(); //new SerializableEntityStateType(typeof(EntityStates.VoidJailer.SpawnState));

			Log.LogTrace("Setting up death animation...");
			CharacterDeathBehavior deathBehavior = _playerBodyPrefab.GetComponent<CharacterDeathBehavior>();
			deathBehavior.deathState = UtilCreateSerializableAndNetRegister<Skills.Death.DeathState>();

			Log.LogTrace("Finished creating the base body prefab. Configuring all data...");
			#endregion

			#region Interactions, Camera, and Body Scale Properties
			
			#endregion

			#region Skills
			Log.LogTrace("Setting up skills.");
			#region Passive
			// Passive stuff:
			Log.LogTrace("Passive skill...");
			SkillLocator skillLoc = _playerBodyPrefab.GetComponent<SkillLocator>();
			skillLoc.passiveSkill = default;
			skillLoc.passiveSkill.enabled = true;
			skillLoc.passiveSkill.keywordToken = Localization.PASSIVE_KEYWORD;
			skillLoc.passiveSkill.skillNameToken = Localization.PASSIVE_NAME;
			skillLoc.passiveSkill.skillDescriptionToken = Localization.PASSIVE_DESC;
			skillLoc.passiveSkill.icon = Images.Portrait;
			_prefabSkillLocator = skillLoc;

			ExtraPassiveSkill modNotice = _playerBodyPrefab.AddComponent<ExtraPassiveSkill>();
			modNotice.beforeNormalPassive = true;
			modNotice.additionalPassive.enabled = !Configuration.HideNotice;
			modNotice.additionalPassive.skillNameToken = Localization.VOID_COMMON_API_MESSAGE_NAME;
			modNotice.additionalPassive.skillDescriptionToken = Localization.VOID_COMMON_API_MESSAGE_DESC;
			modNotice.additionalPassive.keywordToken = Localization.VOID_COMMON_API_MESSAGE_CONTENT;
			modNotice.additionalPassive.icon = Images.GenericWarning;
			_modNotice = modNotice;

			Log.LogTrace("Finished registering passive details...");
			#endregion

			#region Primary (Shotgun)
			Log.LogTrace("Primary Skill (\"Spike\")...");
			SkillDef spike = ScriptableObject.CreateInstance<SkillDef>();
			spike.activationState = UtilCreateSerializableAndNetRegister<SpikeShotgunSkill>();
			spike.activationStateMachineName = "Weapon";
			spike.baseMaxStock = 1;
			spike.baseRechargeInterval = 0f;
			spike.beginSkillCooldownOnSkillEnd = true;
			spike.canceledFromSprinting = false;
			spike.cancelSprintingOnActivation = true;
			spike.dontAllowPastMaxStocks = true;
			spike.forceSprintDuringState = false;
			spike.fullRestockOnAssign = true;
			spike.interruptPriority = InterruptPriority.Skill;
			spike.isCombatSkill = true;
			spike.mustKeyPress = false;
			spike.rechargeStock = spike.baseMaxStock;
			spike.requiredStock = 1;
			spike.stockToConsume = 1;
			spike.skillNameToken = Localization.SKILL_PRIMARY_SHOTGUN_NAME;
			spike.skillDescriptionToken = Localization.SKILL_PRIMARY_SHOTGUN_DESC;
			spike.icon = Images.SpikeIcon;
			// icon
			SurvivorHollower.AddSkill(_playerBodyPrefab, spike, SurvivorHollower.SlotType.Primary, 0);

			Log.LogTrace($"Registering intermediary state {nameof(SpikeShotgunFireSequence)}");
			ContentAddition.AddEntityState<SpikeShotgunFireSequence>(out _);

			Log.LogTrace("Finished registering Spike.");
			#endregion

			#region Primary (Minigun)
			Log.LogTrace("Alt Primary Skill (\"Perforate\")...");
			SkillDef perforate = ScriptableObject.CreateInstance<SkillDef>();
			perforate.activationState = UtilCreateSerializableAndNetRegister<SpikeMinigunSkill>();
			perforate.activationStateMachineName = "Weapon";
			perforate.baseMaxStock = 1;
			perforate.baseRechargeInterval = 0f;
			perforate.beginSkillCooldownOnSkillEnd = false;
			perforate.canceledFromSprinting = false;
			perforate.cancelSprintingOnActivation = false;
			perforate.dontAllowPastMaxStocks = true;
			perforate.forceSprintDuringState = false;
			perforate.fullRestockOnAssign = true;
			perforate.interruptPriority = InterruptPriority.PrioritySkill;
			perforate.isCombatSkill = true;
			perforate.mustKeyPress = false;
			perforate.rechargeStock = perforate.baseMaxStock;
			perforate.requiredStock = 1;
			perforate.stockToConsume = 1;
			perforate.skillNameToken = Localization.SKILL_PRIMARY_MINIGUN_NAME;
			perforate.skillDescriptionToken = Localization.SKILL_PRIMARY_MINIGUN_DESC;
			perforate.icon = Images.PerforateIcon;
			MinigunPrimary = perforate;
			SurvivorHollower.AddNewHiddenSkill(perforate);
			Log.LogTrace("Finished registering Perforate.");
			
			#endregion

			#region Secondary
			Log.LogTrace("Secondary Skill (\"Bind\")...");
			SkillDef bind = ScriptableObject.CreateInstance<SkillDef>();//SurvivorCatalog.FindSurvivorDef("VoidJailer").bodyPrefab.GetComponent<SkillLocator>().secondary.skillDef;
			bind.activationState = UtilCreateSerializableAndNetRegister<CaptureSkill>();
			bind.activationStateMachineName = "Weapon";
			bind.baseMaxStock = 1;
			bind.baseRechargeInterval = Configuration.SecondaryCooldown;
			bind.beginSkillCooldownOnSkillEnd = true;
			bind.canceledFromSprinting = false;
			bind.cancelSprintingOnActivation = true;
			bind.dontAllowPastMaxStocks = true;
			bind.forceSprintDuringState = false;
			bind.fullRestockOnAssign = true;
			bind.interruptPriority = InterruptPriority.Skill;
			bind.isCombatSkill = true;
			bind.mustKeyPress = true;
			bind.rechargeStock = 1;
			bind.requiredStock = 1;
			bind.stockToConsume = 1;
			bind.skillNameToken = Localization.SKILL_SECONDARY_NAME;
			bind.skillDescriptionToken = Localization.SKILL_SECONDARY_DESC;
			bind.icon = Images.BindIcon;
			SurvivorHollower.AddSkill(_playerBodyPrefab, bind, SurvivorHollower.SlotType.Secondary, 0);

			Log.LogTrace($"Registering intermediary states {nameof(CaptureCommonPullSequence)} and {nameof(ExitCapture)}");
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
			dive.baseRechargeInterval = Configuration.UtilityCooldown;
			dive.beginSkillCooldownOnSkillEnd = true;
			dive.canceledFromSprinting = false;
			dive.cancelSprintingOnActivation = false;
			dive.dontAllowPastMaxStocks = false;
			dive.forceSprintDuringState = true;
			dive.fullRestockOnAssign = true;
			dive.interruptPriority = InterruptPriority.PrioritySkill;
			dive.isCombatSkill = false;
			dive.mustKeyPress = false;
			dive.rechargeStock = 1;
			dive.requiredStock = 1;
			dive.stockToConsume = 1;
			dive.skillNameToken = Localization.SKILL_UTILITY_NAME;
			dive.skillDescriptionToken = Localization.SKILL_UTILITY_DESC;
			dive.icon = Images.DiveIcon;
			SurvivorHollower.AddSkill(_playerBodyPrefab, dive, SurvivorHollower.SlotType.Utility, 0);
			if (VRInterop.VRAvailable) {
				VRAPI.VR.AddVignetteState(typeof(DiveSkill));
			}
			Log.LogTrace("Finished registering Dive.");
			#endregion

			#region Special
			Log.LogTrace("Special Skill (\"Fury of the Warden\")...");
			SkillDef rage = ScriptableObject.CreateInstance<SkillDef>();
			rage.activationState = UtilCreateSerializableAndNetRegister<FurySkill>();
			rage.activationStateMachineName = "Weapon";
			rage.baseMaxStock = 1;
			rage.baseRechargeInterval = Configuration.SpecialRageCooldown;
			rage.beginSkillCooldownOnSkillEnd = true;
			rage.canceledFromSprinting = false;
			rage.cancelSprintingOnActivation = false;
			rage.dontAllowPastMaxStocks = false;
			rage.forceSprintDuringState = false;
			rage.fullRestockOnAssign = true;
			rage.interruptPriority = InterruptPriority.PrioritySkill;
			rage.isCombatSkill = false;
			rage.mustKeyPress = false;
			rage.rechargeStock = 1;
			rage.requiredStock = 1;
			rage.stockToConsume = 1;
			rage.skillNameToken = Localization.SKILL_SPECIAL_FURY_NAME;
			rage.skillDescriptionToken = Localization.SKILL_SPECIAL_FURY_DESC;
			rage.keywordTokens = new string[] { Localization.SKILL_SPECIAL_FURY_KEYWORD };
			rage.icon = Images.PerforateIcon; // yes, perforate here
			SurvivorHollower.AddSkill(_playerBodyPrefab, rage, SurvivorHollower.SlotType.Special, 0);
			Log.LogTrace("Finished registering Fury of the Warden.");

			Log.LogTrace("Second Special Skill (\"Cavitation Mortar\")...");
			SkillDef mortar = ScriptableObject.CreateInstance<SkillDef>();
			mortar.activationState = UtilCreateSerializableAndNetRegister<MortarSkill>();
			mortar.activationStateMachineName = "Weapon";
			mortar.baseMaxStock = Configuration.MortarStocks;
			mortar.baseRechargeInterval = Configuration.MortarCooldown;
			mortar.beginSkillCooldownOnSkillEnd = true;
			mortar.canceledFromSprinting = false;
			mortar.cancelSprintingOnActivation = true;
			mortar.dontAllowPastMaxStocks = false;
			mortar.forceSprintDuringState = false;
			mortar.fullRestockOnAssign = true;
			mortar.interruptPriority = InterruptPriority.PrioritySkill;
			mortar.isCombatSkill = true;
			mortar.mustKeyPress = false;
			mortar.rechargeStock = 1;
			mortar.requiredStock = 1;
			mortar.stockToConsume = 1;
			mortar.skillNameToken = Localization.SKILL_SPECIAL_MORTAR_NAME;
			mortar.skillDescriptionToken = Localization.SKILL_SPECIAL_MORTAR_DESC;
			mortar.icon = Images.MortarIcon;
			SurvivorHollower.AddSkill(_playerBodyPrefab, mortar, SurvivorHollower.SlotType.Special, 1);
			Log.LogTrace("Finished registering Cavitation Mortar.");
			#endregion

			#endregion

			#region Survivor

			Log.LogTrace("Creating SurvivorDef instance...");
			SurvivorDef survivorDef = ScriptableObject.CreateInstance<SurvivorDef>();
			survivorDef.bodyPrefab = _playerBodyPrefab;
			survivorDef.descriptionToken = Localization.SURVIVOR_DESC;
			survivorDef.displayNameToken = Localization.SURVIVOR_NAME;
			survivorDef.cachedName = survivorDef.displayNameToken;
			survivorDef.outroFlavorToken = Localization.SURVIVOR_OUTRO;
			survivorDef.mainEndingEscapeFailureFlavorToken = Localization.SURVIVOR_OUTRO_FAILED;
			survivorDef.displayPrefab = playerBodyDisplayPrefab;
			survivorDef.displayPrefab.transform.localScale = Vector3.one * 0.3f;
			survivorDef.primaryColor = new Color(0.5f, 0.5f, 0.5f);
			survivorDef.desiredSortPosition = 44.45f;
			Jailer = survivorDef;
			ContentAddition.AddSurvivorDef(survivorDef);
			Log.LogTrace("Finished registering survivor...");

			SurvivorHollower.FinalizeBody(_playerBodyPrefab.GetComponent<SkillLocator>());
			Log.LogTrace("Survivor object initialization is complete.");
			#endregion

			#region Survivor Hooks
			Log.LogTrace("Creating method hooks...");
			On.RoR2.HealthComponent.TakeDamage += InterceptTakeDamageForNullBuff;
			On.RoR2.HealthComponent.TakeDamage += DoFuryFracture;
			On.RoR2.ModelLocator.Awake += OnModelLocatorAwakened;
			On.RoR2.CharacterBody.UpdateSingleTemporaryVisualEffect_refTemporaryVisualEffect_GameObject_float_bool_string += OnUpdatingVFX;
			On.RoR2.CharacterBody.OnBuffFirstStackGained += OnBuffGained;
			On.RoR2.CharacterBody.OnBuffFinalStackLost += OnBuffLost;
			Configuration.OnStatConfigChanged += StatChanged;
			Configuration.OnHideNoticeChanged += HideNoticeChanged;
			On.RoR2.SurvivorMannequins.SurvivorMannequinSlotController.RebuildMannequinInstance += OnRebuildingMannequin;
			BodyCatalog.availability.CallWhenAvailable(() => {
				JailerIndex = body.bodyIndex;
				XanVoidAPI.RegisterAsVoidEntity(plugin, JailerIndex);
				Configuration.LateInit(plugin, JailerIndex);
				Localization.ReloadStattedTexts(JailerIndex);
			});
			XanVoidAPI.VerifyProperConstruction(body);
			_survivorReady = true;
			Log.LogTrace("Survivor init complete.");
			#endregion
		}

		private static void HideNoticeChanged(bool hide) {
			_modNotice.additionalPassive.enabled = !hide;
		}

		private static void StatChanged() {
			UpdateSettings(_playerBodyPrefab.GetComponent<CharacterBody>());
		}

		/// <summary>
		/// Utility: Use <see cref="ContentAddition.AddEntityState{T}(out bool)"/> to register the provided entity state type, then return a new instance of <see cref="SerializableEntityStateType"/> using the same type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		private static SerializableEntityStateType UtilCreateSerializableAndNetRegister<T>() where T : EntityState {
			// java when the typeof(T)
			// when
			//  the when the t
			// t
			Log.LogTrace($"Registering EntityState {typeof(T).FullName} and returning a new instance of {nameof(SerializableEntityStateType)} of that type...");
			ContentAddition.AddEntityState<T>(out _);
			return new SerializableEntityStateType(typeof(T));
		}


#pragma warning disable Publicizer001
		
		private static void SetupScale(CharacterBody body) {
			Log.LogTrace("Setting up interactions and scale dependent properties...");
			Interactor interactor = body.GetComponent<Interactor>();
			if (Configuration.CommonVoidEnemyConfigs.UseFullSizeCharacter) {
				Log.LogTrace("Using full size character model. Increasing interaction distance...");
				interactor.maxInteractionDistance = 10f;
			} else {
				Log.LogTrace("Using small character model. Decreasing interaction distance, scaling down model and colliders...");
				interactor.maxInteractionDistance = 5.5f;
				GameObject baseTransform = body.GetComponent<ModelLocator>().modelBaseTransform.gameObject;
				baseTransform.transform.localScale = Vector3.one * 0.5f;
				baseTransform.transform.Translate(new Vector3(0f, 4f, 0f));
				foreach (KinematicCharacterMotor kinematicCharacterMotor in body.GetComponentsInChildren<KinematicCharacterMotor>()) {
					// * 0.4f
					// + 1.25f
					// Up to *0.5f and +1.5f
					// This seems to fix the bungus issue!
					kinematicCharacterMotor.SetCapsuleDimensions(kinematicCharacterMotor.Capsule.radius * 0.5f, kinematicCharacterMotor.Capsule.height * 0.5f, 1.5f);
				}
			}
			Log.LogTrace("Done setting up interactions and scale dependent properties.");
		}


		private static void OnModelLocatorAwakened(On.RoR2.ModelLocator.orig_Awake originalMethod, ModelLocator @this) {
			originalMethod(@this);

			CharacterBody body = @this.GetComponent<CharacterBody>();
			if (body != null) {
				if (body.bodyIndex == JailerIndex) {
					Log.LogTrace("Updating transparency systems...");
					TransparencyController ctr = @this.gameObject.AddComponent<TransparencyController>();
					ctr.getTransparencyInCombat = () => Configuration.CommonVoidEnemyConfigs.TransparencyInCombat;
					ctr.getTransparencyOutOfCombat = () => Configuration.CommonVoidEnemyConfigs.TransparencyOutOfCombat;

					Log.LogTrace("Updating camera systems...");
					CameraController cam = @this.gameObject.AddComponent<CameraController>();
					cam.getCameraOffset = () => Configuration.CommonVoidEnemyConfigs.CameraOffset;
					cam.getCameraPivot = () => Configuration.CommonVoidEnemyConfigs.CameraPivotOffset;
					cam.getUseFullSizeCharacter = () => Configuration.CommonVoidEnemyConfigs.UseFullSizeCharacter;

					Log.LogTrace("Updating body size...");
					UpdateSettings(body);
					SetupScale(body);
				}
			}
		}

		private static void OnBuffGained(On.RoR2.CharacterBody.orig_OnBuffFirstStackGained originalMethod, CharacterBody @this, BuffDef buffDef) {
			originalMethod(@this, buffDef);
			if (buffDef == BuffProvider.Fury) {
				@this.skillLocator.primary.SetSkillOverride(@this, MinigunPrimary, GenericSkill.SkillOverridePriority.Upgrade);
			}
		}

		private static void OnBuffLost(On.RoR2.CharacterBody.orig_OnBuffFinalStackLost originalMethod, CharacterBody @this, BuffDef buffDef) {
			originalMethod(@this, buffDef);
			if (buffDef == BuffProvider.Fury) {
				@this.skillLocator.primary.UnsetSkillOverride(@this, MinigunPrimary, GenericSkill.SkillOverridePriority.Upgrade);
			}
		}

		/// <summary>
		/// Intent: Render brainstalks when the player has the "Fury" effect.
		/// </summary>
		/// <param name="originalMethod"></param>
		/// <param name="this"></param>
		private static void OnUpdatingVFX(On.RoR2.CharacterBody.orig_UpdateSingleTemporaryVisualEffect_refTemporaryVisualEffect_GameObject_float_bool_string originalMethod, CharacterBody @this, ref TemporaryVisualEffect tempEffect, GameObject tempEffectPrefab, float effectRadius, bool active, string childLocatorOverride) {
			if (tempEffectPrefab == CharacterBody.AssetReferences.noCooldownEffectPrefab) {
				active = active || @this.HasBuff(BuffProvider.Fury);
			}
			originalMethod(@this, ref tempEffect, tempEffectPrefab, effectRadius, active, childLocatorOverride);
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
				if (damageInfo.HasModdedDamageType(DamageTypeProvider.PerformsFastFracture)) {
					damageInfo.damage += (damageInfo.damage * Configuration.SpecialRageDamageBoostToNullified);
				} else {
					damageInfo.damage += (damageInfo.damage * Configuration.NullifiedDamageBoost);
				}
			}
			originalMethod(@this, damageInfo);
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

		private static void OnRebuildingMannequin(On.RoR2.SurvivorMannequins.SurvivorMannequinSlotController.orig_RebuildMannequinInstance originalMethod, RoR2.SurvivorMannequins.SurvivorMannequinSlotController @this) {
			originalMethod(@this);
			if (@this.mannequinInstanceTransform != null && @this.currentSurvivorDef == Jailer) {
				Animator previewAnimator = @this.mannequinInstanceTransform.Find("mdlVoidJailer").GetComponent<Animator>();
				previewAnimator.SetBool("isGrounded", true); // Fix an animation bug
				previewAnimator.SetFloat("Spawn1.playbackRate", 0.625f);
				previewAnimator.Play("Spawn1");
				Util.PlaySound("Play_voidJailer_spawn", @this.mannequinInstanceTransform.gameObject);

				Transform effectSpawnerTransform = @this.mannequinInstanceTransform.Find("Portal");
				EffectData fx = new EffectData {
					origin = effectSpawnerTransform.position,
					scale = 2.5f,
					// rootObject = @this.mannequinInstanceTransform.gameObject
				};

				EffectManager.SpawnEffect(EffectProvider.SpawnEffect, fx, false);
			}
		}

#pragma warning restore Publicizer001

	}
}

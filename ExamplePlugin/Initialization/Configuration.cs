using BepInEx;
using BepInEx.Configuration;
using RiskOfOptions.Options;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VoidJailerMod.Damage;
using VoidJailerMod.Initialization.Sprites;
using Xan.ROR2VoidPlayerCharacterCommon;
using Xan.ROR2VoidPlayerCharacterCommon.AdvancedConfigs;
using Xan.ROR2VoidPlayerCharacterCommon.AdvancedConfigs.Networked;
using Xan.ROR2VoidPlayerCharacterCommon.ROOInterop;
using static Xan.ROR2VoidPlayerCharacterCommon.AdvancedConfigs.CommonModAndCharacterConfigs;

namespace VoidJailerMod.Initialization {
	public static class Configuration {

		private static ConfigFile _cfg = null;
		private static AdvancedConfigBuilder _advCfg = null;

		/// <summary>
		/// Common configuration values, such as the character scale and base stats.
		/// </summary>
		[ReplicatedConfiguration]
		public static CommonModAndCharacterConfigs CommonVoidEnemyConfigs { get; private set; }

		#region Primary Attack
		/// <summary>
		/// The common base damage of Void Impulse and Void Spread
		/// </summary>
		public static float BasePrimaryDamage => _basePrimaryDamage.Value;

		/// <summary>
		/// The multiplier applied to damage done to nullified targets when hit with the primary, to encourage using the secondary.
		/// </summary>
		public static float NullifiedDamageBoost => _nullifiedDamageBoost.Value;

		/// <summary>
		/// The amount of projectiles that the primary fire shoots.
		/// </summary>
		public static int BasePrimaryProjectileCount => _basePrimaryProjectiles.Value;

		/// <summary>
		/// If true, Jailer Spike projectiles should be faster.
		/// </summary>
		public static bool FasterPrimaryProjectiles => _primaryProjectileBehavior.Value.HasFlag(AimHelperType.FasterProjectiles);

		/// <summary>
		/// If true, Jailer Spike projectiles should home in on targets.
		/// </summary>
		public static bool HomingPrimaryProjectiles => _primaryProjectileBehavior.Value.HasFlag(AimHelperType.HomingProjectiles);

		/// <summary>
		/// If true, Attack Speed increases the damage of Spike and Perforate rather than making them shoot faster.
		/// </summary>
		public static bool ScaleDamageNotSpeed => _scaleDamageNotSpeed.Value;

		#endregion

		#region Secondary Attack
		/// <summary>
		/// The damage that the secondary attack does, as a proportion of the base damage.
		/// </summary>
		public static float BaseSecondaryDamage => _baseSecondaryDamage.Value;

		/// <summary>
		/// The amount of health you heal when hitting a target with Bind.
		/// </summary>
		public static float SecondaryHealAmountOnHit => _baseSecondarySap.Value;

		/// <summary>
		/// The length of nullify on monsters.
		/// </summary>
		public static float SecondaryNullifyDuration => _secondaryNullifyDuration.Value;

		/// <summary>
		/// The length of nullify on bosses.
		/// </summary>
		public static float SecondaryNullifyBossDuration => _secondaryNullifyBoss.Value;

		/// <summary>
		/// The colldown of Bind
		/// </summary>
		public static float SecondaryCooldown => _secondaryCooldown.Value;

		#endregion

		#region Utility
		/// <summary>
		/// The speed at which the player travels whilst using Dive
		/// </summary>
		public static float UtilitySpeed => _utilitySpeed.Value;

		/// <summary>
		/// The amount of health the player heals when using Dive.
		/// </summary>
		public static float UtilityRegeneration => _utilityRegeneration.Value;

		/// <summary>
		/// The amount of time the player travels for in Dive
		/// </summary>
		public static float UtilityDuration => _utilityDuration.Value;

		/// <summary>
		/// The cooldown of Dive.
		/// </summary>
		public static float UtilityCooldown => _utilityCooldown.Value;
		#endregion

		#region Special

		#region Fury of the Warden

		/// <summary>
		/// How long Rage of the Warden lasts.
		/// </summary>
		public static float SpecialRageDuration => _specialRageDuration.Value;

		/// <summary>
		/// How long Rage of the Warden lasts.
		/// </summary>
		public static float SpecialRageCooldown => _specialRageCooldown.Value;

		/// <summary>
		/// The amount of armor the player receives while under the effects of Rage of the Warden.
		/// </summary>
		public static float SpecialRageArmorBoost => _specialRageArmorBoost.Value;

		/// <summary>
		/// The amount of damage the player can do (multiplicatively) while under the effects of Rage of the Warden.
		/// </summary>
		public static float SpecialRageDamageBoost => _specialRageDamageBoost.Value;

		/// <summary>
		/// Use this in place of <see cref="NullifiedDamageBoost"/> when the player fires a Fury projectile.
		/// </summary>
		public static float SpecialRageDamageBoostToNullified => _specialRageDamageBoostToNullified.Value;

		/// <summary>
		/// The amount of projectiles fired with the special
		/// </summary>
		public static int SpecialRageProjectileCount => _specialRageProjectileCount.Value;

		/// <summary>
		/// The amount of times the ability fires per second when fury is active.
		/// </summary>
		public static float SpecialRageFirerateRPS => _specialRageShotsPerSecond.Value;

		#endregion

		#region Mortar

		/// <summary>
		/// If true, the first mortar that gets fired always has perfect accuracy.
		/// </summary>
		public static bool FirstMortarIsAlwaysAccurate => _mortarSpreadFirstAlwaysAccurate.Value;

		/// <summary>
		/// The minimum and maximum spread of the Mortar special.
		/// </summary>
		public static Vector2 MortarSpread => _mortarSpread.Vector;

		/// <summary>
		/// How much damage Mortar does.
		/// </summary>
		public static float MortarDamage => _mortarDamage.Value;

		/// <summary>
		/// The amount of bombs fired in Mortar.
		/// </summary>
		public static int MortarBombCount => _mortarBombCount.Value;

		/// <summary>
		/// The speed of the Mortar projectiles.
		/// </summary>
		public static float MortarSpeed => _mortarSpeed.Value;

		/// <summary>
		/// If true, the mortar behaves like a conditional void kill bomb. If false, the mortar never void kills.
		/// </summary>
		public static bool MortarCanInstakill => _mortarInstakill.Value;

		/// <summary>
		/// How much gravity affects the mortars.
		/// </summary>
		public static float MortarGravity => _mortarGravity.Value;

		/// <summary>
		/// The cooldown of the mortar.
		/// </summary>
		public static float MortarCooldown => _mortarCooldown.Value;

		/// <summary>
		/// The blast radius of Mortars.
		/// </summary>
		public static float MortarBlastRadius => _mortarBlastRadius.Value;

		/// <summary>
		/// The amount of mortar stocks.
		/// </summary>
		public static int MortarStocks => _mortarStocks.Value;

		#endregion

		#endregion

		#region Misc.

		/// <summary>
		/// Provides additional aim compensation in VR mode.
		/// </summary>
		public static bool VRExtendedAimCompensation => _vrAimCompensation.Value;

		/// <summary>
		/// Provides additional aim compensation in VR mode.
		/// </summary>
		public static ConfigEntry<bool> VRExtendedAimCompensationBacking => _vrAimCompensation;

		/// <summary>
		/// If true, the configuration changes notice on the survivor screen are hidden.
		/// </summary>
		public static bool HideNotice => _hideNotice.Value;

		#endregion

		#region Primary Attack
		[ReplicatedConfiguration]
		private static ReplicatedPercentageWrapper _basePrimaryDamage;

		[ReplicatedConfiguration]
		private static ReplicatedPercentageWrapper _nullifiedDamageBoost;

		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<int> _basePrimaryProjectiles;

		private static ConfigEntry<AimHelperType> _primaryProjectileBehavior;

		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<bool> _scaleDamageNotSpeed;
		#endregion

		#region Secondary Attack
		[ReplicatedConfiguration]
		private static ReplicatedPercentageWrapper _baseSecondaryDamage;
		[ReplicatedConfiguration]
		private static ReplicatedPercentageWrapper _baseSecondarySap;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<float> _secondaryNullifyDuration;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<float> _secondaryNullifyBoss;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<float> _secondaryCooldown;
		#endregion

		#region Utility
		[ReplicatedConfiguration]
		private static ReplicatedPercentageWrapper _utilitySpeed;
		[ReplicatedConfiguration]
		private static ReplicatedPercentageWrapper _utilityRegeneration;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<float> _utilityDuration;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<float> _utilityCooldown;
		#endregion

		#region Special
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<float> _specialRageDuration;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<float> _specialRageArmorBoost;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<int> _specialRageProjectileCount;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<float> _specialRageShotsPerSecond;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<float> _specialRageCooldown;
		[ReplicatedConfiguration]
		private static ReplicatedPercentageWrapper _specialRageDamageBoost;
		[ReplicatedConfiguration]
		private static ReplicatedPercentageWrapper _specialRageDamageBoostToNullified;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<bool> _mortarSpreadFirstAlwaysAccurate;
		[ReplicatedConfiguration]
		private static ReplicatedMinMaxWrapper _mortarSpread;
		[ReplicatedConfiguration]
		private static ReplicatedPercentageWrapper _mortarDamage;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<int> _mortarBombCount;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<float> _mortarSpeed;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<bool> _mortarInstakill;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<float> _mortarGravity;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<float> _mortarCooldown;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<float> _mortarBlastRadius;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<int> _mortarStocks;

		#endregion

		#region Misc.
		private static ConfigEntry<bool> _vrAimCompensation;
		private static ConfigEntry<bool> _hideNotice;
		#endregion

		public static void Init(ConfigFile cfg) {
			_cfg = cfg;

			AdvancedConfigBuilder aCfg = new AdvancedConfigBuilder(typeof(Configuration), cfg, Images.Portrait, VoidJailerPlayerPlugin.PLUGIN_GUID, VoidJailerPlayerPlugin.DISPLAY_NAME, "Play as a Void Jailer!\n\nThese settings include all stats for every single individual component of every ability. Handle with care!");
			_advCfg = aCfg;
			CommonVoidEnemyConfigs = new CommonModAndCharacterConfigs(aCfg, new CommonModAndCharacterConfigs.Defaults {
				BaseMaxHealth = 220f,
				LevelMaxHealth = 30f,
				BaseHPRegen = 1f,
				LevelHPRegen = 0.2f,
				BaseArmor = 40f,
				LevelArmor = 0f,
				BaseMaxShield = 0f,
				LevelMaxShield = 0f,
				BaseMoveSpeed = 7f,
				LevelMoveSpeed = 0f,
				SprintSpeedMultiplier = 1.45f,
				BaseAcceleration = 80f,
				BaseJumpCount = 1,
				BaseJumpPower = 20f,
				LevelJumpPower = 0f,
				BaseAttackSpeed = 1.5f,
				LevelAttackSpeed = 0f,
				BaseDamage = 12f,
				LevelDamage = 2.4f,
				BaseCritChance = 1f,
				LevelCritChance = 0f,
				UseFullSizeCharacter = false,
				TransparencyInCombat = 75,
				TransparencyOutOfCombat = 0,
				CameraPivotOffset = -1.75f,
				CameraOffset = new Vector3(0f, 4f, -14f)
			});

			/*
			_sound = cfg.Bind("Sound", "Sound", "Play_voidRaid_m1_explode", "sound for special");
			RiskOfOptions.ModSettingsManager.AddOption(new StringInputFieldOption(_sound, new RiskOfOptions.OptionConfigs.InputFieldConfig {
				category = "Sound",
				name = "Sound",
				submitOn = RiskOfOptions.OptionConfigs.InputFieldConfig.SubmitEnum.OnExitOrSubmit
			}), VoidJailerPlayerPlugin.PLUGIN_GUID, VoidJailerPlayerPlugin.DISPLAY_NAME);
			*/

			aCfg.SetCategory("Spike");
			_basePrimaryDamage = aCfg.BindFloatPercentageReplicated("Void Dart Damage", "The damage done by an individual Void Dart fired by Spike, as a factor of the character's Base Damage.", 125, 0, 500);
			_nullifiedDamageBoost = aCfg.BindFloatPercentageReplicated("Damage Boost to Nullified", "The damage dealt by Void Darts is changed by this amount when hitting a Nullified target.", 200f, 0f, 1000f);
			_basePrimaryProjectiles = aCfg.BindReplicated("Spike Projectiles", "The amount of Void Darts fired with Spike.", 12, 1, 48);
			_primaryProjectileBehavior = aCfg.BindReplicated("Aim Assist", "To make Void Jailers more familiar to players, tweaks can be done to the primary attack. It is strongly recommended to have some sort of assist enabled.", AimHelperType.HomingProjectiles);
			_scaleDamageNotSpeed = aCfg.BindReplicated("Attack Speed Increases Damage", "An experimental option. Rather than increasing the speed at which shots are fired, Attack Speed can be rigged up to boost the damage dealt by an individual dart. This puts you at a direct disadvantage, but it does look way cooler.", false);

			aCfg.SetCategory("Bind");
			_baseSecondaryDamage = aCfg.BindFloatPercentageReplicated("Bind Damage", "The base damage done by the Bind ability, as a factor of the character's Base Damage.", 80f, 0f, 200f);
			_baseSecondarySap = aCfg.BindFloatPercentageReplicated("Bind Lifesteal", "How much health gets stolen by Bind. This amount is actually a percentage of <i>your</i> max health (basically, you heal by this amount)", 15f);
			_secondaryNullifyDuration = aCfg.BindReplicated("Bind Duration (Monsters)", "How long the Nullify effect lasts when applied on monsters.", 10f, 0f, 30f, 1f, formatString: "{0}s");
			_secondaryNullifyBoss = aCfg.BindReplicated("Bind Duration (Bosses)", "How long the Nullify effect lasts when applied to bosses.", 5f, 0f, 30f, 1f, formatString: "{0}s");
			_secondaryCooldown = aCfg.BindReplicated("Bind Cooldown", "The cooldown of the Bind ability", 4f, 0.5f, 20f, 0.5f, formatString: "{0}s");

			aCfg.SetCategory("Dive");
			_utilitySpeed = aCfg.BindFloatPercentageReplicated("Dive Speed Multiplier", "The speed at which Dive moves the player, as a percentage of the base Walk Speed", 400, 0, 1000);
			_utilityRegeneration = aCfg.BindFloatPercentageReplicated("Dive Regeneration", "How much health is restored upon using Dive.", 10f);
			_utilityDuration = aCfg.BindReplicated("Dive Duration", "How long Dive keeps you away from the world.", 1f, 0.5f, 5f, formatString: "{0}s");
			_utilityCooldown = aCfg.BindReplicated("Dive Cooldown", "The amount of time, in seconds, that the player must wait before one stock of Dive recharges.", 6f, 0.5f, 120f, 0.5f, AdvancedConfigBuilder.RestartType.NextRespawn, "{0}s");

			aCfg.SetCategory("Fury of the Warden");
			_specialRageDuration = aCfg.BindReplicated("Fury Duration", "The duration of the Fury of the Warden effect.", 10f, 0f, 60f, formatString: "{0}s");
			_specialRageArmorBoost = aCfg.BindReplicated("Fury Armor Boost", "How much your Armor increase while under the effects of Fury of the Warden", 200f, 0f, 1000f);
			_specialRageDamageBoost = aCfg.BindFloatPercentageReplicated("Fury Damage Boost", "Perforate's damage is relative to that of Spike. This is the percent damage boost for Perforate. 100% means to be equal to Spike.", 150, 0, 1000);
			_specialRageDamageBoostToNullified = aCfg.BindFloatPercentageReplicated("Fury Damage Boost (Nullified)", "When Fury of the Warden is active, <style=cDeath>this overrides <style=cUserSetting>Damage Boost to Nullified</style> (it does not stack with it)!</style> Generally, this value should be lower to account for the rather extreme boost in power granted normally. A value of 100% means to not affect the damage at all, values less than 100% reduce the damage.", 100, 0, 500);
			_specialRageProjectileCount = aCfg.BindReplicated("Fury Projectile Count", "The amount of projectiles fired per shot whilst Fury of the Warden is active.", 3, 1, 48);
			_specialRageShotsPerSecond = aCfg.BindReplicated("Fury Firerate", "<style=cIsUtility>Measured in shots per second</style>, this is the firerate of the primary when Fury of the Warden is active. This scales with attack speed.", 2.5f, 0.1f, 10f, 0.1f);
			_specialRageCooldown = aCfg.BindReplicated("Fury Cooldown", "The cooldown time of Fury of the Warden", 60f, 0.5f, 180f, 5f, AdvancedConfigBuilder.RestartType.NextRespawn, "{0}s");

			aCfg.SetCategory("Cavitation Mortar");
			_mortarSpread = aCfg.BindMinMaxReplicated("Mortar Spread", "<style=cIsUtility>Measured in degrees</style>. The minimum and maximum inaccuracy of the Cavitation Mortar's projectiles.", 4f, 20f, 0f, 90f, AdvancedConfigBuilder.RestartType.NoRestartRequired, "{0}°");
			_mortarSpreadFirstAlwaysAccurate = aCfg.BindReplicated("Mortar First Accuracy", "If enabled, the first bomblet released by the Cavitation Mortar ability will always go perfectly straight without any spread applied. This helps it feel more consistent.", true);
			_mortarDamage = aCfg.BindFloatPercentageReplicated("Mortar Damage", "The amount of damage that a Cavitation Mortar does as a percentage of base damage.\n\n<style=cDeath>Note:</style> If <style=cUserSetting>Mortar Void Kills</style> is enabled, the <style=cIsVoid>Void Common API Fallback Damage</style> stat is used instead of this value.", 3250, 0, 20000);
			_mortarSpeed = aCfg.BindReplicated("Mortar Speed", "The speed at which the Cavitation Mortars travel after being fired.", 80f, 10f, 100f, 1f);
			_mortarGravity = aCfg.BindReplicated("Mortar Gravity", "The mortar is affected by gravity. This is the vertical force applied to them. Negative values go down, positive goes up.", -15, -100, 0, 5f);
			_mortarBlastRadius = aCfg.BindReplicated("Mortar Blast Radius", "The radius of the explosion created by Cavitation Mortars", 8f, 0.5f, 25f, 0.5f, formatString: "{0}m");
			_mortarStocks = aCfg.BindReplicated("Mortar Stocks", "The default amount of stocks that the Cavitation Mortar comes with.", 2, 1, 10, AdvancedConfigBuilder.RestartType.NextRespawn);
			_mortarCooldown = aCfg.BindReplicated("Mortar Cooldown", "The cooldown of the Cavitation Mortar skill.", 35f, 0f, 180f, 5f, AdvancedConfigBuilder.RestartType.NextRespawn, "{0}s");
			_mortarInstakill = aCfg.BindReplicated("Mortar Void Kills", "If true, the mortar causes Void Death (the type of damage in black holes). <style=cIsVoid>Check the Void Common API tab (or global settings) to see what this will (or will not) instantly kill, as this is controlled by black hole settings when enabled.</style>", false);
			_mortarBombCount = aCfg.BindReplicated("Mortar Bomblet Count", "How many bombs are fired when using Cavitation Mortar.\n\n<style=cDeath>Epilepsy warning!</style> Due to how the Void shader works, values larger than 2 or 3 usually result in <b>extremely bright, flashing lights</b> (as a result of multiple instances of the shader overlapping). ", 3, 1, 4);

			aCfg.SetCategory("VR");
			_vrAimCompensation = aCfg.BindLocal("VR Aim Compensation", "When in VR, the scale of the world often makes it more difficult to aim abilities like Spike correctly. This setting increases the auto aim cone of Spike and Bind by a small amount to make it more comfortable to play in VR. <style=cIsDamage>This does nothing to Spike if Aim Compensation is disabled.</style>", true);

			aCfg.SetCategory("Mod Meta, Graphics, Gameplay");
			_hideNotice = aCfg.BindLocal("Hide Config Notice", "Stops showing the warning on the stats screen that (probably) directed you here in the first place. Changes when you click on a different survivor in the pre-game screen.", false, AdvancedConfigBuilder.RestartType.NoRestartRequired);

			_primaryProjectileBehavior.SettingChanged += OnProjectileBehaviorChanged;

			_basePrimaryDamage.SettingChanged += OnFloatChanged;
			_nullifiedDamageBoost.SettingChanged += OnFloatChanged;
			_basePrimaryProjectiles.SettingChanged += OnReplicatedIntChanged;
			_scaleDamageNotSpeed.SettingChanged += OnReplicatedBoolChanged;
			_baseSecondaryDamage.SettingChanged += OnFloatChanged;
			_baseSecondarySap.SettingChanged += OnFloatChanged;
			_secondaryCooldown.SettingChanged += OnReplicatedFloatChanged;
			_secondaryNullifyDuration.SettingChanged += OnReplicatedFloatChanged;
			_secondaryNullifyBoss.SettingChanged += OnReplicatedFloatChanged;
			_utilitySpeed.SettingChanged += OnFloatChanged;
			_utilityRegeneration.SettingChanged += OnFloatChanged;
			_utilityDuration.SettingChanged += OnReplicatedFloatChanged;
			_utilityCooldown.SettingChanged += OnReplicatedFloatChanged;
			_specialRageDuration.SettingChanged += OnReplicatedFloatChanged;
			_specialRageArmorBoost.SettingChanged += OnReplicatedFloatChanged;
			_specialRageProjectileCount.SettingChanged += OnReplicatedIntChanged;
			_specialRageShotsPerSecond.SettingChanged += OnReplicatedFloatChanged;
			_specialRageCooldown.SettingChanged += OnReplicatedFloatChanged;
			_mortarSpread.SettingChanged += OnMinMaxChanged;
			_mortarSpreadFirstAlwaysAccurate.SettingChanged += OnReplicatedBoolChanged;
			_mortarDamage.SettingChanged += OnFloatChanged;
			_mortarBombCount.SettingChanged += OnReplicatedIntChanged;
			_mortarSpeed.SettingChanged += OnReplicatedFloatChanged;
			_mortarInstakill.SettingChanged += OnReplicatedBoolChanged;
			_mortarGravity.SettingChanged += OnReplicatedFloatChanged;
			_mortarCooldown.SettingChanged += OnReplicatedFloatChanged;
			_mortarBlastRadius.SettingChanged += OnReplicatedFloatChanged;
			_mortarStocks.SettingChanged += OnReplicatedIntChanged;

			_hideNotice.SettingChanged += (_, _) => OnHideNoticeChanged?.Invoke(_hideNotice.Value);

			aCfg.CreateConfigAutoReplicator();
		}

		private static void OnProjectileBehaviorChanged(object sender, EventArgs e) {
			ProjectileProvider.UpdateProjectilePrefabs();
		}

		internal static void LateInit(BaseUnityPlugin registrar, BodyIndex bodyIndex) {
			XanVoidAPI.CreateAndRegisterBlackHoleBehaviorConfigs(registrar, _advCfg, bodyIndex);
		}

		private static void OnAnyChanged() => OnStatConfigChanged?.Invoke();
		private static void OnBoolChanged(bool value) => OnStatConfigChanged?.Invoke();
		private static void OnFloatChanged(float value) => OnStatConfigChanged?.Invoke();
		private static void OnIntChanged(float value) => OnStatConfigChanged?.Invoke();
		private static void OnMinMaxChanged(float min, float max) => OnStatConfigChanged?.Invoke();
		private static void OnVectorChanged(Vector3 vector) => OnStatConfigChanged?.Invoke();
		private static void OnReplicatedBoolChanged(bool value, bool fromHost) => OnStatConfigChanged?.Invoke();
		private static void OnReplicatedFloatChanged(float value, bool fromHost) => OnStatConfigChanged?.Invoke();
		private static void OnReplicatedIntChanged(int value, bool fromHost) => OnStatConfigChanged?.Invoke();


		/// <summary>
		/// Fires when any config that pertains to stats changes.
		/// </summary>
		public static event StatConfigChanged OnStatConfigChanged;

		/// <summary>
		/// This event fires when the desire to hide the notice changes.
		/// </summary>
		public static event Action<bool> OnHideNoticeChanged;

		[Flags]
		private enum AimHelperType {
			None,
			FasterProjectiles,
			HomingProjectiles,
			Both
		}

	}
}

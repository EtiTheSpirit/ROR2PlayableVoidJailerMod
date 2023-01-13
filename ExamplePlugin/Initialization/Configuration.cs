using BepInEx.Configuration;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using VoidJailerMod.XansTools;

namespace VoidJailerMod {
	public static class Configuration {

		private static ConfigFile _cfg = null;

		#region Configuration Entries

		#region Public Properties

		#region Base Stats

		public static float BaseMaxHealth => _baseMaxHealth.Value;
		public static float LevelMaxHealth => _levelMaxHealth.Value;

		public static float BaseMoveSpeed => _baseMoveSpeed.Value;
		public static float LevelMoveSpeed => _levelMoveSpeed.Value;

		public static float SprintSpeedMultiplier => _sprintSpeedMultiplier.Value;

		public static float BaseHPRegen => _baseHPRegen.Value;
		public static float LevelHPRegen => _levelHPRegen.Value;

		public static float BaseArmor => _baseArmor.Value;
		public static float LevelArmor => _levelArmor.Value;

		public static float BaseDamage => _baseDamage.Value;
		public static float LevelDamage => _levelDamage.Value;

		public static float BaseCritChance => _baseCritChance.Value;
		public static float LevelCritChance => _levelCritChance.Value;

		public static float BaseMaxShield => _baseMaxShield.Value;
		public static float LevelMaxShield => _levelMaxShield.Value;

		public static float BaseAcceleration => _baseAcceleration.Value;

		public static int BaseJumpCount => _baseJumpCount.Value;

		public static float BaseJumpPower => _baseJumpPower.Value;
		public static float LevelJumpPower => _levelJumpPower.Value;

		public static float BaseAttackSpeed => _baseAttackSpeed.Value;
		public static float LevelAttackSpeed => _levelAttackSpeed.Value;

		#endregion

		#region Primary Attack
		/// <summary>
		/// The common base damage of Void Impulse and Void Spread
		/// </summary>
		public static float BasePrimaryDamage => _basePrimaryDamage.Value;

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


		public static float SecondaryNullifyDuration => _secondaryNullifyDuration.Value;

		public static float SecondaryNullifyBossDuration => _secondaryNullifyBoss.Value;
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
		#endregion

		#region Special

		/// <summary>
		/// How long Rage of the Warden lasts.
		/// </summary>
		public static float SpecialDuration => _specialDuration.Value;

		/// <summary>
		/// The amount of armor the player receives while under the effects of Rage of the Warden.
		/// </summary>
		public static float SpecialArmorBoost => _specialArmorBoost.Value;

		/// <summary>
		/// The amount of damage the player can do (multiplicatively) while under the effects of Rage of the Warden.
		/// </summary>
		public static float SpecialDamageBoost => _specialDamageBoost.Value;

		#endregion

		#region Misc.

		/// <summary>
		/// [Experimental] Allow the scale of the player character to be identical to that of a real reaver.
		/// </summary>
		public static bool UseFullSizeCharacter => _useFullSizeCharacter.Value;

		/// <summary>
		/// If true, the character should be immune to the effects of Void Fog, and the passive damage from void atmospheres (such as the interior of Void Seeds, the ambient atmosphere of the Void Fields and Locus, etc.)
		/// </summary>
		public static bool VoidImmunity => _voidImmunity.Value;

		/// <summary>
		/// If true, debug logging should be done.
		/// </summary>
		public static bool TraceLogging => _traceLogging.Value;

		/// <summary>
		/// The current camera offset preference.
		/// </summary>
		public static Vector3 CameraOffset => new Vector3(_camOffsetX.Value, _camOffsetY.Value, _camOffsetZ.Value);

		/// <summary>
		/// The offset of the camera's pivot point.
		/// </summary>
		public static float CameraPivotOffset => _cameraPivotOffset.Value;

		public static float LocalTransparencyInCombat => _transparencyInCombat.Value;

		public static float LocalTransparencyOutOfCombat => _transparencyOutOfCombat.Value;

		#endregion

		#endregion

		#region Backing Settings Objects
		#region Base Stats
		private static ConfigEntry<float> _baseMaxHealth;
		private static ConfigEntry<float> _levelMaxHealth;

		private static ConfigEntry<float> _baseMoveSpeed;
		private static ConfigEntry<float> _levelMoveSpeed;

		private static ConfigEntry<float> _sprintSpeedMultiplier;

		private static ConfigEntry<float> _baseHPRegen;
		private static ConfigEntry<float> _levelHPRegen;

		private static ConfigEntry<float> _baseArmor;
		private static ConfigEntry<float> _levelArmor;

		private static ConfigEntry<float> _baseDamage;
		private static ConfigEntry<float> _levelDamage;

		private static ConfigEntry<float> _baseCritChance;
		private static ConfigEntry<float> _levelCritChance;

		private static ConfigEntry<float> _baseMaxShield;
		private static ConfigEntry<float> _levelMaxShield;

		private static ConfigEntry<float> _baseAcceleration;

		private static ConfigEntry<int> _baseJumpCount;

		private static ConfigEntry<float> _baseJumpPower;
		private static ConfigEntry<float> _levelJumpPower;

		private static ConfigEntry<float> _baseAttackSpeed;
		private static ConfigEntry<float> _levelAttackSpeed;


		#endregion

		#region Primary Attack
		private static ConfigEntry<float> _basePrimaryDamage;
		private static ConfigEntry<int> _basePrimaryProjectiles;
		private static ConfigEntry<AimHelperType> _primaryProjectileBehavior;
		#endregion

		#region Secondary Attack
		private static ConfigEntry<float> _baseSecondaryDamage;
		private static ConfigEntry<float> _baseSecondarySap;
		private static ConfigEntry<float> _secondaryNullifyDuration;
		private static ConfigEntry<float> _secondaryNullifyBoss;
		#endregion

		#region Utility
		private static ConfigEntry<float> _utilitySpeed;
		private static ConfigEntry<float> _utilityRegeneration;
		private static ConfigEntry<float> _utilityDuration;
		#endregion

		#region Special
		private static ConfigEntry<float> _specialDuration;
		private static ConfigEntry<float> _specialArmorBoost;
		private static ConfigEntry<float> _specialDamageBoost;

		#endregion

		#region Misc.
		private static ConfigEntry<bool> _useFullSizeCharacter;
		private static ConfigEntry<bool> _voidImmunity;
		private static ConfigEntry<bool> _traceLogging;
		#endregion

		#region Camera
		private static ConfigEntry<float> _camOffsetX;
		private static ConfigEntry<float> _camOffsetY;
		private static ConfigEntry<float> _camOffsetZ;
		private static ConfigEntry<float> _cameraPivotOffset;
		private static ConfigEntry<int> _transparencyInCombat;
		private static ConfigEntry<int> _transparencyOutOfCombat;
		#endregion

		#endregion

		#endregion

		#region Backing Code

		/// <summary>
		/// Casts <see cref="ConfigEntryBase.DefaultValue"/> into the type represented by a <see cref="ConfigEntry{T}"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cfg"></param>
		/// <returns></returns>
		private static T DefVal<T>(this ConfigEntry<T> cfg) where T : struct => (T)cfg.DefaultValue;

		private const string FMT_DEFAULT = "The base {0} that the character has on a new run.";
		private const string FMT_LEVELED = "For each level the player earns, their {0} increases by this amount.";
		private const string FMT_DAMAGE = "This value times the base damage determines the damage done by {0}.";
		private const string FMT_COOLDOWN = "The amount of time, in seconds, that the user must wait before {0} recharges.";
		private const string FMT_COORDINATE = "The {0} offset of the camera when using this character.\n\nNOTE: THIS VALUE IS FOR THE FULL SIZE CHARACTER (IT IS DIVIDED BY 2 FOR THE SMALL CHARACTER)";
		private const string FMT_TRANSPARENCY = "The transparency of the character when you are {0}.\n\nThis can be used to make it easier to see enemies by making your body transparent to prevent it from getting in the way.\n\nA value of 0 means fully opaque, and a value of 100 means as invisible as possible.";

#pragma warning disable CS0618
		/// <summary>
		/// An alias to declare a <see cref="ConfigDefinition"/> based on what limit types to include.<para/>
		/// This is a lazy "solution" to custom limits not working very well.
		/// </summary>
		/// <param name="desc">The description of the setting.</param>
		/// <param name="limit">The limit for the setting, which may or may not actually be used.</param>
		/// <returns></returns>
		public static ConfigDescription StaticDeclareConfigDescription(string desc, AcceptableValueBase limit = null) {
			return new ConfigDescription(desc, limit);
		}

		private static ConfigEntry<T> Bind<T>(string category, string name, T def, ConfigDescription desc = null) {
			Log.LogTrace($"Registering configuration entry \"{name}\" in category \"{category}\" with a default value of: {def}");
			return _cfg.Bind(category, name, def, desc);
		}
		private static ConfigEntry<T> Bind<T>(string category, string name, T def, string desc) => Bind(category, name, def, new ConfigDescription(desc));

		private static AcceptableValueRange<float> MinOnlyF(float min = 0) {
			return new AcceptableValueRange<float>(min, float.MaxValue);
		}
		private static AcceptableValueRange<int> MinOnlyI(int min = 0) {
			return new AcceptableValueRange<int>(min, int.MaxValue);
		}

		private const float CAM_RANGE = 30f;
		private static ConfigEntry<float> MakeCameraCoordEntry(string axis, string axisDesc, float defaultValue) {
			ConfigEntry<float> entry = Bind("0. Mod Meta Settings", $"Camera {axis} Offset", defaultValue, StaticDeclareConfigDescription(string.Format(FMT_COORDINATE, $"{axis} ({axisDesc})"), new AcceptableValueRange<float>(-CAM_RANGE, CAM_RANGE)));
			RiskOfOptions.ModSettingsManager.AddOption(new StepSliderOption(entry, new StepSliderConfig {
				name = entry.Definition.Key,
				description = entry.Description.Description,
				category = "Camera and Visuals",
				formatString = "{0}m",
				min = -CAM_RANGE,
				max = CAM_RANGE,
				increment = 0.125f,
				restartRequired = false
			}));
			return entry;
		}

		private static ConfigEntry<int> MakeFloat01Entry(string name, string desc, int defVal) {
			ConfigEntry<int> entry = Bind("0. Mod Meta Settings", name, defVal, StaticDeclareConfigDescription(desc, new AcceptableValueRange<int>(0, 100)));
			RiskOfOptions.ModSettingsManager.AddOption(new IntSliderOption(entry, new IntSliderConfig {
				name = name,
				description = desc,
				category = "Camera and Visuals",
				min = 0,
				max = 100,
				formatString = "{0}",
				restartRequired = false
			}));
			return entry;
		}

		internal static void Init(ConfigFile cfg) {
			if (_cfg != null) throw new InvalidOperationException($"{nameof(Configuration)} has already been initialized!");
			_cfg = cfg;

			// The odd one out:
			_traceLogging = cfg.Bind("0. Mod Meta Settings", "Trace Logging", false, "If true, trace logging is enabled. Your console will practically be spammed as the mod gives status updates on every little thing it's doing, but it will help to diagnose weird issues. Consider using this when bug hunting!");
			_camOffsetX = MakeCameraCoordEntry("X", "-left / +right", 0f);
			_camOffsetY = MakeCameraCoordEntry("Y", "-down / +up", 4f);
			_camOffsetZ = MakeCameraCoordEntry("Z", "-backward / +forward", -14f);

			_cameraPivotOffset = cfg.Bind("0. Mod Meta Settings", "Camera Pivot Offset", -1.75f, StaticDeclareConfigDescription("The vertical offset of the camera's pivot point, or, the point it rotates around. NOTE: THIS VALUE IS FOR THE FULL SIZE CHARACTER (IT IS DIVIDED BY 2 FOR THE SMALL CHARACTER)", new AcceptableValueRange<float>(-4, 4)));
			RiskOfOptions.ModSettingsManager.AddOption(new StepSliderOption(_cameraPivotOffset, new StepSliderConfig {
				name = _cameraPivotOffset.Definition.Key,
				description = _cameraPivotOffset.Description.Description,
				category = "Camera and Visuals",
				formatString = "{0}m",
				min = -4,
				max = 4,
				increment = 0.125f,
				restartRequired = false
			}));

			_transparencyInCombat = MakeFloat01Entry("Transparency In Danger", string.Format(FMT_TRANSPARENCY, "in combat"), 75);
			_transparencyOutOfCombat = MakeFloat01Entry("Transparency Out Of Danger", string.Format(FMT_TRANSPARENCY, "not in combat"), 25);

			cfg.SettingChanged += OnSettingChanged;

			// TODO: I would *like* to get RiskOfOptions support but there are two critical issues preventing that

			_baseMaxHealth = Bind("1. Character Stats", "Base Maximum Health", 200f, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "maximum health"), MinOnlyF(1f)));
			_levelMaxHealth = Bind("1. Character Stats", "Leveled Maximum Health", 40f, StaticDeclareConfigDescription(string.Format(FMT_LEVELED, "maximum health"), MinOnlyF()));
			_baseHPRegen = Bind("1. Character Stats", "Base Health Regeneration Rate", 1f, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "health regeneration"), MinOnlyF()));
			_levelHPRegen = Bind("1. Character Stats", "Leveled Health Regeneration Rate", 0.2f, StaticDeclareConfigDescription(string.Format(FMT_LEVELED, "health regeneration"), MinOnlyF()));
			_baseArmor = Bind("1. Character Stats", "Base Armor", 20f, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "armor"), MinOnlyF()));
			_levelArmor = Bind("1. Character Stats", "Leveled Armor", 0f, StaticDeclareConfigDescription(string.Format(FMT_LEVELED, "armor"), MinOnlyF()));
			_baseMaxShield = Bind("1. Character Stats", "Base Maximum Shield", 0f, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "maximum shield"), MinOnlyF()));
			_levelMaxShield = Bind("1. Character Stats", "Leveled Maximum Shield", 0f, StaticDeclareConfigDescription(string.Format(FMT_LEVELED, "maximum shield"), MinOnlyF()));

			_baseMoveSpeed = Bind("2. Character Agility", "Base Movement Speed", 7f, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "walk speed"), MinOnlyF(0f)));
			_levelMoveSpeed = Bind("2. Character Agility", "Leveled Movement Speed", 0f, StaticDeclareConfigDescription(string.Format(FMT_LEVELED, "walk speed"), MinOnlyF()));
			_sprintSpeedMultiplier = Bind("2. Character Agility", "Sprint Speed Multiplier", 1.45f, StaticDeclareConfigDescription("Your sprint speed is equal to your Base Movement Speed times this value.", MinOnlyF()));
			_baseAcceleration = Bind("2. Character Agility", "Acceleration Factor", 80f, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "movement acceleration") + " This value represents how quickly you speed up. Low values make it much like walking on ice.", MinOnlyF()));
			_baseJumpCount = Bind("2. Character Agility", "Jump Count", 1, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "amount of jumps"), MinOnlyI(1)));
			_baseJumpPower = Bind("2. Character Agility", "Base Jump Power", 20f, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "amount of upward jump force"), MinOnlyF()));
			_levelJumpPower = Bind("2. Character Agility", "Leveled Jump Power", 0f, StaticDeclareConfigDescription(string.Format(FMT_LEVELED, "amount of upward jump force"), MinOnlyF()));

			_basePrimaryDamage = Bind("3a. Character Primary", "Primary Damage", 2f, StaticDeclareConfigDescription(string.Format(FMT_DAMAGE, "Spike"), MinOnlyF()));
			_basePrimaryProjectiles = Bind("3a. Character Primary", "Default Projectile Count", 12, StaticDeclareConfigDescription("The amount of Projectiles used in the Spike ability.", MinOnlyI(1)));
			_primaryProjectileBehavior = Bind("3a. Character Primary", "Projectile Behavior", AimHelperType.HomingProjectiles, StaticDeclareConfigDescription("All other survivors with slow projectiles usually accompany them with auto-aim/homing. This setting can be used to alter the Jailer's projectiles to be faster, to follow targets, or both."));
			// Cooldown?

			_baseSecondaryDamage = Bind("3b. Character Secondary", "Secondary Damage", 0.8f, StaticDeclareConfigDescription(string.Format(FMT_DAMAGE, "Bind"), MinOnlyF()));
			_baseSecondarySap = Bind("3b. Character Secondary", "Lifesteal Percentage", 0.05f, StaticDeclareConfigDescription("The amount of health that you heal when using Bind, as a percentage of your maximum health.", new AcceptableValueRange<float>(0f, 1f)));
			_secondaryNullifyDuration = Bind("3b. Character Secondary", "Nullify Duration", 6f, StaticDeclareConfigDescription("The duration that Nullify lasts for on enemies when hit with Bind.", MinOnlyF(0)));
			_secondaryNullifyBoss = Bind("3b. Character Secondary", "Nullify Duration (Bosses)", 3f, StaticDeclareConfigDescription("The duration that Nullify lasts for on bosses hit with Bind. Set to 0 to prevent it entirely.", MinOnlyF(0)));

			_utilitySpeed = Bind("3c. Character Utility", "Dive Speed", 4f, StaticDeclareConfigDescription("The speed at which Dive moves you, as a multiplied factor of your current movement speed.", MinOnlyF()));
			_utilityRegeneration = Bind("3c. Character Utility", "Dive Health Regeneration", 0.1f, StaticDeclareConfigDescription("The amount of health that dive regenerates, as a percentage.", new AcceptableValueRange<float>(0f, 1f)));
			_utilityDuration = Bind("3c. Character Utility", "Dive Duration", 1f, StaticDeclareConfigDescription("The amount of time, in seconds, that Dive hides and moves the player for.", MinOnlyF()));

			_specialDuration = Bind("3d. Character Special", "Rage Duration", 10f, StaticDeclareConfigDescription("The amount of time, in seconds, that Rage of the Warden lasts.", MinOnlyF()));
			_specialArmorBoost = Bind("3d. Character Special", "Rage Armor Boost", 100f, StaticDeclareConfigDescription("The amount of armor that the player earns while under the effects of Rage of the Warden", MinOnlyF()));
			_specialDamageBoost = Bind("3d. Character Special", "Rage Damage Boost", 3f, StaticDeclareConfigDescription("The amount of damage that the player gets while under the effects of Rage of the Warden. All damage is multiplied by this value.", MinOnlyF()));
			// _specialExtraProjectiles = Bind("3d. Character Special", "Rage Additional Projectiles", 3f, StaticDeclareConfigDescription("The amount of projectiles that the primary fires is multiplied by this amount (and rounded) while under the effects of Rage of the Warden.", MinOnlyF()));

			_baseDamage = Bind("4. Character Combat", "Base Damage", 7f, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "damage output") + " Other damage values are multiplied with this.", MinOnlyF()));
			_levelDamage = Bind("4. Character Combat", "Leveled Damage", 2.4f, StaticDeclareConfigDescription(string.Format(FMT_LEVELED, "damage output") + " Other damage values are multiplied with this.", MinOnlyF()));
			_baseCritChance = Bind("4. Character Combat", "Base Crit Chance", 0f, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "critical hit chance") + " This is an integer percentage from 0 to 100, not 0 to 1.", new AcceptableValueRange<float>(0, 100)));
			_levelCritChance = Bind("4. Character Combat", "Leveled Crit Chance", 0f, StaticDeclareConfigDescription(string.Format(FMT_LEVELED, "critical hit chance") + " This is an integer percentage from 0 to 100, not 0 to 1.", new AcceptableValueRange<float>(0, 100)));
			_baseAttackSpeed = Bind("4. Character Combat", "Base Attack Speed", 1.45f, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "attack rate"), MinOnlyF()));
			_levelAttackSpeed = Bind("4. Character Combat", "Leveled Attack Speed", 0f, StaticDeclareConfigDescription(string.Format(FMT_LEVELED, "attack rate"), MinOnlyF()));

			_useFullSizeCharacter = Bind("5. Character Specifics", "Use Full Size Jailer", false, "This MUST be synchronized between players in multiplayer, or people will sink into the floor / float! By default, the mod sets the Jailer's scale to 50% that of its natural size. Turning this on will make you the same size as a normal Jailer. **WARNING** This setting is known to cause collision issues. Some areas are impossible to reach.");
			_voidImmunity = Bind("5. Character Specifics", "Void Immunity", true, "If enabled, the player will be immune to damage from a void atmosphere and will not have the fog effect applied to them. **WARNING** There isn't actually a way to tell if you are taking damage from the void. The way I do it is an educated guess. This means you may actually resist completely valid damage types from some enemies, but I have yet to properly test this.");

			RiskOfOptions.ModSettingsManager.AddOption(new CheckBoxOption(_useFullSizeCharacter, new CheckBoxConfig {
				name = "Use Full Size Character",
				description = "<style=cDeath>This MUST be synchronized between players in multiplayer, or people will sink into the floor / float!</style>\n\nBy default, the mod sets the Jailer's scale to 50% that of its natural size. Turning this on will make you the same size as a normal Jailer.\n\n<style=cIsDamage>This setting is known to cause collision issues. Additionally, some areas are impossible to reach, making it possible to softlock yourself.</style>",
				category = "Character",
				restartRequired = true
			}));

			Log.LogInfo("User configs initialized.");
		}

		private static void OnSettingChanged(object sender, SettingChangedEventArgs e) {
			if (e.ChangedSetting == _camOffsetX || e.ChangedSetting == _camOffsetY || e.ChangedSetting == _camOffsetZ || e.ChangedSetting == _cameraPivotOffset) {
				CharacterBody clientPlayerBody = Ext.ClientPlayerBody;
				if (clientPlayerBody) {
					CameraTargetParams tParams = clientPlayerBody.GetComponent<CameraTargetParams>();
					if (tParams) {
						tParams.cameraParams.data.idealLocalCameraPos = CameraOffset / (UseFullSizeCharacter ? 1f : 2f);
						tParams.cameraParams.data.pivotVerticalOffset = CameraPivotOffset / (UseFullSizeCharacter ? 1f : 2f);
					}
				}

			}
		}
#pragma warning restore CS0618

		#endregion

		[Flags]
		private enum AimHelperType {
			None,
			FasterProjectiles,
			HomingProjectiles,
			Both
		}
	}
}

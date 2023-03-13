using R2API;
using RoR2;
using RoR2.Skills;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VoidJailerMod.XansTools.ROR2VoidReaverModFixed.XanCode.Data;
using VRAPI;

namespace VoidJailerMod.XansTools {
	internal static class ROR2ObjectCreator {

		/// <summary>
		/// Added by Xan.<br/>
		/// Creates a <see cref="SkillFamily"/> with one variant slot. It has no other data defined.
		/// </summary>
		/// <returns></returns>
		private static SkillFamily CreateSingleVariantFamily() {
			// No ContentAddition here!
			Log.LogTrace("Creating single-variant SkillFamily.");
			SkillFamily skillFamily = ScriptableObject.CreateInstance<SkillFamily>();
			skillFamily.variants = new SkillFamily.Variant[1];
			return skillFamily;
		}

#pragma warning disable Publicizer001
		/// <summary>
		/// Designed by LuaFubuki, modified by Xan.<br/>
		/// Creates a new container for a <see cref="CharacterBody"/> and sets up its default, blank skills.
		/// </summary>
		/// <param name="bodyReplacementName">The name of the body prefab.</param>
		/// <param name="bodyDir">The location of the body prefab.</param>
		/// <returns></returns>
		public static GameObject CreateBody(string bodyReplacementName, string bodyDir) {
			Log.LogTrace($"Creating new CharacterBody of {bodyDir} as {bodyReplacementName}");
			GameObject newBody = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(bodyDir).WaitForCompletion(), bodyReplacementName);

			Log.LogTrace("Destroying all pre-existing skill instances of the duplicate body...");
			foreach (GenericSkill preExistingSkill in newBody.GetComponentsInChildren<GenericSkill>()) {
				Object.DestroyImmediate(preExistingSkill);
			}

			Log.LogTrace("Clearing DeathRewards (if present)...");
			// Prevent it from being classified as a monster.
			DeathRewards deathRewards = newBody.GetComponent<DeathRewards>();
			if (deathRewards != null) Object.Destroy(deathRewards);

			Log.LogTrace("Clearing SkillLocator for new skills...");
			SkillLocator skillLocator = newBody.GetComponent<SkillLocator>();
			skillLocator.allSkills = System.Array.Empty<GenericSkill>();
			skillLocator.primary = newBody.AddComponent<GenericSkill>();
			skillLocator.secondary = newBody.AddComponent<GenericSkill>();
			skillLocator.utility = newBody.AddComponent<GenericSkill>();
			skillLocator.special = newBody.AddComponent<GenericSkill>();

			Log.LogTrace("Adding single-variant placeholder skills...");
			SkillFamily primaryFamily = CreateSingleVariantFamily();
			SkillFamily secondaryFamily = CreateSingleVariantFamily();
			SkillFamily utilityFamily = CreateSingleVariantFamily();
			SkillFamily specialFamily = CreateSingleVariantFamily();

			Log.LogTrace("Assigning empty skills...");
			skillLocator.primary._skillFamily = primaryFamily;
			skillLocator.secondary._skillFamily = secondaryFamily;
			skillLocator.utility._skillFamily = utilityFamily;
			skillLocator.special._skillFamily = specialFamily;

			Log.LogTrace("Body instantiated.");
			return newBody;
		}

		/// <summary>
		/// Added by Xan.
		/// Intended to be called after the Body has all of its stuff attached, this prevents an error in the console that would
		/// result in the rejection of these skill families being added to game data.
		/// </summary>
		/// <param name="skillLocator"></param>
		public static void FinalizeBody(SkillLocator skillLocator) {
			Log.LogTrace("Finalizing body by using ContentAddition to register all skills...");
			ContentAddition.AddSkillFamily(skillLocator.primary._skillFamily);
			ContentAddition.AddSkillFamily(skillLocator.secondary._skillFamily);
			ContentAddition.AddSkillFamily(skillLocator.utility._skillFamily);
			ContentAddition.AddSkillFamily(skillLocator.special._skillFamily);
		}
		/// <summary>
		/// Adds a skill variant to the given slot of a <see cref="CharacterBody"/>-containing <see cref="GameObject"/>.
		/// Originally written by LuaFubuki but redone by Xan.
		/// </summary>
		/// <param name="bodyContainer">The <see cref="GameObject"/> that has a <see cref="CharacterBody"/> component on it.</param>
		/// <param name="definition">The actual skill to add.</param>
		/// <param name="slotName">The name of the slot. This must be <c>primary</c>, <c>secondary</c>, <c>utility</c>, or <c>special</c>.</param>
		/// <param name="variantIndex">The index of this variant. If this index is larger than the number of variants the <see cref="SkillFamily"/> can contain, its array is resized.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">If <paramref name="slotName"/> is not valid.</exception>
#pragma warning disable IDE0066 // The version of C# we are using does not support switch expressions.
		public static void AddSkill(GameObject bodyContainer, SkillDef definition, string slotName = "primary", int variantIndex = 0) {
			Log.LogTrace($"Adding a skill to this character's {slotName} skill slot. Registering skill to ContentAddition...");
			ContentAddition.AddSkillDef(definition);

			SkillLocator skillLocator = bodyContainer.GetComponent<SkillLocator>();
			GenericSkill target;
			slotName = slotName.ToLower();
			switch (slotName) {
				case "primary":
					target = skillLocator.primary;
					break;
				case "secondary":
					target = skillLocator.secondary;
					break;
				case "utility":
					target = skillLocator.utility;
					break;
				case "special":
					target = skillLocator.special;
					break;
				default:
					throw new System.ArgumentOutOfRangeException(nameof(slotName), "Invalid slot name! Expecting either \"primary\", \"secondary\", \"utility\", or \"special\"!");
			}

			Log.LogTrace("Locating Skill Family...");
			SkillFamily family = target.skillFamily;
			SkillFamily.Variant[] variants = family.variants;
			if (variants.Length <= variantIndex) {
				Log.LogTrace("Expanding Skill Family Variants array...");
				System.Array.Resize(ref variants, variantIndex + 1);
			}
			SkillFamily.Variant newVariant = default;
			newVariant.skillDef = definition;
			newVariant.viewableNode = new ViewablesCatalog.Node(definition.skillName + "_VIEW", false, null);
			variants[variantIndex] = newVariant;
			family.variants = variants;
			Log.LogTrace($"Done. Appended new skill in slot \"{slotName}\": {definition.skillNameToken}");
		}
#pragma warning restore IDE0066

		public static void AddNewHiddenSkill(GameObject bodyContainer, SkillDef definition) {
			Log.LogTrace($"Adding a hidden skill to this character. Registering skill to ContentAddition...");
			ContentAddition.AddSkillDef(definition);

			Log.LogTrace("Setting Skill Family...");
			SkillFamily family = ScriptableObject.CreateInstance<SkillFamily>();
			SkillFamily.Variant[] variants = family.variants;
			
			Log.LogTrace("Resizing Skill Family Variants array...");
			System.Array.Resize(ref variants, 1);
			
			SkillFamily.Variant newVariant = default;
			newVariant.skillDef = definition;
			newVariant.viewableNode = null;//new ViewablesCatalog.Node(definition.skillName + "_VIEW", false, null);
			variants[0] = newVariant;
			family.variants = variants;
			ContentAddition.AddSkillFamily(family);
			Log.LogTrace($"Done. Appended new hidden skill in no slot: {definition.skillNameToken}");
		}

		/// <summary>
		/// This is used to swap transparency on the skin materials.
		/// </summary>
		public static readonly Material[] VoidJailerSkinMaterials = new Material[6];

		/// <summary>
		/// Globally modify the shader used on the Void Jailer to enable or disable dithered transparency.
		/// This is done because it tends to become transparent in the character selection screen (which is not desirable, it must be opaque)
		/// but also because it needs to be transparent in game (in which opaqueness causes problems for users where they cannot see in front of them).
		/// </summary>
		/// <param name="isTransparent"></param>
		public static void GloballySetJailerSkinTransparency(bool isTransparent) {
			for (int index = 0; index < VoidJailerSkinMaterials.Length; index++) {
				if (isTransparent) {
					VoidJailerSkinMaterials[index].EnableKeyword("DITHER");
					VoidJailerSkinMaterials[index].SetFloat("_DitherOn", 1f);
				} else {
					VoidJailerSkinMaterials[index].DisableKeyword("DITHER");
					VoidJailerSkinMaterials[index].SetFloat("_DitherOn", 0f);
				}
			}
		}

		/// <summary>
		/// Added by Xan. Temporary solution for the lack of skins on the model.
		/// </summary>
		/// <param name="bodyContainer">The prefab containing the character body and all the related stuff.</param>
		public static void AddJailerSkins(GameObject bodyContainer) {
			Renderer[] renderers = bodyContainer.GetComponentsInChildren<Renderer>();
			ModelLocator component = bodyContainer.GetComponent<ModelLocator>();
			GameObject effectiveRoot = component.modelTransform.gameObject;

			// Clone the materials because I change some parameters
			Log.LogTrace("Cloning the default materials of the Jailer...");
			Material mtl0 = new Material(renderers[0].material);
			Material mtl1 = new Material(renderers[1].material);
			Material mtl2 = new Material(renderers[2].material);
			VoidJailerSkinMaterials[0] = mtl0;
			VoidJailerSkinMaterials[1] = mtl1;
			VoidJailerSkinMaterials[2] = mtl2;


			Log.LogTrace("Instantiating the default skin...");
			LoadoutAPI.SkinDefInfo defaultSkin = new LoadoutAPI.SkinDefInfo {
				Icon = SkinIconCreator.CreateSkinIcon(
					new Color32(24, 1, 33, 255),
					new Color32(52, 84, 108, 255),
					new Color32(239, 151, 227, 255),
					new Color32(11, 34, 127, 255)
				),
				NameToken = Localization.DEFAULT_SKIN,
				RootObject = effectiveRoot,
				BaseSkins = Ext.NewEmpty<SkinDef>(),
				GameObjectActivations = Ext.NewEmpty<SkinDef.GameObjectActivation>(),
				RendererInfos = new CharacterModel.RendererInfo[] {
					// TODO: Do I actually have to create these? Can I copy them from the vanilla reaver?
					new CharacterModel.RendererInfo {
						defaultMaterial = mtl0,
						defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
						ignoreOverlays = false,
						renderer = renderers[0]
					},
					new CharacterModel.RendererInfo {
						defaultMaterial = mtl1,
						defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
						ignoreOverlays = false,
						renderer = renderers[1]
					},
					new CharacterModel.RendererInfo {
						defaultMaterial = mtl2,
						defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
						ignoreOverlays = false,
						renderer = renderers[2]
					},
				},
				ProjectileGhostReplacements = Ext.NewEmpty<SkinDef.ProjectileGhostReplacement>(),
				MinionSkinReplacements = Ext.NewEmpty<SkinDef.MinionSkinReplacement>()
			};

			if (!VRInterop.VRAvailable) {
				Log.LogTrace("Disabling transparency dithering...");
				mtl0.SetFloat("_DitherOn", 0);
				mtl1.SetFloat("_DitherOn", 0);
				mtl2.SetFloat("_DitherOn", 0);
				mtl0.DisableKeyword("DITHER");
				mtl1.DisableKeyword("DITHER");
				mtl2.DisableKeyword("DITHER");
			}

			// lmfao
			// if one of you knows how to actually store + address the materials for this let me know
			GameObject ally = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidJailer/VoidJailerAllyBody.prefab").WaitForCompletion();
			Renderer[] allyRenderers = ally.GetComponentsInChildren<Renderer>();

			Log.LogTrace("Cloning the default materials of the Jailer (ally variant)...");
			mtl0 = new Material(allyRenderers[0].material);
			mtl1 = new Material(allyRenderers[1].material);
			mtl2 = new Material(allyRenderers[2].material);
			VoidJailerSkinMaterials[3] = mtl0;
			VoidJailerSkinMaterials[4] = mtl1;
			VoidJailerSkinMaterials[5] = mtl2;

			Log.LogTrace("Instantiating the ally skin...");
			LoadoutAPI.SkinDefInfo ghostSkin = new LoadoutAPI.SkinDefInfo {
				Icon = SkinIconCreator.CreateSkinIcon(
					new Color32(183, 172, 175, 255),
					new Color32(78, 117, 145, 255),
					new Color32(152, 151, 227, 255),
					new Color32(54, 169, 226, 255)
				),
				NameToken = Localization.GHOST_SKIN,
				RootObject = effectiveRoot,
				BaseSkins = Ext.NewEmpty<SkinDef>(),
				GameObjectActivations = Ext.NewEmpty<SkinDef.GameObjectActivation>(),
				RendererInfos = new CharacterModel.RendererInfo[] {
					// TODO: Do I actually have to create these?
					new CharacterModel.RendererInfo {
						defaultMaterial = mtl0,
						defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
						ignoreOverlays = false,
						renderer = renderers[0]
					},
					new CharacterModel.RendererInfo {
						defaultMaterial = mtl1,
						defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
						ignoreOverlays = false,
						renderer = renderers[1]
					},
					new CharacterModel.RendererInfo {
						defaultMaterial = mtl2,
						defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
						ignoreOverlays = false,
						renderer = renderers[2]
					},
				},
				ProjectileGhostReplacements = Ext.NewEmpty<SkinDef.ProjectileGhostReplacement>(),
				MinionSkinReplacements = Ext.NewEmpty<SkinDef.MinionSkinReplacement>()
			};


			if (!VRInterop.VRAvailable) {
				Log.LogTrace("Disabling transparency dithering...");
				mtl0.SetFloat("_DitherOn", 0);
				mtl1.SetFloat("_DitherOn", 0);
				mtl2.SetFloat("_DitherOn", 0);
				mtl0.DisableKeyword("DITHER");
				mtl1.DisableKeyword("DITHER");
				mtl2.DisableKeyword("DITHER");
			}

			Log.LogTrace("Adding skin controller, if necessary...");
			ModelSkinController ctrl = effectiveRoot.GetOrCreateComponent<ModelSkinController>(out bool justCreatedController);
			if (justCreatedController) {
				ctrl.characterModel = bodyContainer.GetComponent<CharacterModel>();
				ctrl.skins = Ext.NewEmpty<SkinDef>();
			}

			Log.LogTrace("Registering skins with LoadoutAPI...");
			LoadoutAPI.AddSkinToCharacter(bodyContainer, defaultSkin);
			LoadoutAPI.AddSkinToCharacter(bodyContainer, ghostSkin);

			Log.LogTrace("Done creating skins.");
		}
#pragma warning restore Publicizer001

	}
}
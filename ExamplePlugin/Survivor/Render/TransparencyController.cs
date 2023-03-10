using MonoMod.Utils;
using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VoidJailerMod.XansTools;
using VRAPI;

namespace VoidJailerMod.Survivor.Render {
	public class TransparencyController : MonoBehaviour {

		void Awake() {
			body = GetComponent<CharacterBody>();
			renderers = GetComponentsInChildren<Renderer>();
			propertyStorage = new MaterialPropertyBlock();
			SceneCamera.onSceneCameraPreRender += OnSceneCameraPreRender;

			if (VR.enabled && MotionControls.enabled) Log.LogTrace("VR is enabled. Transparency will be forcefully set to full opacity because you cannot see your character in the first place.");
		}

		void OnDestroy() {
			try {
				SceneCamera.onSceneCameraPreRender -= OnSceneCameraPreRender;
			} catch { }
		}

		private void DestructionSequence() {
			SceneCamera.onSceneCameraPreRender -= OnSceneCameraPreRender;
			DestroyImmediate(this);
		}

		private bool DestroyIfNeeded() {
			if (!this) {
				Log.LogTrace("If you are seeing this, something horribly wrong has happened.");
				return true;
			}
			if (!body) {
				Log.LogTrace($"Destroyed {nameof(TransparencyController)}: The {nameof(CharacterBody)} it is operating alongside has been destroyed.");
				DestructionSequence();
				return true;
			}
			if (!gameObject) {
				Log.LogTrace($"Destroyed {nameof(TransparencyController)}: The object it is attached to has been destroyed.");
				DestructionSequence();
				return true;
			}
			if (body && isMine) {
				if (body.gameObject != gameObject) {
					Log.LogTrace($"Destroyed {nameof(TransparencyController)}: The body it is operating is present, but the GameObject it is attached to is not the same as this one.");
					DestructionSequence();
					return true;
				}
			}
			return false;
		}

		void FixedUpdate() {
			isMine = Ext.ClientPlayerBody == body;
			if (!isMine) return;
			if (DestroyIfNeeded()) return;

			if (VR.enabled && MotionControls.enabled) {
				SetTransparency(0);
			} else {
				if (body.outOfDanger && body.outOfCombat) {
					SetTransparency(Configuration.LocalTransparencyOutOfCombat / 100f);
				} else {
					SetTransparency(Configuration.LocalTransparencyInCombat / 100f);
				}
			}
		}

		private void OnSceneCameraPreRender(SceneCamera _) {
			if (DestroyIfNeeded()) return;

			for (int index = 0; index < renderers.Length; index++) {
				Renderer ren = renderers[index];
				UpdateSingle(ren);
			}
		}

		private bool UpdateSingle(Renderer renderer) {
			if (!renderer) return false;
			if (!renderer.isVisible) return false;
			try {
				renderer.GetPropertyBlock(propertyStorage);
				propertyStorage.SetFloat("_Fade", currentOpacity);
				renderer.SetPropertyBlock(propertyStorage);
				return true;
			} catch { }
			return false;
		}

		public void SetTransparency(float transparency) {
			currentOpacity = 1f - transparency;
		}

		private CharacterBody body;

		private Renderer[] renderers;

		private MaterialPropertyBlock propertyStorage;

		private float currentOpacity = 1f;

		private bool isMine;

	}
}

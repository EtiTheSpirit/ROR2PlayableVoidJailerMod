using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace VoidJailerMod.Initialization.Sprites {
	public static class ImageHelper {

		public static Texture2D CreateTexture(Stream fileIn) {
			Texture2D tex = new Texture2D(1, 1);
			using (MemoryStream mStr = new MemoryStream()) {
				fileIn.CopyTo(mStr);
				if (tex.LoadImage(mStr.ToArray())) {
					tex.wrapMode = TextureWrapMode.Clamp;
					return tex;
				}
			}
			Log.LogError("Failed to load image from stream!");
			return null;
		}

		public static Sprite CreateSprite(Texture2D tex) {
			int resolution = tex.width;
			if (resolution != tex.height) {
				Log.LogError("The input texture is not square! It will not render correctly.");
			}
			return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2), resolution);
		}

		public static Sprite CreateSprite(Stream fileIn) => CreateSprite(CreateTexture(fileIn));

	}
}

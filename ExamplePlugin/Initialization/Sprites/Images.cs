using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace VoidJailerMod.Initialization.Sprites {
	public static class Images {

		public static Sprite GenericWarning {
			get {
				if (_genericWarning == null) {
					using (MemoryStream stream = new MemoryStream(Properties.Resources.Generic_Warning)) {
						_genericWarning = ImageHelper.CreateSprite(stream);
					}
				}
				return _genericWarning;
			}
		}
		private static Sprite _genericWarning = null;

		public static Sprite Portrait {
			get {
				if (_portrait == null) {
					using (MemoryStream stream = new MemoryStream(Properties.Resources.Portrait)) {
						_portrait = ImageHelper.CreateSprite(stream);
					}
				}
				return _portrait;
			}
		}
		private static Sprite _portrait = null;

		public static Sprite SpikeIcon {
			get {
				if (_spikeIcon == null) {
					using (MemoryStream stream = new MemoryStream(Properties.Resources.Spike)) {
						_spikeIcon = ImageHelper.CreateSprite(stream);
					}
				}
				return _spikeIcon;
			}
		}
		private static Sprite _spikeIcon = null;


		public static Sprite BindIcon {
			get {
				if (_bindIcon == null) {
					using (MemoryStream stream = new MemoryStream(Properties.Resources.Bind)) {
						_bindIcon = ImageHelper.CreateSprite(stream);
					}
				}
				return _bindIcon;
			}
		}
		private static Sprite _bindIcon = null;


		public static Sprite DiveIcon {
			get {
				if (_diveIcon == null) {
					using (MemoryStream stream = new MemoryStream(Properties.Resources.Dive2)) {
						// TODO: Dive 1 or 2?
						_diveIcon = ImageHelper.CreateSprite(stream);
					}
				}
				return _diveIcon;
			}
		}
		private static Sprite _diveIcon = null;

		public static Sprite PerforateIcon {
			get {
				if (_perforateIcon == null) {
					using (MemoryStream stream = new MemoryStream(Properties.Resources.Perforate)) {
						_perforateIcon = ImageHelper.CreateSprite(stream);
					}
				}
				return _perforateIcon;
			}
		}
		private static Sprite _perforateIcon = null;


		public static Sprite WardensFuryBuffIcon {
			get {
				if (_wardensFuryBuffIcon == null) {
					using (MemoryStream stream = new MemoryStream(Properties.Resources.texVoidExpansionIcon)) {
						_wardensFuryBuffIcon = ImageHelper.CreateSprite(stream);
					}
				}
				return _wardensFuryBuffIcon;
			}
		}
		private static Sprite _wardensFuryBuffIcon = null;

		public static Sprite MortarIcon {
			get {
				if (_mortarIcon == null) {
					using (MemoryStream stream = new MemoryStream(Properties.Resources.Mortar)) {
						_mortarIcon = ImageHelper.CreateSprite(stream);
					}
				}
				return _mortarIcon;
			}
		}
		private static Sprite _mortarIcon = null;

	}
}

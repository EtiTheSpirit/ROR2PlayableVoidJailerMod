#nullable enable
#pragma warning disable IDE0029
using UnityEngine;

namespace VoidJailerMod.XansTools {
	public static class Ext {

		/// <summary>
		/// Implements null coalescense into Unity objects.<para/>
		/// This is required because <see cref="Object"/> declares <see cref="Object.operator =="/> such that it returns <see langword="true"/> for objects scheduled
		/// for destruction in the engine. Consequently, this means that <see cref="object.ReferenceEquals"/> will return <see langword="false"/> when compared to null
		/// while <c>==</c> will return <see langword="true"/>. This wreaks havoc on operators like <c>??</c> because they use reference equality instead of overloads.<br/>
		/// <br/>
		/// This function implements <c>??</c> specifically for <see cref="Object"/> in a shorthand term via an extension method.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static T? Or<T>(this T? left, T? right) where T : Object => left == null ? right : left;

		/// <summary>
		/// Attempts to get an instance of <typeparamref name="T"/> from the given <see cref="GameObject"/>, creating it if no such component exists.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="onObject"></param>
		/// <returns></returns>
		public static T GetOrCreateComponent<T>(this GameObject onObject, out bool createdNew) where T : Component {
			T existing = onObject.GetComponent<T>();
			if (existing) {
				createdNew = false;
				return existing;
			}
			createdNew = true;
			return onObject.AddComponent<T>();
		}

		/// <summary>
		/// An alternative to <see cref="System.Array.Empty{T}"/>.<para/>
		/// This is intended for use when the array will be mutated with functions like <see cref="System.Array.Resize{T}(ref T[], int)"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T[] NewEmpty<T>() => new T[0];

		/// <summary>
		/// An alias to <see cref="Object.Destroy(Object, float)"/> that can be called on an object. It also is okay with being called on a transform.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="afterSeconds"></param>
		public static void Destroy(this Object obj, float afterSeconds = 0f) => Object.Destroy(obj is Transform trs ? trs.gameObject : obj, afterSeconds);

	}
}

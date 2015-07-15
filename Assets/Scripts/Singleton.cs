using UnityEngine;
using System.Collections;

public abstract class Singleton<T> : MonoBehaviour where T: Singleton<T> {

	public static T instance = null;

	protected virtual void Awake () {
		if (instance == null) {
			instance = (T)this;
		} else if (instance != (T)this ) {
			Destroy (gameObject);
		}

		DontDestroyOnLoad (gameObject);
	}
}

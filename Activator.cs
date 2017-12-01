using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Activator : MonoBehaviour {

	[SerializeField] bool onStart = false;
	[SerializeField] bool onEnable = true;
	[SerializeField] bool onDisable = false;

	void Start () {
		if(onStart)
			Activate();
	}

	void OnEnable() {
		if(onEnable)
			Activate();
	}

	void OnDisable() {
		if(onDisable)
			Activate();
	}

	public abstract void Activate();
}

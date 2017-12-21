using UnityEngine;
using System.Collections;
using UnityEditor;

public class MultiEditor : Editor {

	private System.Collections.Generic.List<Object> targetList;

	public MultiEditor() {
		targetList = new System.Collections.Generic.List<Object>(1);
	}

	public virtual void OnMultiEdit(System.Collections.Generic.List<Object> targetList) {
	}

	public virtual void OnSceneGUI() {
		if (targetList.Contains (target)) {
			// We got all targets.
			if (target == targetList [0]) {
				// First call of OnSceneGUI(). Time to relay.
				OnMultiEdit (targetList);
			}
		} else {
			// Build the target list first, skipping the first event.
			// First event usually is a Layout event which is generally not interesting anyway.
			targetList.Add (target);
		}
	}
}

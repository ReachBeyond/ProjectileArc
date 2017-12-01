using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileArcTest : MonoBehaviour {

	public GameObject[] objPrefabs;
	public ProjectileArc arc;

	// Use this for initialization
	void Start () {
		arc.SpawnObjects(objPrefabs);
	}
}

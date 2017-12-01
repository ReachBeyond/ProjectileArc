using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ProjectileArc))]
public class ProjectileArcActivator : Activator {

	[Space(10)]
	[SerializeField] private GameObject projectilePrefab;
	[SerializeField] private ProjectileArc arc;
	[SerializeField] private Transform parentTransform;

	public override void Activate() {
		arc.SpawnObjects(projectilePrefab, parentTransform);
	}
}

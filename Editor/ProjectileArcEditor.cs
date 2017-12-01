using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using System.Collections.Generic;

// Note that this editor was initially on
// https://docs.unity3d.com/ScriptReference/IMGUI.Controls.ArcHandle.html
//
// However, it has recieved some major changes.

[CustomEditor(typeof(ProjectileArc), editorForChildClasses: true)]
[CanEditMultipleObjects]
public class ProjectileArcEditor : MultiEditor {

	const float HANDLE_RADIUS = 3f;
	const float FIRE_LINE_LENGTH = 2f;

	Color fireLineColor = new Color(0f, 1f, 0.3f, 1f);

	ArcHandle m_ArcHandle = new ArcHandle();

	protected virtual void OnEnable() {
		Color handleColor = Color.white;
		float handleAlpha = 0.1f;

		// arc handle has no radius handle by default
		m_ArcHandle.SetColorWithoutRadiusHandle(handleColor, handleAlpha);
	}

	// the OnMultiGUI callback uses the scene view camera for drawing handles by default
	public override void OnMultiEdit(List<Object> targetList) {
		// Only render this stuff if we are selecting one object.
		if(targetList.Count == 1) {
			ProjectileArc arc = (ProjectileArc)target;

			Matrix4x4 handleMatrix;

			SetUpArcHandle(
				ref m_ArcHandle,
				out handleMatrix,
				arc
			);

			using ( new Handles.DrawingScope(handleMatrix) ) {

				// draw the handle
				EditorGUI.BeginChangeCheck();
				m_ArcHandle.DrawHandle();
				if (EditorGUI.EndChangeCheck()) {

					// record the target object before setting new values so changes can be undone/redone
					Undo.RecordObject(arc, "Change Projectile Arc Angle");

					// Just change the one handle; this usually is what runs
					arc.fullAngle = m_ArcHandle.angle;
				}
			}

			// TODO: Make this render the lines even when multiple things are selected

			Handles.color = fireLineColor;

			// Draw lines
			foreach(FirePoint point in arc.points) {
				//Debug.Log(point.position + " faces " + point.rotation.eulerAngles);

				Vector3 startPoint = point.position;
				Vector3 endPoint = startPoint +
					(point.rotation * Vector3.forward * FIRE_LINE_LENGTH * HandleUtility.GetHandleSize(point.position));

				Handles.DrawLine(startPoint, endPoint);
			}
		}
	}

	protected virtual void SetUpArcHandle(
		ref ArcHandle handle,
		out Matrix4x4 matrix,
		ProjectileArc arc)
	{
		// copy the target object's data to the handle
		handle.angle = arc.fullAngle;
		m_ArcHandle.radius = HANDLE_RADIUS * HandleUtility.GetHandleSize(arc.transform.position);

		// set the handle matrix so that angle extends upward from target's facing direction along ground
		Vector3 handleNormal = arc.axis;

		Vector3 handleDirection = arc.facingDirection;
		handleDirection = Quaternion.AngleAxis(-arc.halfAngle, arc.axis) * handleDirection;
		//Debug.Log(handleDirection);

		matrix = Matrix4x4.TRS(
			arc.transform.position,
			Quaternion.LookRotation(handleDirection, handleNormal),
			Vector3.one
		);
	}
}

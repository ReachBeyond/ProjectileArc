using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// This is the core projectile arc component. It provides a convienent editor
/// within Unity's scene view and can be changed within runtime.
/// 
/// Note that the fire locations are not updated until they are needed. Therefor,
/// worst case performance occurs when component gets accessed every FixedUpdate
/// on a continously moving object. Calling it many times within one frame is
/// still very efficient, however.
/// 
/// Without outside input, this component does absolutely nothing.
/// </summary>
public class ProjectileArc : MonoBehaviour {
	
	#region Fields
	/// <summary>
	/// Half of the arc angle. This is an angle from the forward direction.
	/// </summary>
	/// <value>The half angle.</value>
	public float halfAngle {
		get { return m_HalfAngle; }
		set { m_HalfAngle = value; }
	}
	[Tooltip(
		"This is the angle from the forward direction. Note that this goes both" +
		" positive and negative. Because of this, (Half Angle = -Half Angle)."
	)]
	[SerializeField] float m_HalfAngle = 45f;

	/// <summary>
	/// The full angle the arc uses. The forward direction lies in the middle of the
	/// arc this angle produces.
	/// </summary>
	/// <value>The full angle.</value>
	public float fullAngle {
		get { return halfAngle * 2; }
		set {
			if(value != halfAngle) {
				halfAngle = value / 2;
				OnValidate();
			}
		}
	}

	/// <summary>
	/// Axis of which the arc revolves about.
	/// </summary>
	/// <value>The axis.</value>
	public Vector3 axis {
		get {
			//return m_Axis;
			return transform.up;
		} 
		/*set {
			if(value != m_Axis) {
				m_Axis = value;
				OnValidate();
			}
		}*/
	}
	//[SerializeField] Vector3 m_Axis = Vector3.up;


	/// <summary>
	/// Gets or sets the shot count. Must be greater than 0.
	/// </summary>
	/// <value>The shot count.</value>
	public int shotCount {
		get { return m_ShotCount; }
		set {
			if(value != m_ShotCount) {
				m_ShotCount = value;
				OnValidate();
			}
		}
	}
	[Range(1, 100)] [SerializeField] int m_ShotCount = 1;

	/// <summary>
	/// These are the points which things will be fired from.
	/// </summary>
	/// <value>The points.</value>
	[HideInInspector] public FirePoint[] points {
		get {
			if(transform.hasChanged || m_Points == null) {
				OnValidate();
				transform.hasChanged = false;
			}

			return m_Points;
		}
	}

	/// <summary>
	/// Please note that this gets updated in OnValidate()
	/// </summary>
	[HideInInspector] [SerializeField] FirePoint[] m_Points;

	/// <summary>
	/// Center of the arc.
	/// </summary>
	/// <value>The facing direction.</value>
	public Vector3 facingDirection {
		get { return transform.forward; }
	}

	#endregion



	protected virtual void OnValidate() {
		Assert.IsTrue(shotCount > 0);

		m_Points = new FirePoint[shotCount];

		if(shotCount > 1) {
			// The formula is:
			//  angle for i-th shot = i*theta/(n-1)
			// Where n is the number of shots being fired,
			// theta is the angle of the entire arc,
			// and i is the current shot we're considering.
			float angleBetweenShots = fullAngle / (shotCount - 1);

			m_Points = new FirePoint[shotCount];

			for(int i = 0; i < m_Points.Length; i++) {
				float currentAngle = i * angleBetweenShots;

				m_Points[i] = new FirePoint(
					transform.position,
					Quaternion.AngleAxis(currentAngle - halfAngle, axis) * transform.rotation
				);
			}
		}
		else {
			// If we are only shooting one shot, then we will want to
			// just fire it from the center.
			m_Points = new FirePoint[1];
			m_Points[0] = new FirePoint(
				transform.position,
				transform.rotation
			);
		}	
	} // End OnValidate()



	#region Prefab-based firing methods
	/// <summary>
	/// Instantiates the objects at each of the spawn locations and rotations.
	/// </summary>
	/// <returns>An array of the newly instantiated objects.</returns>
	/// <param name="objPrefab">Object prefab to spawn.</param>
	/// <param name="parent">Transform to parent each object to.</param>
	public GameObject[] SpawnObjects(GameObject objPrefab, Transform parent = null) {
		return SpawnObjects( () => { return objPrefab; }, parent );
	}

	/// <summary>
	/// Instantiates each of the objects at each of the spawn locations and rotations.
	/// </summary>
	/// <returns>An array of the newly instantiated objects.</returns>
	/// <param name="objPrefabs">
	/// Prefabs to spawn. Must have at least <seealso cref="shotCount"/> number of elements.
	/// </param>
	/// <param name="parent">Transform to parent each object to.</param>
	public GameObject[] SpawnObjects(GameObject[] objPrefabs, Transform parent = null) {
		return SpawnObjects( (int i) => { return objPrefabs[i]; }, parent );
	}
	#endregion
		
	#region Functor-based firing methods
	/// <summary>
	/// For each spawn point, a the GameObject returned by the provided
	/// builder will be instantiated. The newly created object will copy
	/// the spawn point's position and direction.
	/// </summary>
	/// <returns>An array of the newly instantiated objects.</returns>
	/// <param name="builder">Returns a newly generated GameObject.</param>
	/// <param name="parent"> Each new object has this set as its parent.</param>
	public GameObject[] SpawnObjects(Func<GameObject> builder, Transform parent = null) {
		return SpawnObjects( (int i) => { return builder(); }, parent );
	}

	/// <summary>
	/// For each spawn point, a the GameObject returned by the provided
	/// builder will be instantiated. The newly created object will copy
	/// the spawn point's position and direction.
	/// 
	/// Because the builder recieves the index, it is possible to vary the
	/// object returned based on its place in the point array.
	/// </summary>
	/// <returns>An array of the newly instantiated objects.</returns>
	/// <param name="builder">Returns a newly generated GameObject.</param>
	/// <param name="parent"> Each new object has this set as its parent.</param>
	public GameObject[] SpawnObjects(Func<int, GameObject> builder, Transform parent = null) {
		GameObject[] objects = new GameObject[points.Length];

		for(int i = 0; i < points.Length; i++) {
			objects[i] = Instantiate(builder(i), points[i].position, points[i].rotation, parent) as GameObject;
		}

		return objects;
	}
	#endregion
}

[System.Serializable]
public class FirePoint {

	/// <summary>
	/// Initializes a new instance of the <see cref="FirePoint"/> class.
	/// </summary>
	/// <param name="position">Distance from transform.position</param>
	/// <param name="rotation">Rotation from transform.rotation</param>
	public FirePoint(Vector3 position, Quaternion rotation) {
		m_Position = position;
		m_Rotation = rotation;
	}

	public Vector3 position {
		get { return m_Position; }
	}
	Vector3 m_Position;

	public Quaternion rotation {
		get { return m_Rotation; }
	}
	Quaternion m_Rotation;

}

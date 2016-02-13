using UnityEngine;
using System.Collections;

public class SeekController : MonoBehaviour {
	public GameObject Circle;

	// Use this for initialization
	void Start () {
		iTween.MoveAdd( Circle, new Vector3(588, 0, 0), 180f );
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

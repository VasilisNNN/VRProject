using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotate : MonoBehaviour {

	private Player pl;
	public float speed = 20;
    public Vector3 RotationDirection = new Vector3(1, 1, 1);

	void Start () {
		pl = InitializeOnAwake.pl;
	}
	
	void Update () {
		
		if(pl!=null)
		if (pl.PlayerPause()) return;

		transform.Rotate(RotationDirection * Time.deltaTime * speed);
	}
}

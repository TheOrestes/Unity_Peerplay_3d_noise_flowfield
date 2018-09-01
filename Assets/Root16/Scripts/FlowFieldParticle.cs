using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowFieldParticle : MonoBehaviour 
{
	public float moveSpeed;

	// Use this for initialization
	void Start () 
	{
		
	}

	public void ApplyRotation(Vector3 rotation, float rotateSpeed)
	{
		Quaternion targetRotation = Quaternion.LookRotation(rotation.normalized);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
	}
	
	// Update is called once per frame
	void Update () 
	{
		this.transform.position += transform.forward * moveSpeed * Time.deltaTime;	
	}

}

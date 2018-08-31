using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFlowField : MonoBehaviour 
{
	FastNoise fastNoise;
	public Vector3Int gridSize;
	public float increment;
	public Vector3 offset, offsetSpeed;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	private void OnDrawGizmos() 
	{
		fastNoise = new FastNoise();

		float xOff = 0;
		for(int x = 0 ; x < gridSize.x ; x++)
		{
			float yOff = 0;
			for(int y = 0 ; y < gridSize.y ; y++)
			{
				float zOff = 0;
				for(int z = 0 ; z < gridSize.z ; z++)
				{
					// GetSimplex() returns value in a range of [-1,1], hence by adding one, we are shifting it to [0,2] range
					float noise = fastNoise.GetSimplex(xOff + offset.x, yOff + offset.y, zOff + offset.z) + 1;

					// create noise into direction
					Vector3 noiseDirection = new Vector3(Mathf.Cos(noise * Mathf.PI), Mathf.Sin(noise * Mathf.PI), Mathf.Cos(noise * Mathf.PI));

					Gizmos.color = new Color(noiseDirection.normalized.x, noiseDirection.normalized.y, noiseDirection.z, 1.0f);  
					Vector3 pos = new Vector3(x,y,z) + transform.position;
					Vector3 endPos = pos + Vector3.Normalize(noiseDirection);
					
					Gizmos.DrawLine(pos, endPos);
					zOff += increment;
				}

				yOff += increment;
			}

			xOff += increment;
		}
	}
}

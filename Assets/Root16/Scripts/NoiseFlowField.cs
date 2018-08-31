using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFlowField : MonoBehaviour 
{
	FastNoise fastNoise;
	public Vector3Int gridSize;
	public float cellSize;
	public Vector3[,,] flowFieldDirections;
	public float increment;
	public Vector3 offset, offsetSpeed;

	public GameObject particlePrefab;
	public int amountOfParticles;
	[HideInInspector]
	public List<FlowFieldParticle> particles;
	public float particleScale;
	public float spawnRadius;

	private bool particleSpawnValidation(Vector3 position)
	{
		bool valid = true;
		foreach(FlowFieldParticle particle in particles)
		{
			if(Vector3.Distance(position, particle.transform.position) < spawnRadius)
			{
				valid = false;
				break;
			}
		}

		if(valid)
			return true;
		else
			return false;
	}

	// Use this for initialization
	void Start () 
	{
		flowFieldDirections = new Vector3[gridSize.x, gridSize.y, gridSize.z];
		fastNoise = new FastNoise();

		particles = new List<FlowFieldParticle>();
		for(int i = 0 ; i < amountOfParticles ; i++)
		{
			int attempt = 0;

			while(attempt < 100)
			{
				Vector3 randomPos = new Vector3
									(
										Random.Range(this.transform.position.x, this.transform.position.x + gridSize.x * cellSize),
										Random.Range(this.transform.position.y, this.transform.position.y + gridSize.y * cellSize),
										Random.Range(this.transform.position.z, this.transform.position.z + gridSize.z * cellSize)
									);

				bool isValid = particleSpawnValidation(randomPos);
	
				if(isValid)
				{
					GameObject particleInstance = (GameObject)Instantiate(particlePrefab);
					particleInstance.transform.position = randomPos;
					particleInstance.transform.localScale = new Vector3(particleScale, particleScale, particleScale);
					particleInstance.transform.parent = this.transform;
	
					particles.Add(particleInstance.GetComponent<FlowFieldParticle>());
				}
				else
				{
					attempt++;
				}
			}
			
			Debug.Log(particles.Count);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
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

					flowFieldDirections[x,y,z] = Vector3.Normalize(noiseDirection);
					zOff += increment;
				}

				yOff += increment;
			}

			xOff += increment;
		}
	}

	private void OnDrawGizmos() 
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube(this.transform.position + new Vector3((gridSize.x * cellSize) * 0.5f, (gridSize.y * cellSize) * 0.5f, (gridSize.z * cellSize) * 0.5f), 
							new Vector3((gridSize.x * cellSize), (gridSize.y * cellSize), (gridSize.z * cellSize)));
	}


}

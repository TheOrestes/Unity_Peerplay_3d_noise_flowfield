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
	
	public float spawnRadius;
	public float particleScale, particleMoveSpeed, particleRotateSpeed;

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

	void CalculateFlowFieldDirection()
	{
		offset = new Vector3(offset.x + (offsetSpeed.x * Time.deltaTime), offset.y + (offsetSpeed.y * Time.deltaTime), offset.z + (offsetSpeed.z * Time.deltaTime));
		
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

	void ParticleBehavior()
	{
		foreach(FlowFieldParticle p in particles)
		{
			// X EDGES
			if(p.transform.position.x > this.transform.position.x + (gridSize.x * cellSize))
			{
				p.transform.position = new Vector3(this.transform.position.x, p.transform.position.y, p.transform.position.z);
			}
			if(p.transform.position.x < this.transform.position.x)
			{
				p.transform.position = new Vector3(this.transform.position.x + (gridSize.x * cellSize), p.transform.position.y, p.transform.position.z);
			}

			// Y EDGES
			if(p.transform.position.y > this.transform.position.y + (gridSize.y * cellSize))
			{
				p.transform.position = new Vector3(p.transform.position.x, this.transform.position.y, p.transform.position.z);
			}
			if(p.transform.position.y < this.transform.position.y)
			{
				p.transform.position = new Vector3(p.transform.position.x, this.transform.position.y + (gridSize.y * cellSize), p.transform.position.z);
			}

			// Z EDGES
			if(p.transform.position.z > this.transform.position.z + (gridSize.z * cellSize))
			{
				p.transform.position = new Vector3(p.transform.position.x, p.transform.position.y, this.transform.position.z);
			}
			if(p.transform.position.z < this.transform.position.z)
			{
				p.transform.position = new Vector3(p.transform.position.x, p.transform.position.y, this.transform.position.x + (gridSize.x * cellSize));
			}

			Vector3Int particlePos = new Vector3Int
									 (
										 Mathf.FloorToInt(Mathf.Clamp((p.transform.position.x - this.transform.position.x)/cellSize, 0, gridSize.x-1)),
										 Mathf.FloorToInt(Mathf.Clamp((p.transform.position.y - this.transform.position.y)/cellSize, 0, gridSize.y-1)),
										 Mathf.FloorToInt(Mathf.Clamp((p.transform.position.z - this.transform.position.z)/cellSize, 0, gridSize.z-1)) 
									 );

			p.ApplyRotation(flowFieldDirections[particlePos.x, particlePos.y, particlePos.z], particleRotateSpeed);
			p.moveSpeed = particleMoveSpeed;
			p.transform.localScale = new Vector3(particleScale, particleScale, particleScale);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		CalculateFlowFieldDirection();
		ParticleBehavior();
	}

	private void OnDrawGizmos() 
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube(this.transform.position + new Vector3((gridSize.x * cellSize) * 0.5f, (gridSize.y * cellSize) * 0.5f, (gridSize.z * cellSize) * 0.5f), 
							new Vector3((gridSize.x * cellSize), (gridSize.y * cellSize), (gridSize.z * cellSize)));
	}


}

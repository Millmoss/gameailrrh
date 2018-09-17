using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
	public GameObject wanderGoal;
	public GameObject chaseGoal;
	public GameObject fleeGoal;
	public GameObject head;
	public float moveSpeed = 1;
	public int mapBounds = 100;
	public int randomSeed = 0;
	private Rigidbody selfBody;
	private float moveForce = 3;
	private Vector3 movement;
	private enum status { wandering, chasing, fleeing, pathing };
	public int state = 0;
	public int defaultState = 0;
	public bool willKill = false;
	private float t = 0;
	private bool starting = true;

	void Start ()
	{
		selfBody = gameObject.GetComponent<Rigidbody>();
		Random.InitState(randomSeed);
		transform.position = new Vector3(Mathf.Clamp(Random.value * mapBounds, 10, mapBounds - 10), 10, Mathf.Clamp(Random.value * mapBounds, 10, mapBounds - 10));
		LayerMask groundMask = 1 << 9;
		Ray ray = new Ray(transform.position, -transform.up);
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(ray, out hit, 25f, groundMask))
		{
			selfBody.position = hit.point + new Vector3(0, .8f, 0);
		}
	}
	
	void Update ()
	{
		if (starting)
		{
			t += Time.deltaTime;
			if (t >= 1.5f)
			{
				t = 0;
				starting = false;
			}
		}
		else
		{
			switch (state)
			{
				case ((int)status.wandering):
					LayerMask groundMask = 1 << 9;
					Ray ray;
					RaycastHit hit;
					if (t == 0 || Vector3.Magnitude(transform.position - wanderGoal.transform.position) < 10)
					{
						float rad = Mathf.PI * Random.value * 2;
						Vector3 newPos = transform.position + 30 * head.transform.forward + new Vector3(Mathf.Cos(rad) * 20, 0, Mathf.Sin(rad) * 20);
						newPos = new Vector3(Mathf.Clamp(newPos.x, 10, mapBounds - 10), 10, Mathf.Clamp(newPos.z, 10, mapBounds - 10));
						wanderGoal.transform.position = newPos;
						ray = new Ray(wanderGoal.transform.position, -wanderGoal.transform.up);
						hit = new RaycastHit();
						if (Physics.Raycast(ray, out hit, 25f, groundMask))
						{
							wanderGoal.transform.position = hit.point;
						}
					}
					t += Time.deltaTime;
					if (t > 5)
						t = 0;
					move(wanderGoal.transform.position);
					ray = new Ray(transform.position, (chaseGoal.transform.position - transform.position).normalized);
					hit = new RaycastHit();
					if (Vector3.Distance(transform.position, chaseGoal.transform.position) < 20 && 
						!Physics.Raycast(ray, out hit, Vector3.Distance(transform.position, chaseGoal.transform.position), groundMask))
					{
						state = (int)status.chasing;
						wanderGoal.transform.position = Vector3.zero;
					}
					ray = new Ray(transform.position, (fleeGoal.transform.position - transform.position).normalized);
					hit = new RaycastHit();
					if (Vector3.Distance(transform.position, fleeGoal.transform.position) < 20 &&
						!Physics.Raycast(ray, out hit, Vector3.Distance(transform.position, fleeGoal.transform.position), groundMask))
					{
						state = (int)status.fleeing;
						wanderGoal.transform.position = Vector3.zero;
					}
					break;
				case ((int)status.chasing):
					Vector3 goal = chaseGoal.transform.position;
					if (Vector3.Distance(transform.position, goal) > 5)
					{
						move(goal);
					}
					else
					{
						if (willKill)
						{
							if (Vector3.Distance(transform.position, goal) < 1.2)
							{
								chaseGoal.GetComponent<Character>().killSelf();
								chaseGoal = new GameObject();
								chaseGoal.transform.position = new Vector3(0, 100, 0);
								state = defaultState;
							}
							move(goal);
						}
						else
						{
							approach(goal);
						}
					}
					break;
				case ((int)status.fleeing):
					
					break;
				default:
					break;
			}
		}

		movement = transform.position;
	}

	void FixedUpdate()
	{
		Vector3 goalDir = Vector3.zero;
		switch (state)
		{
			case ((int)status.wandering):
				goalDir = wanderGoal.transform.position - transform.position;
				break;
			case ((int)status.chasing):
				goalDir = chaseGoal.transform.position - transform.position;
				break;
			case ((int)status.fleeing):
				goalDir = transform.position - fleeGoal.transform.position;
				break;
			default:
				break;
		}
		Quaternion look = Quaternion.LookRotation(goalDir + selfBody.velocity * 5);
		head.transform.rotation = Quaternion.Slerp(head.transform.rotation, look, .03f);
		head.transform.position = transform.position + new Vector3(0, .3f, 0);
	}

	void move(Vector3 goal)
	{
		//move to goal
		Vector3 direction = (goal - transform.position).normalized;
		selfBody.AddForce(direction * moveForce * moveSpeed);

		//adjust movement for slope and pushback
		float moveVariance = Vector3.Angle(transform.position - movement, direction);
		if (moveVariance > 30)
			selfBody.AddForce(direction + (direction - (transform.position - movement)) * moveForce * moveVariance / 15);

		selfBody.AddForce(new Vector3(0, (1 - moveSpeed), 0));
	}

	void approach(Vector3 goal)
	{
		//move to goal
		Vector3 direction = (goal - transform.position).normalized * (Vector3.Magnitude(goal - transform.position) / 10);
		selfBody.AddForce(direction * moveForce * moveSpeed);

		//adjust movement for slope and pushback
		float moveVariance = Vector3.Angle(transform.position - movement, direction);
		if (moveVariance > 30)
			selfBody.AddForce(direction + (direction - (transform.position - movement)) * moveForce * moveVariance / 15);

		selfBody.AddForce(new Vector3(0, (1 - moveSpeed), 0));
	}

	public void killSelf()
	{

	}
}

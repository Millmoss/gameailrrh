using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
	public GameObject goal;
	public GameObject head;
	public float moveSpeed = 1;
	public int mapBounds = 100;
	public int randomSeed = 0;
	private Rigidbody selfBody;
	private float moveForce = 3;
	private Vector3 movement;
	private enum status { wandering, chasing, fleeing, pathFinding };
	public int state = 0;
	private float t = 0;

	void Start ()
	{
		selfBody = gameObject.GetComponent<Rigidbody>();
		Random.InitState(randomSeed);
	}
	
	void Update ()
	{
		switch (state)
		{
			case ((int)status.wandering):
				if (t == 0 || Vector3.Magnitude(transform.position - goal.transform.position) < 10)
				{
					float rad = Mathf.PI * Random.value * 2;
					Vector3 newPos = transform.position + 30 * head.transform.forward + new Vector3(Mathf.Cos(rad) * 20, 0, Mathf.Sin(rad) * 20);
					newPos = new Vector3(Mathf.Clamp(newPos.x, 5, mapBounds - 5), 10, Mathf.Clamp(newPos.z, 5, mapBounds - 5));
					goal.transform.position = newPos;
					LayerMask groundMask = 1 << 9;
					Ray ray = new Ray(goal.transform.position, -goal.transform.up);
					RaycastHit hit = new RaycastHit();
					if (Physics.Raycast(ray, out hit, 1000f, groundMask))
					{
						goal.transform.position = hit.point;
					}
				}
				t += Time.deltaTime;
				if (t > 5)
					t = 0;
				move();
				break;
			default:
				break;
		}

		movement = transform.position;
	}

	void FixedUpdate()
	{
		Quaternion look = Quaternion.LookRotation(goal.transform.position - transform.position);
		head.transform.rotation = Quaternion.Slerp(head.transform.rotation, look, .3f);
	}

	void LateUpdate()
	{
		head.transform.position = transform.position + new Vector3(0, .3f, 0);
	}

	void move()
	{
		//move to goal
		Vector3 direction = (goal.transform.position - transform.position).normalized;
		selfBody.AddForce(direction * moveForce * moveSpeed);

		//adjust movement for slope and pushback
		float moveVariance = Vector3.Angle(transform.position - movement, direction);
		if (moveVariance > 30)
			selfBody.AddForce(direction + (direction - (transform.position - movement)) * moveForce * moveVariance / 15);
	}
}

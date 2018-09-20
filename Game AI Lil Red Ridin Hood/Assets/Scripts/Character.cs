using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
	public GameObject wanderGoal;
	public GameObject chaseGoal;
	public GameObject fleeGoal;
	private Vector3 fleeDirection;
	public GameObject pathGoal;
	private GameObject speakGoal;
	private ArrayList pathGoals;
	private int pathIndex;
	private int lookIndex;
	public GameObject head;
	public float moveSpeed = 1;
	public int mapBounds = 100;
	public int randomSeed = 0;
	private Rigidbody selfBody;
	private float moveForce = 3;
	private Vector3 movement;
	private enum status { wandering, chasing, fleeing, pathing, speaking, idle };
	public int state = 0;
	public int defaultState = 0;
	public bool willKill = false;
	private float t = 0;
	private bool starting = true;
	public bool goodSmell = false;
	public bool special = false;
	private GameObject specialGoal;
	public GameObject radiusDisplay;
	public GameObject chaseDisplay;
	public GameObject fleeDisplay;
	public GameObject seeLine;
	public GameObject blindLine;
	public string characterName;
	public UIHandler uiReceive;
	private bool dead = false;
	public string dialogue1;
	public string dialogue2;
	public string dialogue3;

	void Start ()
	{
		specialGoal = wanderGoal;
		selfBody = gameObject.GetComponent<Rigidbody>();
		Random.InitState(randomSeed);
		spawn();
		Transform[] pathTemp = pathGoal.GetComponentsInChildren<Transform>();
		pathGoals = new ArrayList();
		if (pathTemp.Length > 0)
			for (int i = 0; i < pathTemp.Length; i++)
			{
				if (pathTemp[i].gameObject.tag == "Pathing")
				{
					pathGoals.Add(pathTemp[i].gameObject);
				}
			}
		else if (state == (int)status.pathing)
			state = (int)status.idle;
		if (state == (int)status.pathing)
			transform.position = ((GameObject)pathGoals[pathIndex]).transform.position + new Vector3(0, .5f, 0);
		pathIndex = 1;
		lookIndex = 1;
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
				uiReceive.addLine(characterName + " initialized with state " + (status)state);
			}
		}
		else
		{
			switch (state)
			{
				case ((int)status.wandering):
					goalStat(0);
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
					{
						t = 0;
						uiReceive.addLine(characterName + " sets a new goal to wander toward");
					}
					move(wanderGoal.transform.position);
					break;
				case ((int)status.chasing):
					if (characterName != "Wolf" || !willKill)
						goalStat(0);
					t += Time.deltaTime;
					Vector3 goal = chaseGoal.transform.position;
					if (Vector3.Distance(transform.position, goal) > 5)
					{
						move(goal);
						radiusSet(1, Vector3.zero);
					}
					else
					{
						if (willKill)
						{
							if (Vector3.Distance(transform.position, goal) < 1.2)
							{
								uiReceive.addLine(characterName + " catches " + chaseGoal.GetComponent<Character>().characterName + " and murders them");
								chaseGoal.GetComponent<Character>().killSelf();
								chaseGoal = new GameObject();
								chaseGoal.transform.position = new Vector3(0, 100, 0);
								state = defaultState;
								resetDisplay();
							}
							move(goal);
						}
						else
						{
							if (radiusDisplay.transform.localScale.x != 5)
								uiReceive.addLine(characterName + " begins to slow their approach toward " + chaseGoal.GetComponent<Character>().characterName);
							radiusSet(5, chaseGoal.transform.position);
							approach(goal);
							if (Vector3.Distance(chaseGoal.transform.position, transform.position) < 3f)
							{
								uiReceive.addLine(characterName + " stops " + chaseGoal.GetComponent<Character>().characterName + " to talk to them");
								state = (int)status.speaking;
								chaseGoal.GetComponent<Character>().forceTalk(gameObject);
								speakGoal = chaseGoal;
								resetDisplay();
								t = 0;
							}
						}
					}
					break;
				case ((int)status.fleeing):
					goalStat(1);
					t += Time.deltaTime;
					//move to goal
					Vector3 direction = (transform.position - fleeGoal.transform.position).normalized;
					selfBody.AddForce(fleeDirection * moveForce * moveSpeed);

					//adjust movement for slope and pushback
					float moveVariance = Vector3.Angle(transform.position - movement, direction);
					if (moveVariance > 30)
						selfBody.AddForce(direction + (direction - (transform.position - movement)) * moveForce * moveVariance / 3);

					selfBody.AddForce(new Vector3(0, (1 - moveSpeed), 0));
					break;
				case ((int)status.pathing):
					goalStat(1);
					print(pathIndex);
					if (Vector3.Distance(((GameObject)pathGoals[pathIndex]).transform.position, transform.position) < 3)
					{
						if (lookIndex == pathIndex && lookIndex + 1 < pathGoals.Count)
						{
							lookIndex++;
							uiReceive.addLine(characterName + " is within range of path goal and begins steering to next path goal");
						}
						else if (pathIndex + 1 >= pathGoals.Count)
						{
							state = (int)status.idle;
							t = 0;
							resetDisplay();
						}
						move(transform.position + head.transform.forward);
					}
					else if (lookIndex != pathIndex)
					{
						pathIndex++;
						uiReceive.addLine(characterName + " begins moving to next path goal");
					}
					else
					{
						move(((GameObject)pathGoals[pathIndex]).transform.position);
					}
					break;
				case ((int)status.speaking):
					goalStat(1);
					selfBody.velocity = Vector3.zero;
					t += Time.deltaTime;
					print(t);
					if (dialogue1 != "skip")
					{
						uiReceive.addLine(characterName + ": " + dialogue1);
						dialogue1 = "skip";
					}
					else if (t > 1f && dialogue2 != "skip")
					{
						uiReceive.addLine(characterName + ": " + dialogue2);
						dialogue2 = "skip";
					}
					else if (t > 2f && dialogue3 != "skip")
					{
						uiReceive.addLine(characterName + ": " + dialogue3);
						dialogue3 = "skip";
					}
					if (t > 3f && speakGoal.name == chaseGoal.name)
					{
						willKill = true;
						defaultState = (int)status.pathing;
						state = (int)status.pathing;
						uiReceive.addLine(characterName + " is done speaking to " + chaseGoal.GetComponent<Character>().characterName + " and begins pathing to grandma's house");
						fleeGoal.GetComponent<Character>().danger();
						pathIndex = 0;
						lookIndex = 0;
						t = 0;
						resetDisplay();
					}
					else if (t > 3f)
					{
						state = defaultState;
						specialGoal = speakGoal;
						uiReceive.addLine(characterName + " is done speaking to " + speakGoal.GetComponent<Character>().characterName + " and returns to " + (status)state);
						t = 0;
						resetDisplay();
					}
					break;
				case ((int)status.idle):
					selfBody.velocity = Vector3.zero;
					goalStat(0);
					if (special && Vector3.Distance(transform.position, specialGoal.transform.position) < 5)
					{
						uiReceive.addLine(characterName + " pulls a glock from their picnic basket and shoots the " + specialGoal.GetComponent<Character>().characterName + " dead");
						specialGoal.GetComponent<Character>().killSelf();
						uiReceive.addLine(characterName + " doesn't really know what to do now since not only is grandma not home, but also " + characterName + " doesn't actually have a grandma");
					}
					break;
				default:
					break;
			}
		}

		if ((characterName == "Woodsman" || characterName == "Wolf") && chaseGoal.GetComponent<Character>().dead)
		{
			uiReceive.addLine(characterName + "'s target has died, and they have lost all purpose in life");
			killSelf();
		}

		movement = transform.position;

		displayInfo();
		if ((transform.position.x > mapBounds || transform.position.x < 0 || transform.position.z > mapBounds || transform.position.z < 0) && !dead)
		{
			uiReceive.addLine(characterName + " has exited map bounds and will respawn at a random new position");
			spawn();
			starting = true;
			t = 0;
		}
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
			case ((int)status.pathing):
				goalDir = ((GameObject)pathGoals[lookIndex]).transform.position - transform.position;
				break;
			case ((int)status.speaking):
				goalDir = speakGoal.transform.position - transform.position;
				break;
			case ((int)status.idle):
				goalDir = head.transform.forward * 10 + (.5f - Random.value) * head.transform.right;
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
		transform.position = new Vector3(-10, -10, -10);
		selfBody.isKinematic = true;
		dead = true;
		uiReceive.addLine(characterName + " dies horribly");
	}

	void goalStat(int ignore)
	{
		LayerMask groundMask = 1 << 9;
		Ray ray;
		RaycastHit hit;
		if (ignore != 1 && ignore <= 2)
		{
			ray = new Ray(transform.position, (chaseGoal.transform.position - transform.position).normalized);
			hit = new RaycastHit();
			if (Vector3.Distance(transform.position, chaseGoal.transform.position) < 20 &&
				(!Physics.Raycast(ray, out hit, Vector3.Distance(transform.position, chaseGoal.transform.position), groundMask) || goodSmell))
			{
				if (state != (int)status.chasing && state != (int)status.fleeing && willKill)
					uiReceive.addLine(characterName + " detects " + chaseGoal.GetComponent<Character>().characterName + " and begins chasing with killing intent");
				else if (state != (int)status.chasing && state != (int)status.fleeing)
					uiReceive.addLine(characterName + " detects " + chaseGoal.GetComponent<Character>().characterName + " and begins chasing");
				state = (int)status.chasing;
				t = 0;
				resetDisplay();
			}
			else if (Physics.Raycast(ray, out hit, Vector3.Distance(transform.position, chaseGoal.transform.position), groundMask) &&
				state == (int)status.chasing && t > 5)
			{
				uiReceive.addLine(characterName + " no longer detects " + chaseGoal.GetComponent<Character>().characterName + " and ceases chasing");
				state = defaultState;
				t = 0;
				resetDisplay();
			}
		}
		ray = new Ray(transform.position, (fleeGoal.transform.position - transform.position).normalized);
		hit = new RaycastHit();
		if (Vector3.Distance(transform.position, fleeGoal.transform.position) < 16 &&
			(!Physics.Raycast(ray, out hit, Vector3.Distance(transform.position, fleeGoal.transform.position), groundMask) || goodSmell))
		{
			if (state != (int)status.fleeing)
				uiReceive.addLine(characterName + " detects " + fleeGoal.GetComponent<Character>().characterName + " and begins fleeing");
			state = (int)status.fleeing;
			fleeDirection = transform.position - fleeGoal.transform.position;
			fleeDirection = fleeDirection.normalized;
			t = 0;
			resetDisplay();
		}
		else if (Physics.Raycast(ray, out hit, Vector3.Distance(transform.position, fleeGoal.transform.position), groundMask) &&
			state == (int)status.fleeing && t > 7)
		{
			uiReceive.addLine(characterName + " no longer detects " + fleeGoal.GetComponent<Character>().characterName + " and ceases fleeing");
			state = defaultState;
			t = 0;
			resetDisplay();
		}
	}

	void displayInfo()
	{
		switch (state)
		{
			case ((int)status.wandering):
				break;
			case ((int)status.chasing):
				break;
			case ((int)status.fleeing):
				break;
			case ((int)status.pathing):
				if (Vector3.Distance(((GameObject)pathGoals[pathIndex]).transform.position, transform.position) < 5)
				{
					radiusSet(3, transform.position);
				}
				else
				{
					radiusSet(1, Vector3.zero);
				}
				break;
			case ((int)status.speaking):
				break;
			case ((int)status.idle):
				break;
			default:
				break;
		}
		chaseFleeSet();
	}

	void radiusSet(float rad, Vector3 pos)
	{
		radiusDisplay.transform.position = pos;
		radiusDisplay.transform.localScale = new Vector3(rad, .05f, rad);
	}

	void chaseFleeSet()
	{
		if (Vector3.Distance(fleeGoal.transform.position, transform.position) < 20 && Vector3.Distance(fleeGoal.transform.position, transform.position) > 16)
		{
			fleeDisplay.transform.position = transform.position + (fleeGoal.transform.position - transform.position).normalized * 16;
		}
		else
		{
			fleeDisplay.transform.position = Vector3.zero;
		}
		if (Vector3.Distance(chaseGoal.transform.position, transform.position) < 25 && Vector3.Distance(chaseGoal.transform.position, transform.position) > 20)
		{
			chaseDisplay.transform.position = transform.position + (chaseGoal.transform.position - transform.position).normalized * 20;
		}
		else
		{
			chaseDisplay.transform.position = Vector3.zero;
		}
		if (Vector3.Distance(chaseGoal.transform.position, transform.position) < 25)
		{
			LayerMask groundMask = 1 << 9;
			Ray ray;
			RaycastHit hit;
			ray = new Ray(transform.position, (chaseGoal.transform.position - transform.position).normalized);
			hit = new RaycastHit();
			if (!Physics.Raycast(ray, out hit, Vector3.Distance(transform.position, chaseGoal.transform.position), groundMask))
			{
				seeLine.transform.position = (transform.position + chaseGoal.transform.position) / 2;
				seeLine.transform.rotation = Quaternion.LookRotation((transform.position - chaseGoal.transform.position).normalized);
				seeLine.transform.localScale = new Vector3(.1f, .1f, Vector3.Distance(transform.position, chaseGoal.transform.position));
				blindLine.transform.position = Vector3.zero;
			}
			else
			{
				blindLine.transform.position = (transform.position + chaseGoal.transform.position) / 2;
				blindLine.transform.rotation = Quaternion.LookRotation((transform.position - chaseGoal.transform.position).normalized);
				blindLine.transform.localScale = new Vector3(.1f, .1f, Vector3.Distance(transform.position, chaseGoal.transform.position));
				seeLine.transform.position = Vector3.zero;
			}
		}
		else
		{
			blindLine.transform.position = Vector3.zero;
			seeLine.transform.position = Vector3.zero;
		}
	}

	public void forceTalk(GameObject sg)
	{
		state = (int)status.speaking;
		speakGoal = sg;
		resetDisplay();
		t = 0;
		uiReceive.addLine(characterName + " begins talking to " + speakGoal.GetComponent<Character>().characterName + " because " + speakGoal.GetComponent<Character>().characterName + " is bothering them");
	}

	public void danger()
	{
		moveSpeed = moveSpeed / 2;
		uiReceive.addLine(characterName + " detects that something is amiss and starts pathing to grandma's house");
		state = (int)status.pathing;
		pathIndex = 0;
		lookIndex = 0;
	}

	void spawn()
	{
		selfBody.velocity = Vector3.zero;
		transform.position = new Vector3(Mathf.Clamp(Random.value * mapBounds, 10, mapBounds - 10), 10, Mathf.Clamp(Random.value * mapBounds, 10, mapBounds - 10));
		LayerMask groundMask = 1 << 9;
		Ray ray = new Ray(transform.position, -transform.up);
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(ray, out hit, 25f, groundMask))
		{
			selfBody.position = hit.point + new Vector3(0, .8f, 0);
		}
		state = defaultState;
		resetDisplay();
	}

	void resetDisplay()
	{
		wanderGoal.transform.position = Vector3.zero;
		radiusDisplay.transform.position = Vector3.zero;
		radiusDisplay.transform.localScale = new Vector3(1, .05f, 1);
		chaseDisplay.transform.position = Vector3.zero;
		fleeDisplay.transform.position = Vector3.zero;
		blindLine.transform.position = Vector3.zero;
		blindLine.transform.localScale = Vector3.zero;
		seeLine.transform.position = Vector3.zero;
		seeLine.transform.localScale = Vector3.zero;
	}
}

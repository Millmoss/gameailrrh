using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCamera : MonoBehaviour
{
	public Camera unitCamera;
	public Transform cameraFocus;
	public float xSensitivity = 1f;
	public float ySensitivity = -1;
	public float moveSentivity = 1;
	public float scrollPower = 5f;
	private Vector3 playerChange;
	private float xLook;
	private float yLook;
	private float xMove;
	private float zMove;
	private float rotateSpeed;
	private float moveSpeed;
	public bool mouseLock = true;
	private bool rotateCamera = false;
	private float yRotationDif = 0;
	private float scrollPosition;
	private float scrollGoal;

	void Start()
	{
		scrollPosition = 15;
		scrollGoal = 15;
		transform.position = cameraFocus.position - (cameraFocus.position - new Vector3(cameraFocus.transform.position.x, cameraFocus.transform.position.y + 1f, cameraFocus.transform.position.z - 1f)).normalized * 10 / scrollPosition;
		cameraFocus.LookAt(Camera.main.transform);
		unitCamera.transform.Rotate(Vector3.right, 45);
		playerChange = cameraFocus.transform.position;
		rotateSpeed = 20f;
		//transform.RotateAround(cameraFocus.transform.position, Vector3.up, 180);
		yRotationDif = transform.rotation.eulerAngles.y - yRotationDif;
		cameraFocus.transform.Rotate(Vector3.up, yRotationDif);
		yRotationDif = transform.rotation.eulerAngles.y;
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.LeftAlt))
		{
			mouseLock = !mouseLock;
		}

		//camera move input
		
		if (Input.GetMouseButton(1))
		{
			if (Input.GetMouseButtonDown(1))
			{
				Cursor.visible = false;
				rotateCamera = true;
				Cursor.lockState = CursorLockMode.Confined;
			}
			xLook += xSensitivity * Input.GetAxis("Mouse X") * .25f;
			yLook += ySensitivity * Input.GetAxis("Mouse Y") * .25f;
			if (xLook > 180f)
				xLook -= 360f;
			if (xLook < -180f)
				xLook += 360f;
			if (yLook >= 90f)
				yLook = 89f;
			if (yLook < -45f)
				yLook = -45f;
		}
		else if (Input.GetMouseButtonUp(1))
		{
			Cursor.visible = true;
			rotateCamera = false;
			Cursor.lockState = CursorLockMode.None;
		}

		if (Input.GetKey(KeyCode.LeftShift))
			moveSpeed = 2;
		else
			moveSpeed = 5;

		xMove = 0;
		zMove = 0;

		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
		{
			zMove -= 1;
		}
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
		{
			xMove += 1;
		}
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
		{
			zMove += 1;
		}
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
		{
			xMove -= 1;
		}

		Vector3 clampedMove = Vector3.ClampMagnitude(new Vector3(xMove, 0, zMove), 1);
		xMove = clampedMove.x;
		zMove = clampedMove.z;

		scrollGoal -= Input.GetAxis("Mouse ScrollWheel") * scrollPower;
		if (scrollGoal < 1)
			scrollGoal = 1;
		else if (scrollGoal > 25)
			scrollGoal = 25;

		scrollPosition = Mathf.Lerp(scrollPosition, scrollGoal, .4f);

		if (xMove > 0)
			cameraFocus.transform.position -= cameraFocus.transform.right / moveSpeed * Time.deltaTime * 30;
		else if (xMove < 0)
			cameraFocus.transform.position += cameraFocus.transform.right / moveSpeed * Time.deltaTime * 30;
		if (zMove > 0)
			cameraFocus.transform.position -= cameraFocus.transform.forward / moveSpeed * Time.deltaTime * 30;
		else if (zMove < 0)
			cameraFocus.transform.position += cameraFocus.transform.forward / moveSpeed * Time.deltaTime * 30;

		LayerMask groundMask = 1 << 9;
		Ray ray = new Ray(cameraFocus.transform.position, -cameraFocus.up);
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(ray, out hit, 25f, groundMask))
		{
			cameraFocus.position = hit.point + new Vector3(0, .3f);
		}

		transform.eulerAngles = new Vector3(0, xLook, 0);
		unitCamera.transform.localEulerAngles = new Vector3(yLook, 0);
		transform.position = cameraFocus.position - unitCamera.transform.forward * scrollPosition * .6f;
		cameraFocus.transform.eulerAngles = new Vector3(0.0f, xLook, 0.0f);
	}
}

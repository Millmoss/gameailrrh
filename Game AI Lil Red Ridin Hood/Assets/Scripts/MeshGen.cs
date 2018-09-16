using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeshGen : MonoBehaviour
{
	GameObject coneMesh;
	GameObject houseMesh;

	void Start ()
	{
		/*coneMesh = new GameObject("Plane");
		coneMesh.layer = 0;
		coneMesh.AddComponent<MeshFilter>();
		coneMesh.AddComponent<MeshRenderer>();

		Vector3[] coneVertices = new Vector3[26];    //areamap in the first polyscale square, the inbetweens in the second
		int[] coneTriangles = new int[48 * 3];
		coneVertices[24] = new Vector3(0, 1, 0);
		coneVertices[25] = new Vector3(0, 0, 0);
		for (float i = 0; i < 24; i++)
		{
			float rad = i / 24 * 2 * Mathf.PI;
			float x = Mathf.Cos(rad);
			float z = Mathf.Sin(rad);
			coneVertices[(int)i] = new Vector3(x * .5f, 0, z * .5f);
		}

		for (int i = 0; i < 23; i++)
		{
			coneTriangles[i * 6 + 0] = i + 1;
			coneTriangles[i * 6 + 1] = i;
			coneTriangles[i * 6 + 2] = 24;
			coneTriangles[i * 6 + 3] = i;
			coneTriangles[i * 6 + 4] = i + 1;
			coneTriangles[i * 6 + 5] = 25;
		}

		coneTriangles[23 * 6 + 0] = 0;
		coneTriangles[23 * 6 + 1] = 23;
		coneTriangles[23 * 6 + 2] = 24;
		coneTriangles[23 * 6 + 3] = 23;
		coneTriangles[23 * 6 + 4] = 0;
		coneTriangles[23 * 6 + 5] = 25;

		coneMesh.GetComponent<MeshFilter>().mesh.vertices = coneVertices;
		coneMesh.GetComponent<MeshFilter>().mesh.triangles = coneTriangles;
		coneMesh.GetComponent<MeshFilter>().mesh.RecalculateNormals();

		AssetDatabase.CreateAsset(coneMesh.GetComponent<MeshFilter>().mesh, "Assets/ConeMesh");
		AssetDatabase.SaveAssets();*/

		houseMesh = new GameObject("Plane");
		houseMesh.layer = 0;
		houseMesh.AddComponent<MeshFilter>();
		houseMesh.AddComponent<MeshRenderer>();

		Vector3[] houseVertices = new Vector3[10];    //areamap in the first polyscale square, the inbetweens in the second
		int[] houseTriangles = new int[16 * 3];
		houseVertices[0] = new Vector3(0, 0, 0);
		houseVertices[1] = new Vector3(0, 0, 1);
		houseVertices[2] = new Vector3(1, 0, 0);
		houseVertices[3] = new Vector3(1, 0, 1);
		houseVertices[4] = new Vector3(0, 1, 0);
		houseVertices[5] = new Vector3(0, 1, 1);
		houseVertices[6] = new Vector3(1, 1, 0);
		houseVertices[7] = new Vector3(1, 1, 1);
		houseVertices[8] = new Vector3(0, 1.5f, .5f);
		houseVertices[9] = new Vector3(1, 1.5f, .5f);

		houseTriangles[0] = 0;
		houseTriangles[1] = 1;
		houseTriangles[2] = 2;
		houseTriangles[3] = 1;
		houseTriangles[4] = 2;
		houseTriangles[5] = 3;
		houseTriangles[6] = 0;
		houseTriangles[7] = 1;
		houseTriangles[8] = 4;
		houseTriangles[9] = ;
		houseTriangles[10] = ;
		houseTriangles[11] = ;

		houseMesh.GetComponent<MeshFilter>().mesh.vertices = houseVertices;
		houseMesh.GetComponent<MeshFilter>().mesh.triangles = houseTriangles;
		houseMesh.GetComponent<MeshFilter>().mesh.RecalculateNormals();

		AssetDatabase.CreateAsset(houseMesh.GetComponent<MeshFilter>().mesh, "Assets/houseMesh");
		AssetDatabase.SaveAssets();
	}
	
	void Update ()
	{
		
	}
}

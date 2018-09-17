using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeshGen : MonoBehaviour
{
	GameObject coneMesh;
	GameObject triangleMesh;

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

		triangleMesh = new GameObject("Plane");
		triangleMesh.layer = 0;
		triangleMesh.AddComponent<MeshFilter>();
		triangleMesh.AddComponent<MeshRenderer>();

		Vector3[] triangleVertices = new Vector3[18];    //areamap in the first polyscale square, the inbetweens in the second
		int[] triangleTriangles = new int[24];
		triangleVertices[0] = new Vector3(0, 0, 0);
		triangleVertices[1] = new Vector3(0, 0, 1);
		triangleVertices[2] = new Vector3(1, 0, 0);
		triangleVertices[3] = new Vector3(1, 0, 1);
		triangleVertices[4] = new Vector3(0, 0, 0);
		triangleVertices[5] = new Vector3(1, 0, 0);
		triangleVertices[6] = new Vector3(0, .5f, .5f);
		triangleVertices[7] = new Vector3(1, .5f, .5f);
		triangleVertices[8] = new Vector3(0, 0, 1);
		triangleVertices[9] = new Vector3(1, 0, 1);
		triangleVertices[10] = new Vector3(0, .5f, .5f);
		triangleVertices[11] = new Vector3(1, .5f, .5f);
		triangleVertices[12] = new Vector3(0, 0, 0);
		triangleVertices[13] = new Vector3(0, 0, 1);
		triangleVertices[14] = new Vector3(0, .5f, .5f);
		triangleVertices[15] = new Vector3(1, 0, 0);
		triangleVertices[16] = new Vector3(1, 0, 1);
		triangleVertices[17] = new Vector3(1, .5f, .5f);

		triangleTriangles[0] = 1;
		triangleTriangles[1] = 0;
		triangleTriangles[2] = 2;
		triangleTriangles[3] = 1;
		triangleTriangles[4] = 2;
		triangleTriangles[5] = 3;
		triangleTriangles[6] = 5;
		triangleTriangles[7] = 4;
		triangleTriangles[8] = 6;
		triangleTriangles[9] = 5;
		triangleTriangles[10] = 6;
		triangleTriangles[11] = 7;
		triangleTriangles[12] = 8;
		triangleTriangles[13] = 9;
		triangleTriangles[14] = 10;
		triangleTriangles[15] = 10;
		triangleTriangles[16] = 9;
		triangleTriangles[17] = 11;
		triangleTriangles[18] = 12;
		triangleTriangles[19] = 13;
		triangleTriangles[20] = 14;
		triangleTriangles[21] = 16;
		triangleTriangles[22] = 15;
		triangleTriangles[23] = 17;

		triangleMesh.GetComponent<MeshFilter>().mesh.vertices = triangleVertices;
		triangleMesh.GetComponent<MeshFilter>().mesh.triangles = triangleTriangles;
		triangleMesh.GetComponent<MeshFilter>().mesh.RecalculateNormals();

		AssetDatabase.CreateAsset(triangleMesh.GetComponent<MeshFilter>().mesh, "Assets/triangleMesh");
		AssetDatabase.SaveAssets();
	}
	
	void Update ()
	{
		
	}
}

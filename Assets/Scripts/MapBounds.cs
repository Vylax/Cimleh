using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBounds : MonoBehaviour {

    public GameObject BoundCollider;

	public void GenMesh(float length, float width, float height)
    {
        List<GameObject> bounds = new List<GameObject>();
        bounds.Add(Instantiate(BoundCollider, new Vector3(length - 0.5f, width / 2f + 1.5f, height / 2f - 0.5f), Quaternion.Euler(90, -90, 0)));
        bounds.Add(Instantiate(BoundCollider, new Vector3(-0.5f, width / 2f + 1.5f, height / 2f-0.5f), Quaternion.Euler(90, 90, 0)));
        bounds.Add(Instantiate(BoundCollider, new Vector3(length /2f- 0.5f, width / 2f + 1.5f, height -0.5f), Quaternion.Euler(90, 180, 0)));
        bounds.Add(Instantiate(BoundCollider, new Vector3(length /2f- 0.5f, width / 2f + 1.5f, -0.5f), Quaternion.Euler(90, 0, 0)));
        bounds.Add(Instantiate(BoundCollider, new Vector3(length /2f- 0.5f, width+3.5f, height / 2f - 0.5f), Quaternion.Euler(180, 0, 0)));
        bounds.Add(Instantiate(BoundCollider, new Vector3(length /2f- 0.5f, -0.5f, height / 2f - 0.5f), Quaternion.Euler(0, 0, 0)));
        for (int i = 0; i < bounds.Count; i++)
        {
            if(i<4)
            {
                bounds[i].transform.localScale = new Vector3(length / 10f, 1, (height + 8) / 20f);
            }
            else
            {
                bounds[i].transform.localScale = new Vector3(length / 10f, 1, height / 10f);
            }
            bounds[i].tag = "Map";
        }
        MeshFilter filter = GetComponent<MeshFilter>();
        if (!GetComponent<MeshFilter>())
        {
            filter = gameObject.AddComponent<MeshFilter>();
        }
        width += 4;
        transform.position = new Vector3(length / 2f - 0.5f, width / 2f -0.5f, height / 2f - 0.5f);
        Mesh mesh = filter.mesh;
        mesh.Clear();

        #region Vertices
        Vector3 p0 = new Vector3(-length * .5f, -width * .5f, height * .5f);
        Vector3 p1 = new Vector3(length * .5f, -width * .5f, height * .5f);
        Vector3 p2 = new Vector3(length * .5f, -width * .5f, -height * .5f);
        Vector3 p3 = new Vector3(-length * .5f, -width * .5f, -height * .5f);

        Vector3 p4 = new Vector3(-length * .5f, width * .5f, height * .5f);
        Vector3 p5 = new Vector3(length * .5f, width * .5f, height * .5f);
        Vector3 p6 = new Vector3(length * .5f, width * .5f, -height * .5f);
        Vector3 p7 = new Vector3(-length * .5f, width * .5f, -height * .5f);

        Vector3[] vertices = new Vector3[]
        {
	// Bottom
	p0, p1, p2, p3,
 
	// Left
	p7, p4, p0, p3,
 
	// Front
	p4, p5, p1, p0,
 
	// Back
	p6, p7, p3, p2,
 
	// Right
	p5, p6, p2, p1,
 
	// Top
	p7, p6, p5, p4
        };
        #endregion

        #region Normales
        Vector3 up = Vector3.up;
        Vector3 down = Vector3.down;
        Vector3 front = Vector3.forward;
        Vector3 back = Vector3.back;
        Vector3 left = Vector3.left;
        Vector3 right = Vector3.right;

        Vector3[] normales = new Vector3[]
        {
	// Bottom
	down, down, down, down,
 
	// Left
	left, left, left, left,
 
	// Front
	front, front, front, front,
 
	// Back
	back, back, back, back,
 
	// Right
	right, right, right, right,
 
	// Top
	up, up, up, up,
        };
        #endregion

        #region UVs
        Vector2 _00 = new Vector2(0f, 0f);
        Vector2 _10 = new Vector2(1f, 0f);
        Vector2 _01 = new Vector2(0f, 1f);
        Vector2 _11 = new Vector2(1f, 1f);

        Vector2[] uvs = new Vector2[]
        {
	// Bottom
	_11, _01, _00, _10,
 
	// Left
	_11, _01, _00, _10,
 
	// Front
	_11, _01, _00, _10,
 
	// Back
	_11, _01, _00, _10,
 
	// Right
	_11, _01, _00, _10,
 
	// Top
	_11, _01, _00, _10,
        };
        #endregion

        #region Triangles
        int[] triangles = new int[]
        {
	// Bottom
	3, 1, 0,
    3, 2, 1,			
 
	// Left
	3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
    3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
 
	// Front
	3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
    3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
 
	// Back
	3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
    3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
 
	// Right
	3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
    3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
 
	// Top
	3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
    3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,

        };
        #endregion

        mesh.vertices = vertices;
        mesh.normals = normales;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        /*Vector3[] normals = mesh.normals;
        for (int i = 0; i < normals.Length; i++)
            normals[i] = -normals[i];
        mesh.normals = normals;

        for (int m = 0; m < mesh.subMeshCount; m++)
        {
            int[] tri = mesh.GetTriangles(m);
            for (int i = 0; i < tri.Length; i += 3)
            {
                int temp = tri[i + 0];
                tri[i + 0] = tri[i + 1];
                tri[i + 1] = temp;
            }
            mesh.SetTriangles(tri, m);
        }*/

    }
}

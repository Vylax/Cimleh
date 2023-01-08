using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMesh : MonoBehaviour {

    [SerializeField]
    public int id = 0;

    /*new Vector2(.6f, .6f),
                new Vector2(.4f, .6f),
                new Vector2(.4f, .4f),
                new Vector2(.6f, .4f)*/

    private Vector2[][][] mobsUVs = new Vector2[][][]
    {
        new Vector2[][]//bloc id 0
        {
            new Vector2[4]// face id 3 bottom
            {
                new Vector2(.6f, .6f),
                new Vector2(.4f, .6f),
                new Vector2(.4f, .4f),
                new Vector2(.6f, .4f)
            },

            new Vector2[4]// face id 5
            {
                new Vector2(.6f, .6f),
                new Vector2(.4f, .6f),
                new Vector2(.4f, .4f),
                new Vector2(.6f, .4f)
            },

            new Vector2[4]// face id 0
            {
                new Vector2(.4f, 0f),
                new Vector2(.6f, 0f),
                new Vector2(.6f, .2f),
                new Vector2(.4f, .2f),
            },
            new Vector2[4]// face id 4
            {
                new Vector2(.6f, .6f),
                new Vector2(.4f, .6f),
                new Vector2(.4f, .4f),
                new Vector2(.6f, .4f)
            },
            new Vector2[4]// face id 1
            {
                new Vector2(.6f, .6f),
                new Vector2(.4f, .6f),
                new Vector2(.4f, .4f),
                new Vector2(.6f, .4f)
            },

            new Vector2[4]// face id 2 top
            {
                new Vector2(.6f, .6f),
                new Vector2(.4f, .6f),
                new Vector2(.4f, .4f),
                new Vector2(.6f, .4f)
            },
        },
        new Vector2[][]//bloc id 1
        {
            new Vector2[4]// face id 3 bottom
            {
                new Vector2(.2f, .6f),
                new Vector2(0f, .6f),
                new Vector2(0f, .4f),
                new Vector2(.2f, .4f)
            },

            new Vector2[4]// face id 5
            {
                new Vector2(.2f, .6f),
                new Vector2(0f, .6f),
                new Vector2(0f, .4f),
                new Vector2(.2f, .4f)
            },

            new Vector2[4]// face id 0
            {
                new Vector2(1f, .6f),
                new Vector2(.8f, .6f),
                new Vector2(.8f, .4f),
                new Vector2(1f, .4f),
            },
            new Vector2[4]// face id 4
            {
                new Vector2(.2f, .6f),
                new Vector2(0f, .6f),
                new Vector2(0f, .4f),
                new Vector2(.2f, .4f)
            },
            new Vector2[4]// face id 1
            {
                new Vector2(.2f, .6f),
                new Vector2(0f, .6f),
                new Vector2(0f, .4f),
                new Vector2(.2f, .4f)
            },

            new Vector2[4]// face id 2 top
            {
                new Vector2(.2f, .6f),
                new Vector2(0f, .6f),
                new Vector2(0f, .4f),
                new Vector2(.2f, .4f)
            },
        }
    };

    private Vector2[][][] facesUVs = new Vector2[][][]
    {
        new Vector2[][]//bloc id 0
        {
            new Vector2[4]// face id 3 bottom
            {
                new Vector2(.6f, .6f),
                new Vector2(.4f, .6f),
                new Vector2(.4f, .4f),
                new Vector2(.6f, .4f)
            },

            new Vector2[4]// face id 5
            {
                new Vector2(.4f, 0f),
                new Vector2(.6f, 0f),
                new Vector2(.6f, .2f),
                new Vector2(.4f, .2f),
            },

            new Vector2[4]// face id 0
            {
                new Vector2(.4f, 0f),
                new Vector2(.6f, 0f),
                new Vector2(.6f, .2f),
                new Vector2(.4f, .2f),
            },
            new Vector2[4]// face id 4
            {
                new Vector2(.4f, 0f),
                new Vector2(.6f, 0f),
                new Vector2(.6f, .2f),
                new Vector2(.4f, .2f),
            },
            new Vector2[4]// face id 1
            {
                new Vector2(.4f, 0f),
                new Vector2(.6f, 0f),
                new Vector2(.6f, .2f),
                new Vector2(.4f, .2f),
            },
          
            new Vector2[4]// face id 2 top
            {
                new Vector2(1f, .6f),
                new Vector2(.8f, .6f),
                new Vector2(.8f, .4f),
                new Vector2(1f, .4f),
            },
        }
    };

    private Vector2[][] facesUVs2 = new Vector2[][]
    {
        new Vector2[4]// bloc id 0 grass
        {
            new Vector2(.8f, .6f),
            new Vector2(1f, .6f),
            new Vector2(.8f, .4f),
            new Vector2(1f, .4f),
        },
        new Vector2[4]// bloc id 1 stone
        {

            new Vector2(.2f, .6f),
            new Vector2(0f, .6f),
            new Vector2(0f, .4f),
            new Vector2(.2f, .4f)
        },
        new Vector2[4]// bloc id 2 water
        {
            new Vector2(.4f, .8f),
            new Vector2(.6f, .8f),
            new Vector2(.4f, 1f),
            new Vector2(.6f, 1f)
        },
        new Vector2[4]// bloc id 3 dirt a
        {

            new Vector2(.6f, .6f),
            new Vector2(.4f, .6f),
            new Vector2(.4f, .4f),
            new Vector2(.6f, .4f)
        },
        new Vector2[4]// bloc id 4
        {
            new Vector2(1f, .2f),
            new Vector2(.8f, .2f),
            new Vector2(.8f, 0f),
            new Vector2(1f, 0f)
        }
    };

    private void Start()
    {
        Cube();
    }

    void Cube()
    {
        if(GetComponent<MeshFilter>())
        {
            DestroyImmediate(GetComponent<MeshFilter>());
        }
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        Mesh mesh = filter.mesh;
        mesh.Clear();

        float length = 1f;
        float width = 1f;
        float height = 1f;

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
	up, up, up, up
        };
        #endregion

        #region UVs
        Vector2 _00 = new Vector2(0f, 0f);
        Vector2 _10 = new Vector2(1f, 0f);
        Vector2 _01 = new Vector2(0f, 1f);
        Vector2 _11 = new Vector2(1f, 1f);

        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                
                if (id >= 100)
                {
                    uvs.Add(mobsUVs[id-100][i][j]);
                }
                else if (id == 0)
                {
                    uvs.Add(facesUVs[id][i][j]);
                }
                else
                {
                    uvs.Add(facesUVs2[id][j]);
                }
            }
        }

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
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
    }
}

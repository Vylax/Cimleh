using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleFinder : MonoBehaviour {

    Camera cam;
    MapGen2 mg2;
    GameObject player;

    public float interactRange = 3f;

    int[] lastPaintedVertex = new int[6];
    Mesh lastPaintedMesh;

    public bool canInteractWithBlocks = true;

    void Start()
    {
        cam = GetComponent<Camera>();
        mg2 = GameObject.Find("MapGenV2").GetComponent<MapGen2>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    /*Color GetCursorBlockColor ()
    {
        RenderTexture rt = new RenderTexture(1920, 1080, 24);
        Camera.main.targetTexture = rt;
        Texture2D screenShot = new Texture2D(1920, 1080, TextureFormat.RGB24, false);
        Camera.main.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, 1920, 1080), 0, 0);
        Camera.main.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        Destroy(rt);
        return screenShot.GetPixel((int)Input.mousePosition.x, (int)Input.mousePosition.y);
    }*/

    void Update()
    {
        if(!canInteractWithBlocks)
        {
            return;
        }
        RaycastHit hit;
        if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            return;

        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
            return;

        Debug.DrawLine(transform.position, hit.point, Color.red);

        Mesh mesh = meshCollider.sharedMesh;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];

        Transform hitTransform = hit.collider.transform;
        p0 = hitTransform.TransformPoint(p0);
        p1 = hitTransform.TransformPoint(p1);
        p2 = hitTransform.TransformPoint(p2);
        Debug.DrawLine(p0, p1);
        Debug.DrawLine(p1, p2);
        Debug.DrawLine(p2, p0);

        if (hit.transform.GetComponent<ChunkMesh>())
        {
            ChunkMesh chunk = hit.transform.GetComponent<ChunkMesh>();

            Vector3 p3 = vertices[chunk.associatedTrisList[hit.triangleIndex * 3 + 0]];
            Vector3 p4 = vertices[chunk.associatedTrisList[hit.triangleIndex * 3 + 1]];
            Vector3 p5 = vertices[chunk.associatedTrisList[hit.triangleIndex * 3 + 2]];

            p3 = hitTransform.TransformPoint(p3);
            p4 = hitTransform.TransformPoint(p4);
            p5 = hitTransform.TransformPoint(p5);
            Debug.DrawLine(p3, p4);
            Debug.DrawLine(p4, p5);
            Debug.DrawLine(p5, p3);

            Color[] colors = mesh.colors;

            colors[triangles[hit.triangleIndex * 3 + 0]] = Color.grey;
            colors[triangles[hit.triangleIndex * 3 + 1]] = Color.grey;
            colors[triangles[hit.triangleIndex * 3 + 2]] = Color.grey;
            colors[chunk.associatedTrisList[hit.triangleIndex * 3 + 0]] = Color.grey;
            colors[chunk.associatedTrisList[hit.triangleIndex * 3 + 1]] = Color.grey;
            colors[chunk.associatedTrisList[hit.triangleIndex * 3 + 2]] = Color.grey;
            mesh.colors = colors;

            bool aaa = triangles[hit.triangleIndex * 3 + 0] != lastPaintedVertex[0];
            bool bbb = triangles[hit.triangleIndex * 3 + 1] != lastPaintedVertex[1];
            bool ccc = triangles[hit.triangleIndex * 3 + 2] != lastPaintedVertex[2];
            bool ddd = chunk.associatedTrisList[hit.triangleIndex * 3 + 0] != lastPaintedVertex[3];
            bool eee = chunk.associatedTrisList[hit.triangleIndex * 3 + 1] != lastPaintedVertex[4];
            bool fff = chunk.associatedTrisList[hit.triangleIndex * 3 + 2] != lastPaintedVertex[5];

            if (aaa || bbb || ccc || ddd || eee || fff)//if differente face is targeted
            {
                if(lastPaintedMesh)
                {
                    try
                    {
                        Color[] colors2 = lastPaintedMesh.colors;
                        for (int i = 0; i < 6; i++)
                        {
                            //colors2[i] = Color.white;
                            colors2[lastPaintedVertex[i]] = Color.white;
                        }
                        lastPaintedMesh.colors = colors2;
                    }
                    catch
                    {
                        Debug.Log("Paint glitch avoided");
                    }
                }

                lastPaintedMesh = mesh;
                lastPaintedVertex[0] = triangles[hit.triangleIndex * 3 + 0];
                lastPaintedVertex[1] = triangles[hit.triangleIndex * 3 + 1];
                lastPaintedVertex[2] = triangles[hit.triangleIndex * 3 + 2];
                lastPaintedVertex[3] = chunk.associatedTrisList[hit.triangleIndex * 3 + 0];
                lastPaintedVertex[4] = chunk.associatedTrisList[hit.triangleIndex * 3 + 1];
                lastPaintedVertex[5] = chunk.associatedTrisList[hit.triangleIndex * 3 + 2];
            }

            if (Input.GetButtonDown("Fire1"))
            {
                int[] xyz = new int[3]
                {
                    chunk.faceNeighborCube[hit.triangleIndex * 3 + 0],
                    chunk.faceNeighborCube[hit.triangleIndex * 3 + 1],
                    chunk.faceNeighborCube[hit.triangleIndex * 3 + 2]
                };
                
                if (xyz[0] >= 0 && xyz[0] < mg2.sizes[0] && xyz[1] >= 0 && xyz[1] < mg2.sizes[1] && xyz[2] >= 0 && xyz[2] < mg2.sizes[2])//if face neighbor cube is within map bounds
                {
                    if (Vector3.Distance(player.transform.position, hit.point) < interactRange)//if is within player range
                    {
                        //if is over the cube or away from it
                        /*if (Mathf.Abs(player.transform.position.y - xyz[1]) > .6f || Mathf.Abs(player.transform.position.x - xyz[0]) > .6f && Mathf.Abs(player.transform.position.z - xyz[2]) > .6f)
                        {*/
                            mg2.PlaceBlock(xyz[0], xyz[1], xyz[2], player.GetComponent<Inventory>().currentlySelectedItem);/*
                        }
                        else
                        {
                            Debug.Log("trop pres");
                        }*/
                    }
                    else
                    {
                        Debug.Log("trop loin");
                    }
                }
            }

            if (Input.GetButtonDown("Fire2"))
            {
                int[] xyz = new int[3]
                {
                    chunk.faceCube[hit.triangleIndex * 3 + 0],
                    chunk.faceCube[hit.triangleIndex * 3 + 1],
                    chunk.faceCube[hit.triangleIndex * 3 + 2]
                };

                if (xyz[0] >= 0 && xyz[0] < mg2.sizes[0] && xyz[1] >= 0 && xyz[1] < mg2.sizes[1] && xyz[2] >= 0 && xyz[2] < mg2.sizes[2])//if face neighbor cube is within map bounds
                {
                    if(Vector3.Distance(player.transform.position, hit.point) < interactRange)//if is within player range
                    {
                        //if is over the cube or away from player
                        /*if (Mathf.Abs(player.transform.position.y - xyz[1]) > 1f || Mathf.Abs(player.transform.position.x - xyz[0]) > .6f && Mathf.Abs(player.transform.position.z - xyz[2]) > .6f)
                        {*/
                            mg2.DestroyBlock(xyz[0], xyz[1], xyz[2], mg2.blocks[xyz[0], xyz[1], xyz[2]]);/*
                        }
                        else
                        {
                            Debug.Log("trop pres");
                        }*/
                    }
                    else
                    {
                        Debug.Log("trop loin");
                    }
                }
            }
        }
        else if(hit.transform.GetComponent<ItemPickup>())
        {

        }
    }
}

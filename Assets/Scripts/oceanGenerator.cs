using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oceanGenerator : MonoBehaviour
{
    public Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    public float y = 1.250f;
    public int xSize = 254;
    public int zSize = 254;
    // Start is called before the first frame update
    void Awake()
    {
        mesh = new Mesh();

        GetComponent<MeshFilter>().mesh = mesh;


        CreateShape();
        UpdateMesh();
        mesh.RecalculateBounds();
    }

    // Update is called once per frame
    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                vertices[i] = new Vector3(x, z, y);
                i++;
            }
        }
        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
}

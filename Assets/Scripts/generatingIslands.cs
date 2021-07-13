using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class generatingIslands : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    Color[] colors;
    public Vector3[] waterEdge;

    public int xSize = 254;
    public int zSize = 254;

    public Gradient grad;

    public float minHeight;
    public float maxHeight;
    // Start is called before the first frame update
    void Start()
    {
        Random.InitState(42);

        mesh = new Mesh();
        
        GetComponent<MeshFilter>().mesh = mesh;
        

        CreateShape();
        UpdateMesh();

    }

    // Update is called once per frame
    void CreateShape() 
    {
        waterEdge = new Vector3[7700];
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        for (int i = 0, w = 0, z = 0; z <= zSize; z++) 
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x*.025f, z*.025f) * 2f;
                if (y > 1.25f & y < 1.45) 
                {
                    waterEdge[w] = new Vector3(x, y, z);
                    w++;
                }

                if (y < minHeight)
                    minHeight = y;
                
                if (y > maxHeight)
                    maxHeight = y;
        
                vertices[i] = new Vector3(x, y, z);
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

        colors = new Color[vertices.Length];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minHeight, maxHeight, vertices[i].y);
                colors[i] = grad.Evaluate(height);
                i++;
            }
        }
    }

    void UpdateMesh() {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();
    }
}

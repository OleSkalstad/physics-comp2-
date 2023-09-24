using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TrianglesScript : MonoBehaviour
{
    public Vector3[] Vertices;
    public int[] Triangles;

    void Awake()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateSurface();
        
        mesh.Clear();
        mesh.vertices = Vertices;
        mesh.triangles = Triangles;
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();
    }
    void CreateSurface()
    {
        Vertices = new Vector3[]
        {
            new Vector3(0, 0.12f, 0),
            new Vector3(0, 0.08f, 0.56112f),
            new Vector3(0.56f, 0.04f, 0.56112f),
            
            new Vector3(0.56f, 0.003f, 0),
            new Vector3(1.12f, 0.005f, 0),
            new Vector3(1.12f, 0.115f, 0.56112f),
        };

        Triangles = new int[]
        {
            1,3,0,
            3,1,2,
            3,2,5,
            5,4,3,
            
        };

    }
    // Update is called once per frame
    void Update()
    {
        
    }
    
    public Vector3 barycentricCoordinates(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 pt)
    {
        Vector2 p12 = p2 - p1;
        Vector2 p13 = p3 - p1;
        Vector3 n = Vector3.Cross(new Vector3(p12.x, 0.0f, p12.y), new Vector3(p13.x, 0.0f, p13.y));
        float areal123 = n.magnitude;
        Vector3 baryc = default;
        // u
        Vector2 p = p2 - pt;
        Vector2 q = p3 - pt;
        n = Vector3.Cross(new Vector3(p.x, 0.0f, p.y), new Vector3(q.x, 0.0f, q.y));
        baryc.x = n.y / areal123;
        // v
        p = p3 - pt;
        q = p1 - pt;
        n = Vector3.Cross(new Vector3(p.x, 0.0f, p.y), new Vector3(q.x, 0.0f, q.y));
        baryc.y = n.y / areal123;
        // w
        p = p1 - pt;
        q = p2 - pt;
        n = Vector3.Cross(new Vector3(p.x, 0.0f, p.y), new Vector3(q.x, 0.0f, q.y));
        baryc.z = n.y / areal123;

        return baryc;
    }
    
   
    public float GetSurfaceHeight(Vector2 p)
    {
        // Loop through each triangle in the mesh.
        for (int i = 0; i < (Triangles.Length); i+=3)
        {

            var p0 = Vertices[Triangles[i]];
            var p1 = Vertices[Triangles[i+1]];
            var p2 = Vertices[Triangles[i+2]];

            //Vector2 newpos = new Vector2(p.x, p.z);
            var baryCoords = barycentricCoordinates(
                new Vector2(p0.x, p0.z), 
                new Vector2(p1.x, p1.z), 
                new Vector2(p2.x, p2.z), 
                p);
        
            // Check if the player's position is inside the triangle.
            if (baryCoords is { x: >= 0.0f, y: >= 0.0f, z: >= 0.0f })
            {
                // The player's position is inside the triangle.
                // Calculate the height of the surface at the player's position.
                float height = baryCoords.x * p0.y + baryCoords.y * p1.y + baryCoords.z * p2.y;

                // Return the height as the height of the surface at the player's position.
                return height;
            }
        }

        return 0.0f;
    }
    

}

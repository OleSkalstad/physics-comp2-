using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;
using System.Globalization;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter))]
public class TrianglesScript : MonoBehaviour
{
    public Vector3[] Vertices;
    public int[] Triangles;
    public int[] Neighbour;
//rendering param

    public GameObject prefab;


    private int arraySize;
    


    
    void Awake()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        
       
        ReadVertices("StreamingAssetsGrid.txt");
        ReadTriangles("StreamingAssetsTWOIndeciesAndNeighbour.txt");
  
        
        mesh.Clear();
        mesh.vertices = Vertices;
        mesh.triangles = Triangles;
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();
        
    }



    
        void ReadVertices(string filename)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, filename);
            

            List<Vector3> vectorList = new List<Vector3>();

            if (File.Exists(filePath))
            {
                string[] text = File.ReadAllLines(filePath);
                if (text.Length > 0)
                {
                    // Size of array is the first element
                    // Had to devide it because the amount of points was too much the fps was abbysmal
                    
                    if (filename=="Vertecies.txt")
                    {
                        arraySize = int.Parse(text[0])/10;
                    }
                    else
                    {
                        arraySize = int.Parse(text[0]);
                    }
                    //setting the first point the offsett so we can bring everything back to 000;
                    string[] StringOffsett = text[1].Split(' ');
                    
                    
                    // Use: '.' as decimal separator instead of ','
                    CultureInfo cultureInfo = new CultureInfo("en-US");

                    //Offsettfunction
                    float xOffsett = float.Parse(StringOffsett[0], cultureInfo);
                    float yOffsett = float.Parse(StringOffsett[1], cultureInfo);
                    float zOffsett = float.Parse(StringOffsett[2], cultureInfo);

                    
                    for (int i = 1; i <= arraySize; i++)
                    {
                        
                        if (i < text.Length)
                        {
                            string[] strValues = text[i].Split(' ');

                            if (strValues.Length == 3)
                            {
                                float x = float.Parse(strValues[0], cultureInfo);
                                float y = float.Parse(strValues[1], cultureInfo);
                                float z = float.Parse(strValues[2], cultureInfo);

                                //CalculateHight(x, y, z);
                                vectorList.Add(new Vector3(x,y,z));
                                //Spawning  spawning the circles everywhere
                                //Instantiate(prefab, vertex, Quaternion.identity);                               
                            }
                        }
                    }

                    Vertices = vectorList.ToArray();
                 
                }
            }
            else
            {
                Debug.Log("Filepath not found: " + filePath);
            }
        }

     
  

        
        
       
        void ReadTriangles(string filename)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, filename);
            List<int> indices = new List<int>();
            List<int> Nabo = new List<int>();

            if (File.Exists(filePath))
            {
                string[] text = File.ReadAllLines(filePath);

                if (text.Length > 0)
                {
                    // Size of array is the first element, returns number of triangles
                    // * 3 (returns number of vertices)
                    
                    //int arraySize = int.Parse(text[0]) * 3;
                    int arraySize = int.Parse(text[0]);
                
                    for (int i = 1; i <= arraySize; i++)
                    {
                        if (i < text.Length)
                        {
                            string[] strValues = text[i].Split(' ');
                        
                            if (strValues.Length >= 6)
                            {
                                
                                // Only add the first three indices to our array
                                indices.Add(int.Parse(strValues[0]));
                                indices.Add(int.Parse(strValues[1]));
                                indices.Add(int.Parse(strValues[2]));
                                Nabo.Add((int.Parse((strValues[3]))));
                                Nabo.Add((int.Parse((strValues[4]))));
                                Nabo.Add((int.Parse((strValues[5]))));
                    
                                // Skip the next ones (holds neighbor information)
                            }
                        }
                    }

                    Neighbour = Nabo.ToArray();
                    Triangles = indices.ToArray();
                }
            }
            else
            {
                Debug.Log("Filepath not found: " + filePath);
            }
        }
        
        #region BaryCords
        //this is a code that calculates barycentriCordinats
        
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
    #endregion

   //Function is to get the surface hight in the location 
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


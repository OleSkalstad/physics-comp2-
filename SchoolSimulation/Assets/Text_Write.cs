using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine.UIElements;


public class Text_Write : MonoBehaviour
{

    public int[] neighbour;
    int arraySize;
//rendering param

    public GameObject prefab;

    private float[] xCompressedCords = new float[40];
    private float[] yCompressedCords = new float[120];
    private float[,] zCompressedCords = new float[40, 120];
    private float[,] medianDevition = new float[40, 120];
    private int[] Indesies;
    


    void Awake()
    {
        Directory.CreateDirectory(Application.streamingAssetsPath + "/GridTextFile/");
        
        InstantiateCompromisedCords();
        ReadVertices("Vertecies.txt");
        ReadVertices("StreamingAssetsGrid.txt");
        WriteIndesiesAndNeighbours();
        WriteCords();

    }

    //this was the code that made a grid with points that was 5 distance between eachother
    private void InstantiateCompromisedCords()
    {
            for (int i = 0; i < xCompressedCords.Length; i++)
            {
                for (int j = 0; j < yCompressedCords.Length; j++)
                {
                    xCompressedCords[i] = i * 5;
                    yCompressedCords[j] = j * 5;
                }
            }
    }
    
    //this function write the cordinates ive meade into a text file so that other scripts can use it
    private void WriteCords()
    {
        //the name of the file is Grid.txt
        string textDocumentation = Application.streamingAssetsPath + "Grid" + ".txt";
        
        //the start of each txt has to include how many rows it has 
        //this codes
        int startnumber = xCompressedCords.Length * yCompressedCords.Length;
        if (!File.Exists(textDocumentation))
        {
            File.WriteAllText(textDocumentation, startnumber.ToString() + " \n");


            for (int i = 0; i < xCompressedCords.Length; i++)
            {
                for (int j = 0; j < yCompressedCords.Length; j++)
                {
                    float median;
                    //some it happen that some had no points and if thats was the case
                    //then we wouldnt want to have math equation that divides 0
                    if (medianDevition[i,j] == 0.0f)
                    {
                        median = 0;
                    }
                    else
                    {
                        //finds out where the avarage hight is in the that perticaluare cordinate
                        median = zCompressedCords[i, j] / medianDevition[i, j];
                    }
                     //this is where we write the code out on a text file x,z,y cordinates
                     //and leave space between them so that they cna be read correctly   
                    File.AppendAllText(textDocumentation,
                        xCompressedCords[i].ToString() + " " + median.ToString() + " " +
                            yCompressedCords[j].ToString() + "\n");
                }
            }
        }
    }

    //Function writes another text document that writes down the triangle and its neighbour
    private void WriteIndesiesAndNeighbours()
    {
        

        Indesies = new int[yCompressedCords.Length*xCompressedCords.Length*6];
        neighbour = new int[yCompressedCords.Length*xCompressedCords.Length*6];

        int TrianglesInRow = (yCompressedCords.Length-1) * 2;
        int trekant = 0;
        int index = 0;
        
        for (int i = 0; i < xCompressedCords.Length-1; i++)
        {
            for (int j = 0; j < yCompressedCords.Length-1; j++)
            {
          

                //trekant 1
                Indesies[index] = j + i * yCompressedCords.Length;
                neighbour[index] = trekant+1;
                index++;
                
                Indesies[index] = 1 + j + i * yCompressedCords.Length;
                neighbour[index] = trekant - 1;
                //if it is on the bottom set neighbour to -1
                if (j == 0)
                {
                    neighbour[index] = -1;
                }
                index++;
                
                Indesies[index] = j + (1 + i) * yCompressedCords.Length;
                if (i < 1)
                {
                    neighbour[index] = -1;
                }
                else
                {
                    neighbour[index] = 1+trekant-TrianglesInRow;
                }
                index++;
                //trekant 2
                trekant++;
                Indesies[index] =  1 + j + i * yCompressedCords.Length;
                neighbour[index] = trekant + TrianglesInRow - 1;
                if (i == xCompressedCords.Length-2)
                {
                    neighbour[index] = -1;
                }
                index++;
                
                
                Indesies[index] = 1+j + (1 + i) * yCompressedCords.Length;
                neighbour[index] = trekant - 1;
                index++;
                
                
                Indesies[index] = j + (1 + i) * yCompressedCords.Length;
                neighbour[index] = trekant + 1;
                if (j == yCompressedCords.Length-2)
                {
                    neighbour[index] = -1;
                }
                index++;
                trekant++;
            }
            
        }

        PrintOutIandN(index);
    }

    //Functoin prints out the indecies and neigbours
    void PrintOutIandN(int IndexSize)
    {
        string textDocumentation = Application.streamingAssetsPath + "TWOIndeciesAndNeighbour" + ".txt";
        int startnumber = IndexSize/3;

        if (!File.Exists(textDocumentation))
        {
            File.WriteAllText(textDocumentation, startnumber.ToString() + " \n");
            for (int i = 0; i <= IndexSize; i+=3)
            {
                File.AppendAllText(textDocumentation,
                    Indesies[i].ToString() + " " +
                    Indesies[i+1].ToString() + " " +
                    Indesies[i+2].ToString() + " " +
                    neighbour[i].ToString() + " " +
                    neighbour[i+1].ToString() + " " +
                    neighbour[i+2].ToString() + " \n"
                    ); 
            }
        }
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
                    int points = arraySize;
                    //Offsettfunction 
                    //i try to bring the first point to 0,0,0 and then drag all the others points back with it
                    Debug.Log(arraySize);
                    float xOffsett = float.Parse(StringOffsett[0], cultureInfo);
                    float yOffsett = float.Parse(StringOffsett[1], cultureInfo);
                    float zOffsett = float.Parse(StringOffsett[2], cultureInfo);
                    
                    int index = 0;
                    
                    for (int i = 1; i <= arraySize; i++)
                    {
                        
                        if (i < text.Length)
                        {
                            string[] strValues = text[i].Split(' ');

                            if (strValues.Length == 3)
                            {
                                //this offsett bring it all the points with the first one
                                float x = float.Parse(strValues[0], cultureInfo)-xOffsett;
                                float y = float.Parse(strValues[1], cultureInfo)-yOffsett;
                                float z = float.Parse(strValues[2], cultureInfo)-zOffsett;
                                
                  
                                //Calculate height was only to make the first txt 
                                CalculateHight(x, y, z);
                                
                                index++;
                                                           
                            }
                        }
                    }

                    
                 
                }
            }
            else
            {
                Debug.Log("Filepath not found: " + filePath);
            }
        }

        //purpose of of this is to calculate the hight by finding every point
        //within the array we made representating the hight data
        private void CalculateHight(float x, float y, float z)
        {
            
            // checks if its within
            for (int j = 0; j < xCompressedCords.Length - 1; j++)
            {
                for (int k = 0; k < yCompressedCords.Length - 1; k++)
                {
                    if (
                        xCompressedCords[j] <= x &&
                        yCompressedCords[k] <= y &&
                        xCompressedCords[j + 1] > x &&
                        yCompressedCords[k + 1] > y
                    )
                    {
                        //the z cordanate is stored with a array with cordiantes. 
                        //adds all z cordinates that are within the the aria and adds how many
                        // this is so we can calculate the avarage hight
                        zCompressedCords[j,k] += z;
                        medianDevition[j,k] += 1;
                    }
                }
            }
        }

        
        
    
        void ReadTriangles(string filename)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, filename);
            List<int> indices = new List<int>();

            if (File.Exists(filePath))
            {
                string[] text = File.ReadAllLines(filePath);

                if (text.Length > 0)
                {
                    // Size of array is the first element, returns number of triangles
                    // * 3 (returns number of vertices)
                    int arraySize = int.Parse(text[0]) * 3;
                
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
                    
                                // Skip the next ones (holds neighbor information)
                                
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.Log("Filepath not found: " + filePath);
            }
        }
        
}

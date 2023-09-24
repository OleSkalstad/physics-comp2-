using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSript : MonoBehaviour
{
    public TrianglesScript Surface;
    
    [SerializeField] float xPos =0.1f;
    [SerializeField] float yPos =0.1f;
    private Vector3 CurrentVelocity;
    private Vector3 CurrentLocation;
    private Vector3 NextLocation;
    private float gravity = 9.81f;
    public float radius = 0.05f;
    private int PrevuesIndex;
    public Vector3 NextVelocity;
    public Vector3 AfterVelocity;
  
    private Vector3 prevNormal;
    private Vector3 normal;
    
    public Vector3 DisplayNormal;



    
    

    void move()
    {
        for (int i = 0; i < Surface.Triangles.Length; i+=3)
        {
            //going through all the points
            Vector3 p0 = Surface.Vertices[Surface.Triangles[i]];
            Vector3 p1 = Surface.Vertices[Surface.Triangles[i+1]];
            Vector3 p2 = Surface.Vertices[Surface.Triangles[i+2]];
            
            //this is for finding out where the x and z cords is 
            Vector2 BalPos = new(transform.position.x,transform.position.z);
            
            //putting this in a barycords that that will give vector
            var baryCords = Surface.barycentricCoordinates(
                new Vector2(p0.x, p0.z), 
                new Vector2(p1.x, p1.z), 
                new Vector2(p2.x, p2.z), 
                BalPos);
            //if any of the barycords is less than 0 its outside of the triangle
            // so this will find which triangle its in if no one is below 0
            if (baryCords.x >= 0.0f && baryCords.y >= 0.0f && baryCords.z >= 0.0f)
            {
                //this will track wich index basically which will tell later if it has transitioned to a different triangle
                int Currentindex = i / 3;
                
                
                Vector3 v1 = p1 - p0;

                Vector3 v2 = p2 - p0;
                
                //current normall
                normal = Vector3.Cross(v1, v2).normalized;
                
                var acceleration = new Vector3(normal.x*normal.y, normal.y*normal.y-1,normal.z*normal.y)*gravity;
                NextVelocity = CurrentVelocity + acceleration*Time.fixedDeltaTime;
                CurrentVelocity = NextVelocity;
                
          
              
                
                NextLocation = CurrentLocation + CurrentVelocity * Time.fixedDeltaTime;
                CurrentLocation = NextLocation;
                transform.position = NextLocation;
                

                if (PrevuesIndex !=Currentindex)
                {
                    
                    Debug.Log(Currentindex);
                        

                    Vector3 newnormal = (prevNormal + normal).normalized;
                    
                    //18.17
                    AfterVelocity = CurrentVelocity - 2 *Vector3.Dot(CurrentVelocity, newnormal) * newnormal;
                    CurrentVelocity = AfterVelocity + acceleration*Time.fixedDeltaTime;
                    
                    // Oppdatere posisjon i retning den nye
                    NextLocation = CurrentLocation + AfterVelocity * Time.fixedDeltaTime;
                    CurrentLocation = NextLocation;
                    transform.position = NextLocation;

                }

                prevNormal = normal;
                PrevuesIndex = Currentindex;
                
                //this is for looking on the normal
                DisplayNormal = normal;

            }

        }
        
    }
    
    void Correction()
    {
        // Find the point on the ground directly under the center of the ball
        Vector3 p = new Vector3(NextLocation.x, 
            Surface.GetSurfaceHeight(new Vector2(NextLocation.x, NextLocation.z)), 
            NextLocation.z);
        
        // Distance vector from center to p
        Vector3 dist = NextLocation - p;
        
        // Distance vector projected onto normal
        Vector3 b = Vector3.Dot(dist, normal) * normal;

        if (b.magnitude <= radius)
        {
            NextLocation = p + radius * normal;
            transform.position = NextLocation;
        }
    }
   

    // Start is called before the first frame update
    void Start()
    {
        Vector3 Initialize = new Vector3(xPos, 0, yPos);
        var hight = Surface.GetSurfaceHeight(Initialize);
        
        CurrentLocation=new Vector3(xPos,hight,yPos);
        transform.position=CurrentLocation;
    }

    // Update is called once per frame
    void Update()
    {
        move();
        //Correction();
    }

    

}

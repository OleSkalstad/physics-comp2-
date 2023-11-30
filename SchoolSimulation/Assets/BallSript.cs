using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
public class BallSript : MonoBehaviour
{
    private float TD=3;

    private TrianglesScript Surface;

    public float maxX;
    public float maxy;
    public float minx;
    public float miny;
    
    private bool mooving = true;
    
    //spline
    private List<Vector3> controlpoints=new();
    public float tmaks =-2;
    
    //physics variables
    private Vector3 CurrentVelocity;
    private Vector3 CurrentLocation;
    private Vector3 NextLocation;
    private float gravity = 9.81f;

    private Vector3 NextVelocity;
    private Vector3 AfterVelocity;
    public float radius = 0.05f;
    private int PrevuesIndex;

  
    private Vector3 prevNormal;
    private Vector3 normal;
    private Vector3 acceleration;
    private bool hitground = true;
    private Vector3 extraH;
    
    public int Trekant=0;

    [SerializeField] private Spline spline;
    //debug
    public float DebugHeight;

    private void Awake()
    {
        
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        Surface = FindObjectOfType<TrianglesScript>();
 
   
        

    }

    
    // the whole funciton that makes it move on the surface 
   

    void initTrekant()
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
                Trekant = i/3;
                
            }

        }
   
    }
      void move2()
    {
    
            //going through all the points
            Vector3 p0 = Surface.Vertices[Surface.Triangles[Trekant*3]];
            Vector3 p1 = Surface.Vertices[Surface.Triangles[Trekant*3+1]];
            Vector3 p2 = Surface.Vertices[Surface.Triangles[Trekant*3+2]];
            
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
                
                Vector3 v1 = p1 - p0;

                Vector3 v2 = p2 - p0;
                
                //current normall
                normal = Vector3.Cross(v1, v2).normalized;

             
                //akselerasjons formel
                acceleration = new Vector3(normal.x*normal.y, normal.y*normal.y-1,normal.z*normal.y)*gravity;
                //velocity formel 
                NextVelocity = CurrentVelocity + acceleration*Time.fixedDeltaTime;
                CurrentVelocity = NextVelocity;
                
          
              
                
                NextLocation = CurrentLocation + 0.5f*CurrentVelocity * Time.fixedDeltaTime;
                CurrentLocation = NextLocation;
                transform.position = NextLocation;
                
                //physics is going to the next trianglel


                prevNormal = normal;
                
                //this is for looking on the normal
                

            }
            else
            {
                //checks which one of baryCords is below 0 and then brings the neigbour to that point
                if (baryCords.x < 0.0f)
                {
                    Trekant = Surface.Neighbour[Trekant * 3];
                }

                if (baryCords.y < 0.0f)
                {
                    Trekant = Surface.Neighbour[Trekant * 3 + 1];
                }

                if (baryCords.z < 0.0f)
                {
                    Trekant = Surface.Neighbour[Trekant * 3 + 2];
                }

          
                //this code part is when we are between two triangles and have to recalculate the normals
                Vector3 v1 = p1 - p0;

                Vector3 v2 = p2 - p0;
                
                //normal of the new triangel
                normal = Vector3.Cross(v1, v2).normalized;

             

                acceleration = new Vector3(normal.x*normal.y, normal.y*normal.y-1,normal.z*normal.y)*gravity;
                NextVelocity = CurrentVelocity + acceleration*Time.fixedDeltaTime;
                CurrentVelocity = NextVelocity;
                
          
              
                
                NextLocation = CurrentLocation + 0.5f*CurrentVelocity * Time.fixedDeltaTime;
                CurrentLocation = NextLocation;
                transform.position = NextLocation;
                

                    
                    
                        
                    //another calculation that runs when 
                    Vector3 newnormal = (prevNormal + normal).normalized;
                    
                    //18.17
                    AfterVelocity = CurrentVelocity - 2 *Vector3.Dot(CurrentVelocity, newnormal) * newnormal;
                    CurrentVelocity = AfterVelocity + acceleration*Time.fixedDeltaTime;
                    
                    // Oppdatere posisjon i retning den nye
                    NextLocation = CurrentLocation + AfterVelocity * Time.fixedDeltaTime;
                    CurrentLocation = NextLocation;
                    transform.position = NextLocation;

                
           

                prevNormal = normal;

            }

        }
    
    
    
   

    // Start is called before the first frame update
    void Start()
    {
        CurrentLocation = transform.position;
    }

  


    // Update is called once per frame
    void Update()
    {
        
        
        if (mooving)
        {
            TD += Time.deltaTime;
    
            
            //DebugHeigt just to show in the editor what hight it finds
            DebugHeight = Surface.GetSurfaceHeight(new Vector2(transform.position.x, transform.position.z));
            //this if statement is just to find if it isnt above ground
            if (transform.position.y <=
                Surface.GetSurfaceHeight(new Vector2(transform.position.x, transform.position.z)) + radius)
            {

                //i make it so that when it hits it dosent bounce and the y velocity is set to 0
                //ofc i cant have it be 0 all the time so have it only play once
                if (hitground)
                {
                    CurrentVelocity.y = 0;
                    hitground = false;
                    initTrekant();
                }
                
                if (TD>=1)
                {
                    controlpoints.Add(transform.position);
                    tmaks += 1;
                    TD = 0;
                }
                
                move2();
                correction();

            }
            //if it is above the ground i just make it fall straight down
            else
            {
                //accelertation is set to only down and * gravity
                acceleration += new Vector3(0, -1, 0) * gravity;
                NextVelocity = CurrentVelocity + acceleration * Time.fixedDeltaTime;
                CurrentVelocity = NextVelocity;

                NextLocation = CurrentLocation + CurrentVelocity * Time.fixedDeltaTime;
                CurrentLocation = NextLocation;
                transform.position = NextLocation;
            }

            //if condition to see if its inside theterrain. I have to set in what those are in the editor
            if (transform.position.x < minx || transform.position.z < miny || transform.position.x > maxX ||
                transform.position.z > maxy)
            {
                mooving = false;
                SplineSpawn();
                //doing this for optimasatin
                Destroy(gameObject);
            }
        }
    
    }

    
    void SplineSpawn()
    {
        // make the last controll point 
        controlpoints.Add(transform.position);
        tmaks++;
        // gives the spline the controll points and the time
        spline.controlPoints = controlpoints;
        spline.Tmax = tmaks;
        //then spawns the spline
      GameObject Bspline =  Instantiate( spline.gameObject, Vector3.zero, Quaternion.identity);
        
       
    }

    
    // Correction function that makes sure the ball is always the radius away from the terrain.
    void correction()
    {
       
            transform.position = new Vector3(transform.position.x,
                Surface.GetSurfaceHeight(new Vector2(transform.position.x, transform.position.z)),
                transform.position.z);
        
    }

}

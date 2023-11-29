using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Spline : MonoBehaviour
{
    private int n;
    private const int _DegreeI = 2;
   
    public int[] _t;
    [SerializeField] public List<Vector3> controlPoints = new();
    public int[] debug;
    
    private const float H = 0.05f;
    private const float Tmin = 0.0f;
    public  float Tmax = 3.0f;



    private void Awake()
    {
        
        n = controlPoints.Count ;
        InitKjøtevektor();
        debug = _t;
        

    }

    void InitKjøtevektor()
    {
        int index = 0;
        int debug = n + _DegreeI;
        _t = new int[n + _DegreeI + 1];
        _t[0] = 0;
        _t[1] = 0;
        _t[2] = 0;
     
        
        for (int i = 0; i < n-2; i++)
        {
            _t[i + 3] = i+1;
            index++;
            Debug.Log(index);
        }

        Debug.Log(debug+ "  n + grad");
  
        _t[n + _DegreeI-1] = index;
        _t[n + _DegreeI ] = index;
        
       
        
    }

    int FindKnotInterval(float x)
    {
        int my = controlPoints.Count - 1; //Index til siste kontollpunkt
        while (my>=0 && my<_t.Length && x < _t[my] && my > _DegreeI)
            my--; 
        
        return my;
    }
    
    //from the notes dag has made
    public Vector3 EvaluateBSplineCurve(float x)
    {
        int my = FindKnotInterval(x);
        Vector3[] a = new Vector3[_DegreeI+1];
        
        
        for (int j = 0; j <= _DegreeI; j++)
        {
            a[_DegreeI - j] = controlPoints[my - j];
        }


        for (int k = _DegreeI; k >0; k--)
            {
                int j = my - k;
                
                for (int i = 0; i < k; i++)
                {
                    j++;


                    float w = (x - _t[j]) / (_t[j + k] - _t[j]);
                    a[i] = a[i] * (1 - w) + a[i + 1] * w;
                }

            }
        
        return a[0];

    }
    
    private void OnDrawGizmos()
    {
        //drawing the points on the map
        foreach (var pointcont in controlPoints)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(pointcont, 0.2f);
        }

        for (int i = 0; i > controlPoints.Capacity; i++)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(controlPoints[i], controlPoints[i + 1]);
        }

        // drawing the spline 
        if (_t.Length > 0)
        {
            var prev = EvaluateBSplineCurve(Tmin);
            for (var t = Tmin + H; t <= Tmax; t += H)
            {
                //Debug.Log("prev:  "+prev);
                var current = EvaluateBSplineCurve(t);
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(prev, current);
                prev = current;
            }
        }
    }

    Vector3 EvaluateBezier(float x)
    {

        Vector3[] a= new Vector3[4]; //4=d+1 for kubisk bezier
        for(int i = 0; i<4; i++)
        {
            a[i] = controlPoints[i];
        }

        for (int j = _DegreeI; j > 0; j--)  //for (int k=1; k<=d; k++)
        {
            for (int i = 0; i < j; i++) //for (int i=0; i<=d-k;i++)
            {
                a[i] =  a[i] * (1 - x) + a[i + 1] * x;
            }
        }

        return a[0];
    }

  
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    
    public GameObject prefab;
    public float spawntime;
    private float TD;
    


    // Click the "Instantiate!" button and a new `prefab` will be instantiated
    // somewhere within -10.0 and 10.0 (inclusive) on the x-z plane

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TD += Time.deltaTime;
        //code that spawns balls randomly
        if (TD >= spawntime)
        {
            var position = new Vector3(Random.Range(0, 188.0f), 70, Random.Range(0.0f, 588.0f));
            Instantiate(prefab, position, Quaternion.identity);
            TD = 0;
        }
    }
}

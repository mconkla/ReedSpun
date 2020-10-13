using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleportTo0 : MonoBehaviour
{
      public  Vector3 position;
    // Start is called before the first frame update
    void Start()
    {
        position = new Vector3(5.3f, -1.489f, 0 );
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = position;
        }
    }
}

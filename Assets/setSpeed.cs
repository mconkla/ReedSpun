using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setSpeed : MonoBehaviour
{
    public bool Slope=true;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            {
                if (Slope == true)
                {
                    other.gameObject.GetComponent<playerMovement>().increaseSpeed();
                }
                else
                {
                    other.gameObject.GetComponent<playerMovement>().decreaseSpeed();
                }
               
            }
        }

    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (Slope == true)
        {
            
        }
        else
        {
            other.gameObject.GetComponent<playerMovement>().increaseSpeed();
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    [HideInInspector]
    public Transform myTF;

    public grabblingGunScript myGrabblingGun;
    

    Rigidbody2D myRB;

    [Range(0, 1f)]
    public float movementSpeed1 = 0.2f;
    [Range(0, 1f)]
    public float movementSpeed2 = 0.3f;
    [Range(10, 100f)]
    public float movementSpeed3 = 0.4f;
    [Range(0, 1f)]
    public float movementSpeed4 = 0.5f;

    [Range(8, 20)]
    public float jumpForce = 2f;
    float jF;

    [Range(0, 3)]
    public float BoostTime = 1f;

    Vector3 movementVector;
    Vector2 jumpVector;

    bool jumpAllowed = true,moving = false;

    private int speedlevel=3;
    Coroutine co;

    // Start is called before the first frame update
    void Awake()
    {
        jF = jumpForce * 10;
        myTF = GetComponent<Transform>();
        myRB = GetComponent<Rigidbody2D>();
        movementVector = new Vector3(movementSpeed3, 0,0);
        jumpVector = new Vector2(0, jF);
      

    }

    // Update is called once per frame
    void Update()
    {
       
        getInput();
    }



    void getInput()
    {
        float horizValue = Input.GetAxis("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            jump();
        }
        if (Mathf.Abs(horizValue) != 0 && !moving)
        {
            moving = true;
            move(horizValue);
        }
        else if (horizValue == 0 && moving)
        {
            moving = false;
            stop();
        }
    }

    void move(float horizValue)
    {
        switch (speedlevel)
        {
            case 4:
                movementVector = new Vector3(movementSpeed4, 0, 0);
                break;
            case 3:
                movementVector = new Vector3(movementSpeed3, 0, 0);
                break;
            case 2:
                movementVector = new Vector3(movementSpeed2, 0, 0);
                break;
            case 1:
                movementVector = new Vector3(movementSpeed1, 0, 0);
                break;
        }
        myRB.AddForce(movementVector * Mathf.Sign(horizValue));
       // myTF.position = myTF.position + (movementVector);
    }

    void stop()
    {
        myRB.velocity = Vector3.zero;
    }
    void jump()
    {

        if (jumpAllowed && !myGrabblingGun.grabbing)
        {
            myRB.AddForce(jumpVector);
            jumpAllowed = false;
        }
        
    }



  public void increaseSpeed()
    {
        if (speedlevel >=3)
        {
            if (co != null)
            {
                StopCoroutine(co);
            }
            speedlevel = 4;
            co = StartCoroutine(waitThenSetSpeed(BoostTime, 3));
        }
        else
        {
            speedlevel += 1;
        }
        Debug.Log("MovementSpeed: "+ speedlevel);
    }

    public void decreaseSpeed()
    {
        
        if (speedlevel == 1)
        {
            return;
        }
        else 
        {
            speedlevel -= 1;
        }
        Debug.Log("MovementSpeed: " + speedlevel);
    }
    private IEnumerator waitThenSetSpeed(float timeToWait, int newSpeedLevel )
    {
        yield return new WaitForSeconds(timeToWait);
        speedlevel = newSpeedLevel;
        Debug.Log("MovementSpeed: " + speedlevel);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {


        if (collision.gameObject.tag == "floor")
        {
           
            jumpAllowed = true;
        }
    }


   

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    [HideInInspector]
    public Transform myTF;

    public grabblingGunScript myGrabblingGun;

    Rigidbody2D myRB;

    [Range(0, .3f)]
    public float movementSpeed = 0.2f;

    [Range(8, 20)]
    public float jumpForce = 2f;
    float jF;

    [Range(0, 3)]
    public float BoostTime = 1f;

    Vector3 movementVector;
    Vector2 jumpVector;

    bool jumpAllowed = true;

    private int speedlevel=3;
    Coroutine co;

    // Start is called before the first frame update
    void Awake()
    {
        jF = jumpForce * 100f;
        myTF = GetComponent<Transform>();
        myRB = GetComponent<Rigidbody2D>();
        movementVector = new Vector3(movementSpeed, 0,0);
        jumpVector = new Vector2(0, jF);
        

    }

    // Update is called once per frame
    void Update()
    {
        move();
        getInput();
    }

    void getInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
            jump();
        }

    }

    void move()
    {
        myTF.position = myTF.position + (movementVector*speedlevel);
    }
    void jump()
    {

        if (jumpAllowed && !myGrabblingGun.grabbing)
        {
            Debug.Log("JUMP");
            myRB.velocity = Vector3.zero;
            myRB.AddForce(jumpVector);
            jumpAllowed = false;
        }
        
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
       if(collision.gameObject.tag == "floor")
        {
            jumpAllowed = true;
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
}

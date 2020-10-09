using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    [HideInInspector]
    public Transform myTF;

    Rigidbody2D myRB;

    [Range(0, 5)]
    public float movementSpeed = 0.5f;

    [Range(8, 20)]
    public float jumpForce = 2f;
    float jF;

    Vector3 movementVector;
    Vector2 jumpVector;

    bool jumpAllowed = true;

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
        myTF.position = myTF.position + movementVector;
    }
    void jump()
    {

        if (jumpAllowed)
        {
            Debug.Log("JUMP");
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

  
}

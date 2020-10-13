using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{

    public CharacterController2D controller;

    public animationHandling animationController;

    float horizontalMove = 0f;

    public float runSpeed = 40f;
    bool jumpBool = false;
    public bool slide = false;

    /*----------------------------------------*/
    [HideInInspector]
    public Transform myTF;
    Rigidbody2D myRB;
    public grabblingGunScript myGrabblingGun;
    

    


    /*-----------------------------------------*/
    [Range(0, 1f)]
    public float movementSpeed1 = 0.2f;
    [Range(0, 1f)]
    public float movementSpeed2 = 0.3f;
    [Range(10, 100f)]
    public float movementSpeed3 = 0.4f;
    [Range(0, 1f)]
    public float movementSpeed4 = 0.5f;
    [Range(0, 3)]
    public float BoostTime = 1f;
    private int speedlevel=3;
    Coroutine co;



    // Start is called before the first frame update
    void Awake()
    {
        
        myTF = GetComponent<Transform>();
        myRB = GetComponent<Rigidbody2D>();
      

    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            jumpBool = true;
        }

        if (Input.GetButton("Slide"))
        {
            slide = true;
        }else if (Input.GetButtonUp("Slide"))
        {
            slide = false;
        }
      
    }

    private void FixedUpdate()
    {
        //Move the character
        controller.Move(horizontalMove * Time.fixedDeltaTime, slide, jumpBool);
        animationController.runValue = horizontalMove;
        jumpBool = false;
    }



  


   

}

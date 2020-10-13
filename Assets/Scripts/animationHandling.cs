using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationHandling : MonoBehaviour
{

    public Animator animationController;

    public bool run = false,inSpeedBurst = false;

    [HideInInspector]
    public float runValue;
    [HideInInspector]
    public bool jump;
    
    
    // Start is called before the first frame update
    void Start()
    {
    }
    
    // Update is called once per frame
    void Update()
    {

     
        run = runValue != 0;
        //Debug.Log(myPS.time);
        animationController.SetBool("run", run);
        if (jump)
            animationController.Play("jump_player_test");

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationHandling : MonoBehaviour
{

    public Animator animationController;

    [HideInInspector]
    public float runValue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        animationController.SetBool("run", runValue != 0);   
    }
}

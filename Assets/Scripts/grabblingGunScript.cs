using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grabblingGunScript : MonoBehaviour
{
    DistanceJoint2D rope;
    GameObject playerGO;

    public Transform grabSurface;

    [HideInInspector]
    public bool grabbing = false;
    bool fire1 = false;

    [SerializeField]
    [Range(-1.0f, 1.0f)]
    public float ankleValue = 0f;

    bool facingRight = true;

    LineRenderer myLineRenderer;




    Vector3 directionRope = new Vector3();
    // Start is called before the first frame update
    void Start()
    {
        
        myLineRenderer = gameObject.GetComponent<LineRenderer>();
        myLineRenderer.enabled = false;
        playerGO = this.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, directionRope * 20f);
        getInput();
        setDirection();
        rayCasting();

      
        
    }

    void rayCasting()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionRope,50f);
        if(hit.collider != null && hit.collider.gameObject.tag == "grabSurface" && fire1 && !grabbing)
        {
            grabbing = true;
            myLineRenderer.enabled = true;
            rope = playerGO.AddComponent<DistanceJoint2D>();
            rope.autoConfigureDistance = false;

            
            float distance = Vector2.Distance(playerGO.transform.position, hit.point);
            if(distance >= hit.collider.gameObject.GetComponent<grabSurfaceScript>().distance)
            {
                distance = hit.collider.gameObject.GetComponent<grabSurfaceScript>().distance - 2f;
            }
            rope.distance = distance ;
            rope.connectedAnchor = hit.point;
            myLineRenderer.SetPosition(1, hit.point);
            
        }
        
        //Debug.Log(hit.collider.gameObject.tag);
        myLineRenderer.SetPosition(0, this.transform.position);
    }

    void setDirection()
    {
        facingRight = playerGO.GetComponent<CharacterController2D>().m_FacingRight;
        float dirVal = 0;
        if (facingRight)
            dirVal = 1;
        else
            dirVal = -1;
        directionRope = new Vector3(transform.position.x + dirVal, transform.position.y + 1, transform.position.z) - transform.position;

        if (ankleValue < 0)
        {
            directionRope = new Vector3(transform.position.x + dirVal - Mathf.Abs(ankleValue), transform.position.y + 1, transform.position.z) - transform.position;
        }
        else if (ankleValue > 0)
        {
            directionRope = new Vector3(transform.position.x + dirVal, transform.position.y + 1 - Mathf.Abs(ankleValue), transform.position.z) - transform.position;
        }
    }

    void getInput()
    {

        if (Input.GetButtonDown("Fire1"))
        {

            fire1 = true;
           


        }
        else if (Input.GetButtonUp("Fire1"))
        {
            fire1 = false;
            grabbing = false;
            DestroyImmediate(rope);
            myLineRenderer.enabled = false;
        }
        
    }
}

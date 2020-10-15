using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
    [SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching
    [SerializeField] private float maxClimAngle = 80f;
    [SerializeField] private float slopeCheckDistance;
    [SerializeField] private float movementSpeed = 10f;

    CircleCollider2D feet;
    float colliderOffset;
    float colliderRadius;
    float slopeDownAngle;
    float slopeDownAngleOld;
    float slopeSideAngle;
    bool isOnSlope;
    Vector2 slopeNormalPerp;
    public PhysicsMaterial2D fric_mat,no_fric_mat;


    /************Speed level test*************/
    public bool speedLevel_One_bool = false;
    public float speedLevel_One_value = 1.2f;
    /************Speed level test*************/

    const float k_GroundedRadius = .02f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    const float k_CeilingRadius = .02f; // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D m_Rigidbody2D;
    [HideInInspector]
    public bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;

    Vector2 targetVelocity;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;
    private bool m_wasCrouching = false;


    bool canWalkOnSlope = true,isJumping = false,canJump = true;
    private void Awake()
    {
        feet = GetComponent<CircleCollider2D>();
        colliderRadius = feet.radius;
        colliderOffset = feet.offset.y;
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();
    }
  
    void SlopeCheck()
    {
        Vector2 checkPos = this.transform.position - new Vector3(0.0f, colliderRadius + Mathf.Abs(colliderOffset), 0.0f);
        SlopeCheckHorizontal(checkPos);
        SlopeCheckVertical(checkPos);
    
    }


    void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(new Vector2(checkPos.x,checkPos.y+0.1f),transform.right,slopeCheckDistance,m_WhatIsGround);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(new Vector2(checkPos.x, checkPos.y+0.1f), -transform.right, slopeCheckDistance, m_WhatIsGround);
        Debug.DrawRay(slopeHitFront.point, Vector2.right, Color.blue);
        Debug.DrawRay(slopeHitBack.point, -Vector2.right, Color.blue);
        if (slopeHitFront)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
            Debug.Log(slopeSideAngle);
        }
        else if (slopeHitBack)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            slopeSideAngle = 0.0f;
            
        }
    }

    void SlopeCheckVertical(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, m_WhatIsGround);
        //this.gameObject.transform.up = hit.normal;
        if (hit)
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;

            slopeDownAngle = Vector2.Angle(hit.normal,Vector2.up);

            if(slopeDownAngle != slopeDownAngleOld)
            {
                isOnSlope = true;
            }
            slopeDownAngleOld = slopeDownAngle;

            Debug.DrawRay(hit.point, slopeNormalPerp, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.green);
           
        }

        if (slopeDownAngle > maxClimAngle || slopeSideAngle > maxClimAngle)
        {
            canWalkOnSlope = false;
        }
        else
        {
            canWalkOnSlope = true;
        }
    }

    private void Update()
    {
        SlopeCheck();
        
        
    }
    private void FixedUpdate()
    {
        
        bool wasGrounded = m_Grounded;
        m_Grounded = false;
        
        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                isJumping = false;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }

      

    }


    public void Move(float move, bool crouch, bool jump)
    {
       
        /***********Speed level test************/
        if (move == 0)
        {
            speedLevel_One_bool = false;
        }
        /************Speed level test*************/
        // If crouching, check to see if the character can stand up
        if (!crouch)
        {
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
            {
                crouch = true;
            }
        }

        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {

            // If crouching
            if (crouch)
            {
                if (!m_wasCrouching)
                {
                    m_wasCrouching = true;
                    OnCrouchEvent.Invoke(true);
                }

                // Reduce the speed by the crouchSpeed multiplier
                move *= m_CrouchSpeed;

                // Disable one of the colliders when crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = false;
            }
            /************Speed level test*************/
            else if (speedLevel_One_bool)
            {
                move *= speedLevel_One_value;
            }
            /************Speed level test*************/



            else
            {
                // Enable the collider when not crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = true;

                if (m_wasCrouching)
                {
                    m_wasCrouching = false;
                    OnCrouchEvent.Invoke(false);
                }
            }



            if (m_Grounded && !isOnSlope) //if not on slope
            {

                targetVelocity.Set(movementSpeed * move, 0.0f);
                m_Rigidbody2D.velocity = targetVelocity;
                //m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
                // Debug.Log("bewgung uf em boden");
            }
            else if (m_Grounded && isOnSlope && canWalkOnSlope && !isJumping) //If on slope
            {
                targetVelocity.Set(slopeNormalPerp.x * -move * movementSpeed, slopeNormalPerp.y * -move * movementSpeed);
                m_Rigidbody2D.velocity = targetVelocity;
                //m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
                Debug.Log("bewgung uf em slope");
            }
            else if (!m_Grounded) //If in air
            {
                targetVelocity.Set(move * movementSpeed, m_Rigidbody2D.velocity.y);
               //m_Rigidbody2D.velocity = targetVelocity;
                m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
                Debug.Log("bewgung uf inna luft");
            }



            // Move the character by finding the target velocity
            //targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
            if (targetVelocity.x == 0)
            {
                feet.sharedMaterial = fric_mat;
            }
            else
            {
                feet.sharedMaterial = no_fric_mat;
            }



            // And then smoothing it out and applying it to the character
            
            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
           
        }
        // If the player should jump...
        if (m_Grounded && jump)
        {
            isJumping = true;
            // Add a vertical force to the player.
            m_Grounded = false;
            targetVelocity.Set(m_Rigidbody2D.velocity.x, m_JumpForce);
            //m_Rigidbody2D.velocity = targetVelocity;
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
            Debug.Log("bewgung uf inna luft");

           // m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
           
        }


    }
    
    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }


    







}

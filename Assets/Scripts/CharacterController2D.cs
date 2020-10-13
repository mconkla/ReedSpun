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

    /************Speed level test*************/
    public bool speedLevel_One_bool = false;
    public float speedLevel_One_value = 1.5f;
    /************Speed level test*************/

    const float k_GroundedRadius = .02f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    const float k_CeilingRadius = .02f; // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D m_Rigidbody2D;
    [HideInInspector]
    public bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;

    Vector3 targetVelocity;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;
    private bool m_wasCrouching = false;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();
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









            
            // Move the character by finding the target velocity
            targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
            

            RayCast();
            if (!m_Grounded && !walkingUp) 
            {
                m_Rigidbody2D.gravityScale = 20f;
            }
            else
            {
                m_Rigidbody2D.gravityScale = 0f;
            }


            // And then smoothing it out and applying it to the character
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

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
            // Add a vertical force to the player.
            m_Grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
           
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

    bool walkingUp = false;
    void RayCast()
    {
        float dirVal = m_FacingRight ? 1 : -1;
        RaycastHit2D horizontalRayCastHit = Physics2D.Raycast(transform.position, Vector2.right * dirVal, 2f);
        Debug.DrawRay(transform.position, Vector2.right * dirVal * 20f);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 10f);
        Debug.DrawRay(transform.position, -Vector2.up * 20f);

        Debug.Log(horizontalRayCastHit.collider);
        if (horizontalRayCastHit.collider != null && horizontalRayCastHit.collider.gameObject.tag == "slope")
        {
            walkingUp = true;
            float slopeAngle = Vector2.Angle(horizontalRayCastHit.normal, Vector2.up);
            if (slopeAngle < maxClimAngle)
                climbSlope(slopeAngle);
        }
        else if (horizontalRayCastHit.collider == null && hit.collider != null && hit.collider.gameObject.tag == "slope")
        {
            walkingUp = false;
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle < maxClimAngle)
                climbSlope(-slopeAngle);
        }
        else if (hit.collider != null && hit.collider.gameObject.tag != "slope")
        {
            climbSlope(0);
        }
       
        
    }


 
        
        
        
    
    void climbSlope(float slopeAngle)
    {
        float moveDistance = Mathf.Abs(targetVelocity.x);
        targetVelocity.y = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        targetVelocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(targetVelocity.x);

    }
}

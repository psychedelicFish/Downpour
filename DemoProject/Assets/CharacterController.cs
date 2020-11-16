using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float moveSpeed;
    public float gravityScale = 1;
    public float groundedDistance;
    public float jumpSpeed;
    public float airSpeedOffset;
    public float maxSlopeAngle;
    float jumpAccel;
    public LayerMask groundMask;
    float moveAccel = 0f;
    public bool grounded = true;

    Rigidbody2D rbd2D;
    SpriteRenderer sr;

    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        rbd2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (Input.GetButtonDown("Attack"))
        {
            animator.SetBool("Attack", true);
        }
        else
        {
            animator.SetBool("Attack", false);
        }

        if (moveAccel < 0)
        {
            sr.flipX = false;
        }
        if(moveAccel > 0)
        {
            sr.flipX = true;
        }

        moveAccel = Input.GetAxis("Horizontal") * moveSpeed;

        if (moveAccel > 0 || moveAccel < 0)
        {
            animator.SetInteger("AnimState", 2);
        }
        else
        {
            animator.SetInteger("AnimState", 0);
        }

        if (grounded && Input.GetButton("Jump"))
        {
            jumpAccel = jumpSpeed;
            grounded = false;
            animator.SetBool("Grounded", false);
            animator.SetBool("Jump", true);
        }

        if (grounded)
        {
            jumpAccel = 0;
            animator.SetBool("Grounded", true);
            animator.SetBool("Jump", false);
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var tempPos = transform.position;
        
        Vector2 moveOffset = new Vector2(0, 0);
        grounded = isGrounded();

        if (!grounded)
        {
            moveAccel *= airSpeedOffset;
            jumpAccel -= gravityScale * Time.deltaTime;
        }

        tempPos += transform.right * moveAccel * Time.deltaTime;
        tempPos += Vector3.up * jumpAccel * Time.deltaTime;

        if (Mathf.Abs(angleOfGround()) > maxSlopeAngle)
        {
            rbd2D.MovePosition(tempPos);
            return;
        }
        else
        {
            rbd2D.SetRotation(Mathf.LerpAngle(rbd2D.rotation, -angleOfGround(), .2f));
            rbd2D.MovePosition(tempPos);
        }
        
    }

    bool isGrounded()
    {
        //Debug.DrawRay(gameObject.transform.position, Vector2.down);
        RaycastHit2D hit;
        hit = Physics2D.Raycast(gameObject.transform.position, Vector2.down, Mathf.Infinity, groundMask);

        if (hit.collider == null)
        {
            return false;
        }
        if (hit.distance <= groundedDistance)
        {
            return true;
        }

        return false;
    }

    float angleOfGround()
    {
        float angle = 0f;
        var temp = gameObject.transform.position;
        temp.y += 0.25f;

        RaycastHit2D hit;
        hit = Physics2D.Raycast(temp, Vector2.down, Mathf.Infinity, groundMask);

        if (hit.normal.x == 0 || hit.normal.y == 0)
        {
            return 0;
        }

        var tanAngle = hit.normal.x / hit.normal.y; //Calculating the angle of the slope not the normal
        angle = Mathf.Atan(tanAngle);
        angle = angle * Mathf.Rad2Deg;
        
        return angle;
    }

}

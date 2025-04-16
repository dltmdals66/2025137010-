using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    [Header("기본이동 설정")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float turnSpeed = 10f;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2.0f;

    public float coyoteTime = 0.15f;
    private float coyoteTimeCounter;
    private bool readGrounded = true;

    public GameObject gliderObject;
    public float gliderFallSpeed = 1.0f;
    public float gliderMoveSpeed = 7.0f;
    public float gliderMaxTime = 5.0f;
    private float gliderTimeLeft;
    private bool isGliding = false;

    private bool isGrounded = true;

    public int coincount = 0;
    public int totalcois = 5;

    public Rigidbody rb;

    void Start()
    {
        if (gliderObject != null)
        {
            gliderObject.SetActive(false);
        }

        gliderTimeLeft = gliderMaxTime;



        coyoteTimeCounter = 0;
    }

    void Update()
    {
        UpdateGroundedState();

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);

        if (movement.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }


        if (Input.GetKey(KeyCode.G) && !isGrounded && gliderTimeLeft > 0)
        {
            if(!isGrounded)
            {
                EnableGlider();

            }

            gliderTimeLeft -= Time.deltaTime;

            if(gliderTimeLeft <= 0)
            {
                DisalbeGlider();

            }
                


            
        }
        else if(isGliding)
        {
            DisalbeGlider();

        }
        if (isGliding)
        {
            ApplyGliderMovemenl(moveHorizontal, moveVertical);

        }
        else
        {

            rb.velocity = new Vector3(moveVertical * moveSpeed, rb.velocity.y, moveVertical * moveSpeed);


            if (rb.velocity.y < 0)
            {

                rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;

            }
            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {

                rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;

            }





        }




        

            rb.velocity = new Vector3(moveHorizontal * moveSpeed, rb.velocity.y, moveVertical * moveSpeed);

        if (rb.velocity.y > 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            readGrounded = false;
            coyoteTimeCounter = 0;
        }

        if(isGrounded)
        {


            if(isGliding)
            {

                DisalbeGlider();
            }
            gliderTimeLeft = gliderMaxTime;

        }


    }
   
    





    void EnableGlider()
    {

        isGliding = true;

        if (gliderObject != null)
        {

            gliderObject.SetActive(true);

        }

        rb.velocity = new Vector3(rb.velocity.x, -gliderMoveSpeed, rb.velocity.z);
    }

    void DisalbeGlider()
    {

        isGliding = false;

        if (gliderObject != null)
        {

            gliderObject.SetActive(false);
        }

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

    }



    void ApplyGliderMovemenl(float horizontal , float vertical)
    {

        Vector3 gliderVelocity = new Vector3(
            horizontal * gliderFallSpeed,
            -gliderFallSpeed,
            vertical * gliderFallSpeed


        );

        rb.velocity = gliderVelocity;
    }
    



    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            readGrounded = true;
            isGrounded = true;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            readGrounded = true;
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            readGrounded = false;
            isGrounded = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("coin"))
        {
            coincount++;
            Destroy(other.gameObject);
            Debug.Log($"코인 수집 : {coincount}/{totalcois}");
        }

        if (other.CompareTag("Door") && coincount == totalcois)
        {
            Debug.Log("게임 클리어");
        }
    }

    void UpdateGroundedState()
    {
        if (readGrounded)
        {
            coyoteTimeCounter -= Time.deltaTime;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
}
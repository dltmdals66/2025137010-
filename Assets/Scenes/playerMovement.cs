using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    public bool isGrounded = true;

    public int coincount = 0;
    public int totalcois = 5;

    public Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");



        rb.velocity = new Vector3(moveHorizontal * moveSpeed, rb.velocity.y, moveVertical * moveSpeed);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;

        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;

        }


    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("coin"))
        {
            coincount++;
            Destroy(other.gameObject);
            Debug.Log($"코인 수집 : {coincount}/{totalcois}");


        }

        if(other.gameObject.tag == "Door" && coincount == totalcois)
        {
            Debug.Log("게임 클리어");


        }

    }


}

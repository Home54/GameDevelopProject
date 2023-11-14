using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
	public GameObject shottingPosition;
	public float jumpHeight =150f;
    public float gravity = 5f;
	public float scaleShot = 1;
	
	private rotateCamera rc;//transport data-->viewVector
	private Rigidbody rb ;
	private GameObject sp;
	private Vector3 vectorMove;//define the move varible
	
    private float xMove;
    private float yMove;
	private float jump = 0f;//contact with input
    private bool isGround;
    private bool allowAnotherJump= true;//
    
    // Start is called before the first frame update
    void Start()
    {
    	rb = GetComponent<Rigidbody>();
	    rc = GetComponent<rotateCamera>();
        vectorMove = Vector3.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    	xMove = Input.GetAxisRaw("Horizontal");
        yMove = Input.GetAxisRaw("Vertical");
        jump = Input.GetAxisRaw("Jump");
        if( Mathf.Abs(jump) < Mathf.Pow( 10 , -6 ) ){
        	allowAnotherJump=true;
        }
        
        vectorMove = transform.right * xMove + transform.forward * yMove ;// surperimpose the previous vector and the current vector 
        vectorMove.y = 0f;
        vectorMove = vectorMove.normalized;
        if (Mathf.Abs( jump - 1 ) < Mathf.Pow( 10 , -6 ) && isGround && allowAnotherJump)
        {
            rb.AddForce (Vector3.up*jumpHeight);
            allowAnotherJump=false;
        }
    	if( vectorMove == Vector3.zero){
    		return;
    	}
        rb.MovePosition( rb.position + vectorMove * Time.fixedDeltaTime * 5 );
        
    }

    void OnCollisionEnter(Collision other )//error will occur if the collision is not from ground
    {
	    if (other.gameObject.tag == "Ground")
	    {
		    isGround = true;
	    }
    }

    void OnCollisionExit(Collision other )
    {
	    if (other.gameObject.tag == "Ground")
	    {
		    isGround = false;
	    }
    }

}

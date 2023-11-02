using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
public class Movement : MonoBehaviour
{
	public int countDownJump = 10;//num of frames to jump
	public rotateCamera rc;
	private Rigidbody rb ;
	private Vector3 vectorMove;//define the move varible
	private Vector3 viewVector;//Transformed vector from polar axis to 3D axis
    private float xMove;
    private float yMove;
    public float jump = 0f;//contact with input
    public float jumpAmount=10f;
    private bool isGround;
    private bool isJumping;
    private float gravity = 5f;
    private bool newJump = true;//to control if continuous jump is availible 
    
    private Vector3 jumpVector ;
    // Start is called before the first frame update
    void Start()
    {
    	rb = GetComponent<Rigidbody>();
	    rc = GetComponent<rotateCamera>();
        vectorMove = Vector3.zero;
        jumpVector = Vector3.zero;
        viewVector = Vector3.zero;
        isJumping = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    	xMove = Input.GetAxisRaw("Horizontal");
        yMove = Input.GetAxisRaw("Vertical");
        jump = Input.GetAxisRaw("Jump");
		
        if (isGround && Mathf.Abs(jump) < Mathf.Pow(10, -6) )
        {
	        newJump = true;
        }
        
        if ( isGround && Mathf.Abs(jump - 1)  < Mathf.Pow( 10 , -6 ) && newJump )
        {
	        countDownJump = 10;
	        isJumping = true;
	        newJump = false;
        }
        
        jumping();
        viewVector.x = Mathf.Sin(rc.vectorView.x / 180 * Mathf.PI );
        viewVector.z = Mathf.Cos(rc.vectorView.x / 180 * Mathf.PI );
        Vector3 VerticalVector = new Vector3( -viewVector.z , 0 , viewVector.x );
        
        vectorMove = ( -VerticalVector * xMove + viewVector * yMove +  jumpVector ).normalized;// surperimpose the previous vector and the current vector 
    	if( vectorMove == Vector3.zero){
    		return;
    	}
        rb.MovePosition( rb.position + vectorMove * Time.fixedDeltaTime * 5 );
        
    }

    void OnCollisionEnter(Collision collision)
    {
	    isGround = true;
    }

    void OnCollisionExit(Collision collision)
    {
	    isGround = false;
    }

    void jumping()//maintain the Jump vector to be 1
    {
	    if (!isJumping)
	    {
		    return;
	    }
	    if ( countDownJump > 0 )//isJumping is true
	    {
		    jumpVector = new Vector3(0, jump * jumpAmount, 0);//always active when jumping
		    countDownJump -= 1;
	    }

	    if (countDownJump == 0 )
	    {
		    jumpVector = new Vector3(0, 0, 0);
		    isJumping = false;
	    }
    }
}

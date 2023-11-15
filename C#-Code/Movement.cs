using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefaultGameConfigDomain;

public class Movement : MonoBehaviour
{
	public GameObject shottingPosition;
	public float jumpHeight =150f;
    public float gravity = 5f;
	public float scaleShot = 1;
	public Vector3 moveTendency;//logic acceleration
	public float MaxSpeed;//vary according to gun
	public float accelerateScale;
	
	private rotateCamera rc;//transport data-->viewVector
	private Rigidbody rb ;
	private GameObject sp;
	public Vector3 vectorMove;//define the move varible( in debug )

	private float accuracy = Mathf.Pow( 10 , -6 );//used in comparision
    private float xMove;
    private float yMove;
	private float jump = 0f;//contact with input
    private bool isGround;
    private bool allowAnotherJump= true;//in one circle( up and down ) of press can jump once 
    
    public float MaxVectorLength = 20.0f;//the length contrain the moveTendency
    public float decayScale=2.5f;//based on player and not changed
    public float speedScale=0f;
    
    
    // Start is called before the first frame update
    void Start()
    {
    	rb = GetComponent<Rigidbody>();
	    rc = GetComponent<rotateCamera>();
        vectorMove = Vector3.zero;
        moveTendency = Vector3.zero;
        MaxSpeed = DefaultGameConfig.MaxSpeed;
        accelerateScale = DefaultGameConfig.accelerateScale;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    	xMove = Input.GetAxisRaw("Horizontal");
        yMove = Input.GetAxisRaw("Vertical");
        jump = Input.GetAxisRaw("Jump");//-1 , 0 , 1

        if ( Mathf.Abs(xMove) > accuracy )//press x direction
        {
	        if ( Mathf.Abs(moveTendency.x) <= MaxVectorLength )
	        {
		        moveTendency.x += xMove * accelerateScale;
	        }
	        else
	        {
		        moveTendency.x = (moveTendency.x > 0) ? (MaxVectorLength) : (-MaxVectorLength);
	        }
        }
        else//did not press x direction
        {
	        if ( Mathf.Abs(moveTendency.x) > accuracy)
	        {
		        moveTendency.x += ( (moveTendency.x > 0 ) ?  ( -1 ) : ( 1 ) ) * decayScale;
	        }
	        else
	        {
		        moveTendency.x = 0;
	        }
        }

        if ( Mathf.Abs(yMove) > accuracy)
        {
	        
	        if ( Mathf.Abs(moveTendency.z) <= MaxVectorLength )
	        {
		        moveTendency.z += yMove * accelerateScale;
	        }
	        else
	        {
		        moveTendency.z = (moveTendency.z > 0) ? (MaxVectorLength) : (-MaxVectorLength);
	        }
        }
        else
        {
	        if (moveTendency.z > accuracy)
	        {
		        moveTendency.z += ( (moveTendency.z > 0 ) ? ( -1 ) : ( 1 ) )* decayScale;
	        }
	        else
	        {
		        moveTendency.z = 0;
	        }
        }

        if ( xMove * moveTendency.x > accuracy )//when pressing x
        {
	        vectorMove.x = moveTendency.x;
        }
        else//xMove is oppsite from the accelerate or just stay stable
        {
	        vectorMove.x = 0;
        }
        
        if ( yMove * moveTendency.z > accuracy )
        {
	        vectorMove.z = moveTendency.z;
        }
        else//yMove is oppsite from the accelerate or just stay stable
        {
	        vectorMove.z = 0;
        }
        
        speedScale = Mathf.Max( Mathf.Abs(vectorMove.x) , Mathf.Abs(vectorMove.z) )/ MaxVectorLength * MaxSpeed;
        vectorMove.y = 0f;
        vectorMove =  ( transform.right * vectorMove.x + transform.forward * vectorMove.z).normalized ;
       
        
        if( Mathf.Abs(jump) < Mathf.Pow( 10 , -6 ) ){
	        allowAnotherJump=true;
        }
        if (Mathf.Abs( jump - 1 ) < Mathf.Pow( 10 , -6 ) && isGround && allowAnotherJump)
        {
            rb.AddForce (Vector3.up*jumpHeight);
            allowAnotherJump=false;
        }
    	if( vectorMove == Vector3.zero){
    		return;
    	}
        rb.MovePosition( rb.position + vectorMove * Time.deltaTime *speedScale );
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

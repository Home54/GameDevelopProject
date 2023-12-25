using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using DefaultGameConfigDomain;

public class Movement : MonoBehaviour
{
	public GameObject shottingPosition;
	public Vector3 moveTendency; //logic acceleration
	public Vector3 vectorMove; //define the move varible( in debug )
	public float jumpHeight = 300f;
	private float MaxVectorLength = 20.0f; //the length contrain the moveTendency
	public float decayScale = 2.5f; //based on player and not changed
	public float speedScale = 0f;
	public float accelerateScale;
	public float MaxSpeed=0f;
	public float MaxSpeedPreviousFrame=0f;
	public float actualSpread;
	
	private Rigidbody rb;
	private GameObject sp;
	private Animator animator;
	private float speedUpScale=2.0f;
	private float accuracy = Mathf.Pow(10, -6); //used in comparision
	private float xMove;
	private float yMove;
	private float jump = 0f; //contact with input
	private bool walk;
	private bool crouch;
	private bool isGround;
	private bool allowAnotherJump = true; //in one circle( up and down ) of press can jump once 
	private float MaxSpeedRunning=DefaultGameConfig.MaxSpeedRunning; //vary according to gun
	private float MaxSpeedWalking=DefaultGameConfig.MaxSpeedWalking; //vary according to gun
	private float MaxSpeedCrouch=DefaultGameConfig.MaxSpeedCrouch; //vary according to gun
	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		vectorMove = Vector3.zero;
		moveTendency = Vector3.zero;
		accelerateScale = DefaultGameConfig.accelerateScale;
		animator = GetComponent<Animator>();
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		xMove = Input.GetAxisRaw("Horizontal");
		yMove = Input.GetAxisRaw("Vertical");
		jump = Input.GetAxisRaw("Jump"); //-1 , 0 , 1
		walk = Input.GetButton("Walk");
		crouch = Input.GetButton("Crouch");

		 MaxSpeedPreviousFrame = MaxSpeed;
		  if ( isGround )
		  {
		  	if ( walk )
		     {
		 	    MaxSpeed = MaxSpeedWalking; //Mathf.MoveTowards( MaxSpeedPreviousFrame , MaxSpeedWalking , speedUpScale );
		     }
		  	else if ( crouch )
		     {
		 	    MaxSpeed = MaxSpeedCrouch; //Mathf.MoveTowards( MaxSpeedPreviousFrame , MaxSpeedCrouch , speedUpScale );
		     }
		  	else//the case that running
		     {
		 	    MaxSpeed = MaxSpeedRunning; //Mathf.MoveTowards( MaxSpeedPreviousFrame , MaxSpeedRunning , 200.0f); 
		     }
		  }//the case that the player is not on the groud adapts to the maxrunning

		if (Mathf.Abs(xMove) > accuracy) //press x direction
		{
			moveTendency.x = Mathf.MoveTowards( moveTendency.x , MaxVectorLength * xMove , accelerateScale );
		}
		else //did not press x direction
		{
			moveTendency.x = Mathf.MoveTowards( moveTendency.x , 0 , decayScale );
		}

		if (Mathf.Abs(yMove) > accuracy)
		{
			moveTendency.z = Mathf.MoveTowards( moveTendency.z , MaxVectorLength * yMove , accelerateScale );
		}
		else
		{
			moveTendency.z = Mathf.MoveTowards( moveTendency.z , 0 , decayScale );
		}

		if (xMove * moveTendency.x > accuracy) //when pressing x
		{
			vectorMove.x = moveTendency.x;
		}
		else //xMove is oppsite from the accelerate or just stay stable
		{
			vectorMove.x = 0;
		}

		if (yMove * moveTendency.z > accuracy)
		{
			vectorMove.z = moveTendency.z;
		}
		else //yMove is oppsite from the accelerate or just stay stable
		{
			vectorMove.z = 0;
		}
		
		speedScale = Mathf.Max(Mathf.Abs(vectorMove.x), Mathf.Abs(vectorMove.z)) / MaxVectorLength * MaxSpeed;
		vectorMove = (transform.right * vectorMove.x + transform.forward * vectorMove.z);
		vectorMove.y = 0f;
		vectorMove = vectorMove.normalized;
		
		rb.MovePosition(rb.position + vectorMove * Time.fixedDeltaTime * speedScale);
		
		if (Mathf.Abs(jump) < Mathf.Pow(10, -6))
		{
			allowAnotherJump = true;
		}

		if (Mathf.Abs(jump - 1) < Mathf.Pow(10, -6) && isGround && allowAnotherJump)
		{
			rb.AddForce(Vector3.up * jumpHeight);
			allowAnotherJump = false;
		}
		actualSpread = calculateSpread();
		setPara( vectorMove );
	}

	private void OnCollisionEnter(Collision other) //error will occur if the collision is not from ground
	{
		if (other.gameObject.tag == "Ground")
		{
			isGround = true;
		}
	}

	private void OnCollisionExit(Collision other)
	{
		if (other.gameObject.tag == "Ground")
		{
			isGround = false;
		}
	}

	float calculateSpread()
	{
		if ( !isGround )
		{
			return DefaultGameConfig.jumpSpread;
		}//then on the ground
		if (walk)
		{
			return DefaultGameConfig.walkSpread;
		}
		else if (crouch)
		{
			return DefaultGameConfig.crouchSpread;
		}
		else if( moveTendency != Vector3.zero )//running state 
		{
			return DefaultGameConfig.runningBaseSpread +
			       DefaultGameConfig.runningMagnitudeSpread * moveTendency.magnitude;
		}
		else
		{
			return DefaultGameConfig.standSpread;
		}
	}

	private void setPara( Vector3 vector )
	{
		animator.SetBool("Jump", ( ( jump > accuracy) ? true : false ) && allowAnotherJump && isGround );
		animator.SetBool("isCrouching", crouch);
		animator.SetBool("Grounded", isGround);
		if (walk)
		{
			vector = vector * 2.0f;
		}
		else if ( crouch )
		{
			vector = vector * 1.414f;
		}
		else
		{
			vector = vector * 4.5f;
		}
		animator.SetFloat("SpeedH",Mathf.Abs(vector.z) );
		animator.SetFloat("SpeedV",Mathf.Abs(vector.x) );
	}  
}
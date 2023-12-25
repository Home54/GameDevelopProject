using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.Animations;
using DefaultGameConfigDomain;

public class Shoting : MonoBehaviour//To bind with Player
{
    public int BulletNum;
	public int BulletNumRemain;
	public float sensitivity = 5f;
	public float maxYAngle = 80f;//Max angle can be less than 80( might bigger than 80 )
	public Camera PlayerView;
	public Ray ViewRay;
	public Ray ViewRaySpread;
	//public Ray BulletHitActual;
	public RaycastHit hit;
	public RaycastHit hitThirdPerson;
	public GameObject BulletHole;
	public Vector3 vectorView = Vector3.zero;//the final vector to turn towards
    private GameObject aimoGUI;
    private GameObject LookAt;
    private GameObject sp;
    private Animator animator;
	private string text;
	private float reload_Timer;
	private float fire_Timer;
	private float changeOrigin_Timer;//replace the coroutine to limit the time
	private bool isReload = false;
	private bool isFire = false;
	private bool changeOrigin = false;
	private Vector3 basisX;
	private Vector3 basisZ;
	private Vector3 basisY;//the player view vector
	public int accumlateAimo=0;
	//temp varible

	private float PerishBulletHole = 5.0f;
	private int Bullet_Capacity=DefaultGameConfig.Bullet_Capacity;
	private int BulletRemain_Capacity=DefaultGameConfig.BulletRemain_Capacity;
	private float  reloadTime=DefaultGameConfig.reloadTime;
	private float  fireInterval=DefaultGameConfig.fireInterval;
	private float  recoverInterval=DefaultGameConfig.recoverInterval;//the interval that the accumlateAimo reduces
	//private float aimResetSpeed=DefaultGameConfig.aimResetSpeed;//the speed that aim reset
	public float VerticalRecoil;
	public float HorizontalRecoil;
	public Vector3 origin = new Vector3(0,0,0);

	private int hitNum = 0;
	//record the time of the last call of action 

    // Start is called before the first frame update
    void Start()
    {
		BulletNum=Bullet_Capacity;
		BulletNumRemain = BulletRemain_Capacity;
		reload_Timer = Time.time;
		fire_Timer = Time.time;
		changeOrigin_Timer = Time.time;
		
        aimoGUI = GameObject.FindWithTag("AimoDisplay");
        BulletHole = GameObject.Find("/BulletHole");
        animator = GetComponent<Animator>();
        LookAt = GameObject.Find("/ying/LookAt");
        sp = GameObject.Find("shotingPosition");
        
		text= BulletNum.ToString() + "/" +  BulletNumRemain.ToString();
		aimoGUI.GetComponent<Text>().text=text;
    }

    // Update is called once per frame
    void Update()
    {
	    vectorView.x += Input.GetAxis("Mouse X") * sensitivity;//rotate around Y
	    vectorView.y -= Input.GetAxis("Mouse Y") * sensitivity;//rotate around X
	    transform.rotation=Quaternion.Euler(0,vectorView.x,0);//rotate the player
	    VerticalRecoil = 0.0f;
	    HorizontalRecoil = 0.0f;
	    if ( !isFire && changeOrigin && accumlateAimo != 0 )//indicate that the recoil is recovered
	    {
		    // vectorView.x = origin.x;
		    // vectorView.y = origin.y;
		    StartCoroutine( InterPView( fireInterval , vectorView , origin - vectorView) ) ;
		    accumlateAimo=0;
	    }
	    
	    basisZ = PlayerView.transform.TransformDirection(Vector3.forward);
	    basisY = PlayerView.transform.TransformDirection(Vector3.up);
	    basisX = PlayerView.transform.TransformDirection(Vector3.right);
	    
	    checkState();//update the state of fire or reloading
	    setPara();
        updateOrigin();
        if( Input.GetKeyDown(KeyCode.R) )
        {
	        if ( isFire || isReload  ||  BulletNumRemain <= 0 || BulletNum > Bullet_Capacity  )
	        {
	        }
	        else
	        {
		        reload();
	        }
        }
         
        if ( Input.GetKey(KeyCode.Mouse0) )
        {
            //Animation just display the effect on the muzzle
            if ( isReload || isFire || BulletNum <= 0)
            {
            }
            else
            {
	            fire();
	            //vectorView =  vectorView + new Vector3( HorizontalRecoil , -VerticalRecoil, 0 );
            }
        }
        vectorView.x = Mathf.Repeat(vectorView.x, 360);
        vectorView.y = Mathf.Clamp(vectorView.y, -maxYAngle, maxYAngle);//90 is max
        LookAt.transform.rotation = Quaternion.Euler( vectorView.y , vectorView.x , 0 );
    }
    private void reload()
    {
	    if (BulletNum == Bullet_Capacity) {return;}
	    
	    reload_Timer = Time.time;//reset the timer
	    accumlateAimo = 0;	    	
	    int temp = BulletNumRemain;
	    BulletNumRemain -= Mathf.Min( Bullet_Capacity , temp + BulletNum , Bullet_Capacity - BulletNum );
	    BulletNumRemain = (BulletNumRemain >= 0) ? BulletNumRemain : 0;
	    BulletNum = Mathf.Max( Mathf.Min( Bullet_Capacity , BulletNum + temp ) , 0 );
	    text= BulletNum.ToString() + "/" +  BulletNumRemain.ToString();	
	    isReload = true;
    }

    private void fire() //use the camera ray
    {
	    fire_Timer = Time.time;//reset the timer
	    changeOrigin_Timer = Time.time;
	    isFire = true;
	    changeOrigin = false;
	    BulletNum--;
	    accumlateAimo++;
	    text = BulletNum.ToString() + "/" + BulletNumRemain.ToString();
	    aimoGUI.GetComponent<Text>().text = text;//set the parameter
	    
	    VerticalRecoil = 50 * DefaultGameConfig.VerticalRecoilMapping(accumlateAimo);
	    HorizontalRecoil = 50 * DefaultGameConfig.HorizontalRecoilMapping(accumlateAimo);//calculate the vertical and horizontal recoil
	    
	    if( GameObject.Find("/ying/FirstPerson").GetComponent<CinemachineVirtualCamera>().enabled ){
	    	BulletHitEffect( 0.0f , LookAt.transform );//shot from the camera( or the player ) //GetComponent<Movement>().actualSpread
	    }
	    else{
	    	BulletHitEffect( 0.0f , sp.transform );//shot from the shotPosition
	    }
	    StartCoroutine(InterPView( fireInterval, vectorView , new Vector3( HorizontalRecoil , -VerticalRecoil, 0 ) )) ;
    }

    private void checkState()
    {
	    if ( Time.time - reload_Timer > reloadTime )
	    {
		    aimoGUI.GetComponent<Text>().text=text;
		    isReload = false;//the reload is in cooldown
	    }
	    
	    if ( Time.time - fire_Timer > fireInterval )
	    {
		    isFire = false;
	    }

	    if (Time.time - changeOrigin_Timer > recoverInterval)
	    {
		    changeOrigin = true;
	    }
    }

    private IEnumerator DestroyBulletHole( GameObject BulletHole )
    {
	    yield return new WaitForSeconds(PerishBulletHole);
	    Destroy(BulletHole);
    }

    private void BulletHitEffect(float actualSpread , Transform shotPosition ) //used to calculate the actual hit of the bullet
    {
	    //is influenced by the Spread-like varible
	    ViewRay = new Ray( PlayerView.transform.position , basisZ ); //get ray of the playerView

	    ViewRaySpread = new Ray(ViewRay.origin,
		    ViewRay.direction + basisX * Random.Range(-1, 1) * actualSpread +
		    basisY * Random.Range(-1, 1) * actualSpread); //Ray( 3D point , 3D Vector ) 

	    //calculate the actual gun hit
	    bool isHit = false;
	    bool thirdPerson = shotPosition.position != PlayerView.transform.position;
	    int mask = ~( 1 << 2 ) ;// the player layer
	    if ( Physics.Raycast( ViewRaySpread, out hit, 200.0f , mask ) ) //the view hit
	    {
	    	 if( thirdPerson ){
	    		if( Physics.Linecast( shotPosition.position , hit.point , out hitThirdPerson , mask ) ){//the view third person bullet hit
				    isHit = true;
	    		}
	    	}
		     else
		     {
			     isHit = true;
		     }
	    }
	    else{//the view ray is not hit then to check if the real bullet trace hit
	    	if( thirdPerson ){
	    		if ( Physics.Raycast( new Ray( shotPosition.position , basisZ ) , out hitThirdPerson, 200.0f , mask ) ) {
				    isHit = true;
	    		}
	    	}
	    }
	    if( isHit ){
	    	 GameObject temp;
	    	 if( thirdPerson ){
	    	 	temp = Instantiate( BulletHole, hitThirdPerson.point + hitThirdPerson.normal * (0.01f),
			    Quaternion.LookRotation(hitThirdPerson.normal));
	    	 }
		     else{
		     	temp = Instantiate( BulletHole, hit.point + hit.normal * (0.01f),
			     Quaternion.LookRotation( hit.normal));
		     }
		    StartCoroutine(DestroyBulletHole(temp));
		    temp.transform.Rotate(90.0f, 0.0f, 0.0f, Space.Self); //draw the Bullet hole
		    hitNum++;
		    Debug.Log(hitNum);
	    }
    }

    private IEnumerator InterPView( float interval , Vector3 View ,Vector3 offset )//to make the move more smooth
    {
	    float temp_interval = interval;
	    for( int i = 0 ; interval > 0 ; interval -= Time.deltaTime , i++ )
	    {
		    vectorView.x = Mathf.MoveTowards( View.x , View.x + offset.x , ( temp_interval - interval ) / temp_interval * Mathf.Abs(offset.x) );vectorView.y = Mathf.MoveTowards( View.y , View.y + offset.y , ( temp_interval - interval ) / temp_interval * Mathf.Abs(offset.y));
		    yield return new WaitForSeconds(0);
	    }
	    vectorView = View + offset;
    } 
	
    private void setPara()
    {
	    animator.SetBool("isFiring", isFire);
	    animator.SetBool("isReloading", isReload);
	    animator.SetBool("isAiming", !( isFire || isReload ) );
    }

    private void updateOrigin()//update the oringin of the view -- used to reset the aimo or calculate the recoil will based on this view vector
    {
	    if (changeOrigin && accumlateAimo == 0 )
	    {
		    origin.x = vectorView.x;
		    origin.y = vectorView.y;
	    }
    }
}


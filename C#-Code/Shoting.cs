using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DefaultGameConfigDomain;

public class Shoting : MonoBehaviour//To bind with Player
{
    public int BulletNum;
	public int BulletNumRemain;
	public float sensitivity = 5f;
	public float maxYAngle = 80f;//Max angle can be less than 80( might bigger than 80 )
	public float Realspread;
	public Ray ViewRay;
	public Ray BulletHit;
	public GameObject BulletHole;
	public RaycastHit hit;
	public Camera PlayerView;
	public Vector3 vectorView = Vector3.zero;//the final vector to turn towards
	public Vector3 vectorRecoil = Vector3.zero;
	
    private GameObject aimoGUI;
	private string text;
	private float reload_Timer;
	private float fire_Timer;//replace the coroutine to limit the time
	private float reset_Timer;
	private bool isReload = false;
	private bool isFire = false;
	private bool isReset = false;
	private Vector3 basisX;
	private Vector3 basisZ;
	private Vector3 basisY;//the player view vector
	public int accumlateAimo=0;
	//use to form the normalized flat
	//temp varible

	private float PerishBulletHole = 5.0f;
	private int Bullet_Capacity=DefaultGameConfig.Bullet_Capacity;
	private int BulletRemain_Capacity=DefaultGameConfig.BulletRemain_Capacity;
	private float  reloadTime=DefaultGameConfig.reloadTime;
	private float  fireInterval=DefaultGameConfig.fireInterval;
	private float  recoverInterval=DefaultGameConfig.recoverInterval;//the interval that the accumlateAimo reduces
	public float VerticalRecoil;
	public float HorizontalRecoil;
	private float standSpread=DefaultGameConfig.standSpread;
	public float aimResetSpeed=DefaultGameConfig.aimResetSpeed;//the speed that aim reset
	//record the time of the last call of action 

    // Start is called before the first frame update
    void Start()
    {
		BulletNum=Bullet_Capacity;
		BulletNumRemain = BulletRemain_Capacity;
		reload_Timer = Time.time;
		fire_Timer = Time.time;
		
        aimoGUI = GameObject.FindWithTag("AimoDisplay");
        BulletHole = GameObject.Find("/BulletHole");
        
		text= BulletNum.ToString() + "/" +  BulletNumRemain.ToString();
		aimoGUI.GetComponent<Text>().text=text;
    }

    // Update is called once per frame
    void Update()
    {
	    vectorView.x += Input.GetAxis("Mouse X") * sensitivity;//rotate around Y
	    vectorView.y -= Input.GetAxis("Mouse Y") * sensitivity;//rotate around X
	    transform.rotation=Quaternion.Euler(vectorView.y,vectorView.x,0);//rotate the player
	    
	    if (Input.GetKey(KeyCode.Mouse1))
		    Cursor.lockState = CursorLockMode.Locked;
	    
	    basisZ = PlayerView.transform.TransformDirection(Vector3.forward);
	    basisY = PlayerView.transform.TransformDirection(Vector3.up);
	    basisX = PlayerView.transform.TransformDirection(Vector3.right);

	    checkState();//update the state of fire or reloading
        if( Input.GetKeyDown(KeyCode.R) )
        {
	        if ( isFire || isReload  ||  BulletNumRemain <= 0 || BulletNum > Bullet_Capacity  )
	        {
		        if (isReload)
		        {
			        //frame based animation
		        }
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
	            if (isFire)
	            {
					vectorRecoil.x = Mathf.MoveTowards( vectorRecoil.x , vectorRecoil.x + HorizontalRecoil, 5 * Time.deltaTime );//should be scaled
					vectorRecoil.y = Mathf.MoveTowards( vectorRecoil.y , vectorRecoil.y - VerticalRecoil , 5 * Time.deltaTime );//should be scaled
					vectorView = Vector2.MoveTowards( vectorView , vectorView + vectorRecoil , 5 * Time.deltaTime ) ;
	            }
            }
            else
            {
	            fire();
            }
        }
		//recovery modular
	    if ( !isFire )
	    {
			vectorRecoil.x = Mathf.MoveTowards( vectorRecoil.x , 0 , 3 * Time.deltaTime );
			vectorRecoil.y = Mathf.MoveTowards( vectorRecoil.y , 0 , 3 * Time.deltaTime );
			vectorView = Vector2.MoveTowards( vectorView , vectorView - vectorRecoil , 3 * Time.deltaTime ) ;
	    }
      
        if ( !isFire && !isReset && accumlateAimo > 0 )
        {
	     	isReset = true;
	        reset_Timer = Time.time;
	        accumlateAimo--;
        }//tested
		
        vectorView.x = Mathf.Repeat(vectorView.x, 360);
        vectorView.y = Mathf.Clamp(vectorView.y, -maxYAngle, maxYAngle);//90 is max
        PlayerView.transform.rotation = Quaternion.Euler( vectorView.y , vectorView.x , 0 );
    }

    private void reload()
    {
	    reload_Timer = Time.time;//reset the timer
	    accumlateAimo = 0;
	    
	    int temp = BulletNumRemain;
	    BulletNumRemain -= Mathf.Min( Bullet_Capacity , temp + BulletNum , Bullet_Capacity - BulletNum );
	    BulletNum = Mathf.Max( Mathf.Min( Bullet_Capacity , BulletNum + temp ) , 0 );
	    text= BulletNum.ToString() + "/" +  BulletNumRemain.ToString();	
	    isReload = true;
    }

    private void fire() //use the camera ray
    {
	    fire_Timer = Time.time;//reset the timer
	    BulletHitEffect(0);//calculate the hit of the bullet
	    VerticalRecoil =  10.0f * DefaultGameConfig.VerticalRecoilMapping(accumlateAimo);
	    HorizontalRecoil =  1.0f * DefaultGameConfig.HorizontalRecoilMapping(accumlateAimo);//calculate the vertical and horizontal recoil

	    BulletNum--;
	    accumlateAimo++;
	    text = BulletNum.ToString() + "/" + BulletNumRemain.ToString();
	    aimoGUI.GetComponent<Text>().text = text;
	    isFire = true;
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

	    if (Time.time - reset_Timer > recoverInterval)
	    {
		    isReset = false;
	    }
    }

    private IEnumerator DestroyBulletHole( GameObject BulletHole )
    {
	    yield return new WaitForSeconds(PerishBulletHole);
	    Destroy(BulletHole);
    }

    private void BulletHitEffect(float actualSpread) //used to calculate the actual hit of the bullet
    {
	    //is influenced by the Spread-like varible
	    basisY = transform.TransformVector(Vector3.forward);
	    ViewRay = new Ray(transform.position, basisY); //get ray of the playerView
	    Vector3.OrthoNormalize(ref basisY, ref basisX, ref basisZ); //to get OrthoNormalize flat of the playerView
	    BulletHit = new Ray(ViewRay.origin,
		    ViewRay.direction + basisX * Random.Range(-1, 1) * actualSpread +
		    basisZ * Random.Range(-1, 1) * actualSpread); //Ray( 3D point , 3D Vector ) 

	    //calculate the actual gun hit
	    if (Physics.Raycast(BulletHit, out hit, 200.0f)) //the view hit
	    {
		    GameObject temp;
		    temp = Instantiate(BulletHole, hit.point + hit.normal * (0.025f + 0.01f),
			    Quaternion.LookRotation(hit.normal));
		    temp.transform.Rotate(90.0f, 0.0f, 0.0f, Space.Self); //draw the Bullet hole
		    StartCoroutine(DestroyBulletHole(temp));
	    }
    }
}

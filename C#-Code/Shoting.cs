using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DefaultGameConfigDomain;

public class Shoting : MonoBehaviour//To bind with Player
{
    //public GameObject projectilePrefab;
    //public Transform projectileSpawn;
    //public float projectileSpeed = 1000f;
    //public float lifeTime = 1f;
    //use for enitity bullet
    
    public int BulletNum;
	public int BulletNumRemain;
	public Ray ray;
	public RaycastHit hit;
	public Camera PlayerView;
	
    private GameObject aimoGUI;
	private string text;
	private int Bullet_Capacity;
	private int BulletRemain_Capacity;
	private float  reloadTime ;
	private float  fireInterval;
	private bool isReload = false;
	private bool isFire = false;

    // Start is called before the first frame update
    void Start()
    {
	    BulletRemain_Capacity=DefaultGameConfig.BulletRemain_Capacity;
	    Bullet_Capacity = DefaultGameConfig.Bullet_Capacity;
	    reloadTime = DefaultGameConfig.reloadTime;
	    fireInterval = DefaultGameConfig.fireInterval;//read the gun config
	    
		BulletNum=Bullet_Capacity;
		BulletNumRemain = BulletRemain_Capacity;
        aimoGUI = GameObject.FindWithTag("AimoDisplay");
        
		text= BulletNum.ToString() + "/" +  BulletNumRemain.ToString();
		aimoGUI.GetComponent<Text>().text=text;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R) && BulletNumRemain > 0 && BulletNum < Bullet_Capacity && !isReload && !isFire )
        {
	        StartCoroutine( reload() );
	        //Animation ( both on UI and the hand model)
        }
         
        if (  Input.GetKey(KeyCode.F) && BulletNum > 0 && !isReload && !isFire )//Mathf.Abs(Input.GetAxis("Fire 1") ) < Mathf.Pow( 10 , -6 )
        {
            //Animation just display the effect on the muzzle
            StartCoroutine( fire() );
        }
    }

    private IEnumerator reload()
    {
	    isReload = true;
	    yield return new WaitForSeconds( reloadTime );
	    int temp = BulletNumRemain;
	    BulletNumRemain -= Mathf.Min( Bullet_Capacity , temp + BulletNum , Bullet_Capacity - BulletNum );
	    BulletNum = Mathf.Min( Bullet_Capacity , BulletNum + temp );
	    text= BulletNum.ToString() + "/" +  BulletNumRemain.ToString();	
	    aimoGUI.GetComponent<Text>().text=text;
	    isReload = false;
    }

    private IEnumerator fire() //use the camera ray
    {
	    isFire = true;
	    if (Physics.Raycast( transform.position , transform.TransformDirection(Vector3.forward) , out hit,200.0f) )
	    {
		    if (hit.collider.gameObject.GetComponent<Renderer>() != null)
		    {
			    hit.collider.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
		    }
	    }
	    BulletNum--;
	    text = BulletNum.ToString() + "/" + BulletNumRemain.ToString();
	    aimoGUI.GetComponent<Text>().text = text;
	    yield return new WaitForSeconds(fireInterval);
	    isFire = false;
    }
}

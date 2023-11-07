using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shoting : MonoBehaviour//To bind with Player
{
    public GameObject projectilePrefab;
    public Transform projectileSpawn;
    public float projectileSpeed = 0f;
    public float lifeTime = 1f;
	public int Bullet_Capacity = 30;
	public int BulletRemain_Capacity = 90;

    public int BulletNum;
	public int BulletNumRemain;

    private GameObject aimoGUI;
	private string text;
    
    // Start is called before the first frame update
    void Start()
    {
		BulletNum=Bullet_Capacity;
		BulletNumRemain=BulletRemain_Capacity;
        aimoGUI = GameObject.FindWithTag("AimoDisplay");
		text= BulletNum.ToString() + "/" +  BulletNumRemain.ToString();
		//Debug.Log(aimoGUI.GetComponent<Text>().text);
    }

    // Update is called once per frame
    void Update()
    {
       
        if(Input.GetKeyDown(KeyCode.R) && BulletNumRemain > 0 && BulletNum < Bullet_Capacity )
        {
			int temp = BulletNumRemain;
			BulletNumRemain -= Mathf.Min( Bullet_Capacity , temp + BulletNum , Bullet_Capacity - BulletNum );
            BulletNum = Mathf.Min( Bullet_Capacity , BulletNum + temp );
			text= BulletNum.ToString() + "/" +  BulletNumRemain.ToString();			
            //Animation ( both on UI and the hand model)
            Debug.Log("Reload complete");
            }
         
        if (  Input.GetKeyDown(KeyCode.F) && BulletNum > 0 )//Mathf.Abs(Input.GetAxis("Fire 1") ) < Mathf.Pow( 10 , -6 )
        {
            Fire();
			Debug.Log("Fire");
            //Animation just display the effect on the muzzle
            BulletNum--;
			text= BulletNum.ToString() + "/" +  BulletNumRemain.ToString();
			Debug.Log(text);				
        }
		aimoGUI.GetComponent<Text>().text=text;
    }
    private void Fire()
    {
        GameObject projectile = Instantiate(projectilePrefab);//clone the bullet 
        Physics.IgnoreCollision( projectile.GetComponent<Collider>(), GetComponent<Collider>());//Disable the collision of bullets and the player
        projectile.transform.position = projectileSpawn.position;
        Vector3 rotation = projectile.transform.rotation.eulerAngles;
        projectile.transform.rotation = Quaternion.Euler(rotation.x + 90 , transform.eulerAngles.y, rotation.z);//let the gun rotate with the player 
        projectile.GetComponent<Rigidbody>().AddForce( projectileSpawn.forward * projectileSpeed, ForceMode.Impulse);//give the bullet a impulse
        StartCoroutine(DestroyProjectile( projectile , lifeTime ) );
    }
    
    private IEnumerator DestroyProjectile (GameObject projectile, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(projectile);
    }

}

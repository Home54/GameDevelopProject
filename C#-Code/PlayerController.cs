using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour//also control the aim
{
	private bool changeForView;
	private float timerTochange;
	private bool isReady = true;
	private int state = 0;	
	private GameObject up;
	private GameObject down;
	private GameObject left;
	private GameObject right;

    // Start is called before the first frame update
    void Start()
    {
        timerTochange = Time.time;
		up=GameObject.Find("/ying/ShootingInf/Crosshair/TopToSpread");
		down=GameObject.Find("/ying/ShootingInf/Crosshair/BottomToSpread");
		left=GameObject.Find("/ying/ShootingInf/Crosshair/LeftToSpread");
		right=GameObject.Find("/ying/ShootingInf/Crosshair/RightToSpread");
    }

    // Update is called once per frame
    void Update()
    {
    	changeForView = Input.GetKey(KeyCode.C);
		if( isReady ){
			if( changeForView && state == 0 ){
				GameObject.Find("/ying/FirstPerson").GetComponent<CinemachineVirtualCamera>().enabled = false;
				GameObject.Find("/ying/ThirdPerson").GetComponent<CinemachineVirtualCamera>().enabled = true;
				state = 1;
				isReady = false;
				return;
			}
			if( changeForView && state == 1 ){
				GameObject.Find("/ying/FirstPerson").GetComponent<CinemachineVirtualCamera>().enabled = true;
				GameObject.Find("/ying/ThirdPerson").GetComponent<CinemachineVirtualCamera>().enabled = false;
				state = 0;
				isReady = false;
				return;
			}
			
		}
		if( Time.time - timerTochange > 0.5f ){
			isReady = true;
			timerTochange = Time.time;
		}
		up.transform.localPosition =  new Vector3( 0 , 10 + GetComponent<Movement>().actualSpread * 50.0f , 0 );
		down.transform.localPosition =  new Vector3( 0 , -10 - GetComponent<Movement>().actualSpread * 50.0f , 0 );
		left.transform.localPosition =  new Vector3(  10 + GetComponent<Movement>().actualSpread * 50.0f , 0 , 0 );
		right.transform.localPosition =  new Vector3( -10 - GetComponent<Movement>().actualSpread * 50.0f , 0 , 0 );
    }
}

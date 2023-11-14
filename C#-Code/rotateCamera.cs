using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateCamera : MonoBehaviour
{
	public Vector2 vectorView;
	public Camera PlayerView;
	public float sensitivity = 5f;
	public float maxYAngle = 80f;//Max angle can be less than 80( might bigger than 80 )
	public Ray ray;
	//public RaycastHit hit;

	private GameObject player;
	
    // Start is called before the first frame update
    void Start()
    {
	    player = GameObject.FindWithTag("Player");
        vectorView = Vector3.zero;
        ray = PlayerView.ScreenPointToRay(Input.mousePosition);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    	vectorView.x += Input.GetAxis("Mouse X") * sensitivity;
        vectorView.y -= Input.GetAxis("Mouse Y") * sensitivity;
        vectorView.x = Mathf.Repeat(vectorView.x, 360);
        vectorView.y = Mathf.Clamp(vectorView.y, -maxYAngle, maxYAngle);//90 is max

        PlayerView.transform.rotation = Quaternion.Euler(vectorView.y,vectorView.x,0);
        player.transform.rotation=Quaternion.Euler(vectorView.y,vectorView.x,0);
        
        ray = PlayerView.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay( transform.position , transform.TransformDirection(Vector3.forward) * 1000, Color.white);

        if (Input.GetMouseButtonDown(0))
            Cursor.lockState = CursorLockMode.Locked;
    }
}

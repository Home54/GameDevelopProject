using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateCamera : MonoBehaviour
{
	private Vector2 vectorView1;
	public Vector2 vectorView;
	public Camera PlayerView;
	
	public float sensitivity = 5f;
	public float maxYAngle = 100f;
    // Start is called before the first frame update
    void Start()
    {
         vectorView = Vector3.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    	vectorView1.x += Input.GetAxis("Mouse X") * sensitivity;
        vectorView1.y -= Input.GetAxis("Mouse Y") * sensitivity;
        vectorView1.x = Mathf.Repeat(vectorView1.x, 360);
        vectorView1.y = Mathf.Clamp(vectorView1.y, -maxYAngle, maxYAngle);
        vectorView = vectorView1;
        PlayerView.transform.rotation = Quaternion.Euler(vectorView.y,vectorView.x,0);
        if (Input.GetMouseButtonDown(0))
            Cursor.lockState = CursorLockMode.Locked;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    
    public float zoomSpeed = 10f;

   
    public float minZ = -25f;
    public float maxZ = 5f;

    void Update()
    {
        
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0f)
        {
      
            Vector3 currentPosition = transform.position;

   
            float newZ = currentPosition.z + scrollInput * zoomSpeed;


            newZ = Mathf.Clamp(newZ, minZ, maxZ);


            transform.position = new Vector3(currentPosition.x, currentPosition.y, newZ);
        }
    }
}

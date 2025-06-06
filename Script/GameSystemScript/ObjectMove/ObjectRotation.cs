using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotation : MonoBehaviour
{

    public float rotationSpeed = 5f;

    public float snapSpeed = 2f;

    public float minXRotation = -90f;
    public float maxXRotation = 90f;

    public float minYRotation = -90f;
    public float maxYRotation = 90f;

    public float minZRotation = -90f;
    public float maxZRotation = 90f;
    public int[] FieldPosition = new int[]{0, 0};

    private Vector3 targetEulerAngles;

    private bool isSnapping = false;

    void Update()
    {

        if (Input.GetMouseButton(0))
        {
            isSnapping = false;

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            Vector3 rotation = new Vector3(-mouseY, mouseX, 0) * rotationSpeed;
           

            transform.Rotate(rotation, Space.World);
           

            


        }


        if (Input.GetMouseButtonUp(0))
        {
            isSnapping = true; 

            Vector3 currentEulerAngles = transform.eulerAngles;
            float normalizedX = NormalizeAngle(currentEulerAngles.x);
            float normalizedY = NormalizeAngle(currentEulerAngles.y);

            float AdjustAxisRotationX = AdjustAxisRotation(normalizedX, 0);
            float AdjustAxisRotationY = AdjustAxisRotation(normalizedY, 1);
            if (FieldPosition[0] == 1) targetEulerAngles = new Vector3(90, 0, 0);
            else if(FieldPosition[0] == 2) targetEulerAngles = new Vector3(-90, 0, 0);
            else targetEulerAngles = new Vector3(
                AdjustAxisRotationX,
                AdjustAxisRotationY,
                0
            );
        }

        
        if (isSnapping)
        {   
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.Euler(targetEulerAngles),
                Time.deltaTime * snapSpeed
            );
            if (Quaternion.Angle(transform.rotation, Quaternion.Euler(targetEulerAngles)) < 0.1f)
            {
                isSnapping = false;
                transform.rotation = Quaternion.Euler(targetEulerAngles);
            }
        }
    }

   

    
    private float NormalizeAngle(float angle)
    {
        if (angle > 180) angle -= 360;
        return angle;
    }

    // Ž²‚ÌŠp“x‚ðŽw’è‚³‚ê‚½”ÍˆÍ‚ÉŠî‚Ã‚¢‚Ä’²®‚·‚éŠÖ”
    private float AdjustAxisRotation(float angle,int field)
    {
        if (angle >= -45f && angle < 45f)
        {
            FieldPosition[field] = 0;
            return 0f;
        }
        else if (angle >= 45f && angle < 135f)
        {
            FieldPosition[field] = 1;
            return 90f;
        }
        else if (angle >= -135f && angle < -45f)
        {   
            FieldPosition[field] = 2;
            return -90f;
        }
        else
        {
            FieldPosition[field] = 3;
            return 180f;
        }

     
    }
}

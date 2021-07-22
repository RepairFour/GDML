using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{

    public InputActionMap cameraControls;
    Transform camTransform;
   
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;
    public float inputGravity = 1f;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;


    Vector2 rotationVector;
    float inputRotationX = 0F;
    float inputRotationY = 0F;

    private List<float> rotArrayX = new List<float>();
    float rotAverageX = 0F;

    private List<float> rotArrayY = new List<float>();
    float rotAverageY = 0F;

    public float frameCounter = 20;

    Quaternion originalRotation;

    float currentRotationX;
    float currentRotationY;


    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb)
            rb.freezeRotation = true;
        originalRotation = transform.localRotation;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraControls.Enable();
        camTransform = Camera.main.transform;
    }

    void Update()
    {
         

        inputRotationY += cameraControls.FindAction("TrackCamera").ReadValue<Vector2>().y * sensitivityY * Time.deltaTime;
        inputRotationX += cameraControls.FindAction("TrackCamera").ReadValue<Vector2>().x * sensitivityX * Time.deltaTime;

        inputRotationY = Mathf.Clamp(inputRotationY, minimumY, maximumY);

        currentRotationX = Mathf.MoveTowards(inputRotationX, currentRotationX, inputGravity * Time.deltaTime);
        currentRotationY = Mathf.MoveTowards(inputRotationY, currentRotationY, inputGravity * Time.deltaTime);

        transform.eulerAngles = new Vector3(-currentRotationY, currentRotationX, 0);
        //rotAverageY = 0f;
        //rotAverageX = 0f;

        //rotationY += cameraControls.FindAction("TrackCamera").ReadValue<Vector2>().y * sensitivityY;
        //rotationX += cameraControls.FindAction("TrackCamera").ReadValue<Vector2>().x * sensitivityX;


        //rotArrayY.Add(rotationY);
        //rotArrayX.Add(rotationX);


        //if (rotArrayY.Count >= frameCounter)
        //{
        //    rotArrayY.RemoveAt(0);
        //}

        //if (rotArrayX.Count >= frameCounter)
        //{
        //    rotArrayX.RemoveAt(0);
        //}

        //for (int j = 0; j < rotArrayY.Count; j++)
        //{
        //    rotAverageY += rotArrayY[j];
        //}
        //for (int i = 0; i < rotArrayX.Count; i++)
        //{
        //    rotAverageX += rotArrayX[i];
        //}

        //rotAverageY /= rotArrayY.Count;
        //rotAverageX /= rotArrayX.Count;

        //rotAverageY = ClampAngle(rotAverageY, minimumY, maximumY);
        //rotAverageX = ClampAngle(rotAverageX, minimumX, maximumX);

        //Quaternion yQuaternion = Quaternion.AngleAxis(rotAverageY, Vector3.left);
        //Quaternion xQuaternion = Quaternion.AngleAxis(rotAverageX, Vector3.up);

        //transform.localRotation = originalRotation * xQuaternion * yQuaternion;


        if (cameraControls.FindAction("ShowCursor").triggered)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        
    }


    public static float ClampAngle(float angle, float min, float max)
    {
        angle = angle % 360;
        if ((angle >= -360F) && (angle <= 360F))
        {
            if (angle < -360F)
            {
                angle += 360F;
            }
            if (angle > 360F)
            {
                angle -= 360F;
            }
        }
        return Mathf.Clamp(angle, min, max);
    }
}

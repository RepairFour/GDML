using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class GrappleTargetting : MonoBehaviour
{
    bool grappleInputHeld;
    public bool StartGrapple;
    public GameObject currentTargettedObject;
    public Transform feelerPoint;
    public Vector3 feelerLowTierHalfExtents;
    public Vector3 feelerMidTierExtents;
    public Vector3 feelerOuterTierExtents;
    public float feelerRange;
    public LayerMask feelerMask;

    PlayerMap input;
    private ButtonControl grappleButton;

    // Start is called before the first frame update
    void Start()
    {
        input = new PlayerMap();
        input.Enable();

        grappleButton = (ButtonControl)input.Player.Hook.controls[0];
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        FeelerRay();
        HandleOutline();
    }

    void HandleOutline()
    {
        if (grappleInputHeld)
        {
            if(currentTargettedObject != null)
            {
                currentTargettedObject.GetComponent<Outline>().OutlineWidth = 5f;
            }
        }
        else
        {
            if(currentTargettedObject != null)
            {
                currentTargettedObject.GetComponent<Outline>().OutlineWidth = 0f;
            }
        }
    }

    void HandleInput()
    {
        if (grappleButton.wasPressedThisFrame)
        {
            grappleInputHeld = true;
        }
        if (grappleButton.wasReleasedThisFrame)
        {
            grappleInputHeld = false;
            if(currentTargettedObject != null)
            {
                StartGrapple = true;
            }
        }
    }

    void FeelerRay()
    {
        //DrawBoxCast.DrawBoxCastBox(feelerPoint.position, feelerLowTierHalfExtents, transform.rotation, transform.forward, feelerRange, Color.red);
        //DrawBoxCast.DrawBoxCastBox(feelerPoint.position, feelerMidTierExtents, transform.rotation, transform.forward, feelerRange, Color.blue);
        //DrawBoxCast.DrawBoxCastBox(feelerPoint.position, feelerOuterTierExtents, transform.rotation, transform.forward, feelerRange, Color.green);
        if (!StartGrapple)
        {
            RaycastHit hit;
            if (Physics.BoxCast(feelerPoint.position, feelerLowTierHalfExtents, feelerPoint.forward, out hit, Quaternion.identity, feelerRange, feelerMask))
            {
                if (currentTargettedObject != null)
                {
                    currentTargettedObject.GetComponent<Outline>().OutlineWidth = 0f;
                }
                if (currentTargettedObject != hit.collider.gameObject)
                {
                    currentTargettedObject = hit.collider.gameObject;
                    //strikeLocked = true;
                }
            }
            else if (Physics.BoxCast(feelerPoint.position, feelerMidTierExtents, feelerPoint.forward, out hit, Quaternion.identity, feelerRange, feelerMask))
            {
                if (currentTargettedObject != null)
                {
                    currentTargettedObject.GetComponent<Outline>().OutlineWidth = 0f;
                }
                if (currentTargettedObject != hit.collider.gameObject)
                {
                    currentTargettedObject = hit.collider.gameObject;
                }
            }
            else if (Physics.BoxCast(feelerPoint.position, feelerOuterTierExtents, feelerPoint.forward, out hit, Quaternion.identity, feelerRange, feelerMask))
            {
                if (currentTargettedObject != null)
                {
                    currentTargettedObject.GetComponent<Outline>().OutlineWidth = 0f;
                }
                if (currentTargettedObject != hit.collider.gameObject)
                {
                    currentTargettedObject = hit.collider.gameObject;
                }
            }
            else
            {

                if (currentTargettedObject != null)
                {
                    currentTargettedObject.GetComponent<Outline>().OutlineWidth = 0f;
                    currentTargettedObject = null;
                }
            }
        }
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The Drag drop manager

///manager tracks when i mouse down over a draggable
///it then keeps track of the draggables container
///when i hover a slot, it tells the manager that we are hovering and sends the slot game object to the manager
///if i mouse up then the draggable gets parented to the slot.
///the manager tell new container to add this item
///then the manager tell the previous container to remove that item
/// </summary>
public class DragDropManager : MonoBehaviour
{
	#region  Variables
	public static DragDropManager instance;
    Draggable beingDragged = null;
    bool hoveringSlot = false;
    Slot slotBeingHovered = null;

	#endregion

	#region Methods
	// Start is called before the first frame update
	private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            Release();
        }
    }


    public void BeingDragged(Draggable item)
    {
        beingDragged = item;
    }

    public void HoveringSlot(bool answer, Slot slot)
    {
        hoveringSlot = answer;
        slotBeingHovered = slot;
    }

    private void Release()
    {
        if (hoveringSlot && beingDragged != null && 
            slotBeingHovered.GetComponentInParent<Container>().containerType != ContainerType.ReadOnly)
        {
            if (beingDragged.itemType == slotBeingHovered.itemsAccepted || slotBeingHovered.itemsAccepted == "All")
            {
                if (slotBeingHovered.GetComponentInParent<Container>() == beingDragged.container &&
                    slotBeingHovered.GetComponentInParent<Container>().containerType == ContainerType.WriteOnly)
                {
                    if (!slotBeingHovered.HasItem()) //successful drop
                    {
                        beingDragged.OnSuccessfulDrop();
                        slotBeingHovered.AddItem(beingDragged);
                        beingDragged.parent.RemoveItem();
                        beingDragged.transform.SetParent(slotBeingHovered.transform);
                    }
                    else
                    {
                        beingDragged.OnDropFail();
                    }
                }
                else
                {
                   
                    if (!slotBeingHovered.HasItem() && beingDragged.container.containerType != ContainerType.WriteOnly) //successful drop
                    {
                        beingDragged.OnSuccessfulDrop();
                        slotBeingHovered.AddItem(beingDragged);
                        beingDragged.parent.RemoveItem();
                        beingDragged.transform.SetParent(slotBeingHovered.transform);
                    }
                    else
                    {
                        beingDragged.OnDropFail();
                    }
                }
            }
            else
            {
                beingDragged.OnDropFail();
            }
            Reset();
        }
        else {
            if (beingDragged != null)
            {
                beingDragged.OnDropFail();
            }
            Reset();
        }
    }

    private void Reset()
    {
        if (beingDragged)
        {
            beingDragged.transform.localPosition = new Vector3(0, 0, 0);
            beingDragged = null;
        }
        hoveringSlot = false;
        slotBeingHovered = null;  
    }

	#endregion
}

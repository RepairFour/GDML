using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ContainerType
{
    ReadAndWrite,
    ReadOnly,
    WriteOnly
}

public class Container : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region Variables
    public ContainerType containerType;
    [HideInInspector]
    public List<Slot> slots;
    public UnityEvent OnHover;
    public UnityEvent OffHover;
    List<Container> containers;
    private int id = 0;

	#endregion

	#region Methods
	private void Start()
    {
        slots = new List<Slot>(GetComponentsInChildren<Slot>());
        containers = new List<Container>(FindObjectsOfType<Container>());
        foreach(var container in containers)
        {
            if(container == this)
            {
                break;
            }
            ++id;
        }
    }

    public void UpdateList()
    {
        slots.RemoveRange(0, slots.Count);
        slots = new List<Slot>(GetComponentsInChildren<Slot>());
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHover.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OffHover.Invoke();
    }
	#endregion
}

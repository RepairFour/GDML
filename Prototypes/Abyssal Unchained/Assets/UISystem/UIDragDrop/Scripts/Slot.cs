using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	#region Variables
	private Draggable item = null;
    public UnityEvent OnHover;
    public UnityEvent OffHover;
    public UnityEvent OnSlot;
    private string itemsAcceptedChecker;
    

    [HideInInspector]
    public string itemsAccepted;
    [HideInInspector]
    public int editorIndex;

    #endregion

    #region Methods

    private void Start()
    {
        item = GetComponentInChildren<Draggable>();
        itemsAcceptedChecker = itemsAccepted;
    }

	private void Update()
	{
		if(itemsAcceptedChecker != itemsAccepted)
		{
            Debug.Log(gameObject.name.ToString() + "Changed var");
		}
	}
	public bool HasItem()
     {
        return item != null;
    }

    public bool AddItem(Draggable _item)
    {
        if (!HasItem())
        {
            item = _item;
            OnSlot.Invoke();
            return true;
        }
        return false;
    }

    public void RemoveItem()
    {
        item = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHover.Invoke();
        DragDropManager.instance.HoveringSlot(true, gameObject.GetComponent<Slot>());        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DragDropManager.instance.HoveringSlot(false, gameObject.GetComponent<Slot>());
        OffHover.Invoke();
    }
	#endregion
}

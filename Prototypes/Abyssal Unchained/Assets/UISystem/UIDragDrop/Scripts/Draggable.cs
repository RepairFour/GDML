using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
//using UnityEngine.UIElements;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	#region Variables
	public UnityEvent OnDrag;
    public UnityEvent OnDrop;
    public UnityEvent OnFailDrop;
    public UnityEvent OnDragStart;
    private ContainerType containerType;
    private Vector3 scale;

    [HideInInspector]
    public int editorIndex;
    [HideInInspector]
    public Slot parent;
    [HideInInspector]
    public string itemType;
    [HideInInspector]
    public Container container;

	#endregion

	#region Methods

	void Start()
    {
        OnDrag.AddListener(Drag);
        container = GetComponentInParent<Container>();
        scale = transform.localScale;
    }


	void IDragHandler.OnDrag(PointerEventData eventData)
    {
       OnDrag.Invoke();  
    }

	void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        GetComponent<Image>().raycastTarget = true;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        parent = GetComponentInParent<Slot>();
        containerType = GetComponentInParent<Container>().containerType;
        container = GetComponentInParent<Container>();

        gameObject.transform.parent = gameObject.transform.root;
        OnDragStart.Invoke();
        GetComponent<Image>().raycastTarget = false;
        
    }

	public void Drag()
    {
        Transform root = gameObject.transform.root;
        Canvas myCanvas = root.GetComponent<Canvas>();

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out pos);
        transform.position = myCanvas.transform.TransformPoint(pos);

        Draggable temp = gameObject.GetComponent<Draggable>();

        DragDropManager.instance.BeingDragged(temp);
    }

	public void OnDropFail()
    {
        OnFailDrop.Invoke();
        transform.parent = parent.transform;
        transform.localScale = scale;
        transform.localPosition = new Vector3(0, 0, 0);

        Debug.Log("Drop Failed");
    }

    public void OnSuccessfulDrop()
	{
        OnDrop.Invoke();
    }
	#endregion
}

using UnityEngine;
using UnityEngine.EventSystems;

public class UIDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject prefabToDrag;
    private GameObject draggedObject;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (PlacementSystem.current.isDeleteMode) return;
        draggedObject = PlacementSystem.current.StartDragging(prefabToDrag);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedObject != null)
        {
            PlacementSystem.current.UpdateDragging(draggedObject);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedObject != null)
        {
            PlacementSystem.current.StopDragging(draggedObject);
        }
    }
}
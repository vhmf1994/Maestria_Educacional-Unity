using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropZone : MonoBehaviour
{
    [SerializeField] private Transform dropZoneTransform;
    public Transform DropZoneTransform => dropZoneTransform;

    [SerializeField] private int maxNumberOfChildren;
    [SerializeField] private float scaleOnDrop;

    public bool theIsSpaceAvailable => GetComponentsInChildren<Draggable>().Length < maxNumberOfChildren || maxNumberOfChildren == -1;

    public void SetDropZoneParent(RectTransform rectTransform)
    {
        rectTransform.SetParent(dropZoneTransform);

        //rectTransform.anchoredPosition = newAnchoredPosition;
        rectTransform.localScale = Vector3.one * scaleOnDrop;

        rectTransform.anchorMin = Vector3.one / 2;
        rectTransform.anchorMax = Vector3.one / 2;
    }
}
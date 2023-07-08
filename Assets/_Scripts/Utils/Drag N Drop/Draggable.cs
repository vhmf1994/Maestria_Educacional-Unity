using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas), typeof(CanvasGroup), typeof(GraphicRaycaster))]
public abstract class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    protected RectTransform rectTransform;
    protected RectTransform containerTransformer;

    protected Canvas mainCanvas;
    protected Canvas draggableCanvas;

    protected CanvasGroup draggableCanvasGroup;

    protected Vector2 lastPosition;
    [SerializeField] protected DropZone lastDropZone;

    public UnityEvent OnDrop;
    public UnityEvent OnSwap;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        containerTransformer = GameObject.Find("[Empty] - Conteudo Minigame").GetComponent<RectTransform>();
        mainCanvas = GameObject.Find("Canvas Main").GetComponent<Canvas>();
        draggableCanvas = GetComponent<Canvas>();
        draggableCanvasGroup = GetComponent<CanvasGroup>();

        GetLastDropZone();
    }

    protected void GetLastDropZone()
    {
        lastPosition = rectTransform.anchoredPosition;
        lastDropZone = GetComponentInParent<DropZone>();
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        draggableCanvasGroup.alpha = 0.75f;
        draggableCanvasGroup.blocksRaycasts = false;

        draggableCanvas.overrideSorting = true;

        GetLastDropZone();
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / (mainCanvas.scaleFactor * containerTransformer.localScale.x);
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        draggableCanvasGroup.alpha = 1f;
        draggableCanvasGroup.blocksRaycasts = true;

        draggableCanvas.overrideSorting = false;

        AudioController.Instance.PlaySfx(SoundType.Pop);
        Vibrator.Vibrate(25);

        ///

        DropZone dropZoneTarget;
        Draggable draggableTarget;

        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            if (eventData.pointerCurrentRaycast.gameObject.TryGetComponent(out dropZoneTarget))
            {
                if (!dropZoneTarget.theIsSpaceAvailable)
                {
                    Debug.Log("Encontrei uma dropZone, mas não tem espaço disponível");
                    rectTransform.anchoredPosition = lastPosition;
                    lastDropZone.SetDropZoneParent(rectTransform);
                    return;
                }

                Debug.Log("Encontrei uma dropZone");
                RectTransformUtility.ScreenPointToLocalPointInRectangle(dropZoneTarget.DropZoneTransform as RectTransform,
                                                                        eventData.position,
                                                                        Camera.main,
                                                                        out Vector2 newAnchoredPosition);

                dropZoneTarget.SetDropZoneParent(rectTransform);
                rectTransform.anchoredPosition = newAnchoredPosition;

                OnDrop?.Invoke();
            }
            else if (eventData.pointerCurrentRaycast.gameObject.TryGetComponent(out draggableTarget))
            {
                Debug.Log("Encontrei um draggable");

                Vector2 lastPositionTarget = draggableTarget.rectTransform.anchoredPosition;

                draggableTarget.lastDropZone.SetDropZoneParent(rectTransform);
                lastDropZone.SetDropZoneParent(draggableTarget.rectTransform);

                rectTransform.anchoredPosition = lastPositionTarget;
                draggableTarget.rectTransform.anchoredPosition = lastPosition;

                OnSwap?.Invoke();
                draggableTarget.OnSwap?.Invoke();
            }
            else
            {
                Debug.Log("Não encontrei nenhuma dropZone e nenhum draggable");
                lastDropZone.SetDropZoneParent(rectTransform);
                rectTransform.anchoredPosition = lastPosition;
            }
        }
        else
        {
            Debug.Log("Não encontrei nenhuma dropZone e nenhum draggable");
            lastDropZone.SetDropZoneParent(rectTransform);
            rectTransform.anchoredPosition = lastPosition;
        }
    }
}
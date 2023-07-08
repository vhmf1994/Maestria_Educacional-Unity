using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickVibration : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Selectable selectable = GetComponent<Selectable>();

        if (!selectable.interactable) return;

        if (AudioController.Instance != null)
            AudioController.Instance.PlaySfx(SoundType.Clique);

        Vibrator.Vibrate(25);
    }
}

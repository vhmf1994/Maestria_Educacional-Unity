using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CartaCombinarImagens : Draggable
{
    [SerializeField] private ParCombinarImagens parCombinarImagens;

    private void OnEnable()
    {
        OnSwap.AddListener(GetNewParentGroup);
    }

    private void OnDisable()
    {
        OnSwap.RemoveListener(GetNewParentGroup);
    }

    public void GetNewParentGroup()
    {
        parCombinarImagens = GetComponentInParent<ParCombinarImagens>();
        parCombinarImagens.SetCartaMovel(this);

        GetLastDropZone();
    }
}

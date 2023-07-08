using LightScrollSnap;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ScrollSnapController : MonoBehaviour
{
    [SerializeField] private ScrollSnap scrollSnap;
    [Space(10)]
    [SerializeField] private ItemData[] items;

    [Space(10)]
    [SerializeField] private int idItemSelecionado;

    private void Start()
    {
        SetupItems();
    }

    public void SetupItems()
    {
        for (int i = 0; i < scrollSnap.Content.childCount; i++)
        {
            scrollSnap.Content.GetChild(i).GetChild(0).GetComponent<Image>().sprite = items[i].itemSprite;
        }

        scrollSnap.OnItemSelected.AddListener((rect , i) =>
        {
            Debug.Log($"Item {i} selecionado");
            idItemSelecionado = i;
        });
    }
}

[Serializable]
public class ItemData
{
    [SerializeField] private bool m_respostaCerta;
    [SerializeField] private Sprite m_itemSprite;

    public bool respostaCerta => m_respostaCerta;
    public Sprite itemSprite => m_itemSprite;
}

using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CartaJogoDos8 : MonoBehaviour, IPointerClickHandler
{
    [Header("Dados Carta")]
    [SerializeField, ReadOnly] private DadosCartaJogoDos8 dadosCartaOriginal;
    [SerializeField, ReadOnly] private DadosCartaJogoDos8 dadosCartaAtual;

    [Header("Componentes")]
    [SerializeField] private Image imageCarta;
    [SerializeField] private TMP_Text textoIdCarta;

    [SerializeField] private bool m_interactable;
    [SerializeField] private UnityEvent m_onClick;

    public DadosCartaJogoDos8 DadosCartaJogoDos8Original => dadosCartaOriginal;
    public DadosCartaJogoDos8 DadosCartaJogoDos8Atual => dadosCartaAtual;

    public bool interactable { get { return m_interactable; } set { m_interactable = value; } }
    public UnityEvent onClick => m_onClick;

    public void ConfigurarCartaOriginal(DadosCartaJogoDos8 dadosCartaOriginal, Sprite imageCarta)
    {
        this.dadosCartaOriginal = dadosCartaOriginal;

        this.textoIdCarta.SetText((this.dadosCartaOriginal.IdPosicao + 1).ToString());
        this.imageCarta.sprite = imageCarta;
    }
    public void ConfigurarCartaAtual(DadosCartaJogoDos8 dadosCartaAtual) => this.dadosCartaAtual = dadosCartaAtual;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!m_interactable) return;

        AudioController.Instance.PlaySfx(SoundType.Pop);
        Vibrator.Vibrate(25);
        m_onClick?.Invoke();
    }
}

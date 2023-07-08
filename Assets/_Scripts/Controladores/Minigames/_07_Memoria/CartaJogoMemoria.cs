using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CartaJogoMemoria : Button, IPointerClickHandler
{
    [Header("Configuração da Carta")]
    [SerializeField, ReadOnly] private DadosJogoMemoria dadosJogoMemoria;

    [SerializeField] private Image imagemFeedback;
    [SerializeField] private Image imagemCarta;
    [SerializeField] private Animator animatorCarta;
    public DadosJogoMemoria DadosJogoMemoria => dadosJogoMemoria;

    public void ConfigurarCarta(DadosJogoMemoria dadosJogoMemoria)
    {
        this.dadosJogoMemoria = dadosJogoMemoria;

        imagemFeedback.enabled = false;
        imagemCarta.sprite = this.dadosJogoMemoria.ImagemJogoMemoria;

        //Esconder carta
    }

    public void DefinirFeedback(bool acerto)
    {
        Color corErro = new Color(0.9f, 0.15f, 0.15f, 1f);
        Color corAcerto = new Color(0.15f, 0.75f, 0.15f, 1f);

        imagemFeedback.color = acerto ? corAcerto : corErro;

        animatorCarta.Play("Feedback");
    }

    public void Show() => animatorCarta.Play("Show");
    public void Hide() => animatorCarta.Play("Hide");
}

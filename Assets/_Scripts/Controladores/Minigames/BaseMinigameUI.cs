using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseMinigameUI : MonoSingleton<BaseMinigameUI>
{
    [Header("Temporizador")]
    [SerializeField] protected TMP_Text textoTemporizador;

    [Header("Pause")]
    [SerializeField] protected GameObject painelPause;
    [Space(5)]
    [SerializeField] protected Button pauseButton;

    [Header("Confirmação")]
    [SerializeField] protected GameObject painelConfirmacao;

    [Header("Tutorial")]
    [SerializeField] protected GameObject painelTutorial;

    [Header("Tela Finalização")]
    [SerializeField] protected GameObject painelFinalizacao;

    protected override void InitializeBehaviour()
    {
        base.InitializeBehaviour();

        pauseButton.onClick.RemoveAllListeners();
        pauseButton.onClick.AddListener(OnPauseMenu);
    }

    public void AtualizarTemporizador(string textoTempo)
    {
        textoTemporizador.SetText(textoTempo);
    }

    public void OnPauseMenu()
    {
        MinigamesController.Instance.CurrentBaseMinigame.PauseMinigame();

        painelPause.SetActive(true);
    }

    public void OnConfirmacaoMenu(bool isConfirming)
    {
        painelConfirmacao.SetActive(isConfirming);
    }
    public void OnFinalizacaoMenu(bool vitoria, string textoTempo, string textoExtra)
    {
        StartCoroutine(MostrarFinalizacaoCoroutine(vitoria, textoTempo, textoExtra));
    }
    protected virtual IEnumerator MostrarFinalizacaoCoroutine(bool vitoria, string textoTempo, string textoExtra)
    {
        yield return new WaitForSeconds(2f);

        painelFinalizacao.SetActive(true);
        TelaFinalizacaoController.Instance.ConfigurarUI(vitoria, textoTempo, textoExtra);
    }

    public void MostrarTutorial(Action onTutorialCompleted)
    {
        StartCoroutine(MostrarTutorialCoroutine(onTutorialCompleted));
    }
    protected virtual IEnumerator MostrarTutorialCoroutine(Action onTutorialCompleted)
    {
        if (painelTutorial != null)
        {
            painelTutorial.SetActive(true);
            painelTutorial.GetComponent<Animator>().Play("Tutorial_Show", -1, 0);

            yield return new WaitForSeconds(5f);

            painelTutorial.SetActive(false);
        }

        onTutorialCompleted.Invoke();
    }
}
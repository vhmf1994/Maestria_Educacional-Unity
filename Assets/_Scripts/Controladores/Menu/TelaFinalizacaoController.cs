using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TelaFinalizacaoController : MonoSingleton<TelaFinalizacaoController>
{
    [Header("Finalização")]
    [SerializeField] private Image painelFinalizacao;
    [Space(10)]
    [SerializeField] private Sprite painelVitoria;
    [SerializeField] private Sprite painelDerrota;
    [Space(10)]
    [SerializeField] private Button buttonJogarDeNovo;
    [SerializeField] private Button buttonFecharJogo;
    [Space(10)]
    [SerializeField] private TMP_Text textoTemporizador;
    [SerializeField] private TMP_Text textoContador;

    private void Start()
    {
        buttonJogarDeNovo.onClick.RemoveAllListeners();
        buttonFecharJogo.onClick.RemoveAllListeners();

        buttonJogarDeNovo.onClick.AddListener(OnJogarDeNovo);
        buttonFecharJogo.onClick.AddListener(OnFecharJogo);
    }

    public void ConfigurarUI(bool vitoria, string textoTempo, string textoExtra)
    {
        painelFinalizacao.sprite = vitoria ? painelVitoria : painelDerrota;

        textoTemporizador.SetText(textoTempo);
        textoContador.SetText(textoExtra);
    }

    private void OnJogarDeNovo()
    {
        gameObject.SetActive(false);

        MinigamesController.Instance.InitializeMinigame();
    }
    private void OnFecharJogo()
    {
        BaseMinigameUI.Instance.OnConfirmacaoMenu(true);

        ConfirmMenuController.Instance.SetupConfirmMenu("Tem certeza que deseja sair?", () =>
        {
            SceneController.Instance.beforeSceneTransition += () =>
            {
                if (AudioController.Instance != null)
                    AudioController.Instance.StopMusic();

                return true;
            };

            SceneController.Instance.LoadScene(SceneController.Scenes._01_Menu);
        });
    }
}

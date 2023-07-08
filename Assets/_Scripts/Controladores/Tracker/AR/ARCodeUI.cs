using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARCodeUI : MonoBehaviour
{
    [SerializeField] private ARImageTrackerController controller;

    [Space]
    [SerializeField] private Button botaoSair;
    [SerializeField] private Button botaoAjuda;
    [Space]
    [SerializeField] private GameObject painelAjuda;
    [SerializeField] private Button botaoEntendi;
    [Space]
    [SerializeField] private GameObject painelConfirmar;

    private void OnEnable()
    {
        botaoSair.onClick.AddListener(onBotaoSair);
        botaoAjuda.onClick.AddListener(OnBotaoAjuda);
        botaoEntendi.onClick.AddListener(OnBotaoEntendi);
    }
    private void OnDisable()
    {
        botaoSair.onClick.RemoveListener(onBotaoSair);
        botaoAjuda.onClick.RemoveListener(OnBotaoAjuda);
        botaoEntendi.onClick.RemoveListener(OnBotaoEntendi);
    }

    private void onBotaoSair()
    {
        painelConfirmar.SetActive(true);

        ConfirmMenuController.Instance.SetupConfirmMenu("Tem certeza que deseja sair?", () =>
        {
            SceneController.Instance.LoadScene(SceneController.Scenes._01_Menu);
        });
    }
    private void OnBotaoAjuda()
    {
        painelAjuda.SetActive(true);
        controller.scanAtivo = false;
    }
    private void OnBotaoEntendi()
    {
        painelAjuda.SetActive(false);
        controller.scanAtivo = true;
    }
}
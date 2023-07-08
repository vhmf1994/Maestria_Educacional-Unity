using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmMenuController : MonoSingleton<ConfirmMenuController>
{
    [Header("Confirmação")]
    [SerializeField] private TMP_Text textoMotivo;

    [SerializeField] private Button buttonYes;
    [SerializeField] private Button buttonNo;

    public void SetupConfirmMenu(string motivo, Action onButtonYes)
    {
        textoMotivo.SetText(motivo);

        buttonYes.onClick.RemoveAllListeners();
        buttonNo.onClick.RemoveAllListeners();

        buttonYes.onClick.AddListener(() =>
        {
            onButtonYes?.Invoke();
        });
        buttonNo.onClick.AddListener(() =>
        {
            CloseConfirmMenu();
        });
    }

    private void CloseConfirmMenu()
    {
        gameObject.SetActive(false);
    }
}

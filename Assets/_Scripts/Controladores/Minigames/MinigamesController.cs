using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MinigamesController : MonoSingleton<MinigamesController>
{
    [Header("Minigames Disponíveis")]
    [SerializeField] private BaseMinigame currentBaseMinigame;
    public BaseMinigame CurrentBaseMinigame => currentBaseMinigame;

    [SerializeField] private BaseMinigame[] allBaseMinigames;

    public void SetCurrentBaseMinigame(BaseMinigame newBaseMinigame)
    {
        currentBaseMinigame = newBaseMinigame;
    }

    public void InitializeMinigame()
    {
        FindMinigame(currentBaseMinigame.CodigoMinigame);
    }

    protected override void InitializeBehaviour()
    {
        base.InitializeBehaviour();

        InitializeMinigame();
    }

    private void Update()
    {
        currentBaseMinigame?.UpdateTimer();
    }

    public void FindMinigame(string codMinigame)
    {
        if (!ContainsCod(codMinigame))
        {
            SceneController.Instance.LoadScene(SceneController.Scenes._01_Menu);
            return;
        }

        currentBaseMinigame = GetMinigame(codMinigame);

        if (currentBaseMinigame == null)
        {
            SceneController.Instance.LoadScene(SceneController.Scenes._01_Menu);
            return;
        }

        GetComponentInChildren<TMP_Text>()?.SetText($"Current Minigame : {codMinigame}");
        currentBaseMinigame.InitializeMinigame();
    }

    private bool ContainsCod(string codMinigame)
    {
        bool contains = false;

        for (int i = 0; i < allBaseMinigames.Length; i++)
        {
            if (allBaseMinigames[i].CodigoMinigame == codMinigame)
            {
                contains = true;
                break;
            }
        }

        return contains;
    }

    private BaseMinigame GetMinigame(string codMinigame)
    {
        BaseMinigame minigame = null;

        for (int i = 0; i < allBaseMinigames.Length; i++)
        {
            if (allBaseMinigames[i].CodigoMinigame == codMinigame)
            {
                minigame = allBaseMinigames[i];

                break;
            }
        }

        return minigame;
    }

    [Button]
    private void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
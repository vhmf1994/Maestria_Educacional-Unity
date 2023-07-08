using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoSingleton<PauseMenuController>
{
    [Header("Pause")]
    [SerializeField] private Button resumeButton;
    [Space(5)]
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button closeButton;

    protected override void OnEnable()
    {
        base.OnEnable();

        resumeButton.onClick.AddListener(OnResumeMenu);
        tutorialButton.onClick.AddListener(OnTutorialButton);
        closeButton.onClick.AddListener(OnCloseButton);
    }
    protected override void OnDisable()
    {
        base.OnDisable();

        resumeButton.onClick.RemoveListener(OnResumeMenu);
        tutorialButton.onClick.RemoveListener(OnTutorialButton);
        closeButton.onClick.RemoveListener(OnCloseButton);
    }

    private void OnResumeMenu()
    {
        MinigamesController.Instance.CurrentBaseMinigame.ResumeMinigame();

        gameObject.SetActive(false);
    }
    private void OnTutorialButton()
    {
        MinigamesController.Instance.CurrentBaseMinigame.ChangeMinigameState(MinigameState.Tutorial);

        BaseMinigameUI.Instance.MostrarTutorial(MinigamesController.Instance.CurrentBaseMinigame.ResumeMinigame);

        gameObject.SetActive(false);
    }
    private void OnCloseButton()
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
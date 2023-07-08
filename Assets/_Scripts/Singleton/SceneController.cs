using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoSingleton<SceneController>
{
    public Func<bool> beforeSceneTransition;
    public Func<bool> afterSceneTransition;

    public enum Scenes
    {
        _00_Splash,
        _01_Menu,
        _02_TelaAR,
        _03_TelaQR,
        _04_Minigame_QuizTexto,
        _05_Minigame_QuizImagem,
        _06_Minigame_GrupoImagens,
        _07_Minigame_Memoria,
        _08_Minigame_CombinarImagens,
        _09_Minigame_JogosDos8,
        _10_Minigame_SudokuImagens,
    }

    private Scenes m_currentScene;
    public Scenes currentScene => m_currentScene;

    protected override void InitializeBehaviour()
    {
        m_currentScene = Scenes._00_Splash;
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);

        LoadScene(Scenes._01_Menu);
    }

    public void LoadScene(Scenes nextScene)
    {
        StartCoroutine(LoadSceneAsync((int)nextScene));
    }
    public void LoadScene(int nextScene)
    {
        StartCoroutine(LoadSceneAsync(nextScene));
    }

    private IEnumerator LoadSceneAsync(int nextScene)
    {
#if !UNITY_EDITOR
        // Executa o efeito de transição
        yield return FadeController.Instance.FadeOut();
#endif 

        // Executa o método de finalização de transição
        if (beforeSceneTransition != null)
        {
            yield return new WaitUntil(beforeSceneTransition.Invoke);
            beforeSceneTransition = null;
        }

        // Executa a troca de cena
        yield return SceneManager.LoadSceneAsync(((Scenes)nextScene).ToString());

        // Execura o método de início de transição
        if (afterSceneTransition != null)
        {
            yield return new WaitUntil(afterSceneTransition.Invoke);
            afterSceneTransition = null;
        }

        yield return new WaitForSeconds(0.1f);

#if !UNITY_EDITOR
        // Executa o efeito de transição
        yield return FadeController.Instance.FadeIn();
#endif

        // Coloquei esse método apenas pra caso tenha algo que necessite de atualização
        // Evita alguns bugs de canvas com grids e groups
        Canvas.ForceUpdateCanvases();
    }
}
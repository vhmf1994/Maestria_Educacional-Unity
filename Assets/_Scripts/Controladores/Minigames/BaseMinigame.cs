using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMinigame : SerializedScriptableObject
{
    protected BaseMinigameUI baseMinigameUI => BaseMinigameUI.Instance;
    protected AudioController audioController => AudioController.Instance;

    [Header("Configuração base")]
    [SerializeField] protected string codigoMinigame;

    [SerializeField, ReadOnly] protected MinigameState previousMinigameState;
    [SerializeField, ReadOnly] protected MinigameState currentMinigameState;

    [SerializeField, ReadOnly] protected float timer;
    protected float currentTimer;

    public MinigameState PreviousMinigameState => previousMinigameState;
    public MinigameState CurrentMinigameState => currentMinigameState;
    public string CodigoMinigame => codigoMinigame;
    public float CurrentTimer => currentTimer;

    protected virtual void OnValidate()
    {
        currentMinigameState = MinigameState.Tutorial;
    }

    public virtual void InitializeMinigame()
    {
        currentMinigameState = MinigameState.Tutorial;
        currentTimer = timer;

        // Aguardar tutorial finalizar
        baseMinigameUI.MostrarTutorial(ResumeMinigame);
    }

    protected virtual void FinalizeMinigame()
    {
        ChangeMinigameState(MinigameState.Game_Over);
    }

    public void PauseMinigame()
    {
        if (currentMinigameState != MinigameState.Playing) return;

        ChangeMinigameState(MinigameState.Paused);
    }
    public void ResumeMinigame()
    {
        if (currentMinigameState == MinigameState.Game_Over) return;

        ChangeMinigameState(MinigameState.Playing);
    }

    public void ChangeMinigameState(MinigameState newMinigameState)
    {
        previousMinigameState = currentMinigameState;

        currentMinigameState = newMinigameState;
    }

    public virtual void UpdateTimer()
    {
        if (currentMinigameState != MinigameState.Playing) return;

        currentTimer -= Time.deltaTime;

        if (CheckIfIsOver())
            FinalizeMinigame();
    }

    protected virtual bool CheckIfIsOver() { return currentTimer < 0; }
}

public enum MinigameState
{
    Tutorial,
    Playing,
    Paused,
    Game_Over,
}
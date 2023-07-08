using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Jogo de Combinar Pares", menuName = "Minigames/Jogo de Combinar Pares", order = 0)]
public class CombinarImagens : BaseMinigame
{
    protected CombinarImagensUI CombinarMinigameUI => baseMinigameUI as CombinarImagensUI;

    [Header("Configuração Jogo de Combinar Pares")]
    [SerializeField] private List<DadosCombinarImagens> dadosDadosCombinarImagens;

    private int contadorAcertos;
    private int contadorErros;

    protected override void OnValidate()
    {
        base.OnValidate();

        if(dadosDadosCombinarImagens != null)
        {
            for (int i = 0; i < dadosDadosCombinarImagens.Count; i++)
            {
                dadosDadosCombinarImagens[i].ParSprite.SetLength(2);
            }
        }

        timer = 600;
    }

    public override void InitializeMinigame()
    {
        contadorAcertos = 0;
        contadorErros = 0;

        CombinarMinigameUI.ConfigurarUI(dadosDadosCombinarImagens);

        base.InitializeMinigame();
    }

    protected override void FinalizeMinigame()
    {
        base.FinalizeMinigame();

        bool vitoria = contadorAcertos > contadorErros && !CheckIfIsOver();
        string textoTimer = System.TimeSpan.FromSeconds(currentTimer).ToString("mm':'ss");
        string textoExtra = $"{contadorAcertos} | {contadorErros}";

        if (audioController != null)
            audioController.PlaySfx(vitoria ? SoundType.Vitoria : SoundType.Derrota);

        JogoQuizImagemUI.Instance.OnFinalizacaoMenu(vitoria, textoTimer, textoExtra);
    }

    public override void UpdateTimer()
    {
        base.UpdateTimer();

        baseMinigameUI.AtualizarTemporizador(System.TimeSpan.FromSeconds(currentTimer).ToString("mm':'ss"));
    }

    public void Responder(List<ParCombinarImagens> paresCombinarImagens)
    {
        contadorAcertos = 0;
        contadorErros = 0;

        for (int i = 0; i < paresCombinarImagens.Count; i++)
        {
            if (paresCombinarImagens[i].CompararCartas())
                contadorAcertos++;
            else
                contadorErros++;
        }

        CombinarMinigameUI.AtualizarContador(contadorAcertos, contadorErros);

        FinalizeMinigame();
    }
}

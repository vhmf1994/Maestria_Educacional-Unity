using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Jogo dos 8", menuName = "Minigames/Jogo dos 8", order = 0)]
public class JogoDos8 : BaseMinigame
{
    protected JogoDos8UI JogoDos8MinigameUI => baseMinigameUI as JogoDos8UI;

    [Header("Configuração Jogo dos 8")]
    [SerializeField] private DadosJogoDos8 dadosJogoDos8;

    private int contadorTentativas;

    protected override void OnValidate()
    {
        base.OnValidate();

        timer = 600;

        contadorTentativas = 0;
    }

    public override void InitializeMinigame()
    {
        base.InitializeMinigame();

        // Enviar os dados do jogo pra UI
        JogoDos8MinigameUI.ConfigurarUI(dadosJogoDos8);

        timer = 600;

        contadorTentativas = 0;
    }

    protected override void FinalizeMinigame()
    {
        base.FinalizeMinigame();

        bool vitoria = !CheckIfIsOver();
        string textoTimer = System.TimeSpan.FromSeconds(currentTimer).ToString("mm':'ss");
        string textoExtra = $"{contadorTentativas}";

        if (audioController != null)
            audioController.PlaySfx(vitoria ? SoundType.Vitoria : SoundType.Derrota);

        JogoDos8MinigameUI.OnFinalizacaoMenu(vitoria, textoTimer, textoExtra);
        JogoDos8MinigameUI.DesativarTodasCartas();
    }

    public override void UpdateTimer()
    {
        base.UpdateTimer();

        baseMinigameUI.AtualizarTemporizador(System.TimeSpan.FromSeconds(currentTimer).ToString("mm':'ss"));
    }

    public void VerificarPosicoes(List<DadosCartaJogoDos8> dadosCartaJogoDos8)
    {
        if (currentMinigameState != MinigameState.Playing)
            return;

        contadorTentativas++;

        bool fimDeJogo = true;

        for (int i = 0; i < dadosCartaJogoDos8.Count; i++)
        {
            if (dadosCartaJogoDos8[i].IdPosicao != i)
            {
                Debug.Log($"Carta {dadosCartaJogoDos8[i].IdPosicao} é != {i}");

                fimDeJogo = false;
                break;
            }
        }

        if (fimDeJogo)
            FinalizeMinigame();

        JogoDos8MinigameUI.AtualizarContador(contadorTentativas);
    }
}

[System.Serializable]
public class DadosJogoDos8
{
    [SerializeField] private Sprite imagemJogoDos8;
    public Sprite ImagemJogoDos8 => imagemJogoDos8;
}

[System.Serializable]
public class DadosCartaJogoDos8
{
    [SerializeField, ReadOnly] private Vector2Int gridPosicao;
    [SerializeField, ReadOnly] private int idPosicao;

    public Vector2Int GridPosicao { get { return gridPosicao; } set { gridPosicao = value; } }
    public int IdPosicao { get { return idPosicao; } set { idPosicao = value; } }
}
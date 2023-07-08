using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Jogo da Memoria", menuName = "Minigames/Jogo da Memoria", order = 0)]
public class JogoMemoria : BaseMinigame
{
    protected JogoMemoriaUI MemoriaMinigameUI => baseMinigameUI as JogoMemoriaUI;

    [Header("Configuração Jogo Memoria")]
    [SerializeField] private List<DadosJogoMemoria> dadosJogoMemoria;

    private int contadorParesRestantes;
    private int contadorTentativas;

    protected override void OnValidate()
    {
        base.OnValidate();

        if (dadosJogoMemoria != null)
        {
            for (int i = 0; i < dadosJogoMemoria.Count; i++)
            {
                int idCarta = i;

                dadosJogoMemoria[idCarta].idCarta = idCarta;
                dadosJogoMemoria[idCarta].acertou = false;
            }

            timer = 600;
        }
    }

    public override void InitializeMinigame()
    {
        // Enviar cartas para UI
        MemoriaMinigameUI.ConfigurarUI(dadosJogoMemoria);

        contadorParesRestantes = dadosJogoMemoria.Count;
        contadorTentativas = 0;

        base.InitializeMinigame();
    }

    protected override void FinalizeMinigame()
    {
        base.FinalizeMinigame();

        bool vitoria = contadorTentativas < dadosJogoMemoria.Count * 2 && !CheckIfIsOver();
        string textoTimer = System.TimeSpan.FromSeconds(currentTimer).ToString("mm':'ss");
        string textoExtra = $"{contadorTentativas} | {dadosJogoMemoria.Count}";

        if (audioController != null)
            audioController.PlaySfx(vitoria ? SoundType.Vitoria : SoundType.Derrota);

        JogoMemoriaUI.Instance.OnFinalizacaoMenu(vitoria, textoTimer, textoExtra);
    }

    public override void UpdateTimer()
    {
        base.UpdateTimer();

        baseMinigameUI.AtualizarTemporizador(System.TimeSpan.FromSeconds(currentTimer).ToString("mm':'ss"));
    }

    public bool VerificarPares(CartaJogoMemoria carta1, CartaJogoMemoria carta2)
    {
        contadorTentativas++;

        bool par = false;

        if (carta1.DadosJogoMemoria.idCarta == carta2.DadosJogoMemoria.idCarta)
        {
            contadorParesRestantes--;

            carta1.DadosJogoMemoria.acertou = carta2.DadosJogoMemoria.acertou = true;

            carta1.interactable = false;
            carta2.interactable = false;

            par = true;
        }
        else
        {
            par = false;
        }

        if (contadorParesRestantes == 0)
            FinalizeMinigame();

        // Atualizar Tentativas
        MemoriaMinigameUI.AtualizarTentativas(contadorTentativas);

        return par;
    }

    public int GerarDica()
    {
        int idDica = -1;

        do
        {
            idDica = Random.Range(0, dadosJogoMemoria.Count);
        }
        while (dadosJogoMemoria[idDica].acertou);

        contadorTentativas++;
        MemoriaMinigameUI.AtualizarTentativas(contadorTentativas);

        return idDica;
    }
}

[System.Serializable]
public class DadosJogoMemoria
{
    [ReadOnly] public int idCarta;
    [ReadOnly] public bool acertou;

    [SerializeField] private Sprite imagemJogoMemoria;
    public Sprite ImagemJogoMemoria => imagemJogoMemoria;

    public DadosJogoMemoria CriarCopia()
    {
        DadosJogoMemoria copia = new DadosJogoMemoria()
        {
            idCarta = idCarta,
            acertou = acertou,
            imagemJogoMemoria = imagemJogoMemoria
        };

        return copia;
    }
}
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Grupo de Imagens", menuName = "Minigames/GrupoImagens", order = 0)]
public class JogoGrupoImagens : BaseMinigame
{
    protected JogoGrupoImagensUI BaseMinigameUI => baseMinigameUI as JogoGrupoImagensUI;

    [Header("Configuração Grupos de Imagens")]
    [SerializeField] private List<DadosGruposImagem> dadosGruposImagem;

    [SerializeField] private List<DadosCartaGruposImagens> dadosCartaGruposImagens;

    //variaveis de controle
    private int contadorAcertos;
    private int contadorErros;

    protected override void OnValidate()
    {
        base.OnValidate();

        foreach (DadosGruposImagem d in dadosGruposImagem)
        {
            if (d.CartasGrupoImagem != null && d.CartasGrupoImagem.Count > 0)
            {
                for (int i = 0; i < d.CartasGrupoImagem.Count; i++)
                {
                    d.CartasGrupoImagem[i].ConfigurarCarta(dadosGruposImagem.ToList().IndexOf(d));
                }
            }
        }

        dadosCartaGruposImagens = new List<DadosCartaGruposImagens>();

        timer = 600;
    }

    public override void InitializeMinigame()
    {
        contadorAcertos = 0;
        contadorErros = 0;

        ConfigurarCartas();

        base.InitializeMinigame();
    }

    protected override void FinalizeMinigame()
    {
        base.FinalizeMinigame();

        bool vitoria = contadorAcertos > contadorErros && !CheckIfIsOver(); ;
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

    private void ConfigurarCartas()
    {
        dadosCartaGruposImagens = new List<DadosCartaGruposImagens>();

        for (int i = 0; i < dadosGruposImagem.Count; i++)
        {
            for (int j = 0; j < dadosGruposImagem[i].CartasGrupoImagem.Count; j++)
            {
                dadosCartaGruposImagens.Add(dadosGruposImagem[i].CartasGrupoImagem[j]);
            }
        }

        // Enviar configurações para UI
        BaseMinigameUI.ConfigurarUI(dadosGruposImagem, dadosCartaGruposImagens);
    }

    public void Responder(List<DadosGruposImagem> dadosGruposImagemResposta)
    {
        contadorAcertos = 0;
        contadorErros = 0;

        for (int i = 0; i < dadosGruposImagemResposta.Count; i++)
        {
            for (int j = 0; j < dadosGruposImagemResposta[i].CartasGrupoImagem.Count; j++)
            {
                if (dadosGruposImagem[i].CartasGrupoImagem.Contains(dadosGruposImagemResposta[i].CartasGrupoImagem[j]))
                    contadorAcertos++;
                else
                    contadorErros++;
            }
        }

        Debug.Log($"Acertos: {contadorAcertos} | Erros: {contadorErros}");

        BaseMinigameUI.AtualizarContador(contadorAcertos, contadorErros);

        FinalizeMinigame();
    }
}

[System.Serializable]
public class DadosGruposImagem
{
    [SerializeField] private int idGrupo;
    [SerializeField] private string nomeGrupo;
    [SerializeField] private List<DadosCartaGruposImagens> cartasGrupoImagem;

    public int IdGrupo
    {
        get { return idGrupo; } set { idGrupo = value; }
    }
    public string NomeGrupo => nomeGrupo;
    public List<DadosCartaGruposImagens> CartasGrupoImagem
    {
        get { return cartasGrupoImagem; } set { cartasGrupoImagem = value; }
    }
}

[System.Serializable]
public class DadosCartaGruposImagens
{
    [SerializeField, ReadOnly] private int idGrupoPertencente;
    [SerializeField] private Sprite imagemCarta;

    public int IdGrupoPertencente => idGrupoPertencente;
    public Sprite ImagemCarta => imagemCarta;

    public void ConfigurarCarta(int idGrupo) => idGrupoPertencente = idGrupo;
}
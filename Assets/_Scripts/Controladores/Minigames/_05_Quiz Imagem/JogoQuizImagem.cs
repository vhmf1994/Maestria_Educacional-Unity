using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Quiz Imagem", menuName = "Minigames/Quiz/Imagem", order = 0)]
public class JogoQuizImagem : BaseMinigame
{
    protected JogoQuizImagemUI JogoQuizgameUI => baseMinigameUI as JogoQuizImagemUI;

    [Header("Configuração Quiz Imagem")]
    [SerializeField] private List<DadosQuizImagem> dadosQuizImagem;

    private List<int> respostasDadas;

    // variaveis de controle
    private int idDadoQuizAtual;
    private int contadorAcertos;
    private int contadorErros;

    protected override void OnValidate()
    {
        base.OnValidate();

        foreach (DadosQuizImagem d in dadosQuizImagem)
        {
            if (d.Respostas.Length > 4)
                Debug.LogError($"O dado quiz imagem de id: {dadosQuizImagem.ToList().IndexOf(d)} possui mais imagens do que pode!");

            bool respostaCertaDefinida = false;

            foreach (var resposta in d.Respostas)
            {
                if (resposta.RespostaCerta)
                {
                    respostaCertaDefinida = true;

                    break;
                }
            }

            if (!respostaCertaDefinida)
                Debug.LogError($"O dado quiz imagem de id: {dadosQuizImagem.ToList().IndexOf(d)} não possui resposta certa definida!");
        }

        timer = 600;
    }

    public override void InitializeMinigame()
    {
        idDadoQuizAtual = -1;
        contadorAcertos = 0;
        contadorErros = 0;

        JogoQuizgameUI.DefinirContador(dadosQuizImagem.Count);

        ConfigurarProximaPergunta();

        base.InitializeMinigame();
    }

    protected override void FinalizeMinigame()
    {
        base.FinalizeMinigame();

        bool vitoria = contadorAcertos > contadorErros || !CheckIfIsOver(); ;
        string textoTimer = System.TimeSpan.FromSeconds(currentTimer).ToString("mm':'ss");
        string textoExtra = $"{contadorAcertos} | {contadorErros}";

        if (audioController != null)
            audioController.PlaySfx(vitoria ? SoundType.Vitoria : SoundType.Derrota);

        JogoQuizImagemUI.Instance.OnFinalizacaoMenu(vitoria, textoTimer, textoExtra);
    }

    public override void UpdateTimer()
    {
        base.UpdateTimer();

        JogoQuizgameUI.AtualizarTemporizador(System.TimeSpan.FromSeconds(currentTimer).ToString("mm':'ss"));
    }

    public void ConfigurarProximaPergunta()
    {
        idDadoQuizAtual++;

        if (idDadoQuizAtual >= dadosQuizImagem.Count) return;

        EmbaralharRespostas(dadosQuizImagem[idDadoQuizAtual]);

        JogoQuizgameUI.ConfigurarUI(dadosQuizImagem[idDadoQuizAtual]);
        JogoQuizgameUI.DefinirContadorAtual(idDadoQuizAtual + 1);
        JogoQuizgameUI.AtualizarContador(contadorAcertos, contadorErros);

        respostasDadas = new List<int>();
    }

    private void EmbaralharRespostas(DadosQuizImagem dadosQuizImagem)
    {
        int n = dadosQuizImagem.Respostas.Length;
        System.Random rng = new System.Random();

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            QuizImagemResposta value = dadosQuizImagem.Respostas[k];
            dadosQuizImagem.Respostas[k] = dadosQuizImagem.Respostas[n];
            dadosQuizImagem.Respostas[n] = value;
        }
    }

    public void Responder(int idResposta)
    {
        respostasDadas.Add(idResposta);

        if (dadosQuizImagem[idDadoQuizAtual].Respostas[idResposta].RespostaCerta)
        {
            Debug.Log("Resposta Certa");

            contadorAcertos++;

            JogoQuizgameUI.MostrarFeedback(idResposta, true);

            //Feedback e Mostrar próxima pergunta
            if (idDadoQuizAtual >= dadosQuizImagem.Count - 1)
                FinalizeMinigame();
        }
        else
        {
            contadorErros++;

            Debug.Log("Resposta Errada");
            JogoQuizgameUI.MostrarFeedback(idResposta, false);
        }

        JogoQuizgameUI.AtualizarContador(contadorAcertos, contadorErros);
    }

    public int GerarDica()
    {
        int idDica = -1;

        // Se a única resposta disponível for a resposta certa
        if (respostasDadas.Count > 2)
        {
            for (int i = 0; i < respostasDadas.Count; i++)
            {
                if (!respostasDadas.Contains(i))
                {
                    idDica = i;
                    Responder(i);

                    break;
                }
            }
        }
        // Se não, gera uma resposta errada aleatória
        else
        {
            do
            {
                idDica = Random.Range(0, dadosQuizImagem[idDadoQuizAtual].Respostas.Length);
            }
            while (dadosQuizImagem[idDadoQuizAtual].Respostas[idDica].RespostaCerta || respostasDadas.Contains(idDica));
        }

        return idDica;
    }
}

[System.Serializable]
public class DadosQuizImagem
{
    [SerializeField][TextArea(2, 5)] private string pergunta;
    [SerializeField] private QuizImagemResposta[] respostas;

    public string Pergunta => pergunta;
    public QuizImagemResposta[] Respostas => respostas;
}
[System.Serializable]
public class QuizImagemResposta
{
    [SerializeField] private bool respostaCerta;
    [SerializeField] private Sprite resposta;

    public bool RespostaCerta => respostaCerta;
    public Sprite Resposta => resposta;
}
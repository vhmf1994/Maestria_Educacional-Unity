using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quiz Texto", menuName = "Minigames/Quiz/Texto", order = 0)]
public class JogoQuizTexto : BaseMinigame
{
    private JogoQuizTextoUI JogoQuizgameUI => baseMinigameUI as JogoQuizTextoUI;

    [Header("Configuração Quiz Texto")]
    [SerializeField] private DadosQuizTexto[] dadosQuizTexto;

    private List<int> respostasDadas;

    // variaveis de controle
    private int idDadoQuizAtual;
    private int contadorAcertos;
    private int contadorErros;

    protected override void OnValidate()
    {
        base.OnValidate();

        timer = 600;
    }

    public override void InitializeMinigame()
    {
        idDadoQuizAtual = -1;
        contadorAcertos = 0;
        contadorErros = 0;

        JogoQuizgameUI.DefinirContador(dadosQuizTexto.Length);

        ConfigurarProximaPergunta();

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

        JogoQuizgameUI.AtualizarTemporizador(System.TimeSpan.FromSeconds(currentTimer).ToString("mm':'ss"));
    }

    public void ConfigurarProximaPergunta()
    {
        idDadoQuizAtual++;

        if (idDadoQuizAtual >= dadosQuizTexto.Length) return;

        EmbaralharRespostas(dadosQuizTexto[idDadoQuizAtual]);

        JogoQuizgameUI.ConfigurarUI(dadosQuizTexto[idDadoQuizAtual]);
        JogoQuizgameUI.DefinirContadorAtual(idDadoQuizAtual + 1);
        JogoQuizgameUI.AtualizarContador(contadorAcertos, contadorErros);

        respostasDadas = new List<int>();
    }

    private void EmbaralharRespostas(DadosQuizTexto dadosQuizTexto)
    {
        int n = dadosQuizTexto.Respostas.Length;
        System.Random rng = new System.Random();

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            QuizTextoResposta value = dadosQuizTexto.Respostas[k];
            dadosQuizTexto.Respostas[k] = dadosQuizTexto.Respostas[n];
            dadosQuizTexto.Respostas[n] = value;
        }
    }

    public void Responder(int idResposta)
    {
        respostasDadas.Add(idResposta);

        if (dadosQuizTexto[idDadoQuizAtual].Respostas[idResposta].RespostaCerta)
        {
            Debug.Log("Resposta Certa");

            contadorAcertos++;

            JogoQuizgameUI.MostrarFeedback(idResposta, true);

            //Feedback e Mostrar próxima pergunta
            if (idDadoQuizAtual >= dadosQuizTexto.Length - 1)
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
                idDica = Random.Range(0, dadosQuizTexto[idDadoQuizAtual].Respostas.Length);
            }
            while (dadosQuizTexto[idDadoQuizAtual].Respostas[idDica].RespostaCerta || respostasDadas.Contains(idDica));
        }

        return idDica;
    }
}

[System.Serializable]
public class DadosQuizTexto
{
    [SerializeField] private Sprite imagemPergunta;
    [SerializeField][TextArea(2, 5)] private string pergunta;
    [SerializeField] private QuizTextoResposta[] respostas;

    public Sprite ImagemPergunta => imagemPergunta;
    public string Pergunta => pergunta;
    public QuizTextoResposta[] Respostas => respostas;
}
[System.Serializable]
public class QuizTextoResposta
{
    [SerializeField] private bool respostaCerta;
    [SerializeField][TextArea(2, 5)] private string resposta;

    public bool RespostaCerta => respostaCerta;
    public string Resposta => resposta;
}
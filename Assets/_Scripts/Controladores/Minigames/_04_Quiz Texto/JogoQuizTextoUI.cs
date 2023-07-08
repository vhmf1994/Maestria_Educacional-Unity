using LightScrollSnap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JogoQuizTextoUI : BaseMinigameUI
{
    [Header("Contador Dados Quiz Texto")]
    [SerializeField] private Transform contadorVisual;

    [Header("Contador")]
    [SerializeField] private TMP_Text textoContador;

    [Header("Dica")]
    [SerializeField] private Button botaoDica;

    [Header("Configurações Quiz")]
    [SerializeField] private Image imagemPergunta;
    [SerializeField] private TMP_Text textoPergunta;

    [SerializeField] private Button[] botoesRespostas;

    private int numAcertos;
    private int numErros;

    private void OnValidate()
    {
        DefinirContador(10);
        DefinirContadorAtual(10);
    }

    private void Start()
    {
        numAcertos = 0;
        numErros = 0;

        AtualizarContador(numAcertos, numErros);
    }

    public void DefinirContador(int numDados)
    {
        for (int i = contadorVisual.childCount - 1; i >= numDados; i--)
        {
            contadorVisual.GetChild(i).gameObject.SetActive(false);
        }
    }
    public void DefinirContadorAtual(int contadorAtual)
    {
        for (int i = 0; i < contadorAtual; i++)
        {
            contadorVisual.GetChild(i).GetChild(0).GetComponent<Image>().enabled = true;
        }

        for (int i = contadorAtual; i < contadorVisual.childCount; i++)
        {
            contadorVisual.GetChild(i).GetChild(0).GetComponent<Image>().enabled = false;
        }
    }

    public void ConfigurarUI(DadosQuizTexto dadosQuizTexto)
    {
        // Verificar se existe imagem na pergunta
        if (dadosQuizTexto.ImagemPergunta == null)
        {
            imagemPergunta.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            imagemPergunta.transform.parent.gameObject.SetActive(true);
            imagemPergunta.sprite = dadosQuizTexto.ImagemPergunta;
        }

        // Define o texto da pergunta
        textoPergunta.SetText(dadosQuizTexto.Pergunta);

        // Define o texto e onClick dos botões
        for (int i = 0; i < dadosQuizTexto.Respostas.Length; i++)
        {
            int index = i;

            botoesRespostas[i].gameObject.SetActive(true);

            botoesRespostas[i].transform.GetChild(0).GetComponent<TMP_Text>().SetText(dadosQuizTexto.Respostas[i].Resposta);

            botoesRespostas[i].GetComponent<Button>().onClick.RemoveAllListeners();
            botoesRespostas[i].GetComponent<Button>().onClick.AddListener(() => OnBotaoRespostaClick(index));

            botoesRespostas[i].GetComponent<Button>().interactable = true;

            Image imagemFeedback = botoesRespostas[index].transform.GetChild(1).GetComponent<Image>();
            imagemFeedback.enabled = false;
        }

        // Desativando botoes que não estão sendo usados pra questão
        for (int i = botoesRespostas.Length - 1; i >= dadosQuizTexto.Respostas.Length; i--)
        {
            botoesRespostas[i].gameObject.SetActive(false);
        }

        // Configurando o botão de dicas
        botaoDica.onClick.RemoveAllListeners();
        botaoDica.onClick.AddListener(OnBotaoDicaClick);

        botaoDica.interactable = true;
    }

    public void AtualizarContador(int numAcertos, int numErros)
    {
        this.numAcertos = numAcertos;
        this.numErros = numErros;

        textoContador.SetText($"{numAcertos} | {numErros}");
    }

    private void OnBotaoRespostaClick(int idResposta)
    {
        (MinigamesController.Instance.CurrentBaseMinigame as JogoQuizTexto).Responder(idResposta);

        if (AudioController.Instance != null)
            AudioController.Instance.PlaySfx(SoundType.Clique);

        Vibrator.Vibrate(25);
    }

    private void OnBotaoDicaClick()
    {
        DesativarBotao((MinigamesController.Instance.CurrentBaseMinigame as JogoQuizTexto).GerarDica());

        if (AudioController.Instance != null)
            AudioController.Instance.PlaySfx(SoundType.Clique);

        Vibrator.Vibrate(25);

        botaoDica.interactable = false;
    }

    public void DesativarBotao(int idBotao)
    {
        botoesRespostas[idBotao].interactable = false;
    }

    public void MostrarFeedback(int idBotao, bool respostaCerta)
    {
        StartCoroutine(MostrarFeedbackCoroutine(idBotao, respostaCerta));
    }
    private IEnumerator MostrarFeedbackCoroutine(int idBotao, bool respostaCerta)
    {
        List<bool> previousStates = new List<bool>();

        foreach (Button botao in botoesRespostas)
        {
            previousStates.Add(botao.interactable);
            botao.interactable = false;
        }

        // Animação de feedback
        bool swap = false;

        Image imagemFeedback = botoesRespostas[idBotao].transform.GetChild(1).GetComponent<Image>();

        Color corErro = new Color(0.9f, 0.15f, 0.15f, 1f);
        Color corAcerto = new Color(0.15f, 0.75f, 0.15f, 1f);

        imagemFeedback.color = respostaCerta ? corAcerto : corErro;

        for (int i = 0; i < 8; i++)
        {
            swap = !swap;

            imagemFeedback.enabled = swap;

            yield return new WaitForSeconds(.2f);
        }

        foreach (Button botao in botoesRespostas)
        {
            botao.interactable = previousStates[botoesRespostas.ToList().IndexOf(botao)];
        }

        DesativarBotao(idBotao);

        if (respostaCerta)
            (MinigamesController.Instance.CurrentBaseMinigame as JogoQuizTexto).ConfigurarProximaPergunta();
    }
}
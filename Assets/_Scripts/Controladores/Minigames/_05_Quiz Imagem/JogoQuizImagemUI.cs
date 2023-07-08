using LightScrollSnap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class JogoQuizImagemUI : BaseMinigameUI
{
    [Header("Contador Dados Quiz Imagem")]
    [SerializeField] private Transform contadorVisual;

    [Header("Contador")]
    [SerializeField] private TMP_Text textoContador;

    [Header("Dica")]
    [SerializeField] private Button botaoDica;

    [Header("Configurações Quiz")]
    [SerializeField] private TMP_Text textoPergunta;

    [SerializeField] private Button[] botoesRespostas;
    [SerializeField] private Button botaoConfirmarResposta;

    private ScrollSnap scrollSnap;
    private ScrollRect scrollRect;

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

    public void ConfigurarUI(DadosQuizImagem dadosQuizImagem)
    {
        // Define o texto da pergunta
        textoPergunta.SetText(dadosQuizImagem.Pergunta);

        // Define o texto e onClick dos botões
        for (int i = 0; i < dadosQuizImagem.Respostas.Length; i++)
        {
            int index = i;

            botoesRespostas[index].gameObject.SetActive(true);

            botoesRespostas[index].transform.GetChild(0).GetComponent<Image>().sprite = dadosQuizImagem.Respostas[index].Resposta;

            botoesRespostas[index].GetComponent<Button>().interactable = true;

            Image imagemFeedback = botoesRespostas[index].transform.GetChild(1).GetComponent<Image>();
            imagemFeedback.enabled = false;
        }

        // Desativando botoes que não estão sendo usados pra questão
        for (int i = botoesRespostas.Length - 1; i >= dadosQuizImagem.Respostas.Length; i--)
        {
            botoesRespostas[i].gameObject.SetActive(false);
        }

        // Configurando o botão de dicas
        botaoDica.onClick.RemoveAllListeners();
        botaoDica.onClick.AddListener(OnBotaoDicaClick);

        botaoDica.interactable = true;

        // Configurando o botão de confirmar respostas
        botaoConfirmarResposta.onClick.RemoveAllListeners();
        botaoConfirmarResposta.onClick.AddListener(OnBotaoRespostaClick);

        botaoConfirmarResposta.interactable = true;

        // Resetar o ScrollSnap
        if (scrollSnap == null)
            scrollSnap = FindObjectOfType<ScrollSnap>();

        scrollSnap.enabled = true;

        scrollSnap.OnItemSelected.RemoveAllListeners();
        scrollSnap.OnItemSelected.AddListener(OnItemChanged);

        scrollSnap.ScrollToItem(0);

        // Resetar o ScrollRect
        if (scrollRect == null)
            scrollRect = scrollSnap.GetComponent<ScrollRect>();
    }

    public void AtualizarContador(int numAcertos, int numErros)
    {
        this.numAcertos = numAcertos;
        this.numErros = numErros;

        textoContador.SetText($"{numAcertos} | {numErros}");
    }

    private void OnItemChanged(RectTransform rt, int id)
    {
        botaoConfirmarResposta.interactable = botoesRespostas[id].interactable;
        Vibrator.Vibrate(25);
    }

    private void OnBotaoRespostaClick()
    {
        (MinigamesController.Instance.CurrentBaseMinigame as JogoQuizImagem).Responder(scrollSnap.SelectedItemIndex);

        if (AudioController.Instance != null)
            AudioController.Instance.PlaySfx(SoundType.Clique);

        Vibrator.Vibrate(25);
    }

    private void OnBotaoDicaClick()
    {
        int idDica = (MinigamesController.Instance.CurrentBaseMinigame as JogoQuizImagem).GerarDica();

        DesativarBotao(idDica);

        if (AudioController.Instance != null)
            AudioController.Instance.PlaySfx(SoundType.Clique);

        Vibrator.Vibrate(25);

        botaoDica.interactable = false;
    }

    public void DesativarBotao(int idBotao)
    {
        botoesRespostas[idBotao].interactable = false;
        botaoConfirmarResposta.interactable = botoesRespostas[scrollSnap.SelectedItemIndex].interactable;
    }

    public void MostrarFeedback(int idBotao, bool respostaCerta)
    {
        StartCoroutine(MostrarFeedbackCoroutine(idBotao, respostaCerta));
    }
    private IEnumerator MostrarFeedbackCoroutine(int idBotao, bool respostaCerta)
    {
        scrollRect.enabled = false;
        scrollSnap.enabled = false;

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

        scrollRect.enabled = true;
        scrollSnap.enabled = true;

        if (respostaCerta)
            (MinigamesController.Instance.CurrentBaseMinigame as JogoQuizImagem).ConfigurarProximaPergunta();
    }
}

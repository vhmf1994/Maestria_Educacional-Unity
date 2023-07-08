using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JogoMemoriaUI : BaseMinigameUI
{
    [Header("Contador Dados Jogo da Memoria")]
    [SerializeField] private TMP_Text textoContador;

    [Header("Dica")]
    [SerializeField] private Button botaoDica;

    [Header("Configurações Jogo da Memoria")]
    [SerializeField] private Transform cartasContainer;
    [SerializeField] private CanvasGroup containerCanvasGroup;
    [Space(10)]
    [SerializeField] private CartaJogoMemoria cartaPrefab;

    private List<DadosJogoMemoria> todosDadosJogoMemoria;

    [Header("Controle de cartas")]
    [SerializeField, ReadOnly] private CartaJogoMemoria carta_1;
    [SerializeField, ReadOnly] private CartaJogoMemoria carta_2;

    public void ConfigurarUI(List<DadosJogoMemoria> dadosJogoMemoria)
    {
        if (cartasContainer.childCount > 0)
        {
            for (int i = 0; i < cartasContainer.childCount; i++)
            {
                Destroy(cartasContainer.GetChild(i).gameObject);
            }
        }

        todosDadosJogoMemoria = new List<DadosJogoMemoria>(dadosJogoMemoria);

        for (int i = 0; i < dadosJogoMemoria.Count; i++)
        {
            todosDadosJogoMemoria.Add(dadosJogoMemoria[i].CriarCopia());
        }

        // Embaralhar as Cartas
        int n = todosDadosJogoMemoria.Count;
        System.Random rng = new System.Random();

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            DadosJogoMemoria value = todosDadosJogoMemoria[k];
            todosDadosJogoMemoria[k] = todosDadosJogoMemoria[n];
            todosDadosJogoMemoria[n] = value;
        }

        // Criar visual das Cartas
        for (int i = 0; i < todosDadosJogoMemoria.Count; i++)
        {
            CartaJogoMemoria novaCarta = Instantiate(cartaPrefab, cartasContainer);

            // Configurar visual da nova carta;
            novaCarta.ConfigurarCarta(todosDadosJogoMemoria[i]);

            // Adicionar evento de clique na carta
            novaCarta.onClick.AddListener(() => SelecionarCarta(novaCarta));
        }

        AtualizarTentativas(0);

        botaoDica.onClick.RemoveAllListeners();
        botaoDica.onClick.AddListener(FeedbackDica);
    }

    private void SelecionarCarta(CartaJogoMemoria cartaJogoMemoria)
    {
        if (carta_1 == null)
        {
            // Seleciona a carta
            carta_1 = cartaJogoMemoria;
            carta_1.Show();
        }
        else
        {
            if (carta_1 == cartaJogoMemoria)
            {
                // Desselecionar a carta
                carta_1.Hide();
                carta_1 = null;
            }
            else
            {
                // Selecionar a carta
                carta_2 = cartaJogoMemoria;
                carta_2.Show();

                FeedbackCartas();
            }
        }

        AudioController.Instance.PlaySfx(SoundType.Pop);
        Vibrator.Vibrate(25);
    }

    public void AtualizarTentativas(int numTentativas)
    {
        textoContador.SetText($"{numTentativas} | {todosDadosJogoMemoria.Count / 2}");
    }

    public void FeedbackCartas()
    {
        StartCoroutine(FeedbackAnimacao());
    }
    private IEnumerator FeedbackAnimacao()
    {
        containerCanvasGroup.interactable = false;

        yield return new WaitForSeconds(.5f);

        // Verificar o par
        bool acertou = (MinigamesController.Instance.CurrentBaseMinigame as JogoMemoria).VerificarPares(carta_1, carta_2);

        // Feedback de acerto
        carta_1.DefinirFeedback(acertou);
        carta_2.DefinirFeedback(acertou);

        yield return new WaitForSeconds(1f);

        if (!acertou)
        {
            carta_1.Hide();
            carta_2.Hide();
        }

        // Desseleciona as duas cartas
        carta_1 = null;
        carta_2 = null;

        yield return new WaitForSeconds(.5f);

        containerCanvasGroup.interactable = true;
    }

    public void FeedbackDica()
    {
        StartCoroutine(FeedbackDicaAnimacao());
    }
    private IEnumerator FeedbackDicaAnimacao()
    {
        containerCanvasGroup.interactable = false;
        botaoDica.interactable = false;

        yield return new WaitForSeconds(.5f);

        DadosJogoMemoria[] todasCartasNaoEscolhidas = todosDadosJogoMemoria.Where(c => !c.acertou).ToArray();

        // buscar duas cartas certas que ainda não foram encontradas
        int cartaRandom = UnityEngine.Random.Range(0, todasCartasNaoEscolhidas.Length);

        CartaJogoMemoria[] parSorteado = cartasContainer.GetComponentsInChildren<CartaJogoMemoria>().ToList().Where(c => c.DadosJogoMemoria.idCarta == todasCartasNaoEscolhidas[cartaRandom].idCarta).ToArray();

        for (int i = 0; i < parSorteado.Length; i++)
        {
            parSorteado[i].Show();
        }

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < parSorteado.Length; i++)
        {
            parSorteado[i].Hide();
        }

        yield return new WaitForSeconds(.5f);

        containerCanvasGroup.interactable = true;
    }

    protected override IEnumerator MostrarTutorialCoroutine(Action onTutorialCompleted)
    {
        if (painelTutorial != null)
        {
            painelTutorial.SetActive(true);
            botaoDica.interactable = false;

            yield return new WaitForSeconds(5f);

            painelTutorial.SetActive(false);
        }

        for (int i = 0; i < cartasContainer.childCount; i++)
        {
            CartaJogoMemoria carta = cartasContainer.GetChild(i).GetComponent<CartaJogoMemoria>();

            carta.Show();
        }

        yield return new WaitForSeconds(5f);

        for (int i = 0; i < cartasContainer.childCount; i++)
        {
            CartaJogoMemoria carta = cartasContainer.GetChild(i).GetComponent<CartaJogoMemoria>();

            carta.Hide();
        }

        botaoDica.interactable = true;
        onTutorialCompleted.Invoke();
    }
}

using LightScrollSnap;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JogoGrupoImagensUI : BaseMinigameUI
{
    [Header("Contador")]
    [SerializeField] private TMP_Text textoContador;

    [Header("Confirmar")]
    [SerializeField] private Button botaoConfirmar;

    [Header("Configurações Grupo Imagens")]
    [SerializeField] private GrupoImagens grupoPrefab;
    [SerializeField] private CartaGrupoImagens cartaPrefab;
    [Space(5)]
    [SerializeField] private Transform gruposContainer;
    [SerializeField] private Transform cartasContainer;

    private int numAcertos;
    private int numErros;

    [SerializeField, ReadOnly] private int numCartasRestantes;

    private void OnValidate()
    {
        numAcertos = 0;
        numErros = 0;

        AtualizarContador(numAcertos, numErros);
    }

    private void FixedUpdate()
    {
        if (cartasContainer.childCount != numCartasRestantes)
            numCartasRestantes = cartasContainer.childCount;

        botaoConfirmar.interactable = numCartasRestantes == 0;
    }

    public void ConfigurarUI(List<DadosGruposImagem> dadosGruposImagens, List<DadosCartaGruposImagens> dadosCartaGruposImagens)
    {
        numAcertos = 0;
        numErros = 0;

        AtualizarContador(numAcertos, numErros);

        if (gruposContainer.childCount > 0)
        {
            for (int i = 0; i < gruposContainer.childCount; i++)
            {
                Destroy(gruposContainer.GetChild(i).gameObject);
            }
        }

        if (cartasContainer.childCount > 0)
        {
            for (int i = 0; i < cartasContainer.childCount; i++)
            {
                Destroy(cartasContainer.GetChild(i).gameObject);
            }
        }

        Debug.Log(dadosGruposImagens.Count);
        Debug.Log(dadosCartaGruposImagens.Count);

        // Criando os grupos na UI
        for (int i = 0; i < dadosGruposImagens.Count; i++)
        {
            GrupoImagens novoGrupo = Instantiate(grupoPrefab, gruposContainer);

            novoGrupo.ConfigurarGrupo(dadosGruposImagens[i]);
        }

        // Criando as imagens na UI
        for (int i = 0; i < dadosCartaGruposImagens.Count; i++)
        {
            CartaGrupoImagens novaCarta = Instantiate(cartaPrefab, cartasContainer);

            novaCarta.ConfigurarCarta(dadosCartaGruposImagens[i]);

            Vector2 randomPos = new Vector2(Random.Range(-20, 21) * 15, Random.Range(-20, 21) * 15);

            (novaCarta.transform as RectTransform).anchoredPosition = randomPos;
        }

        numCartasRestantes = dadosCartaGruposImagens.Count;

        // Configurando o botão de resposta
        botaoConfirmar.onClick.RemoveAllListeners();
        botaoConfirmar.onClick.AddListener(OnBotaoResponder);

        botaoConfirmar.interactable = false;
    }

    public void AtualizarContador(int numAcertos, int numErros)
    {
        this.numAcertos = numAcertos;
        this.numErros = numErros;

        textoContador.SetText($"{numAcertos} | {numErros}");
    }

    public void OnBotaoResponder()
    {
        List<DadosGruposImagem> dadosGruposImagensÀtual = new List<DadosGruposImagem>();
        List<GrupoImagens> gruposImagensÀtual = gruposContainer.GetComponentsInChildren<GrupoImagens>().ToList();

        for (int i = 0; i < gruposImagensÀtual.Count; i++)
        {
            dadosGruposImagensÀtual.Add(gruposImagensÀtual[i].GruposImagemAtual);
        }

        (MinigamesController.Instance.CurrentBaseMinigame as JogoGrupoImagens).Responder(dadosGruposImagensÀtual);
    }
}
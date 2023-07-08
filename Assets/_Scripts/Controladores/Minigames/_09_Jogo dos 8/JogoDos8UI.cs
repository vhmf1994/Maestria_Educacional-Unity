using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class JogoDos8UI : BaseMinigameUI
{
    [Header("Configurações Dados Jogo dos 8")]
    [SerializeField] private TMP_Text textoContador;
    [SerializeField] private Transform cartasContainer;
    [Space(10)]
    [SerializeField] private CartaJogoDos8 cartaPrefab;
    [SerializeField] private EspacoJogoDos8 espacoJogoDos8;
    [Space(10)]
    [SerializeField, ReadOnly] private List<Vector2Int> posicoesGrid;
    [SerializeField, ReadOnly] private List<Vector2Int> posicoesGridDisponiveis;
    [Space(10)]
    [SerializeField, ReadOnly] private List<CartaJogoDos8> todasCartasJogoDos8;

    private void OnValidate()
    {
        CalcularPosicoes();
    }

    private void CalcularPosicoes()
    {
        posicoesGrid = new List<Vector2Int>();

        int posX = 0;
        int posY = 0;

        for (int i = 0; i < 9; i++)
        {
            posicoesGrid.Add(new Vector2Int(posX, posY));

            posX++;

            if (posX == 3)
            {
                posX = 0;
                posY++;
            }
        }
    }

    public void ConfigurarUI(DadosJogoDos8 dadosJogoDos8)
    {
        // Crias as novas cartas
        todasCartasJogoDos8 = new List<CartaJogoDos8>();

        // Destroi cartas já existentes 
        if (cartasContainer.childCount > 1)
        {
            todasCartasJogoDos8 = cartasContainer.GetComponentsInChildren<CartaJogoDos8>().ToList();
        }
        else
        {
            List<Sprite> spriteCartas = CortarSprite(dadosJogoDos8.ImagemJogoDos8);

            for (int i = 0; i < 8; i++)
            {
                CartaJogoDos8 novaCarta = Instantiate(cartaPrefab, cartasContainer);

                // Dados originais
                DadosCartaJogoDos8 dadosCartaJogoDos8Original = new DadosCartaJogoDos8()
                {
                    IdPosicao = i,
                    GridPosicao = posicoesGrid[i]
                };

                novaCarta.ConfigurarCartaOriginal(dadosCartaJogoDos8Original, spriteCartas[i]);

                novaCarta.name = $"{cartaPrefab.name} [{i + 1}]";

                novaCarta.onClick.AddListener(() => TrocarCartaDePosicao(novaCarta));

                todasCartasJogoDos8.Add(novaCarta);
            }
        }

        espacoJogoDos8.transform.SetAsLastSibling();

        // Calculando os Dados Atuais das cartas
        CalcularDadosAtuais();

        // Embaralhar as cartas
        int n = 1000;
        System.Random rng = new System.Random();

        while (n > 1)
        {
            n--;

            posicoesGridDisponiveis = espacoJogoDos8.VerificarPosicoesAtivas();

            List<CartaJogoDos8> cartasDisponiveis = todasCartasJogoDos8.Where(c => posicoesGridDisponiveis.Contains(c.DadosCartaJogoDos8Atual.GridPosicao)).ToList();

            CartaJogoDos8 cartaEscolhida = cartasDisponiveis[rng.Next(0, cartasDisponiveis.Count)];

            TrocarCartaDePosicao(cartaEscolhida);
        }
    }

    private List<Sprite> CortarSprite(Sprite sprite)
    {
        List<Sprite> sprites = new List<Sprite>();

        float sizeX = Mathf.FloorToInt(sprite.rect.width / 3);
        float sizeY = Mathf.FloorToInt(sprite.rect.height / 3);

        for (int j = 2; j >= 0; j--)
        {
            for (int k = 0; k <= 2; k++)
            {
                Rect rect = new Rect(k * sizeX, j * sizeY, sizeX, sizeY);

                Sprite newSprite = Sprite.Create(sprite.texture, rect, new Vector2(0.5f, 0.5f));

                newSprite.name = $"{k}/{j}";

                sprites.Add(newSprite);
            }
        }

        return sprites;
    }

    private void CalcularDadosAtuais()
    {
        List<DadosCartaJogoDos8> todosDadosAtuais = new List<DadosCartaJogoDos8>();

        for (int i = 0; i < todasCartasJogoDos8.Count; i++)
        {
            int id = todasCartasJogoDos8[i].transform.GetSiblingIndex();

            // Dados atuais
            DadosCartaJogoDos8 dadosCartaJogoDos8Atual = new DadosCartaJogoDos8()
            {
                IdPosicao = id,
                GridPosicao = posicoesGrid[id]
            };

            todasCartasJogoDos8[i].ConfigurarCartaAtual(dadosCartaJogoDos8Atual);

            todosDadosAtuais.Add(dadosCartaJogoDos8Atual);
        }

        // Espaco atual
        DadosCartaJogoDos8 dadosEspacoJogoDos8Atual = new DadosCartaJogoDos8()
        {
            IdPosicao = espacoJogoDos8.transform.GetSiblingIndex(),
            GridPosicao = posicoesGrid[espacoJogoDos8.transform.GetSiblingIndex()]
        };

        espacoJogoDos8.ConfigurarEspacoAtual(dadosEspacoJogoDos8Atual);

        // Validar cartas disponiveis
        posicoesGridDisponiveis = espacoJogoDos8.VerificarPosicoesAtivas();

        for (int i = 0; i < todasCartasJogoDos8.Count; i++)
        {
            todasCartasJogoDos8[i].interactable = posicoesGridDisponiveis.Contains(todasCartasJogoDos8[i].DadosCartaJogoDos8Atual.GridPosicao);
        }

        // Verificar se finalizou
        (MinigamesController.Instance.CurrentBaseMinigame as JogoDos8).VerificarPosicoes(todosDadosAtuais);
    }

    private void TrocarCartaDePosicao(CartaJogoDos8 cartaJogoDos8)
    {
        int idPosicaoAtual = cartaJogoDos8.DadosCartaJogoDos8Atual.IdPosicao;
        int idPosicaoEspacoAtual = espacoJogoDos8.DadosEspacoAtual.IdPosicao;

        espacoJogoDos8.transform.SetSiblingIndex(idPosicaoAtual);
        cartaJogoDos8.transform.SetSiblingIndex(idPosicaoEspacoAtual);

        CalcularDadosAtuais();
    }

    public void AtualizarContador(int numTentativas)
    {
        textoContador.SetText($"{numTentativas}");
    }

    public void DesativarTodasCartas()
    {
        for (int i = 0; i < todasCartasJogoDos8.Count; i++)
        {
            todasCartasJogoDos8[i].interactable = false;
        }
    }
}

using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SudokuUI : BaseMinigameUI
{
    [Header("Configurações Dados Sudoku")]
    [SerializeField] private TMP_Text textoContador;
    [SerializeField] private Transform cartasContainer;
    [Space(10)]
    [SerializeField, ReadOnly] private List<CartaSudoku> todasCartasSudoku;

    [Header("Controle")]
    [SerializeField] private CartaSudoku cartaSelecionada;
    public CartaSudoku CartaSelecionada { get { return cartaSelecionada; } set { cartaSelecionada = value; } }

    private void OnValidate()
    {
        todasCartasSudoku = null;
    }

    public void ConfigurarUI(List<Sprite> dadosSudoku)
    {
        todasCartasSudoku = FindObjectsOfType<CartaSudoku>().ToList();

        todasCartasSudoku.Sort((a, b) =>
        {
            if (a.transform.GetSiblingIndex() < b.transform.GetSiblingIndex())
                return -1;
            else return 1;
        });

        // Limpar a arte de todas as cartas
        foreach (CartaSudoku carta in todasCartasSudoku)
        {
            carta.onClick.RemoveAllListeners();
            carta.onClick.AddListener(() => OnCartaClick(carta));

            carta.ConfigurarOpcoes(dadosSudoku);
        }
    }

    public void ConfigurarCarta(int posCarta, int valorCarta, bool interactable = true)
    {
        todasCartasSudoku[posCarta].ConfigurarCartaAtual(valorCarta);

        if (interactable)
            todasCartasSudoku[posCarta].CartaDisponivel();
        else
            todasCartasSudoku[posCarta].CartaIndisponivel();
    }

    public void AtualizarContador(int numTentativas)
    {
        textoContador.SetText($"{numTentativas}");
    }

    public void OnCartaClick(CartaSudoku cartaSudoku)
    {
        if (cartaSelecionada == null)
        {
            cartaSelecionada = cartaSudoku;
            cartaSelecionada.Selecionar();
        }
    }
}

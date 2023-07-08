using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Sudoku", menuName = "Minigames/Sudoku", order = 0)]
public class Sudoku : BaseMinigame
{
    protected SudokuUI SudokuMinigameUI => baseMinigameUI as SudokuUI;

    [Header("Configuração Jogo dos 8")]
    [SerializeField] private List<Sprite> dadosSudoku;

    [Header("Dados Atuais")]
    [SerializeField, ReadOnly] private List<List<int>> colunas;
    [SerializeField, ReadOnly] private List<List<int>> linhas;
    [SerializeField, ReadOnly] private List<List<int>> quadrantes;

    private int contadorTentativas;

    private Dictionary<int, int> sudoku = new Dictionary<int, int>();

    protected override void OnValidate()
    {
        base.OnValidate();

        if (dadosSudoku == null)
        {
            dadosSudoku = new List<Sprite>();
        }

        Validate();
    }

    private void Validate()
    {
        sudoku = new Dictionary<int, int>();

        // Inicializando os dicionarios
        colunas = new List<List<int>>();
        linhas = new List<List<int>>();
        quadrantes = new List<List<int>>();

        // Criar listas para colunas, linhas e quadrantes
        for (int i = 0; i < 4; i++)
        {
            colunas.Add(new List<int>());
            linhas.Add(new List<int>());
            quadrantes.Add(new List<int>());
        }

        // Inicializar listas
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                colunas[i].Add(0);
                linhas[i].Add(0);
                quadrantes[i].Add(0);
            }
        }

        contadorTentativas = 0;
        timer = 600;
    }

    public override void InitializeMinigame()
    {
        base.InitializeMinigame();

        Validate();

        // Enviar os dados do jogo pra UI
        SudokuMinigameUI.ConfigurarUI(dadosSudoku);

        contadorTentativas = 0;
        SudokuMinigameUI.AtualizarContador(contadorTentativas);

        // Posicionar algumas peças
        PreencherSudokuAleatoriamente();

        List<int> idsRemovidos = new List<int>();

        do
        {
            int rndPos = Random.Range(0, 16);

            if (!idsRemovidos.Contains(rndPos))
            {
                sudoku[rndPos] = 0;
                AdicionarValor(rndPos, 0);
                idsRemovidos.Add(rndPos);
            }
        }
        while (idsRemovidos.Count < 9);

        foreach (var item in sudoku.Keys)
        {
            bool interactable = sudoku.GetValueOrDefault(item) == 0;
            SudokuMinigameUI.ConfigurarCarta(item, sudoku.GetValueOrDefault(item), interactable);
        }
    }

    private void PreencherSudokuAleatoriamente()
    {
        sudoku = new Dictionary<int, int>();

        PreencherSudokuRecursivamente(sudoku, 0);
    }

    private bool PreencherSudokuRecursivamente(Dictionary<int, int> sudoku, int posicao)
    {
        if (posicao == 16)
        {
            //ImprimirSudoku(sudoku);
            return true;
        }

        if (sudoku.ContainsKey(posicao))
        {
            return PreencherSudokuRecursivamente(sudoku, posicao + 1);
        }

        List<int> numerosDisponiveis = ObterNumerosDisponiveis(sudoku, posicao);
        ShuffleList(numerosDisponiveis);

        foreach (int numero in numerosDisponiveis)
        {
            sudoku[posicao] = numero;
            AdicionarValor(posicao, numero);

            if (PreencherSudokuRecursivamente(sudoku, posicao + 1))
                return true;

            sudoku.Remove(posicao);
            AdicionarValor(posicao, 0);
        }

        return false;
    }

    private List<int> ObterNumerosDisponiveis(Dictionary<int, int> sudoku, int posicao)
    {
        List<int> numerosDisponiveis = new List<int>() { 1, 2, 3, 4 };

        int row = posicao / 4;
        int col = posicao % 4;

        for (int c = 0; c < 4; c++)
        {
            int posCarta = row * 4 + c;
            if (sudoku.ContainsKey(posCarta))
            {
                numerosDisponiveis.Remove(sudoku[posCarta]);
            }
        }

        for (int r = 0; r < 4; r++)
        {
            int posCarta = r * 4 + col;
            if (sudoku.ContainsKey(posCarta))
            {
                numerosDisponiveis.Remove(sudoku[posCarta]);
            }
        }

        int blockRow = row / 2;
        int blockCol = col / 2;

        for (int r = blockRow * 2; r < blockRow * 2 + 2; r++)
        {
            for (int c = blockCol * 2; c < blockCol * 2 + 2; c++)
            {
                int posCarta = r * 4 + c;
                if (sudoku.ContainsKey(posCarta))
                {
                    numerosDisponiveis.Remove(sudoku[posCarta]);
                }
            }
        }

        return numerosDisponiveis;
    }

    private void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            int k = UnityEngine.Random.Range(0, n--);
            T temp = list[n];
            list[n] = list[k];
            list[k] = temp;
        }
    }

    protected override void FinalizeMinigame()
    {
        base.FinalizeMinigame();

        bool vitoria = !CheckIfIsOver();

        string textoTimer = System.TimeSpan.FromSeconds(currentTimer).ToString("mm':'ss");
        string textoExtra = $"{contadorTentativas}";

        if (audioController != null)
            audioController.PlaySfx(vitoria ? SoundType.Vitoria : SoundType.Derrota);

        JogoDos8UI.Instance.OnFinalizacaoMenu(vitoria, textoTimer, textoExtra);
    }

    public override void UpdateTimer()
    {
        base.UpdateTimer();

        baseMinigameUI.AtualizarTemporizador(System.TimeSpan.FromSeconds(currentTimer).ToString("mm':'ss"));
    }

    public bool ValidarValor(int posCarta, int idCarta)
    {
        if (currentMinigameState == MinigameState.Playing)
        {
            contadorTentativas++;
            SudokuMinigameUI.AtualizarContador(contadorTentativas);
        }

        int linha = posCarta / 4;
        int coluna = posCarta % 4;
        int quadrante = (linha / 2) * 2 + (coluna / 2);

        int quadranteLinha = linha % 2;
        int quadranteColuna = coluna % 2;

        if (idCarta == 0)
        {
            return true;
        }
        else
        {
            if (colunas[coluna].Contains(idCarta) || linhas[linha].Contains(idCarta) || quadrantes[quadrante].Contains(idCarta))
            {
                if (colunas[coluna][linha] == idCarta || linhas[linha][coluna] == idCarta || quadrantes[quadrante][(quadranteLinha * 2) + quadranteColuna] == idCarta)
                    return true;
                else
                    return false;
            }
            else
            {
                return true;
            }
        }
    }

    public void AdicionarValor(int posCarta, int idCarta)
    {
        int linha = posCarta / 4;
        int coluna = posCarta % 4;
        int quadrante = (linha / 2) * 2 + (coluna / 2);

        int quadranteLinha = linha % 2;
        int quadranteColuna = coluna % 2;

        colunas[coluna][linha] = idCarta;
        linhas[linha][coluna] = idCarta;
        quadrantes[quadrante][(quadranteLinha * 2) + quadranteColuna] = idCarta;

        if (currentMinigameState == MinigameState.Playing)
            VerificarSeAcabou();
    }

    public void VerificarSeAcabou()
    {
        int numCartas = 0;

        for (int i = 0; i < colunas.Count; i++)
        {
            for (int j = 0; j < colunas[i].Count; j++)
            {
                if (colunas[i][j] != 0)
                    numCartas++;
            }
        }

        if (numCartas == 16)
        {
            FinalizeMinigame();
        }
    }
}
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EspacoJogoDos8 : MonoBehaviour
{
    [Header("Dados Carta")]
    [SerializeField, ReadOnly] private DadosCartaJogoDos8 dadosEspacoOriginal;
    [SerializeField, ReadOnly] private DadosCartaJogoDos8 dadosEspacoAtual;

    public DadosCartaJogoDos8 DadosEspacoAtual => dadosEspacoAtual;

    [Header("Components")]
    [SerializeField] private GameObject setaBaixo;
    [SerializeField] private GameObject setaCima;
    [SerializeField] private GameObject setaDireita;
    [SerializeField] private GameObject setaEsquerda;

    [SerializeField, ReadOnly] private List<GameObject> setas;

    private void OnValidate()
    {
        Initialize();
    }

    private void Start()
    {
        dadosEspacoOriginal = new DadosCartaJogoDos8()
        {
            IdPosicao = 8,
            GridPosicao = new Vector2Int(2, 2)
        };

        Initialize();
    }

    private void Initialize()
    {
        setaBaixo = transform.GetChild(0).gameObject;
        setaCima = transform.GetChild(1).gameObject;
        setaDireita = transform.GetChild(2).gameObject;
        setaEsquerda = transform.GetChild(3).gameObject;

        setas = new List<GameObject>();

        setas.Add(setaBaixo);
        setas.Add(setaCima);
        setas.Add(setaDireita);
        setas.Add(setaEsquerda);
    }

    public void ConfigurarEspacoAtual(DadosCartaJogoDos8 dadosEspacoAtual) => this.dadosEspacoAtual = dadosEspacoAtual;

    public List<Vector2Int> VerificarPosicoesAtivas()
    {
        List<Vector2Int> posicoesGridDisponiveis = new List<Vector2Int>();

        int xAtual = dadosEspacoAtual.GridPosicao.x;
        int yAtual = dadosEspacoAtual.GridPosicao.y;

        Vector2Int posicaoBaixo;
        Vector2Int posicaoCima;
        Vector2Int posicaoDireita;
        Vector2Int posicaoEsquerda;

        posicaoBaixo = new Vector2Int(xAtual, yAtual + 1);
        posicaoCima = new Vector2Int(xAtual, yAtual - 1);
        posicaoDireita = new Vector2Int(xAtual + 1, yAtual);
        posicaoEsquerda = new Vector2Int(xAtual - 1, yAtual);

        posicoesGridDisponiveis.Add(posicaoBaixo);
        posicoesGridDisponiveis.Add(posicaoCima);
        posicoesGridDisponiveis.Add(posicaoDireita);
        posicoesGridDisponiveis.Add(posicaoEsquerda);

        for (int i = posicoesGridDisponiveis.Count - 1; i >= 0; i--)
        {
            if (posicoesGridDisponiveis[i].x < 0 || posicoesGridDisponiveis[i].x > 2 ||
                posicoesGridDisponiveis[i].y < 0 || posicoesGridDisponiveis[i].y > 2)
            {
                setas[i].gameObject.SetActive(false);
                posicoesGridDisponiveis.RemoveAt(i);
            }
            else
            {
                setas[i].gameObject.SetActive(true);
            }
        }

        return posicoesGridDisponiveis;
    }
}

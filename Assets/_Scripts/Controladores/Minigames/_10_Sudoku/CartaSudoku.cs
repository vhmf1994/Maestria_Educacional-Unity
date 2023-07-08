using LightScrollSnap;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CartaSudoku : MonoBehaviour, IPointerClickHandler
{
    private SudokuUI SudokuMinigameUI => BaseMinigameUI.Instance as SudokuUI;
    [SerializeField] private List<Sprite> opcoesDisponiveis;

    [Header("Dados Carta")]
    [SerializeField, ReadOnly] private int posCartaAtual;

    [Header("Componentes")]
    [SerializeField, ReadOnly] private Animator m_animatorCarta;
    [Space(10)]
    [SerializeField] private ScrollSnap m_scrollSnap;
    [SerializeField, ReadOnly] private Image m_cartaBackgroundImage;
    [SerializeField] private Image[] contentImages;
    [Space(10)]
    [SerializeField] private Button botaoConfirmar;
    [SerializeField] private Button botaoDireita;
    [SerializeField] private Button botaoEsquerda;
    [Space(10)]
    [SerializeField] private bool m_interactable;
    [Space(10)]
    [SerializeField] private UnityEvent m_onClick;

    [Header("Disponibilidade")]
    [SerializeField] private Sprite cartaDisponivel;
    [SerializeField] private Sprite cartaNaoDisponivel;

    public Animator animatorCarta => m_animatorCarta;
    public ScrollSnap scrollSnap => m_scrollSnap;

    public bool interactable { get { return m_interactable; } set { m_interactable = value; } }
    public UnityEvent onClick => m_onClick;

    private void OnEnable()
    {
        botaoConfirmar.onClick.AddListener(OnBotaoConfirmar);
        botaoDireita.onClick.AddListener(OnBotaoDireita);
        botaoEsquerda.onClick.AddListener(OnBotaoEsquerda);

        m_scrollSnap.OnItemSelected.AddListener(OnItemChanged);
    }
    private void OnDisable()
    {
        botaoConfirmar.onClick.RemoveListener(OnBotaoConfirmar);
        botaoDireita.onClick.RemoveListener(OnBotaoDireita);
        botaoEsquerda.onClick.RemoveListener(OnBotaoEsquerda);

        m_scrollSnap.OnItemSelected.RemoveListener(OnItemChanged);
    }

    public void ConfigurarOpcoes(List<Sprite> opcoesDisponiveis)
    {
        this.opcoesDisponiveis = opcoesDisponiveis;

        m_animatorCarta = GetComponent<Animator>();
        m_cartaBackgroundImage = GetComponent<Image>();

        posCartaAtual = transform.GetSiblingIndex();

        ConfigurarCartaAtual(0);

        for (int i = 0; i < contentImages.Length; i++)
        {
            int index = i;
            contentImages[index].sprite = this.opcoesDisponiveis[index];
        }

        LimparCarta();
    }
    private void LimparCarta()
    {
        m_scrollSnap.SmoothScrollToItem(0);

        CartaDisponivel();
    }

    public void ConfigurarCartaAtual(int valorCartaAtual)
    {
        m_scrollSnap.SmoothScrollToItem(valorCartaAtual);
    }

    public void CartaIndisponivel()
    {
        m_animatorCarta.enabled = false;
        m_interactable = false;

        m_cartaBackgroundImage.sprite = cartaNaoDisponivel;
    }

    public void CartaDisponivel()
    {
        m_animatorCarta.enabled = true;
        m_interactable = true;

        m_cartaBackgroundImage.sprite = cartaDisponivel;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!m_interactable) return;

        m_onClick?.Invoke();
    }

    public void Selecionar()
    {
        m_animatorCarta.Play("botao_selecionado");
        
        AudioController.Instance.PlaySfx(SoundType.Pop);
        Vibrator.Vibrate(25);
    }

    private void Desselecionar()
    {
        m_animatorCarta.Play("botao_desselecionado");

        AudioController.Instance.PlaySfx(SoundType.Pop);
        Vibrator.Vibrate(25);
    }

    private void OnBotaoConfirmar()
    {
        // Validar se a figura escolhida é válida na posição atual
        Sudoku sudoku = MinigamesController.Instance.CurrentBaseMinigame as Sudoku;

        if (sudoku.ValidarValor(posCartaAtual, m_scrollSnap.SelectedItemIndex))
        {
            sudoku.AdicionarValor(posCartaAtual, m_scrollSnap.SelectedItemIndex);
            Desselecionar();
            SudokuMinigameUI.CartaSelecionada = null;
        }
        else
        {
            // Feedback de "erro"
        }
    }
    private void OnBotaoDireita()
    {
        AudioController.Instance.PlaySfx(SoundType.Clique);
        m_scrollSnap.SmoothScrollToNextItem();
    }
    private void OnBotaoEsquerda()
    {
        AudioController.Instance.PlaySfx(SoundType.Clique);
        m_scrollSnap.SmoothScrollToPreviousItem();
    }

    private void OnItemChanged(RectTransform rt, int id)
    {
        if (MinigamesController.Instance.CurrentBaseMinigame.CurrentMinigameState == MinigameState.Playing)
        {
            Vibrator.Vibrate(25);
        }
    }
}

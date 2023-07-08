using UnityEngine;
using UnityEngine.UI;

public class CartaGrupoImagens : Draggable
{
    [SerializeField] private DadosCartaGruposImagens dadosCartaGruposImagens;
    public DadosCartaGruposImagens DadosCartaGruposImagens => dadosCartaGruposImagens;

    [Header("Imagens")]
    [SerializeField] private Image imagemFeedback;
    [SerializeField] private Image imagemCarta;

    private void Start()
    {
        imagemFeedback.enabled = false;
    }

    public void ConfigurarCarta(DadosCartaGruposImagens dadosCartaGruposImagens)
    {
        this.dadosCartaGruposImagens = dadosCartaGruposImagens;
        imagemCarta.sprite = dadosCartaGruposImagens.ImagemCarta;
    }

    public void DefinirFeedback(bool acerto)
    {
        Color corErro = new Color(0.9f, 0.15f, 0.15f, 1f);
        Color corAcerto = new Color(0.15f, 0.75f, 0.15f, 1f);

        imagemFeedback.color = acerto ? corAcerto : corErro;
    }
}
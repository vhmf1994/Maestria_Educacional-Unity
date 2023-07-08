using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GrupoImagens : MonoBehaviour
{
    [SerializeField] private DadosGruposImagem gruposImagemOriginal;
    [SerializeField] private DadosGruposImagem gruposImagemAtual;

    public DadosGruposImagem GruposImagemAtual => gruposImagemAtual;

    [SerializeField] private TMP_Text textoNomeGrupo;

    int lastChildCount;

    public void ConfigurarGrupo(DadosGruposImagem gruposImagem)
    {
        gruposImagemOriginal = gruposImagem;
        textoNomeGrupo.SetText(gruposImagem.NomeGrupo);
    }

    public void ValidarGrupo()
    {
        Debug.Log($"Validando Grupo {gruposImagemOriginal.NomeGrupo}");

        gruposImagemAtual = new DadosGruposImagem()
        {
            IdGrupo = gruposImagemOriginal.IdGrupo
        };

        gruposImagemAtual.CartasGrupoImagem = new List<DadosCartaGruposImagens>();

        CartaGrupoImagens[] cartaGrupoImagens = GetComponentsInChildren<CartaGrupoImagens>();

        for (int i = 0; i < cartaGrupoImagens.Length; i++)
        {
            gruposImagemAtual.CartasGrupoImagem.Add(cartaGrupoImagens[i].DadosCartaGruposImagens);
        }
    }

    private void FixedUpdate()
    {
        if (transform.GetChild(0).childCount != lastChildCount)
        {
            lastChildCount = transform.GetChild(0).childCount;
            ValidarGrupo();
        }
    }
}

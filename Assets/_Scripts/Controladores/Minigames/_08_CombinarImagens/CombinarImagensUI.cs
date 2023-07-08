using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombinarImagensUI : BaseMinigameUI
{
    [Header("Contador Dados Jogo de Combinar Pares")]
    [SerializeField] private TMP_Text textoContador;

    [Header("Confirmar")]
    [SerializeField] private Button botaoConfirmar;

    [Header("Configurações Jogo de Combinar Pares")]
    [SerializeField] private Transform paresContainer;
    [Space(10)]
    [SerializeField] private ParCombinarImagens parCombinarPrefab;
    [Space(10)]
    [SerializeField] private CartaCombinarImagens cartaFixaPrefab;
    [SerializeField] private CartaCombinarImagens cartaMovelPrefab;

    [Header("Controle de cartas")]
    [SerializeField, ReadOnly] private List<DadosCombinarImagens> dadosDadosCombinarImagens;

    private int numAcertos;
    private int numErros;

    private void OnValidate()
    {
        numAcertos = 0;
        numErros = 0;

        AtualizarContador(numAcertos, numErros);
    }

    public void ConfigurarUI(List<DadosCombinarImagens> dadosDadosCombinarImagens)
    {
        numAcertos = 0;
        numErros = 0;

        AtualizarContador(numAcertos, numErros);

        // Definindo as cartas
        this.dadosDadosCombinarImagens = new List<DadosCombinarImagens>(dadosDadosCombinarImagens);

        for (int i = 0; i < this.dadosDadosCombinarImagens.Count; i++)
        {
            ParCombinarImagens novoPar = Instantiate(parCombinarPrefab, paresContainer);

            CartaCombinarImagens cartaFixa = Instantiate(cartaFixaPrefab, novoPar.transform);
            CartaCombinarImagens cartaMovel = Instantiate(cartaMovelPrefab, novoPar.transform);

            cartaFixa .transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = this.dadosDadosCombinarImagens[i].ParSprite[0];
            cartaMovel.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = this.dadosDadosCombinarImagens[i].ParSprite[1];

            cartaFixa.GetNewParentGroup();
            cartaMovel.GetNewParentGroup();

            novoPar.ConfigurarCarta(cartaFixa, cartaMovel);
        }

        // Embaralhando as cartas
        int n = this.dadosDadosCombinarImagens.Count;
        System.Random rng = new System.Random();

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            CartaCombinarImagens carta_1_temp = paresContainer.GetChild(k).GetComponent<ParCombinarImagens>().CartaMovel;
            CartaCombinarImagens carta_2_temp = paresContainer.GetChild(n).GetComponent<ParCombinarImagens>().CartaMovel;

            Transform pai = carta_1_temp.transform.parent;
            carta_1_temp.transform.SetParent(carta_2_temp.transform.parent);
            carta_2_temp.transform.SetParent(pai);

            carta_1_temp.GetNewParentGroup();
            carta_2_temp.GetNewParentGroup();

            for (int i = 0; i < carta_1_temp.GetComponents<MonoBehaviour>().Length; i++)
            {
                carta_1_temp.GetComponents<MonoBehaviour>()[i].enabled = true;
            }
            for (int i = 0; i < carta_2_temp.GetComponents<MonoBehaviour>().Length; i++)
            {
                carta_2_temp.GetComponents<MonoBehaviour>()[i].enabled = true;
            }
        }

        // Configurando o botão de resposta
        botaoConfirmar.onClick.RemoveAllListeners();
        botaoConfirmar.onClick.AddListener(OnBotaoResponder);

        if (paresContainer.childCount > this.dadosDadosCombinarImagens.Count)
        {
            for (int i = this.dadosDadosCombinarImagens.Count; i < paresContainer.childCount; i++)
            {
                Destroy(paresContainer.GetChild(i).gameObject);
            }
        }
    }

    public void AtualizarContador(int numAcertos, int numErros)
    {
        this.numAcertos = numAcertos;
        this.numErros = numErros;

        textoContador.SetText($"{numAcertos} | {numErros}");
    }

    public void OnBotaoResponder()
    {
        List<ParCombinarImagens> paresCombinarImagens = FindObjectsOfType<ParCombinarImagens>().ToList();

        (MinigamesController.Instance.CurrentBaseMinigame as CombinarImagens).Responder(paresCombinarImagens);
    }
}

[System.Serializable]
public class DadosCombinarImagens
{
    [SerializeField] private List<Sprite> parSprite;
    public List<Sprite> ParSprite => parSprite;
}
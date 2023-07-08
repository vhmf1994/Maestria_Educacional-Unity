using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParCombinarImagens : MonoBehaviour
{
    [Header("Carta Original")]
    [SerializeField] private CartaCombinarImagens cartaFixa_Original;
    [SerializeField] private CartaCombinarImagens cartaMovel_Original;

    [Header("Carta durante jogo")]
    [SerializeField] private CartaCombinarImagens cartaMovel;
    public CartaCombinarImagens CartaMovel { get { return cartaMovel; } set { cartaMovel = value; } }

    public void ConfigurarCarta(CartaCombinarImagens cartaFixa, CartaCombinarImagens cartaMovel)
    {
        this.cartaFixa_Original = cartaFixa;
        this.cartaMovel_Original = cartaMovel;
    }
    public void SetCartaMovel(CartaCombinarImagens cartaMovel)
    {
        this.cartaMovel = cartaMovel;
    }

    public bool CompararCartas()
    {
        return cartaMovel_Original.Equals(cartaMovel);
    }
}

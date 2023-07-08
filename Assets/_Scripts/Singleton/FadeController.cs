using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeController : MonoSingleton<FadeController>
{
    private const string FADE_IN = "Fade In";
    private const string FADE_OUT = "Fade Out";

    private Animator m_animator;

    protected override void InitializeBehaviour()
    {
        m_animator = GetComponent<Animator>();
    }

    public IEnumerator FadeIn()
    {
        m_animator.Play(FADE_IN);

        yield return new WaitForSeconds(m_animator.GetCurrentAnimatorStateInfo(0).length);
    }

    public IEnumerator FadeOut()
    {
        m_animator.Play(FADE_OUT);

        yield return new WaitForSeconds(m_animator.GetCurrentAnimatorStateInfo(0).length);
    }
}

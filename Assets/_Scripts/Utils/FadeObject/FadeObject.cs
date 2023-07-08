using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeObject : MonoBehaviour
{
    [SerializeField] private List<Renderer> Renderers = new List<Renderer>();
    [SerializeField] private List<Material> Materials = new List<Material>();

    [SerializeField][Range(0, 1f)] private float FadedAlpha = 1f;
    private float LastFadedAlpha;

    private void OnEnable()
    {
        if (Renderers.Count == 0)
        {
            Renderers.AddRange(GetComponentsInChildren<Renderer>());

            foreach (Renderer renderer in Renderers)
            {
                Materials.AddRange(renderer.materials);
            }
        }

        LastFadedAlpha = FadedAlpha;
    }

    private void FixedUpdate()
    {
        if (LastFadedAlpha != FadedAlpha)
            ConfigFadeObject();
    }

    private void ConfigFadeObject()
    {
        foreach (Material material in Materials)
        {
            if (material.HasProperty("_BaseColor"))
            {
                material.color = new Color(
                    material.color.r,
                    material.color.g,
                    material.color.b,
                    Mathf.Lerp(FadedAlpha, LastFadedAlpha, Time.deltaTime)
                );
            }
        }

        float alphaClip = 0.98f;

        foreach (Material material in Materials)
        {
            material.SetInt("_SrcBlend", FadedAlpha > alphaClip ? (int)UnityEngine.Rendering.BlendMode.One : (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", FadedAlpha > alphaClip ? (int)UnityEngine.Rendering.BlendMode.Zero : (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", FadedAlpha > alphaClip ? 1 : 0);
            material.SetInt("_Surface", FadedAlpha > alphaClip ? 0 : 1);

            material.renderQueue = FadedAlpha > alphaClip ? (int)UnityEngine.Rendering.RenderQueue.Geometry : (int)UnityEngine.Rendering.RenderQueue.Transparent;

            material.SetShaderPassEnabled("DepthOnly", FadedAlpha > alphaClip);
            material.SetShaderPassEnabled("SHADOWCASTER", FadedAlpha > alphaClip);

            material.SetOverrideTag("RenderType", FadedAlpha > alphaClip ? "Opaque" : "Transparent");

            if (FadedAlpha > alphaClip)
            {
                material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            }
            else
            {
                material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            }
        }

        LastFadedAlpha = FadedAlpha;
    }
}

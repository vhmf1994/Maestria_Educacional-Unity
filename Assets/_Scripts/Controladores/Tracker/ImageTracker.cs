using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public abstract class ImageTracker<T> : MonoBehaviour
{
    [SerializeField] protected ARSession arSession;
    [SerializeField] protected ARTrackedImageManager arTrackedImageManager;

    protected event EventHandler<ARTrackedImage> OnARImageTracked_Added;
    protected event EventHandler<ARTrackedImage> OnARImageTracked_Updated;
    protected event EventHandler<ARTrackedImage> OnARImageTracked_Removed;

    protected Dictionary<string, T> trackedImagesDictionary;

    [SerializeField] protected TMP_Text textoFeedback;

    public bool scanAtivo;

    private void OnValidate()
    {
        if (!Application.isEditor) return;

        arSession = FindObjectOfType<ARSession>();
        arTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
    }
    protected virtual void Start()
    {
        trackedImagesDictionary = new Dictionary<string, T>();
    }
    protected virtual void OnEnable()
    {
        // Subscreve o evento de quando é encontrado imagens AR/QR
        arTrackedImageManager.trackedImagesChanged += OnImageTrackedEvent;
    }
    protected virtual void OnDisable()
    {
        // "Dessubscreve" o evento de quando é encontrado imagens AR/QR
        arTrackedImageManager.trackedImagesChanged -= OnImageTrackedEvent;
    }

    public void EnableTracking()
    {
        // Ativa o component responsável por verificar imagens
        arTrackedImageManager.enabled = true;
    }
    public void DisableTracking()
    {
        // Desativa o component responsável por verificar imagens
        arTrackedImageManager.enabled = false;
    }
    public void ResetTracking()
    {
        // Reseta o component responsável por verificar imagens
        arSession.Reset();
    }

    protected virtual void OnImageTrackedEvent(ARTrackedImagesChangedEventArgs eventArgs)
    {
        if (!scanAtivo) return;

        // Handle added event
        foreach (ARTrackedImage addedImage in eventArgs.added)
            OnARImageTracked_Added?.Invoke(this, addedImage);

        // Handle updated event
        foreach (ARTrackedImage updatedImage in eventArgs.updated)
            OnARImageTracked_Updated?.Invoke(this, updatedImage);

        // Handle removed event
        foreach (ARTrackedImage removedImage in eventArgs.removed)
            OnARImageTracked_Removed?.Invoke(this, removedImage);
    }
}

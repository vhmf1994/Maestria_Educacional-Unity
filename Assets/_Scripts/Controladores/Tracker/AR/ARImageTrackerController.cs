using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARImageTrackerController : ImageTracker<ARTrackedImageData>
{
    [Header("Configurações AR Tracker Controller")]
    [SerializeField] private List<ARTrackedImageData> referenciaARTrackedImage;

    [SerializeField] private bool showMarkers = false;

    private void OnValidate()
    {
        //CheckARObjects();
    }

    private void Awake()
    {
        scanAtivo = false;

        CheckARObjects();
    }

    private void CheckARObjects()
    {
        if (transform.childCount > 0)
        {
            referenciaARTrackedImage = new List<ARTrackedImageData>();

            for (int i = 0; i < transform.childCount; i++)
            {
                ARTrackedImageData newARTrackedImage = new ARTrackedImageData()
                {
                    trackedImageName = transform.GetChild(i).gameObject.name,
                    trackedImageObject = transform.GetChild(i).gameObject,
                    trackedImageMarkerObject = transform.GetChild(i).GetComponentInChildren<SpriteRenderer>(true).gameObject,
                    lastTrackingState = TrackingState.None,
                };

                newARTrackedImage.trackedImageObject.SetActive(false);
                newARTrackedImage.trackedImageMarkerObject.SetActive(showMarkers);

                referenciaARTrackedImage.Add(newARTrackedImage);
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        OnARImageTracked_Added += ARImageTrackerController_OnARImageTracked_Added;
        OnARImageTracked_Updated += ARImageTrackerController_OnARImageTracked_Updated;
        OnARImageTracked_Removed += ARImageTrackerController_OnARImageTracked_Removed;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        OnARImageTracked_Added -= ARImageTrackerController_OnARImageTracked_Added;
        OnARImageTracked_Updated -= ARImageTrackerController_OnARImageTracked_Updated;
        OnARImageTracked_Removed -= ARImageTrackerController_OnARImageTracked_Removed;
    }

    private void ARImageTrackerController_OnARImageTracked_Added(object sender, ARTrackedImage addedImage)
    {
        string addedImageName = addedImage.referenceImage.name;

        ARTrackedImageData aRTrackedImageData = referenciaARTrackedImage.Find((ar) => ar.trackedImageName == addedImageName);

        if (aRTrackedImageData == null)
        {
            textoFeedback.SetText($"{addedImageName} não existe");
            return;
        }

        Readjust(addedImage.transform, aRTrackedImageData);

        aRTrackedImageData.trackedImageObject.SetActive(true);
        aRTrackedImageData.lastTrackingState = addedImage.trackingState;
    }
    private void ARImageTrackerController_OnARImageTracked_Updated(object sender, ARTrackedImage updatedImage)
    {
        string addedImageName = updatedImage.referenceImage.name;

        ARTrackedImageData aRTrackedImageData = referenciaARTrackedImage.Find((ar) => ar.trackedImageName == addedImageName);

        if (aRTrackedImageData == null)
        {
            textoFeedback.SetText($"{addedImageName} não existe");
            return;
        }

        // Se antes o estado era "TRACKING" e agora é "LIMITED", quer dizer que ENCONTROU um AR Code
        if (updatedImage.trackingState == TrackingState.Tracking && aRTrackedImageData.lastTrackingState != TrackingState.Tracking)
        {
            textoFeedback.SetText($"{addedImageName}");

            aRTrackedImageData.trackedImageObject.SetActive(true);
            Readjust(updatedImage.transform, aRTrackedImageData);
        }

        // Se antes o estado era "LIMITED" e agora é "TRACKING", quer dizer que NÃO ENCONTROU um AR Code
        if (updatedImage.trackingState != TrackingState.Tracking && aRTrackedImageData.lastTrackingState == TrackingState.Tracking)
        {
            aRTrackedImageData.trackedImageObject.SetActive(false);
            Readjust(transform, aRTrackedImageData);
        }

        aRTrackedImageData.lastTrackingState = updatedImage.trackingState;
    }
    private void ARImageTrackerController_OnARImageTracked_Removed(object sender, ARTrackedImage removedImage)
    {
        string addedImageName = removedImage.referenceImage.name;

        ARTrackedImageData aRTrackedImageData = referenciaARTrackedImage.Find((ar) => ar.trackedImageName == addedImageName);

        if (aRTrackedImageData == null)
        {
            textoFeedback.SetText($"{addedImageName} não existe");
            return;
        }

        Readjust(transform, aRTrackedImageData);

        aRTrackedImageData.trackedImageObject.SetActive(false);
        aRTrackedImageData.lastTrackingState = removedImage.trackingState;
    }

    private void Readjust(Transform parent, ARTrackedImageData aRTrackedImageData)
    {
        if (aRTrackedImageData.trackedImageObject.transform == parent) return;

        // Armazena as informações antigas
        Vector3 oldPosition = aRTrackedImageData.trackedImageObject.transform.localPosition;
        Vector3 oldRotation = aRTrackedImageData.trackedImageObject.transform.localEulerAngles;

        // Coloca o modelo para seguir o marcador
        aRTrackedImageData.trackedImageObject.transform.SetParent(parent);

        // Reajusta a posição e rotação
        aRTrackedImageData.trackedImageObject.transform.localPosition = oldPosition;
        aRTrackedImageData.trackedImageObject.transform.localEulerAngles = oldRotation;
    }
}

[System.Serializable]
public class ARTrackedImageData
{
    public string trackedImageName;
    public GameObject trackedImageObject;
    public GameObject trackedImageMarkerObject;
    public TrackingState lastTrackingState;
}
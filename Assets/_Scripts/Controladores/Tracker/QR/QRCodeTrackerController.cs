using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using static SceneController;

public class QRCodeTrackerController : MonoBehaviour
{
    [Header("Configurações QR Tracker Controller")]
    [SerializeField] private RawImage cameraDisplay;
    [SerializeField] private AspectRatioFitter cameraAspectRatioFitter;
    [SerializeField] private TMP_Text textoFeedback;
    [Space(10)]
    [SerializeField] private Button QRCodeButton;
    [SerializeField] private Animator QRCodeButtonAnimator;

    [Header("QR Codes")]
    [ShowInInspector, ReadOnly] private Dictionary<string, Scenes> allQRCodes = new Dictionary<string, Scenes>();
    [SerializeField] private string qrCodesCSVFileName;

    private WebCamTexture webcamTexture;
    private BarcodeReader barcodeReader;
    private Color32[] pixels;

    [SerializeField] private string currentQRCode;

    public bool scanAtivo;

    private void Awake()
    {
        scanAtivo = false;

        while (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Camera))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.Camera);
        }
    }

    private void Start()
    {
        CSVReader.ConvertCSVIntoDictionary(qrCodesCSVFileName, (s) =>
        {
            Scenes convertedScene = (Scenes)int.Parse(s);

            return convertedScene;
        }, out allQRCodes);

        PlayerPrefs.DeleteAll();

        VerificarDisponibilidadeDeCamera();
    }

    private void OnEnable()
    {
        QRCodeButton.onClick.AddListener(OnQRCodeButtonClick);
    }

    private void OnDisable()
    {
        QRCodeButton.onClick.RemoveListener(OnQRCodeButtonClick);
    }

    private void Update()
    {
        ScanArea();

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Return))
            OnQRCodeButtonClick();
#endif
    }

    public void VerificarDisponibilidadeDeCamera()
    {
        WebCamDevice[] camerasDispositivo = WebCamTexture.devices;

        //Checar se possui câmera
        if (camerasDispositivo.Length == 0)
            return;

        webcamTexture = new WebCamTexture(Screen.width, Screen.height);
        cameraDisplay.texture = webcamTexture;
        webcamTexture.Play(); //Renderiza a imagem da camera do dispositivo

        barcodeReader = new BarcodeReader();
    }

    private void ScanArea()
    {
        if (webcamTexture == null || !webcamTexture.isPlaying || !scanAtivo) return;

        try
        {
            pixels = webcamTexture.GetPixels32();

            int width = webcamTexture.width;
            int height = webcamTexture.height;

            // decode the current frame
            var result = barcodeReader.Decode(pixels, width, height);

            if (result != null)
            {
                // Verificar se é um qr code válido para o projeto
                if (allQRCodes.ContainsKey(result.Text))
                {
                    if (currentQRCode != result.Text)
                        Vibrator.Vibrate(25);

                    ShowQRCodeButton();

                    currentQRCode = result.Text;
                    textoFeedback.SetText(result.Text);
                }
                else
                {
                    throw new Exception($"Esse QR Code: [{result.Text}] não existe na lista definida");
                }
            }
        }
        catch (Exception ex)
        {
            if (currentQRCode == string.Empty) return;

            currentQRCode = string.Empty;
            textoFeedback.SetText(ex.Message);

            HideQRCodeButton();
        }
    }

    private void ShowQRCodeButton()
    {
        QRCodeButtonAnimator.SetBool("Tracking", true);
    }
    private void HideQRCodeButton()
    {
        QRCodeButtonAnimator.SetBool("Tracking", false);
    }
    private void OnQRCodeButtonClick()
    {
        if (currentQRCode == string.Empty) return;

        SceneController.Instance.afterSceneTransition += () =>
        {
            MinigamesController.Instance.FindMinigame(currentQRCode);

            if (AudioController.Instance != null)
                AudioController.Instance.PlayMusic();

            return true;
        };

        allQRCodes.TryGetValue(currentQRCode, out Scenes sceneToGo);

        Debug.Log($"Scene to go: {sceneToGo}");

        SceneController.Instance.LoadScene(sceneToGo);
    }
}
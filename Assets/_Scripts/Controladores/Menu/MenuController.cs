using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoSingleton<MenuController>
{
    [SerializeField] private Button ARButton;
    [SerializeField] private Button QRButton;

    protected override void InitializeBehaviour()
    {
        Application.targetFrameRate = 120;

        ARButton.onClick.RemoveAllListeners();
        QRButton.onClick.RemoveAllListeners();

        ARButton.onClick.AddListener(OnARButtonClick);
        QRButton.onClick.AddListener(OnQRButtonClick);
    }

    private void OnARButtonClick()
    {
        SceneController.Instance.LoadScene(SceneController.Scenes._02_TelaAR);
    }
    private void OnQRButtonClick()
    {
        SceneController.Instance.LoadScene(SceneController.Scenes._03_TelaQR);
    }
}
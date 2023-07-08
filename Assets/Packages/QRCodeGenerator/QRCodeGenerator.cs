using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

public class QRCodeGenerator
{
    public static Sprite CreateQRCode(string text)
    {
        Texture2D texture = new Texture2D(256, 256);
        Color32[] color32 = Encode(text, texture.width, texture.height);
        texture.SetPixels32(color32);
        texture.Apply();

        Rect rect = new Rect(0, 0, texture.width, texture.height);
        Sprite newQrCode = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));

        return newQrCode;
    }
    public static string ReadQRCode(Texture2D qrCodeContainer)
    {
        BarcodeReader reader = new BarcodeReader();
        Result result = reader.Decode(qrCodeContainer.GetPixels32(), qrCodeContainer.width, qrCodeContainer.height);

        return result.Text;
    }

    private static Color32[] Encode(string textForEncoding, int width, int height)
    {
        BarcodeWriter writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }
}

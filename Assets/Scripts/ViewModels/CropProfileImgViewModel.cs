using ImageCropperNamespace;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CropProfileImgViewModel : ViewModel
{
    [Space]
    [Header("Crop Profile Object References and Settings")]
    [SerializeField] private float m_minAspectRatio, m_maxAspectRatio = 1.0f;
    [SerializeField] private bool m_ovalSelectionInput, m_autoZoomInput = true;
    [SerializeField] private Image m_avatarImage;
    [SerializeField] private Image m_spriteRefAvatarImage;

    public override void Initialize(params object[] list)
    {
        //Sprite sprite = (Sprite)list[0]
        m_avatarImage.sprite = (Sprite)list[0];
        var aspectFitter = m_avatarImage.GetComponent<AspectRatioFitter>();
        if (!aspectFitter)
            aspectFitter = m_avatarImage.gameObject.AddComponent<AspectRatioFitter>();

        aspectFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        aspectFitter.aspectRatio = (float)m_avatarImage.sprite.texture.width / m_avatarImage.sprite.texture.height;
        OnCropButton_OnClick();

        //FitImageToScreen(m_avatarImage, m_avatarImage.sprite.texture.width, sprite.texture.height);
    }

    public void OnExitButton_OnClick()
    {
        NewScreenManager.instance.BackToPreviousView();
    }

    public void OnCropButton_OnClick()
    {
        if (ImageCropper.Instance.IsOpen)
            return;

        StartCoroutine(CR_Crop());
    }

    private IEnumerator CR_Crop()
    {
        
        yield return new WaitForEndOfFrame();

        bool ovalSelection = m_ovalSelectionInput;
        bool autoZoom = m_autoZoomInput;

        float minAspectRatio, maxAspectRatio;
        minAspectRatio = m_minAspectRatio;
        maxAspectRatio = m_maxAspectRatio;

        Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenshot.Apply();

        ImageCropper.Instance.Show(screenshot, (bool result, Texture originalImage, Texture2D croppedImage) =>
        {
            NewScreenManager.instance.ShowLoadingScreen(true);
            // If screenshot was cropped successfully
            if (result)
            {
                m_spriteRefAvatarImage.sprite = Sprite.Create(croppedImage, new Rect(0, 0, croppedImage.width, croppedImage.height), new Vector2(0.5f, 0.5f));
                string base64Image = System.Convert.ToBase64String(SpriteToByteArray(m_spriteRefAvatarImage.sprite));
                UploadImageRequest uploadData = new UploadImageRequest();
                uploadData.image = "data:image/png;base64," + base64Image;
                ApiManager.instance.UpdateUsersImage(uploadData, (object[] response) =>
                {
                    long responseCode = (long)response[0];
                    string responseText = response[1].ToString();
                    if (responseCode == 200)
                    {
                        ImageResponse imageResponse = JsonUtility.FromJson<ImageResponse>(responseText);
                        ApiManager.instance.UpdateImageLocally(imageResponse.image);
                        NewScreenManager.instance.BackToPreviousView();
                        ((ProfileViewModel)NewScreenManager.instance.GetCurrentView()).SetInfo();
                    }
                    else
                    {
                        NewScreenManager.instance.BackToPreviousView();
                    }

                    NewScreenManager.instance.ShowLoadingScreen(false);
                });
            }
            else
            {
                NewScreenManager.instance.BackToPreviousView();
            }

            // Destroy the screenshot as we no longer need it in this case
            Destroy(screenshot);
        },
        settings: new ImageCropper.Settings()
        {
            ovalSelection = ovalSelection,
            autoZoomEnabled = autoZoom,
            imageBackground = Color.clear, // transparent background
            selectionMinAspectRatio = minAspectRatio,
            selectionMaxAspectRatio = maxAspectRatio,
            markTextureNonReadable = false
        },
        croppedImageResizePolicy: (ref int width, ref int height) =>
        {
            // uncomment lines below to save cropped image at half resolution
            //width /= 2;
            //height /= 2;
        });
    }

    private static byte[] SpriteToByteArray(Sprite sprite)
    {
        if (sprite == null)
        {
            return null;
        }
        
        Texture2D texture = sprite.texture;

        Texture2D croppedTexture = new Texture2D(
            (int)sprite.rect.width,
            (int)sprite.rect.height,
            TextureFormat.RGBA32,
            false
        );

        Color[] pixels = texture.GetPixels(
            (int)sprite.rect.x,
            (int)sprite.rect.y,
            (int)sprite.rect.width,
            (int)sprite.rect.height
        );
        croppedTexture.SetPixels(pixels);
        croppedTexture.Apply();

        byte[] bytes = croppedTexture.EncodeToPNG();
        
        UnityEngine.Object.Destroy(croppedTexture);

        return bytes;
    }

    /*private void FitImageToScreen(Image image, float texWidth, float texHeight)
    {
        RectTransform rt = image.rectTransform;

        // Get canvas scaling reference
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        float referenceHeight = scaler.referenceResolution.y;
        float referenceWidth = scaler.referenceResolution.x;

        // Get the aspect ratios
        float textureAspect = texWidth / texHeight;
        float screenAspect = referenceWidth / referenceHeight;

        // Start with full size
        float targetWidth = referenceWidth;
        float targetHeight = referenceHeight;

        // Adjust to maintain aspect ratio within screen
        if (textureAspect > screenAspect)
        {
            // Image is wider � limit by width
            targetHeight = targetWidth / textureAspect;
        }
        else
        {
            // Image is taller � limit by height
            targetWidth = targetHeight * textureAspect;
        }

        // Apply size delta
        rt.sizeDelta = new Vector2(targetWidth, targetHeight);
        rt.anchoredPosition = Vector2.zero; // Center it (optional)
    }*/
}

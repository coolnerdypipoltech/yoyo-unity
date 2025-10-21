using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
public class ProfileViewModel : ViewModel
{
    public User currentUser;
    public TextMeshProUGUI nameValueText, phoneValueText, emailValueText, idValueText, drinkValueText, foodValueText, musicValueText, pointsValueText;
    public GameObject profileDefault;
    public Image avatarImage;

    void OnEnable()
    {
        SetInfo();
    }

    public void SetInfo()
    {
        currentUser = ApiManager.instance.GetUser();
        if (currentUser != null)
        {
            nameValueText.text = currentUser.name;
            phoneValueText.text = currentUser.related.phone;
            emailValueText.text = currentUser.email;
            idValueText.text = "ID: " + currentUser.id.ToString();
            drinkValueText.text = currentUser.related.taste_drink;
            foodValueText.text = currentUser.related.taste_food;
            musicValueText.text = currentUser.related.taste_music;
            pointsValueText.text = currentUser.related.points.ToString() + " points";
            if (currentUser.related.image != null)
            {
                if (currentUser.related.image.absolute_url != null && currentUser.related.image.absolute_url != "")
                {
                    avatarImage.gameObject.SetActive(true);
                    profileDefault.SetActive(false);
                    ApiManager.instance.SetImageFromUrl(currentUser.related.image.absolute_url, (Sprite response) =>
                    {
                        avatarImage.sprite = response;
                    });
                }
                else
                {
                    profileDefault.SetActive(true);
                    avatarImage.gameObject.SetActive(false);
                }

            }
            else
            {
                profileDefault.SetActive(true);
                avatarImage.gameObject.SetActive(false);
            }
        }
    }
    
    public void OnClickEdit(GameObject menu)
    {
        if(avatarImage.sprite != null && avatarImage.gameObject.activeSelf)
            menu.SetActive(true);
        else
            ShowMediaPicker();
    }

    public void deleteAvatar()
    {
        UploadImageRequest uploadData = new UploadImageRequest();
        uploadData.image = "";
        ApiManager.instance.UpdateUsersImage(uploadData, (object[] response) =>
            {
                long responseCode = (long)response[0];
                string responseText = response[1].ToString();
                if (responseCode == 200)
                {
                    ImageResponse imageResponse = JsonUtility.FromJson<ImageResponse>(responseText);
                    ApiManager.instance.UpdateImageLocally(imageResponse.image);
                    avatarImage.gameObject.SetActive(false);
                    profileDefault.SetActive(true);
                }
                NewScreenManager.instance.ShowLoadingScreen(false);
            });
    }
    

    public void ShowMediaPicker()
    {
        NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                // Create Texture from selected image

                Texture2D texture = NativeGallery.LoadImageAtPath(path, 1000);
                texture = MakeTextureReadable(texture);
                if (texture == null)
                {
                    return;
                }

                avatarImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                string base64Image = System.Convert.ToBase64String(SpriteToByteArray(avatarImage.sprite));
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
                            avatarImage.gameObject.SetActive(true);
                            profileDefault.SetActive(false);  
                        }
                        else
                        {
                            profileDefault.SetActive(true);
                            avatarImage.gameObject.SetActive(false);
                        }
                        NewScreenManager.instance.ShowLoadingScreen(false);
                    });

                
            }
        } , "Select a picture", "image/*");
    }
    
    private static Texture2D MakeTextureReadable(Texture2D texture)
    {
        RenderTexture tmp = RenderTexture.GetTemporary(
            texture.width,
            texture.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear
        );

        Graphics.Blit(texture, tmp);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = tmp;

        Texture2D readableTex = new Texture2D(texture.width, texture.height);
        readableTex.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
        readableTex.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(tmp);

        return readableTex;
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

        Object.Destroy(croppedTexture);

        return bytes;
    }

    public void OnClickDeleteAccount()
    {
        NewScreenManager.instance.ChangeToMainView(ViewID.DeleteAccountViewModel, true);
    }

    public void OnClickOpenTastes()
    {
        NewScreenManager.instance.ChangeToMainView(ViewID.EditTastesViewModel, true);
        NewScreenManager.instance.GetCurrentView().GetComponent<EditTastesViewModel>().OnSetup(currentUser);
    }

    public void OnClickOpenProfile()
    {
        NewScreenManager.instance.ChangeToMainView(ViewID.EditProfileViewModel, true);
        NewScreenManager.instance.GetCurrentView().GetComponent<EditProfileViewModel>().OnSetup(currentUser);
    }

}

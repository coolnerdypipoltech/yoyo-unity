using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;
using UnityEngine.Video;


public class ApiManager : MonoBehaviour
{
    private static ApiManager _instance;

    public static ApiManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindAnyObjectByType<ApiManager>();
            }
            return _instance;
        }
    }

    public string accessToken = "";
    private User currentUser;
    private static string phoneNumber = "8331021023";
    private static string BASE_API_URL = "http://64.227.105.243/api/v1";
    private static string NEXT_URL = "http://64.227.105.243/api";
    private static string SIGNIN_ENDPOINT = BASE_API_URL + "/auth/signin";
    private static string LOGIN_ENDPOINT = BASE_API_URL + "/auth/login";
    private static string RESET_PASSWORD_ENDPOINT = BASE_API_URL + "/auth/passwords/reset";
    private static string CHECK_ACCESS_CODE_ENDPOINT = BASE_API_URL + "/auth/access-codes/verification";
    private static string GET_REWARDS_ENDPOINT = BASE_API_URL + "/rewards";
    public static string GET_PARTNERS_ENDPOINT = BASE_API_URL + "/partners";
    private static string GET_PLACES_ENDPOINT = BASE_API_URL + "/consumption-centers";
    private static string GET_EVENTS_ENDPOINT = BASE_API_URL + "/events";
    private static string DELETE_USER_ENDPOINT = BASE_API_URL + "/auth";
    private static string UPDATE_USER_ENDPOINT = BASE_API_URL + "/auth/info";
    private static string GET_ADVERTISEMENTS_ENDPOINT = BASE_API_URL + "/advertisements";
    private static string UPDATE_POINTS_ENDPOINT = BASE_API_URL + "/auth/points";
    private static string UPLOAD_IMAGE_ENDPOINT = BASE_API_URL + "/auth/image";

    
    public User GetUser()
    {
        return currentUser;
    }

    public string GetUserId()
    {
        return currentUser.id.ToString().PadLeft(6, '0');
    }

    public void GenerateWhatsAppMessage(string message)
    {
        string whatsappURL = $"https://api.whatsapp.com/send?phone=+52{phoneNumber}&text={message}";
        Application.OpenURL(whatsappURL);
    }

    //sets the current user and updates their information on the server

    public void SetUser(User user)
    {
        currentUser = user;
        UpdateUserRequest updateData = new UpdateUserRequest
        {
            id = user.id.ToString(),
            name = user.name,
            age = user.related.age,
            gender = user.related.gender,
            phone = user.related.phone,
            pronouns = user.related.pronouns,
            taste_drink = user.related.taste_drink,
            taste_music = user.related.taste_music,
            taste_food = user.related.taste_food
        };
        UpdateUser(updateData, accessToken, (response) =>
        {
            long responseCode = (long)response[0];
            string responseText = response[1].ToString();

            if (responseCode == 200)
            {
                UpdateUserResponse updateResponse = JsonUtility.FromJson<UpdateUserResponse>(responseText);
            }
        });
    }

    public void UpdateImageLocally(Gallery _image)
    {
        currentUser.related.image = _image;
    }

    //API Methods

    public void SignIn(SignInRequest signInData, Action<object[]> callback)
    {
        string jsonData = JsonUtility.ToJson(signInData);
        StartCoroutine(MakePostRequest(SIGNIN_ENDPOINT, jsonData, callback, false));
    }

    public int GetUsersPoints()
    {
        return currentUser.related.points;
    }

    public void UpdateUsersPoints(Action<object[]> callback)
    {
        StartCoroutine(MakeGetRequest(UPDATE_POINTS_ENDPOINT, (response) =>
        {
            long responseCode = (long)response[0];
            string responseText = response[1].ToString();

            if (responseCode == 200)
            {
                PointsResult pointsResponse = JsonUtility.FromJson<PointsResult>(responseText);
                currentUser.related.points = pointsResponse.points;
                currentUser.related.total_points = pointsResponse.total_points;
            }
            callback(response);
        }, accessToken));

    }
    

    public void LogIn(LoginRequest loginData, Action<object[]> callback)
    {
        string jsonData = JsonUtility.ToJson(loginData);
        StartCoroutine(MakePostRequest(LOGIN_ENDPOINT, jsonData, (response) =>
        {
            long responseCode = (long)response[0];
            string responseText = response[1].ToString();

            if (responseCode == 200)
            {
                LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(responseText);
                accessToken = loginResponse.access_token;
                currentUser = loginResponse.user;
            }
            callback(response);
        }, false));
    }

    public void ResetPassword(ResetPasswordRequest resetData, Action<object[]> callback)
    {
        string jsonData = JsonUtility.ToJson(resetData);
        StartCoroutine(MakePostRequest(RESET_PASSWORD_ENDPOINT, jsonData, callback, false));
    }

    public void UpdateUsersImage(UploadImageRequest uploadData, Action<object[]> callback)
    {
        string jsonData = JsonUtility.ToJson(uploadData);
        StartCoroutine(MakePostRequest(UPLOAD_IMAGE_ENDPOINT, jsonData, callback, true, accessToken));
    }

    public void CheckAccessCode(CheckAccessCodeRequest accessCodeData, Action<object[]> callback)
    {
        string jsonData = JsonUtility.ToJson(accessCodeData);
        StartCoroutine(MakePostRequest(CHECK_ACCESS_CODE_ENDPOINT, jsonData, callback, false));
    }

    public void GetRewards(int limit, int offset, Action<object[]> callback)
    {
        string endpoint = $"{GET_REWARDS_ENDPOINT}/{limit}/{offset}";
        StartCoroutine(MakeGetRequest(endpoint, callback, accessToken));
    }

    public void GetAdvertisements(Action<object[]> callback)
    {
        string endpoint = $"{GET_ADVERTISEMENTS_ENDPOINT}";
        StartCoroutine(MakeGetRequest(endpoint, callback, accessToken));
    }

    public void GetMoreRewards(string nextUrl, Action<object[]> callback)
    {
        string endpoint = NEXT_URL + nextUrl;
        StartCoroutine(MakeGetRequest(endpoint, callback, accessToken));
    }

    public void GetPlaces(int limit, int offset, Action<object[]> callback)
    {
        string endpoint = $"{GET_PLACES_ENDPOINT}/{limit}/{offset}";
        StartCoroutine(MakeGetRequest(endpoint, callback, accessToken));
    }

    public void GetMorePlaces(string nextUrl, Action<object[]> callback)
    {
        string endpoint = NEXT_URL + nextUrl;

        StartCoroutine(MakeGetRequest(endpoint, callback, accessToken));
    }

    public void GetEvents(int limit, int offset, Action<object[]> callback)
    {
        string endpoint = $"{GET_EVENTS_ENDPOINT}/{limit}/{offset}";
        StartCoroutine(MakeGetRequest(endpoint, callback, accessToken));
    }

    public void GetMoreEvents(string nextUrl, Action<object[]> callback)
    {
        string endpoint = NEXT_URL + nextUrl;

        StartCoroutine(MakeGetRequest(endpoint, callback, accessToken));
    }

    public void GetPartners(int limit, int offset, Action<object[]> callback)
    {
        string endpoint = $"{GET_PARTNERS_ENDPOINT}/{limit}/{offset}";
        StartCoroutine(MakeGetRequest(endpoint, callback, accessToken));
    }

    public void GetMorePartners(string nextUrl, Action<object[]> callback)
    {
        string endpoint = NEXT_URL + nextUrl;
        StartCoroutine(MakeGetRequest(endpoint, callback, accessToken));
    }

    public void DeleteUser(DeleteUserRequest deleteData, Action<object[]> callback)
    {
        string jsonData = JsonUtility.ToJson(deleteData);
        StartCoroutine(MakeDeleteRequest(DELETE_USER_ENDPOINT, jsonData, callback, accessToken));
    }

    public void UpdateUser(UpdateUserRequest updateData, string token, Action<object[]> callback)
    {
        string jsonData = JsonUtility.ToJson(updateData);
        StartCoroutine(MakePutRequest(UPDATE_USER_ENDPOINT, jsonData, callback, token));
    }

    public void SetImageFromUrl(string url, Action<Sprite> callback)
    {
        StartCoroutine(LoadSpriteFromUrl(url, callback));
    }
    
    #region Private Helper Methods

    private IEnumerator LoadSpriteFromUrl(string url, Action<Sprite> callback)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                callback?.Invoke(null);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                Sprite sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );
                callback?.Invoke(sprite);
            }
        }
    }

    private string ConvertJsonToQueryParams(string jsonData)
    {
        if (string.IsNullOrEmpty(jsonData))
            return "";

        try
        {
            string cleanedJson = jsonData.Trim().TrimStart('{').TrimEnd('}');
            if (string.IsNullOrEmpty(cleanedJson))
                return "";

            var queryParams = new List<string>();
            string[] pairs = cleanedJson.Split(',');

            foreach (string pair in pairs)
            {
                if (string.IsNullOrEmpty(pair.Trim()))
                    continue;

                string[] keyValue = pair.Split(':');
                if (keyValue.Length == 2)
                {
                    string key = keyValue[0].Trim().Trim('"');
                    string value = keyValue[1].Trim().Trim('"');

                    if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                    {
                        string encodedKey = UnityEngine.Networking.UnityWebRequest.EscapeURL(key);
                        string encodedValue = UnityEngine.Networking.UnityWebRequest.EscapeURL(value);
                        queryParams.Add($"{encodedKey}={encodedValue}");
                    }
                }
            }

            return string.Join("&", queryParams);
        }
        catch (Exception e)
        {
            return "";
        }
    }

    private IEnumerator MakePostRequest(string endpoint, string jsonData, Action<object[]> callback, bool requiresAuth, string token = "")
    {
        using (UnityEngine.Networking.UnityWebRequest request = new UnityEngine.Networking.UnityWebRequest(endpoint, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");

            if (requiresAuth && !string.IsNullOrEmpty(token))
            {
                request.SetRequestHeader("Authorization", "Bearer " + token);
            }

            yield return request.SendWebRequest();

            HandleResponse(request, callback);
        }
    }

    private IEnumerator MakeGetRequest(string endpoint, Action<object[]> callback, string token = "")
    {
        using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(endpoint))
        {
            request.SetRequestHeader("Content-Type", "application/json");

            if (!string.IsNullOrEmpty(token))
            {
                request.SetRequestHeader("Authorization", "Bearer " + token);
            }

            yield return request.SendWebRequest();

            HandleResponse(request, callback);
        }
    }

    private IEnumerator MakeDeleteRequest(string endpoint, string jsonData, Action<object[]> callback, string token = "")
    {
        using (UnityEngine.Networking.UnityWebRequest request = new UnityEngine.Networking.UnityWebRequest(endpoint, "DELETE"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + token);

            yield return request.SendWebRequest();

            HandleResponse(request, callback);
        }
    }

    private IEnumerator MakePutRequest(string endpoint, string jsonData, Action<object[]> callback, string token = "")
    {

        string queryParams = ConvertJsonToQueryParams(jsonData);
        string fullEndpoint = string.IsNullOrEmpty(queryParams) ? endpoint : $"{endpoint}?{queryParams}";

        using (UnityEngine.Networking.UnityWebRequest request = new UnityEngine.Networking.UnityWebRequest(fullEndpoint, "PUT"))
        {
            request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + token);

            yield return request.SendWebRequest();

            HandleResponse(request, callback);
        }
    }

    private void HandleResponse(UnityEngine.Networking.UnityWebRequest request, Action<object[]> callback)
    {
        long responseCode = request.responseCode;

        if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            string responseText = request.downloadHandler.text;
            callback(new object[] { responseCode, responseText });
        }
        else
        {
            string errorText = request.error;
            callback(new object[] { responseCode, errorText });
        }
    }

    #endregion
}

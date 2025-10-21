using UnityEngine;
using TMPro;

public class LogInViewModel : ViewModel
{
    public TMP_InputField emailInput, passwordInput;
    public GameObject errorMessage, validateEmailMessage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDisable()
    {
        errorMessage.SetActive(false);
        emailInput.text = "";
        passwordInput.text = "";
        validateEmailMessage.SetActive(false);
    }


    public void OnClickShowPassword()
    {
        if (passwordInput.contentType == TMP_InputField.ContentType.Password)
        {
            passwordInput.contentType = TMP_InputField.ContentType.Standard;
            passwordInput.ForceLabelUpdate();
        }
        else
        {
            passwordInput.contentType = TMP_InputField.ContentType.Password;
            passwordInput.ForceLabelUpdate();
        }
    }

    public void showValidateEmailMessage()
    {
        validateEmailMessage.SetActive(true);
    }

    public void OnClickForgotPassword()
    {
        NewScreenManager.instance.ChangeToMainView(ViewID.PasswordViewModel, true);
    }
    
    public void OnClickLogIn()
    {
        if (string.IsNullOrEmpty(emailInput.text) || string.IsNullOrEmpty(passwordInput.text))
        {
            errorMessage.SetActive(true);
            return;
        }
        LoginRequest loginData = new LoginRequest
        {
            email = emailInput.text,
            password = passwordInput.text
        };
        NewScreenManager.instance.ShowLoadingScreen(true);

        ApiManager.instance.LogIn(loginData, (response) =>
        {
            long responseCode = (long)response[0];
            string responseText = response[1].ToString();
            NewScreenManager.instance.ShowLoadingScreen(false);
            if (responseCode == 200)
            {
                NewScreenManager.instance.ChangeToMainView(ViewID.PlacesViewModel, false);
                LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(responseText);
                emailInput.text = "";
                passwordInput.text = "";

            }
            else if (responseCode == 401)
            {
                errorMessage.SetActive(true);
                ErrorResponse errorResponse = JsonUtility.FromJson<ErrorResponse>(responseText);

            }
            else
            {
                errorMessage.SetActive(true);
            }
            
        });
    }
}

using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using Unity.VisualScripting;

public class RegisterViewModel : ViewModel
{
    public TMP_InputField emailInput, passwordInput, confirmPasswordInput, pronounsInput, ageInput, phoneInput, nameInput, accessCodeInput;
    public Boolean termsAccepted = false;
    public TextMeshProUGUI countryCode;
    public string Gender;
    public GameObject errorMessage, passwordErrorMessage, termsAcceptedGraphic, spacerErrorText;
    public RectTransform formContent;

    public ScrollRect scrollRect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnValueChanged_Gender(int index)
    {
        switch (index)
        {
            case 0:
                Gender = "Women";
                break;
            case 1:
                Gender = "Men";
                break;
            case 2:
                Gender = "I prefer not to say it";
                break;
        }
    }

    public void OnClickGoToLogin()
    {
        NewScreenManager.instance.BackToPreviousView();
        NewScreenManager.instance.ChangeToMainView(ViewID.LogInViewModel, true);
    }

    public void SetCode(string code)
    {
        accessCodeInput.text = code;
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

    public void OnClickShowPasswordConfirm()
    {
        if (confirmPasswordInput.contentType == TMP_InputField.ContentType.Password)
        {
            confirmPasswordInput.contentType = TMP_InputField.ContentType.Standard;
            confirmPasswordInput.ForceLabelUpdate();
        }
        else
        {
            confirmPasswordInput.contentType = TMP_InputField.ContentType.Password;
            confirmPasswordInput.ForceLabelUpdate();
        }
    }

    void OnDisable()
    {
        errorMessage.SetActive(false);
        spacerErrorText.SetActive(false);
        errorMessage.GetComponentInChildren<TextMeshProUGUI>().text = "";
        emailInput.text = ""; passwordInput.text = ""; ; confirmPasswordInput.text = "";
        pronounsInput.text = ""; ageInput.text = ""; phoneInput.text = ""; nameInput.text = ""; accessCodeInput.text = "";
    }

    public void OnValueChanged_TermsAccepted(bool value)
    {
        termsAccepted = value;
        termsAcceptedGraphic.SetActive(value);
    }

    private bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
    }

    private void ProcessErrorText(string errorText)
    {
        errorMessage.SetActive(true);
        spacerErrorText.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(formContent);
        if (errorText.Contains("email"))
        {
            errorMessage.GetComponentInChildren<TextMeshProUGUI>().text = "Invalid email format.";
        }
        if (errorText.Contains("code"))
        {
            errorMessage.GetComponentInChildren<TextMeshProUGUI>().text = "Invalid code";
        }
        else if (errorText.Contains("age"))
        {
            errorMessage.GetComponentInChildren<TextMeshProUGUI>().text = "Age must be greater than 18.";
        }
        else if (errorText.Contains("password"))
        {
            errorMessage.GetComponentInChildren<TextMeshProUGUI>().text = "Password must be at least 8 characters long and include uppercase, lowercase, digit, and special character.";
        }
        else if (errorText.Contains("terms"))
        {
            errorMessage.GetComponentInChildren<TextMeshProUGUI>().text = "Please accept the terms and conditions.";
        }
        else if (errorText.Contains("matchKeys"))
        {
            errorMessage.GetComponentInChildren<TextMeshProUGUI>().text = "Passwords do not match.";
        }
        else if (errorText.Contains("api.error.already_exists"))
        {
            errorMessage.GetComponentInChildren<TextMeshProUGUI>().text = "An account with this email already exists.";
        }
        else
        {
            errorMessage.GetComponentInChildren<TextMeshProUGUI>().text = "Please make sure all fields are completed.";
        }
    }

    private bool CheckAllFields()
    {
        bool allFieldsFilled = true;
        string fields = "";
        if (string.IsNullOrEmpty(emailInput.text))
        {
            allFieldsFilled = false;
            fields += "email, ";
        }

        if (string.IsNullOrEmpty(passwordInput.text) || string.IsNullOrEmpty(confirmPasswordInput.text))
        {
            allFieldsFilled = false;
            fields += "password, ";
        }
        if (string.IsNullOrEmpty(ageInput.text))
        {
            allFieldsFilled = false;
            fields += "age, ";
        }
        if (string.IsNullOrEmpty(nameInput.text))
        {
            allFieldsFilled = false;
            fields += "name, ";
        }
        if (string.IsNullOrEmpty(accessCodeInput.text))
        {
            allFieldsFilled = false;
            fields += "access code, ";
        }
        if (string.IsNullOrEmpty(phoneInput.text))
        {
            allFieldsFilled = false;
            fields += "phone, ";
        }

        if (!allFieldsFilled)
        {
            fields = fields.TrimEnd(' ', ',');
            errorMessage.GetComponentInChildren<TextMeshProUGUI>().text = "Please make sure all fields are completed. Missing fields: " + fields;
        }
        return allFieldsFilled;
    }

    public void OnClickRegister()
    {
        if (!CheckAllFields())
        {
            scrollRect.normalizedPosition = new Vector2(0, 0);
            errorMessage.SetActive(true);
            spacerErrorText.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(formContent);
            
            return;
        }

        if (!IsValidEmail(emailInput.text))
        {
            ProcessErrorText("email");
            scrollRect.normalizedPosition = new Vector2(0, 0);
            return;
        }

        if (!IsValidPassword(passwordInput.text))
        {
            ProcessErrorText("password");
            scrollRect.normalizedPosition = new Vector2(0, 0);
            return;
        }

        if (ageInput.text != null && (int.Parse(ageInput.text) < 18))
        {
            ProcessErrorText("age");
            scrollRect.normalizedPosition = new Vector2(0, 0);
            return;
        }

        if (!termsAccepted)
        {
            ProcessErrorText("terms");
            scrollRect.normalizedPosition = new Vector2(0, 0);
            return;
        }

        if (passwordInput.text != confirmPasswordInput.text)
        {
            ProcessErrorText("matchKeys");
            scrollRect.normalizedPosition = new Vector2(0, 0);
            return;
        }

        SignInRequest signInData = new SignInRequest
        {
            name = nameInput.text,
            email = emailInput.text,
            age = int.Parse(ageInput.text),
            gender = Gender,
            phone = countryCode.text + " " + phoneInput.text,
            password = passwordInput.text,
            points = 0,
            pronouns = pronounsInput.text,
            access_code = accessCodeInput.text
        };
        spacerErrorText.SetActive(false);
        errorMessage.SetActive(false);
        LayoutRebuilder.ForceRebuildLayoutImmediate(formContent);
        NewScreenManager.instance.ShowLoadingScreen(true);
        ApiManager.instance.SignIn(signInData, (response) =>
        {
            long responseCode = (long)response[0];
            string responseText = response[1].ToString();
            if (responseCode == 200)
            {
                NewScreenManager.instance.ChangeToMainView(ViewID.LogInViewModel, true);
                NewScreenManager.instance.GetMainView(ViewID.LogInViewModel).GetComponent<LogInViewModel>().showValidateEmailMessage();

            }

            else if (responseCode == 401)
            {
                ProcessErrorText(responseText);
            }
            else if (responseCode == 400)
            {
                ProcessErrorText("code");
            }
            else
            {
                ProcessErrorText(responseText);
            }
            NewScreenManager.instance.ShowLoadingScreen(false);
        });
    }
}

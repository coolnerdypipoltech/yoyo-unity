using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class EditProfileViewModel : ViewModel
{
    public TMP_InputField phoneInputText;
    private User currentUser;
    public TextMeshProUGUI countryValueText;
    public PhoneSelectorHandler phoneSelectorHandler;
    public GameObject clearPhoneButton;

    public Button SaveButton;
    public void OnSetup(User _currentUser)
    {
        currentUser = _currentUser;
        if (currentUser.related.phone.Length > 0)
        {
            phoneInputText.text = currentUser.related.phone.Split(' ')[1];
            phoneSelectorHandler.searchText = currentUser.related.phone.Split(' ')[0].Replace("(", "").Replace(")", "");
        }
    }

    void OnDisable()
    {
        phoneInputText.text = "";
        clearPhoneButton.SetActive(true);
        SaveButton.interactable = true;
    }

    public void ClearPhoneInput()
    {
        phoneInputText.text = "";
        clearPhoneButton.SetActive(false);
        SaveButton.interactable = false;
    }


    private void UpdateButton()
    {
        if (phoneInputText.text.Length > 9)
        {
            SaveButton.interactable = true;
        }
        else
        {
            SaveButton.interactable = false;
        }
    }

    public void OnChangePhoneInput()
    {
        if (phoneInputText.text.Length > 9)
        {
            clearPhoneButton.SetActive(true);
            UpdateButton();
        }
        else
        {
            clearPhoneButton.SetActive(false);
        }
    }

    public void SaveNewProfileInfo()
    {

        if (phoneInputText.text != "")
        {
            currentUser.related.phone = $"{countryValueText.text} {phoneInputText.text}";
        }
        else
        {
            return;
        }

        NewScreenManager.instance.BackToPreviousView();
        ApiManager.instance.SetUser(currentUser);
        NewScreenManager.instance.GetCurrentView().GetComponent<ProfileViewModel>().SetInfo();
    }
}

using UnityEngine;
using TMPro;
using System;

public class PhoneSelectorHandler : MonoBehaviour
{
    public TextMeshProUGUI phoneInput, countryValue;
    public TMP_Dropdown countryDropdown;
    public string jsonData;
    public string countryCode;

    public string searchText = "";

    void Start()
    {
        CountriesWrapper countriesWrapper = JsonUtility.FromJson<CountriesWrapper>(jsonData);

        countryDropdown.options.Clear();
        foreach (var country in countriesWrapper.countries)
        {
            string optionText = $"{country.name} ({country.code})";
            countryDropdown.options.Add(new TMP_Dropdown.OptionData(optionText));
        }
        if(searchText.Length > 0)
        {
            SearchSpecificCountry(searchText);
        }
        else
        {
            countryDropdown.value = 140;
        }
                
        


    }

    public void SearchSpecificCountry(string searchText)
    {
        for (int i = 0; i < countryDropdown.options.Count; i++)
        {
            string optionText = countryDropdown.options[i].text;

            int startIndex = optionText.IndexOf("(+");
            int endIndex = optionText.IndexOf(")", startIndex);

            if (startIndex != -1 && endIndex != -1)
            {
                string countryCodeFromOption = optionText.Substring(startIndex + 1, endIndex - startIndex - 1);
                if (countryCodeFromOption.Equals(searchText))
                {
                    countryDropdown.value = i;
                    string[] stringParse = countryValue.text.Split('(');
                    countryCode = stringParse[1].Replace(")", "").Trim();
                    phoneInput.text = countryCode;
                    return;
                }
            }
        }
    }

    public void OnValueChanged_Country()
    {
        string[] stringParse = countryValue.text.Split('(');
        countryCode = stringParse[1].Replace(")", "").Trim();
        phoneInput.text = countryCode;
    }

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            string newJson = "{\"Items\":" + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.Items;
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
}
[System.Serializable]
public class CountriesWrapper
{
    public CountryData[] countries;
}


[System.Serializable]
public class CountryData
{
    public string code;
    public string name;
}


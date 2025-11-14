using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class PlaceInterface : MonoBehaviour
{
    public TextMeshProUGUI titleText;

    public ImageInterface placeImage;

    public Place place;

    private bool isFromPlaces;


    public void SetPlace(Place _place, bool _isFromPlaces)
    {
        titleText.text = _place.name;
        place = _place;
        if (_place.media != null && _place.media.Count != 0)
        {
            placeImage.takeOutMask();
            ApiManager.instance.SetImageFromUrl(_place.media[0].absolute_url, (Sprite response) =>
            {
                
                placeImage.setImage(response);
                placeImage.image.preserveAspect = false;
            });
        }
        isFromPlaces = _isFromPlaces;

    }

    public void OnClick()
    {
        NewScreenManager.instance.ChangeToMainView(ViewID.PlacesInfoViewModel, true);
        NewScreenManager.instance.GetCurrentView().GetComponent<PlacesInfoViewModel>().InitializeViewModel(place, isFromPlaces);
    }




}

using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class RewardInterface : MonoBehaviour
{
    public TextMeshProUGUI titleText;

    public ImageInterface placeImage;

    public ResultObject result;
    private bool isFromRewards = false;


    public void SetPlace(ResultObject _result, bool _isFromRewards)
    {
        titleText.text = _result.name;
        isFromRewards = _isFromRewards;
        result = _result;
        if (_result.media != null && _result.media.Length != 0)
        {
            placeImage.takeOutMask();
            ApiManager.instance.SetImageFromUrl(_result.media[0].absolute_url, (Sprite response) =>
            {
                
                placeImage.setImage(response);
                placeImage.image.preserveAspect = false;
            });
        }
    }

    public void OnClick()
    {
        NewScreenManager.instance.ChangeToMainView(ViewID.RewardsInfoViewModel, true);
        NewScreenManager.instance.GetCurrentView().GetComponent<RewardsInfoViewModel>().InitializeViewModel(result, isFromRewards);
    }




}

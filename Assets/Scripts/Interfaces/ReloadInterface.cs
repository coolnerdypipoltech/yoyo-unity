using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class ReloadInterface : MonoBehaviour
{

    private const float BASE_TOP = -25f;
    private const float BASE_BOTTOM = 0f;

    public List<Sprite> topLabels;
    public Image topLabelImage;
    private bool _canLoad = false;
    private bool _isLoading = false;
    private bool _visualsSetAtTheStart = false;

    [Header("Settings")]
    public bool isTopLoader = true;

    [Header("Visuals")]
    public bool visualsActive = true;
    public RectTransform Scroll;

    [Header("Pull coefficient")]
    [Range(-50f, 50f)]
    public float loadMoreObjectsLimiter = 0.15f;
    [Range(-100f, 100f)]
    public float limiterToStartReload = 0.15f;
    private void Start()
    {
        SetVisualsBeforeStartScroll();
    }

    public void OnValueChanged(Vector2 _vector)
    {
        //Debug.Log(Scroll.anchoredPosition.y);
        if (isTopLoader)
        {
            TopLoaderCheck(Scroll.anchoredPosition.y);
        }
        else
        {
            //BottomLoaderCheck(Scroll.anchoredPosition.y);
        }

    }

    private void TopLoaderCheck(float _vector)
    {
        if (_vector < BASE_TOP && !_isLoading && !_visualsSetAtTheStart)
        {
            SetVisualsAtStartScroll();
            _visualsSetAtTheStart = true;
        }

        if (_vector < -20 && _vector >-50)
        {
            topLabelImage.sprite = topLabels[1];
        }

        if (_vector < (BASE_TOP + loadMoreObjectsLimiter))
        {

            if (!_isLoading)
            {
                PrepareReload();
            }
        }
        else
        {
            if (!_isLoading)
            {
                _canLoad = false;
                _visualsSetAtTheStart = false;
            }
        }

        if (!_canLoad) { return; }

        if ((_vector > BASE_TOP + limiterToStartReload) && _canLoad)
        {
            topLabelImage.sprite = topLabels[2];
            Reload();
        }
    }

    private void BottomLoaderCheck(float _vector)
    {
        if (_vector < BASE_BOTTOM && !_isLoading && !_visualsSetAtTheStart)
        {
            SetVisualsAtStartScroll();
            _visualsSetAtTheStart = true;
        }

        Debug.Log(_vector + " " + (BASE_BOTTOM - loadMoreObjectsLimiter));

        if (_vector < (BASE_BOTTOM - loadMoreObjectsLimiter)/2)
        {
            topLabelImage.sprite = topLabels[1];
        }

        if (_vector < (BASE_BOTTOM - loadMoreObjectsLimiter))
        {
            if (!_isLoading)
            {
                PrepareReload();
            }
        }
        else
        {
            if (!_isLoading) _canLoad = false;
        }

        if (!_isLoading)
        {
            _canLoad = false;
            _visualsSetAtTheStart = false;
        }

        if ((_vector > BASE_BOTTOM - limiterToStartReload) && _canLoad)
        {
            Reload();
        }
    }

    public void SetVisualsBeforeStartScroll()
    {
        if (visualsActive && topLabelImage != null)
        {
            topLabelImage.gameObject.SetActive(false);
            topLabelImage.sprite = topLabels[0];
        }
    }

    private void SetVisualsAtStartScroll()
    {
        if (visualsActive && topLabelImage != null)
        {
            topLabelImage.gameObject.SetActive(true);
            topLabelImage.sprite = topLabels[0];
        }
    }

    private void PrepareReload()
    {
        topLabelImage.sprite = topLabels[2];
        _canLoad = true;
        _isLoading = true;
    }

    private void Reload()
    {
        SetVisualsBeforeStartScroll();

        _canLoad = false;
        _isLoading = false;
        _visualsSetAtTheStart = false;

        ViewModel _currentViewModel = NewScreenManager.instance.GetCurrentView().GetComponent<ViewModel>();
        if(_currentViewModel.viewID == ViewID.PlacesViewModel)
            NewScreenManager.instance.GetCurrentView().GetComponent<PlacesViewModel>().ReloadAll();
        else if(_currentViewModel.viewID == ViewID.RewardsViewModel)
            NewScreenManager.instance.GetCurrentView().GetComponent<RewardsViewModel>().ReloadAll();


    }
}

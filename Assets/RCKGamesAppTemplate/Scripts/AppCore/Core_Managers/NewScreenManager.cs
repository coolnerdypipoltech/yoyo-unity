using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NewScreenManager : Manager
{
    private const int MIN_BACK_STACK_COUNT = 1;
    public GameObject loadingScreen;

    public Stack<ViewModel> backViewStack;
    public List<ViewModel> spawnedViewsList;

    [Header("Views Array")]
    [SerializeField]
    private ViewModel[] mainViews = null;

    [Header("Current View")]
    [SerializeField]
    private ViewModel currentView = null;

    [Header("Header View")]
    [SerializeField]
    private ViewModel headerView = null;

    [Header("General Loading Canvas")]
    [SerializeField]
    private GameObject loadingCanvas = null;

    [Header("View Types Data")]
    [SerializeField]
    private SpawnableViewModelTypesScriptableObject viewModelTypesData;

    private bool isCurrentViewSpawned = false;

    public RectTransform spawnedViewsParent;

    public VideoInterface videoInterface;

    private static NewScreenManager _instance;

    public static NewScreenManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<NewScreenManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        backViewStack = new Stack<ViewModel>();
        spawnedViewsList = new List<ViewModel>();
        backViewStack.Push(currentView);
    }

    public void ShowLoadingScreen(bool _value)
    {
        if (_value)
        {
            loadingScreen.SetActive(true);
        }
        else
        {
            loadingScreen.GetComponentInChildren<BunnyInterface>().StopNextCycle();
        }
        
    }

    public void BackToPreviousView()
    {
        if (backViewStack.Count >= MIN_BACK_STACK_COUNT)
        {
            isCurrentViewSpawned = CeckIfCurrentViewSpawned(currentView);

            if (isCurrentViewSpawned)
            {
                RemoveSpawnedViewFromList(currentView);
                Destroy(currentView.gameObject);
            }
            else
            {
                currentView.SetActive(false);
            }

            currentView = backViewStack.Peek();
            currentView.SetActive(true);
            backViewStack.Pop();
            VideoInterfaceMatrix();
        }
    }

    public void ChangeToMainView(ViewID _viewID, bool _isSubMainView = false)
    {
        SetChangeOfMainViews(_viewID, _isSubMainView);
        VideoInterfaceMatrix();
    }

    public void ChangeToSpawnedView(string _SpawnedViewType)
    {
        SetChangeOfSpawnedViews(_SpawnedViewType);
    }

    public ViewModel GetMainView(ViewID viewID)
    {
        foreach (ViewModel viewInstance in mainViews)
        {
            if (((ViewModel)(viewInstance.GetComponent(typeof(ViewModel)))).GetViewID() == viewID)
            {
                return viewInstance;
            }
        }

        return null;
    }

    private void VideoInterfaceMatrix()
    {
        switch (currentView.viewID)
        {
            case ViewID.RewardsViewModel:
                videoInterface.SetClip(3);
                break;
            case ViewID.PlacesViewModel:
                videoInterface.SetClip(2);
                break;
            case ViewID.RegisterViewModel:
                videoInterface.SetClip(1);
                break;
            case ViewID.ProfileViewModel:
                videoInterface.SetClip(4);
                break;
            case ViewID.WelcomeViewModel:
                videoInterface.SetClip(0);
                break;
            default:
                videoInterface.SetClip(0);
                videoInterface.CustomAlpha(0.2f);
                break;
        }
        videoInterface.videoPlayer.Play();
    }
    

    public ViewModel GetCurrentView()
    {
        return currentView;
    }

    public ViewModel GetHeaderView()
    {
        return headerView;
    }

    public void SetHeaderViewActive(bool _value)
    {
        headerView.SetActive(_value);
    }

    public bool HasSubViews()
    {
        if (backViewStack.Count <= 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void SetCurrentViewSiblingIndex(int _value)
    {
        currentView.transform.SetSiblingIndex(_value);
    }

    public void SetCurrentViewAsFirstSibling(int _value)
    {
        currentView.transform.SetAsFirstSibling();
    }

    public void SetCurrentViewAsLastSibling(int _value)
    {
        currentView.transform.SetAsLastSibling();
    }

    public void SetMainViewSiblingIndex(ViewID _id, int _value)
    {
        GetMainView(_id).transform.SetSiblingIndex(_value);
    }

    public void SetMainViewAsFirstSibling(ViewID _id)
    {
        GetMainView(_id).transform.SetAsFirstSibling();
    }

    public void SetMainViewAsLastSiblig(ViewID _id)
    {
        GetMainView(_id).transform.SetAsLastSibling();
    }

    private void SetChangeOfMainViews(ViewID _viewID, bool _isSubView = false)
    {

        if (_isSubView)
        {
            if (_viewID != ((ViewModel)currentView.GetComponent(typeof(ViewModel))).GetViewID())
                backViewStack.Push(currentView);
        }
        else
        {
            
            DestroyAllSpawnedViews();
            backViewStack.Clear();
        }
        ClearMainViews();
        ChangeToMainView(_viewID);
    }

    private void SetChangeOfSpawnedViews(string _SpawnedViewType)
    {
        backViewStack.Push(currentView);

        SpawnViewOfType(_SpawnedViewType);
    }

    private void ChangeToMainView(ViewID _viewID)
    {
        foreach (ViewModel viewInstance in mainViews)
        {
            if (((ViewModel)viewInstance.GetComponent(typeof(ViewModel))).GetViewID() == _viewID)
            {
                this.currentView = viewInstance;

                ((ViewModel)(viewInstance.GetComponent(typeof(ViewModel)))).SetActive(true);
            }
        }
    }

    private void ClearMainViews()
    {
        foreach (ViewModel viewInstance in mainViews)
        {
            ((ViewModel)viewInstance.GetComponent(typeof(ViewModel))).SetActive(false);
        }
    }

    private void SpawnViewOfType(string _type)
    {
        GameObject searchedView = viewModelTypesData.viewTypes.Find(x => x.viewModelType.Equals(_type)).viewModelPrefab;

        ViewModel spawnedView = Instantiate(searchedView, spawnedViewsParent).GetComponent<ViewModel>();

        currentView = spawnedView;

        spawnedViewsList.Add(spawnedView);

        spawnedView.transform.SetAsLastSibling();

        LayoutRebuilder.ForceRebuildLayoutImmediate(spawnedViewsParent);
    }

    private void DestroyAllSpawnedViews()
    {
        foreach(ViewModel view in spawnedViewsList)
        {
            view.gameObject.SetActive(false);
        }

        spawnedViewsList.Clear();
    }

    private void RemoveSpawnedViewFromList(ViewModel _currentView)
    {
        if (CeckIfCurrentViewSpawned(_currentView))
        {
            spawnedViewsList.Remove(_currentView);
        }
    }

    private bool CeckIfCurrentViewSpawned(ViewModel _currentView)
    {
        return spawnedViewsList.Contains(_currentView);
    }
}

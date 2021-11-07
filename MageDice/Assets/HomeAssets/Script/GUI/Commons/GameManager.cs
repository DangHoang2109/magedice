using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoSingleton<GameManager>
{
#if UNITY_EDITOR
    public static bool isApplicationQuit = false;
#endif

    private List<BaseDialog> baseDialogs;
    private Dictionary<string, BaseDialog> tempDialog;

    private Dictionary<PopupSortingType, int> dicSortingNumbers;
    public event System.Action<BaseDialog> OnADialogSummoned; 
    
    

    public override void Init()
    {
        base.Init();
        this.baseDialogs = new List<BaseDialog>();
        this.tempDialog = new Dictionary<string, BaseDialog>();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        //sorting
        this.dicSortingNumbers = new Dictionary<PopupSortingType, int>();
        this.dicSortingNumbers.Add(PopupSortingType.BellowBottomBar, 0);
        this.dicSortingNumbers.Add(PopupSortingType.CenterBottomAndTopBar, 0);
        this.dicSortingNumbers.Add(PopupSortingType.OnTopBar, 0);

        Application.targetFrameRate = 60;
        // TODO! !!!!
        Application.runInBackground = true;
    }
    #region DIALOG
    public T OnShowDialog<T>(string path, object data = null, UnityAction callback = null) where T : BaseDialog
    {

        if (!this.tempDialog.ContainsKey(path))
        {
            GameObject o = LoaderUtility.Instance.GetAsset<GameObject>(path);
            if (o != null)
            {
                T target = ((GameObject)Instantiate(o, this.scene.dialog)).GetComponent<T>();
                if (target != null)
                {
                    target.gameObject.SetActive(true);

                    target.transform.localScale = Vector3.one;
                    target.transform.localPosition = Vector3.zero;
                    target.transform.SetAsLastSibling();
                    target.OnShow(data, callback);
                    this.baseDialogs.Add(target);
                    this.tempDialog.Add(path, target);

                    this.OnADialogSummoned?.Invoke(target);
                    return target;
                }
            }
            else
            {
                Debug.LogError("CAN NOT LOAD DATA BY PATH: " + path);
            }
        }
        else
        {
            BaseDialog dialog = this.tempDialog[path];
            dialog.gameObject.SetActive(true);
            dialog.transform.localScale = Vector3.one;
            dialog.transform.localPosition = Vector3.zero;
            dialog.transform.SetAsLastSibling();
            dialog.OnShow(data, callback);
            this.baseDialogs.Add(dialog);
            
            this.OnADialogSummoned?.Invoke(dialog);
            return (T)dialog;
        }
        return null;
    }

    private int GetDialogSortingNumber(PopupSortingType sortingType)
    {
        if (this.dicSortingNumbers != null)
        {
            if (this.dicSortingNumbers.ContainsKey(sortingType))
            {
                this.dicSortingNumbers[sortingType] += 1;
                return this.dicSortingNumbers[sortingType];
            }
        }
        return 0;
    }

    public T OnShowDialogWithSorting<T>(string path, PopupSortingType sortingType,  object data = null, UnityAction callback = null) where T : BaseSortingDialog
    {
        BaseSortingDialog dialog = OnShowDialog<BaseSortingDialog>(path, data, callback);
        if (dialog != null)
        {
            //dialog mở sau sẽ được sorting cao hơn
            dialog.SetSortingOrder((int)sortingType + GetDialogSortingNumber(sortingType));
            return (T)dialog;
        }
        return null;
    }
    public void OnHideDialog(BaseDialog dialog)
    {
        dialog.OnHide();
        if (this.baseDialogs.Contains(dialog))
        {
            this.baseDialogs.Remove(dialog);
        }
    }

    public void CloseDialog<T>() where T : BaseDialog
    {
        foreach (BaseDialog dialog in this.baseDialogs)
        {
            if (dialog is T)
            {
                this.OnHideDialog(dialog);
                return;
            }
        }
    }
    public bool IsDialog<T>() where T : BaseDialog
    {
        foreach (BaseDialog dialog in this.baseDialogs)
        {
            if (dialog is T)
            {
                return true;
            }

        }
        return false;
    }
    public void CloseAllDialog()
    {
        foreach (BaseDialog dialog in this.baseDialogs)
        {
            dialog.OnCloseDialog();

        }
    }
    #endregion

    #region SCENE
   
    private BaseScene scene;
    /// <summary>
    /// callback if there is transition between scenes have started
    /// </summary>
    public event System.Action OnSceneChanging;
    /// <summary>
    /// callback when a new scene bound,
    /// </summary>
    public event System.Action<BaseScene> OnSceneBound;
    public void setBaseScene(BaseScene scene)
    {
        this.scene = scene;
        this.OnSceneBound?.Invoke(scene);
    }
    public BaseScene GetScene()
    {
        return this.scene;
    }

    public void OnLoadScene(string sceneName, object data = null, UnityAction callback = null)
    {
        LoadingManager.Instance.LoadScene(true, () =>
        {

            this.baseDialogs.Clear();
            this.tempDialog.Clear();
            this.scene?.OnClear();

            // Debug.LogError("LOAD HOME: " + sceneName);
            StartCoroutine(this.OnWaitingLoadScene(sceneName, callback));
        });

    }
    private IEnumerator OnWaitingLoadScene(string sceneName, UnityAction callback = null)
    {
        yield return new WaitForEndOfFrame();
        this.OnSceneChanging?.Invoke();
        yield return new WaitForSeconds(0.2f);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
    
        while (!asyncLoad.isDone)
        {
            LoadingManager.callbackProgress.Invoke(asyncLoad.progress);
            yield return null;
        }

        yield return new WaitForEndOfFrame();
        LoadingManager.callbackProgress.Invoke(asyncLoad.progress);
        if (callback != null)
        {
            callback.Invoke();
        }
        LoadingManager.Instance.LoadScene(false);
        LoadingManager.Instance.ShowLoading(false);

    }
    public string GetCurrentScene()
    {
        return SceneManager.GetActiveScene().name;
    }

    public bool IsCurrentScene(string sceneName)
    {
        return this.GetCurrentScene().Equals(sceneName);
    }
    #endregion

    #region Network
    public class NetworkCallback : UnityEvent<bool>
    {

    }
    private NetworkCallback callbackNetwork;
    private bool isInternet = false;
    public static bool IsInternet
    {
        get
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return false;

            }
            return true;
        }
    }
    public void OpenGame()
    {
        try
        {
            this.isInternet = IsInternet;
            this.ChangeNetwork(this.isInternet);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Debug.LogException(e);
        }
        
    }
    
    public void AddCallbackNetWork(UnityAction<bool> callback)
    {
        if (this.callbackNetwork == null)
        {
            this.callbackNetwork = new NetworkCallback();
        }
        this.callbackNetwork?.AddListener(callback);
    }
    public void RemoveCallbackNetwork(UnityAction<bool> callback)
    {
        if (this.callbackNetwork == null)
        {
            this.callbackNetwork = new NetworkCallback();
        }
        this.callbackNetwork?.RemoveListener(callback);
    }

    public void ChangeNetwork(bool isNetwork)
    {
        if (this.callbackNetwork == null)
        {
            this.callbackNetwork = new NetworkCallback();
        }
        this.callbackNetwork?.Invoke(isNetwork);
    }
    private float timeDelay = 0;
    private void Check()
    {
        bool isConnect = IsInternet;
        if (isConnect != this.isInternet)
        {
            this.isInternet = isConnect;
            this.ChangeNetwork(this.isInternet);
        }

    }
    private void Update()
    {
        if (!this.isInternet)
        {
            this.timeDelay += Time.deltaTime;
            if (timeDelay > 10.0f)
            {
                this.Check();
                this.timeDelay = 0;
            }
        }
    }
    

    #endregion

    public void LogMessage(string msg)
    {
        
    }
#if UNITY_EDITOR
    public void OnApplicationQuit()
    {
        isApplicationQuit = true;
    }
#endif

}

public enum PopupSortingType
{
    //Để số lớn để sắp xếp các dialog (dialog mở sau sẽ được sorting cao hơn 1 bậc)
    BellowBottomBar = 5,            //ở dưới bottom 
    CenterBottomAndTopBar = 1500,   //ở giữa bottom and top bar
    OnTopBar = 3000,                //ở trên cùng
}

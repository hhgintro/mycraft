using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageController : MonoBehaviour {
    [SerializeField]
    private string ftp_url;// = "ftp://112.171.173.154:11021/lime/";
    [SerializeField]
    private string ftp_user;
    [SerializeField]
    private string ftp_passwd;
    private string download_path;// = Application.persistentDataPath + "/bundles";

    public GameObject prefabStageManager;
    public GameObject stageUI;
    public Text stageInfo;

    public Image progressBar;
    public Text downloadFilename;
    public Text progressPercent;
    public GameObject patchFailed;

    PatchManager patcher;// = new PatchManager();


    //long _TotalBytes = 0;
    long _ReceivedBytes = 0;
    long _currentBytes = 0;
    bool _NextSceneCall = false;    // 다운로드가 완료되면 true 된다.
    string downLoadFileText;
    int current_download = 0;


    private void Awake()
    {
        if (null == StageManager.Instance)
        {
            //Debug.Log("Not Found StageManager");
            Instantiate(prefabStageManager);
        }
    }

    // Use this for initialization
    void Start () {

        //StartCoroutine(NextScene());
        SetProgressBar(0f);
        downloadFilename.text = "";
        patchFailed.SetActive(false);

        download_path = Application.persistentDataPath + "/AssetBundles";

        patcher = new PatchManager(ftp_url, ftp_user, ftp_passwd);
        patcher.CallbackCompletedEvent(new AsyncCompletedEventHandler(Completed));
        patcher.CallbackDownloadProgressChangedEvent(new DownloadProgressChangedEventHandler(ProgressChanged));


        stageUI.SetActive(false);

        StartCoroutine(DownLoadAssetBundles());
    }

    // Update is called once per frame
    void Update () {
        if (patcher._TotalBytes <= 0)
            return;

        //0.95 이상에서는 끝난시점을 정확히 찾기위해 Lerp를 사용하지 않는다.
        double percentage = (double)(_currentBytes + _ReceivedBytes) / (double)patcher._TotalBytes;
        if (0.95 < percentage)
        {
            //progressBar.fillAmount = (float)percentage;
            SetProgressBar((float)percentage);
            DownLoadCompleted();
            return;
        }

        //progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, (float)percentage, Time.deltaTime * 10f);
        SetProgressBar(Mathf.Lerp(progressBar.fillAmount, (float)percentage, Time.deltaTime * 10f));
        //progressBar.fillAmount = (float)percentage;

    }

    IEnumerator DownLoadAssetBundles()
    {
        //HG_TODO : ftp 연결지연이 발생하는 경우...서버동작이 멈추는 현상을 해결해야 한다.
        //yield return new WaitForSeconds(5f);
        if (false == patcher.LoadRemoteAssetBunleList(ftp_url + "AssetBundleList.txt"))
            patchFailed.SetActive(true);
        //else
        //{
        //    patcher.LoadLocalOneAssetBundle(download_path, StageManager.Instance.bundlename);

        //    if (false == patcher.DownLoad())
        //    {
        //        //progress를 100%로 보이기 위한 설정입니다.
        //        //_TotalBytes = 1;
        //        _ReceivedBytes = patcher._TotalBytes;
        //        _currentBytes = 0;
        //        //progressBar.fillAmount = 1f;//Error

        //        //Debug.Log("no patch");
        //        _NextSceneCall = true;
        //    }
        //}
        yield return null;
    }

    void SetProgressBar(float amount)
    {
        downloadFilename.text = downLoadFileText;

        progressBar.fillAmount = amount;
        int percent = (int)(amount * 100);
        progressPercent.text = percent.ToString() + "%";
    }

    void DownLoadCompleted()
    {
        if (false == _NextSceneCall)
            return;
        //또 호출되지 않도록(꼭,필요한지는 모름.혹시나해서)
        _NextSceneCall = false;

        //LoadLobbyScene();
    }

    private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        //_ReceivedBytes는 완료될 때 갱신된다.
        _currentBytes = e.BytesReceived;
        //double percentage = (double)(_currentBytes + _ReceivedBytes) / (double)_TotalBytes;
        //Debug.Log((_currentBytes + _ReceivedBytes).ToString("") + "/" + _TotalBytes.ToString("") + "/" + percentage.ToString("N2"));

        downLoadFileText = (patcher.GetDownloadPatchInfo(current_download)._filename + " 다운로딩중...");
    }

    private void Completed(object sender, AsyncCompletedEventArgs e)
    {
        _currentBytes = 0;
        PatchInfo pi = patcher.GetDownloadPatchInfo(current_download);
        _ReceivedBytes += pi._totalByte;
        downLoadFileText = pi._filename + " 다운로드 완료";

        //next
        pi = patcher.GetDownloadPatchInfo(++current_download);
        if (null == pi)
        {
            Debug.Log("patch All Completed");
            patcher.DownloadCompleted();
            _NextSceneCall = true;
            return;
        }

        //next download
        patcher.DownLoad(pi._filename);
    }

    //IEnumerator NextScene()
    //{
    //    yield return new WaitForSeconds(1);

    //    //Application.LoadLevel(Application.loadedLevel + 1);
    //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    //}

    private void Download()
    {
        //download
        patcher.LoadLocalOneAssetBundle(download_path, StageManager.Instance.bundlename);
        if (false == patcher.DownLoad())
        {
            //progress를 100%로 보이기 위한 설정입니다.
            //_TotalBytes = 1;
            _ReceivedBytes = patcher._TotalBytes = 1;
            _currentBytes = 0;
            //progressBar.fillAmount = 1f;//Error

            //Debug.Log("no patch");
            _NextSceneCall = true;
        }

    }
    public void LoadStage1()
    {
        StageManager.Instance.bundlename = "stage_guitar2";
        StageManager.Instance.audio = "기타2";
        StageManager.Instance.track = "Guitar2_Track";

        //UI
        stageInfo.text = "Stage 1";
        stageUI.SetActive(true);


        //patch download
        Download();
    }

    public void LoadStage2()
    {
        StageManager.Instance.bundlename = "stage_guitar2_smooth";
        StageManager.Instance.audio = "어쿠스틱";
        StageManager.Instance.track = "PianoTrack";

        //UI
        stageInfo.text = "Stage 2";
        stageUI.SetActive(true);

        //patch download
        Download();
    }

    public void CloseStageUI()
    {
        stageUI.SetActive(false);
    }

    public void LoadingScene()
    {
        //Application.LoadLevel(Application.loadedLevel + 1);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        LoadingSceneManager.LoadScene("InGame");
    }

    public void GoLobbyScene()
    {
        //Application.LoadLevel(Application.loadedLevel + 1);
        SceneManager.LoadScene("Lobby");
        //LoadingSceneManager.LoadScene("InGame");
    }

}

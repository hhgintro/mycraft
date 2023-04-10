using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PatchController : MonoBehaviour {

    [SerializeField]
    private string ftp_url;// = "ftp://112.171.173.154:11021/lime/";
    [SerializeField]
    private string ftp_user;
    [SerializeField]
    private string ftp_passwd;
    private string download_path;// = Application.persistentDataPath + "/bundles";

    public Image progressBar;
    public Text downloadFilename;
    public Text progressPercent;
    public GameObject patchFailed;

    PatchManager patcher;// = new PatchManager();

    //WebClient _wc = new WebClient();

    ////서버의 에셋번들 정보(번들이름,해시)
    //Dictionary<string, string> remoteAssetBundles;
    ////다운받아야하는 에셋번들리스트(메니페스트도 같이 받는다.)
    //List<PatchInfo> downloadList;

    //long _TotalBytes = 0;
    long _ReceivedBytes = 0;
    long _currentBytes = 0;
    bool _NextSceneCall = false;    // 다운로드가 완료되면 true 된다.
    string downLoadFileText;
    int current_download = 0;

    //private delegate void SafeSetText();


    //void DelegateDownLoadAssetBundles()
    //{
    //    Debug.Log("DelegateDownLoadAssetBundles");
    //    if (true == LoadRemoteAssetBundle())
    //    {
    //        LoadLocalAssetBundle();

    //        DownLoad();
    //    }
    //}

    // Use this for initialization
    void Start () {

        SetProgressBar(0f);
        downloadFilename.text = "";
        patchFailed.SetActive(false);

        download_path = Application.persistentDataPath + "/AssetBundles";

        patcher = new PatchManager(ftp_url, ftp_user, ftp_passwd);
        patcher.CallbackCompletedEvent(new AsyncCompletedEventHandler(Completed));
        patcher.CallbackDownloadProgressChangedEvent(new DownloadProgressChangedEventHandler(ProgressChanged));
        //_wc.Credentials = new NetworkCredential(ftp_user, ftp_passwd);
        //_wc.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
        //_wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);

        //remoteAssetBundles = new Dictionary<string, string>();
        //downloadList = new List<PatchInfo>();


        StartCoroutine(DownLoadAssetBundles());
        //SafeSetText SafeSetTextMethod = new SafeSetText(DelegateDownLoadAssetBundles);
        //SafeSetTextMethod();
        //Debug.Log("Start()");
    }

    //float tick = 0f;
    private void Update()
    {
        //tick += Time.deltaTime;
        //if(1 < tick)
        //{
        //    tick = 0;
        //    Debug.Log("update()");
        //}

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
        else
        {
            patcher.LoadLocalAllAssetBundle(download_path);

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

        LoadLobbyScene();
    }

    public void LoadLobbyScene()
    {
        SceneManager.LoadSceneAsync("Lobby");
    }



    #region PatchManager OLD version Source
    //private bool LoadRemoteAssetBundle()
    //{
    //    //WebClient request = new WebClient();

    //    //string url = FtpPath + FileName;
    //    //string version = "";

    //    //request.Credentials = new NetworkCredential(MailLibConfigurations.ftp_user, MailLibConfigurations.ftp_pas);

    //    ////request.DownloadFile(new Uri(FtpPath) + FileName, "D:\\sdc_version.txt");
    //    try {
    //        byte[] filedata = _wc.DownloadData(ftp_url + "AssetBundleList.txt");
    //        string fileString = System.Text.Encoding.UTF8.GetString(filedata);
    //        string[] bundleList = fileString.Split('\n');

    //        foreach (string bundle in bundleList)
    //        {
    //            if (0 == bundle.CompareTo("")) continue;

    //            string[] info = bundle.Split(',');
    //            //Debug.Log("remote =>" + info[0] + "," + info[1]);
    //            remoteAssetBundles.Add(info[0], info[1].Trim());
    //        }
    //    }
    //    catch (WebException e) {
    //        Debug.LogError("Error: " + e.ToString());
    //        patchFailed.SetActive(true);
    //        return false;
    //    }

    //    //if (true == Directory.Exists(download_path))
    //    //{
    //    //    foreach (string bundle in bundleList)
    //    //    {
    //    //        if (0 == bundle.CompareTo("")) continue;

    //    //        string[] info = bundle.Split(',');
    //    //        //Debug.Log("remote =>" + info[0] + "," + info[1]);
    //    //        remoteAssetBundle.Add(info[0], info[1]);
    //    //        //string filename = download_path + "/" + bundle.Trim();// + ".unity3d";
    //    //        //if (true == File.Exists(filename))
    //    //        //{
    //    //        //    AssetBundle assetBundle = AssetBundle.LoadFromFile(filename);
    //    //        //    if (null == assetBundle) continue;
    //    //        //    AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
    //    //        //    //Debug.Log(manifest.ToString());
    //    //        //}
    //    //        //else
    //    //        //{
    //    //        //    //(파일자체가 없으니)manifest 체크없이 받는다.
    //    //        //    Debug.Log("not found file: " + filename.ToString());
    //    //        //    //File.Create(filename);
    //    //        //}
    //    //    }
    //    //}
    //    //else
    //    //{
    //    //    //(폴더자체가 없으니)manifest 체크없이 bundleList 모두를 받는다.
    //    //    //..
    //    //}

    //    return true;
    //}

    //private void LoadLocalAssetBundle()
    //{
    //    //AssetBundles 파일이 없이면 무조건 다시 받는다.
    //    string filePath = download_path + "/" + "AssetBundles";
    //    if (false == File.Exists(filePath))
    //    {
    //        foreach (var pair in remoteAssetBundles)
    //        {
    //            downloadList.Add(new PatchInfo(pair.Key, 0));
    //            downloadList.Add(new PatchInfo(pair.Key + ".manifest", 0));
    //        }
    //        return;
    //    }

    //    //AssetBundle.UnloadAllAssetBundles(true);
    //    AssetBundle assetBundle = AssetBundle.LoadFromFile(download_path + "/" + "AssetBundles");
    //    //foreach (string n in assetBundle.AllAssetNames()) Debug.Log(n);
    //    AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");


    //    //remote 목록을 기준으로 받아야 합니다.
    //    foreach (var pair in remoteAssetBundles)
    //    {
    //        //파일이 없으면...다운로드목록에 등록.
    //        if (false == File.Exists(download_path + "/" + pair.Key))
    //        {
    //            downloadList.Add(new PatchInfo(pair.Key, 0));
    //            downloadList.Add(new PatchInfo(pair.Key + ".manifest", 0));
    //            continue;
    //        }

    //        //Debug.Log("local =>" + bundle + ": " + manifest.GetAssetBundleHash(bundle).ToString());
    //        //bundle이름으로 remote의 해시를 비교합니다.(다르면...다운로드할 목록에 추가)
    //        string localHash = manifest.GetAssetBundleHash(pair.Key).ToString();
    //        //Debug.Log(localHash + "/" + remoteAssetBundle[bundle]);
    //        if (0 != pair.Value.CompareTo(localHash.Trim()))
    //        {
    //            downloadList.Add(new PatchInfo(pair.Key, 0));
    //            downloadList.Add(new PatchInfo(pair.Key + ".manifest", 0));
    //        }
    //    }
    //    assetBundle.Unload(true);
    //}


    //public void DownLoad()
    //{
    //    if (false == Directory.Exists(download_path))
    //        Directory.CreateDirectory(download_path);

    //    //받을 patch없다.
    //    if(downloadList.Count <= 0)
    //    {
    //        //progress를 100%로 보이기 위한 설정입니다.
    //        //_TotalBytes = 1;
    //        _ReceivedBytes = patcher._TotalBytes;
    //        _currentBytes = 0;
    //        //progressBar.fillAmount = 1f;//Error

    //        Debug.Log("no patch");
    //        _NextSceneCall = true;
    //        return;
    //    }

    //    //download List가 있으면 "AssetBundles" 파일도 다시 받아줍니다.
    //    downloadList.Add(new PatchInfo("AssetBundles",0));
    //    downloadList.Add(new PatchInfo("AssetBundles.manifest",0));

    //    //다운받을 파일들 total byte
    //    _TotalBytes = GetTotalByteDownLoad();

    //    //foreach (string bundlename in downloadList)
    //    //    DownLoad(bundlename);
    //    DownLoad(downloadList[current_download]._filename);
    //}

    //private void DownLoad(string bundlename)
    //{
    //    //// 익스플로러의 헤더 에이전트 추가해서 문제 해결
    //    //_wc.Headers.Add( "User-Agent", "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.3; WOW64; Trident/7.0)" ); 


    //    //
    //    //string bundlename = "guitar2";
    //    string url = ftp_url + bundlename;

    //    //_TotalBytes = GetTotalByte(url);

    //    //_wc.Credentials = new NetworkCredential("ftp", "meta2017");
    //    _wc.DownloadFileAsync(new Uri(url), download_path + "/" + bundlename);
    //    //webClient.DownloadFileAsync(new Uri("http://mysite.com/myfile.txt"), @"c:\myfile.txt");
    //}

    //public void DownLoadRemoteFile(string filename)
    //{
    //    if (false == Directory.Exists(download_path))
    //        Directory.CreateDirectory(download_path);

    //    string url = ftp_url + filename;

    //    //_TotalBytes = GetTotalByte(url);
    //    //_wc.Credentials = new NetworkCredential("ftp", "meta2017");
    //    _wc.DownloadFileAsync(new Uri(url), download_path + "/" + filename);
    //}

    ////다운로드할 파일들의 total byte
    //private long GetTotalByteDownLoad()
    //{
    //    long total = 0;
    //    //bundle / menifest
    //    foreach (PatchInfo pi in downloadList)
    //    {
    //        pi._totalByte = GetTotalByte(ftp_url + pi._filename);
    //        total += pi._totalByte;
    //        //Debug.Log(bundlename + " : " + len.ToString() + "/" + total.ToString());
    //    }
    //    return total;
    //}

    //private long GetTotalByte(string url)
    //{
    //    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
    //    request.Method = WebRequestMethods.Ftp.GetFileSize;
    //    request.Credentials = _wc.Credentials;
    //    FtpWebResponse response = (FtpWebResponse)request.GetResponse();

    //    Stream responseStream = response.GetResponseStream();
    //    long totalbytes = response.ContentLength; //this is an int member variable stored for later
    //    response.Close();
    //    return totalbytes;
    //}

    //private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    //{
    //    progressBar.Value = e.ProgressPercentage;
    //}

    //private void Completed(object sender, AsyncCompletedEventArgs e)
    //{
    //    MessageBox.Show("Download completed!");
    //}
    #endregion



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

}

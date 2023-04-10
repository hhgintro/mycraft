using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using UnityEngine;

public class PatchManager {

    private string ftpUrl;// = "ftp://112.171.173.154:11021/lime/";
    private string ftpUser;
    private string ftpPasswd;
    private string downloadPath;

    //서버의 에셋번들 정보(번들이름,해시)
    Dictionary<string, string> remoteAssetBundles = new Dictionary<string, string>();
    //다운받아야하는 에셋번들리스트(메니페스트도 같이 받는다.)
    private List<PatchInfo> downloadList = new List<PatchInfo>();

    WebClient _wc = new WebClient();

    public long _TotalBytes = 0;


    public PatchManager(string ftp_url, string ftp_user, string ftp_passwd)
    {
        this.ftpUrl = ftp_url; this.ftpUser = ftp_user; this.ftpPasswd = ftp_passwd;

        _wc.Credentials = new NetworkCredential(ftp_user, ftp_passwd);
        //_wc.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
        //_wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
    }

    public void Init(string ftp_url, string ftp_user, string ftp_passwd)
    {
        this.ftpUrl = ftp_url; this.ftpUser = ftp_user; this.ftpPasswd = ftp_passwd;

        _wc.Credentials = new NetworkCredential(ftp_user, ftp_passwd);
    }

    //private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    //{ }
    //private void Completed(object sender, AsyncCompletedEventArgs e)
    //{ }
    public void CallbackCompletedEvent(AsyncCompletedEventHandler handle)
    {
        //_wc.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
        _wc.DownloadFileCompleted += handle;
    }
    public void CallbackDownloadProgressChangedEvent(DownloadProgressChangedEventHandler hangle)
    {
        //_wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
        _wc.DownloadProgressChanged += hangle;
    }

    public bool LoadRemoteAssetBunleList(string url)
    {
        try
        {
            //url : ftp_url + "AssetBundleList.txt"
            byte[] filedata = _wc.DownloadData(url);
            string fileString = System.Text.Encoding.UTF8.GetString(filedata);
            string[] bundleList = fileString.Split('\n');

            foreach (string bundle in bundleList)
            {
                if (0 == bundle.CompareTo("")) continue;

                string[] info = bundle.Split(',');
                //Debug.Log("remote =>" + info[0] + "," + info[1]);
                remoteAssetBundles.Add(info[0], info[1].Trim());
            }
        }
        catch (WebException e)
        {
            Debug.LogError("Error: " + e.ToString());
            //patchFailed.SetActive(true);
            return false;
        }
        return true;
    }

    public void LoadLocalAllAssetBundle(string downloadPath)
    {
        this.downloadPath = downloadPath;

        //AssetBundles 파일이 없이면 무조건 다시 받는다.
        string filePath = downloadPath + "/" + "AssetBundles";
        if (false == File.Exists(filePath))
        {
            foreach (var pair in remoteAssetBundles)
                AddDownloadList(pair.Key);

            AddDownloadList("AssetBundles");
            return;
        }

        //AssetBundle.UnloadAllAssetBundles(true);
        AssetBundle assetBundle = AssetBundle.LoadFromFile(downloadPath + "/" + "AssetBundles");
        if (null == assetBundle) return;
        //foreach (string n in assetBundle.AllAssetNames()) Debug.Log(n);
        AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        if (null == manifest) return;

        //"AssetBundle"파일을 다운해야할지 판단
        //"stage_"관련파일이 변경되더라도 받지 않기때문에.
        //달라지는 부분이 있을때는 "AssetBundle" 파일을 받아야 합니다.
        bool bDownloadAssetBundleFile = false;
        //remote 목록을 기준으로 받아야 합니다.
        foreach (var pair in remoteAssetBundles)
        {
            //파일이 없으면...다운로드목록에 등록.
            if (false == File.Exists(downloadPath + "/" + pair.Key))
            {
                AddDownloadList(pair.Key);
                bDownloadAssetBundleFile = true;
                continue;
            }

            //Debug.Log("local =>" + bundle + ": " + manifest.GetAssetBundleHash(bundle).ToString());
            //bundle이름으로 remote의 해시를 비교합니다.(다르면...다운로드할 목록에 추가)
            string localHash = manifest.GetAssetBundleHash(pair.Key).ToString();
            //Debug.Log(localHash + "/" + remoteAssetBundle[bundle]);
            if (0 != pair.Value.CompareTo(localHash.Trim()))
            {
                AddDownloadList(pair.Key);
                bDownloadAssetBundleFile = true;
            }
        }
        assetBundle.Unload(true);

        //HG_TODO : 어떻게든지 1번은 받아야 한다.
        if (true == bDownloadAssetBundleFile)
            AddDownloadList("AssetBundles");

    }
    public void LoadLocalOneAssetBundle(string downloadPath, string bundlename)
    {
        this.downloadPath = downloadPath;

        //AssetBundles 파일이 없이면 무조건 다시 받는다.
        string filePath = downloadPath + "/" + "AssetBundles";
        if (false == File.Exists(filePath))
        {
            AddDownloadList(bundlename, false);
            AddDownloadList("AssetBundles");
            return;
        }

        //AssetBundle.UnloadAllAssetBundles(true);
        AssetBundle assetBundle = AssetBundle.LoadFromFile(downloadPath + "/" + "AssetBundles");
        if (null == assetBundle) return;
        //foreach (string n in assetBundle.AllAssetNames()) Debug.Log(n);
        AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        if (null == manifest) return;

        //remote 목록을 기준으로 받아야 합니다.
        if (false == remoteAssetBundles.ContainsKey(bundlename))
        {
            Debug.LogError("not found remote AssetBundle: " + bundlename);
            return; //remote의 목록에 없다
        }

        //파일이 없으면...다운로드목록에 등록.
        if (false == File.Exists(downloadPath + "/" + bundlename))
        {
            AddDownloadList(bundlename, false);
            AddDownloadList("AssetBundles");
        }

        //Debug.Log("local =>" + bundle + ": " + manifest.GetAssetBundleHash(bundle).ToString());
        //bundle이름으로 remote의 해시를 비교합니다.(다르면...다운로드할 목록에 추가)
        string localHash = manifest.GetAssetBundleHash(bundlename).ToString();
        //Debug.Log(localHash + "/" + remoteAssetBundle[bundle]);
        if (0 != remoteAssetBundles[bundlename].CompareTo(localHash.Trim()))
        {
            AddDownloadList(bundlename, false);
            AddDownloadList("AssetBundles");
        }
        assetBundle.Unload(true);

    }

    public bool DownLoad()
    {
        if (false == Directory.Exists(downloadPath))
            Directory.CreateDirectory(downloadPath);

        //받을 patch없다.
        if (downloadList.Count <= 0)
        {
            Debug.Log("no patch");
            return false;
        }

        //download List가 있으면 "AssetBundles" 파일도 다시 받아줍니다.
        //downloadList.Add(new PatchInfo("AssetBundles", 0));
        //downloadList.Add(new PatchInfo("AssetBundles.manifest", 0));
        //AddDownloadList("AssetBundles");

        //다운받을 파일들 total byte
        _TotalBytes = GetTotalByteDownLoad();

        //foreach (string bundlename in downloadList)
        //    DownLoad(bundlename);
        DownLoad(downloadList[0]._filename);
        return true;
    }

    public void DownLoad(string bundlename)
    {
        //// 익스플로러의 헤더 에이전트 추가해서 문제 해결
        //_wc.Headers.Add( "User-Agent", "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.3; WOW64; Trident/7.0)" ); 


        //
        //string bundlename = "guitar2";
        string url = ftpUrl + bundlename;

        //_TotalBytes = GetTotalByte(url);

        //_wc.Credentials = new NetworkCredential("ftp", "meta2017");
        _wc.DownloadFileAsync(new Uri(url), downloadPath + "/" + bundlename);
        //webClient.DownloadFileAsync(new Uri("http://mysite.com/myfile.txt"), @"c:\myfile.txt");
    }

    //excludeStage : true이면 "stage_"키워드로 시작하는 bundle은 포함에서 배제합니다.
    private void AddDownloadList(string bundlename, bool excludeStage=true)
    {
        //stage_를 키워드로 시작하는 AssetBundle은 개별로 진행되면
        //일관처리에서는 배재합니다.
        if (true == excludeStage)
        {
            if ("stage_" == bundlename.Substring(0, 6))
                return;
        }

        downloadList.Add(new PatchInfo(bundlename, 0));
        downloadList.Add(new PatchInfo(bundlename + ".manifest", 0));
}

    //public void DownLoadRemoteFile(string filename)
    //{
    //    if (false == Directory.Exists(download_path))
    //        Directory.CreateDirectory(download_path);

    //    string url = ftp_url + filename;

    //    //_TotalBytes = GetTotalByte(url);
    //    //_wc.Credentials = new NetworkCredential("ftp", "meta2017");
    //    _wc.DownloadFileAsync(new Uri(url), download_path + "/" + filename);
    //}

    //다운로드할 파일들의 total byte
    private long GetTotalByteDownLoad()
    {
        long total = 0;
        //bundle / menifest
        foreach (PatchInfo pi in downloadList)
        {
            pi._totalByte = GetTotalByte(ftpUrl + pi._filename);
            total += pi._totalByte;
            //Debug.Log(bundlename + " : " + len.ToString() + "/" + total.ToString());
        }
        return total;
    }

    private long GetTotalByte(string url)
    {
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
        request.Method = WebRequestMethods.Ftp.GetFileSize;
        request.Credentials = _wc.Credentials;
        FtpWebResponse response = (FtpWebResponse)request.GetResponse();

        Stream responseStream = response.GetResponseStream();
        long totalbytes = response.ContentLength; //this is an int member variable stored for later
        response.Close();
        return totalbytes;
    }


    public PatchInfo GetDownloadPatchInfo(int index)
    {
        if (downloadList.Count <= index) return null;
        return downloadList[index];
    }

    //다운로드가 완료되면...재사용을 위해 비운다.
    public void DownloadCompleted()
    {
        downloadList.Clear();
    }

}

using System;
using System.Collections;
using UnityEngine;

public class LoadAssetBundleExample : MonoBehaviour {

    // 번들 다운 받을 서버의 주소(필자는 임시방편으로 로컬 파일 경로 쓸 것임)
    public string BundleURL;
    // 번들의 version
    public int version;

    void Start()
    {
        StartCoroutine (LoadAssetBundle());
    }

    IEnumerator LoadAssetBundle()
    {
        while (!Caching.ready)
            yield return null;

        // 불러올 파일 경로.
        // pc의 경우 file:/// 로 시작하셔야 합니다.
        //string path = "file:///" + Application.dataPath + "/myAssetBundle";
        using (WWW www = WWW.LoadFromCacheOrDownload(BundleURL, version))
        {
            yield return www;
            if (www.error != null)
                throw new Exception("WWW 다운로드에 에러가 생겼습니다.:" + www.error);

            AssetBundle bundle = www.assetBundle;

            for (int i = 0; i < 3; i++)
            {
                AssetBundleRequest request = bundle.LoadAssetAsync("Cube " + (i + 1), typeof(GameObject));
                yield return request;

                GameObject obj = Instantiate(request.asset) as GameObject;
                obj.transform.position = new Vector3(-10.0f + (i * 10), 0.0f, 0.0f);
            }

            bundle.Unload(false);
            www.Dispose();
        } // using문은 File 및 Font 처럼 컴퓨터 에서 관리되는 리소스들을 쓰고 나서 쉽게 자원을 되돌려줄수 있도록 기능을 제공
    }
}

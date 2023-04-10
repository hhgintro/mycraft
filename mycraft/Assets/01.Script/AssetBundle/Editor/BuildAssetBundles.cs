using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class BuildAssetBundles : MonoBehaviour {
    //bundle 저장경로
    static string path = "../AssetBundles";
    //AssetBundle 리스트 파일명.(이것을 먼저 다운받는다.)
    public static string AssetBundleListFilename = "AssetBundleList.txt";


    #region private function
    static void CheckDirectory(string path)
    {
        if (false == Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    static void WriteBundleList(string filename)
    {

        //append = false
        using (StreamWriter file = new StreamWriter(path + "/" + filename, false))
        {
            AssetBundle assetBundle = AssetBundle.LoadFromFile(path + "/" + "AssetBundles");
            //foreach (string n in assetBundle.AllAssetNames()) Debug.Log(n);
            AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            foreach (string bundle in manifest.GetAllAssetBundles())
            {
                //name, hash
                string txt = bundle + "," + manifest.GetAssetBundleHash(bundle).ToString();
                //Debug.Log(txt);
                file.WriteLine(txt);
            }
            assetBundle.Unload(true);
        }
    }

    #endregion


    #region Build AssetBundle
    /*********************************************************************** 
     * 용도 : MenuItem을 사용하면 메뉴창에 새로운 메뉴를 추가할 수 있습니다.	
     * (아래의 코드에서는 Bundles 항목에 하위 항목으로 Build AssetBundles 항목을 추가.)
     ***********************************************************************/
    [MenuItem("Assets/Build All AssetBundles")]
    static void BuildAllAssetBundles()
    {
        //폴더가 있는지 확인.
        CheckDirectory(path);

        /*********************************************************************** 
         * 이름 : BuildPipeLine.BuildAssetBundles()
         * 용도 : BuildPipeLine 클래스의 함수 BuildAssetBundles()는 에셋번들을 만들어줍니다.
         * 매개변수에는 String 값을 넘기게 되며, 빌드된 에셋 번들을 저장할 경로입니다.
         * 예를 들어 Assets 하위 폴더에 저장하려면 "Assets/AssetBundles"로 입력해야합니다.
         * Manifest File
         *      애셋번들을 Export 하면 Manifest도 함께 생성된다.
         *     이 파일은 애셋번들의 해시정보와 CRC 정보를 포함한다.
         *     CRC는 해당 애셋번들이 서버에 올라가고 클라이언트에 다운로드 되면서
         *     변조되지 않았는지 체크할 때 사용된다. 
         ***********************************************************************/
        BuildAssetBundleOptions option = BuildAssetBundleOptions.CollectDependencies    //의존관계의 리소스를 함께 압축한다.
            | BuildAssetBundleOptions.CompleteAssets
            | BuildAssetBundleOptions.DeterministicAssetBundle //리빌드시의 해시 변경을 막을 수 있었습니다.
            | BuildAssetBundleOptions.UncompressedAssetBundle;
        //EditorUtility.SaveFilePanel("", "", "", "unity3d");

        //빌드시 대상 platform 선택
        BuildPipeline.BuildAssetBundles(path, option, BuildTarget.Android);

        //생성된 asset bundle을 목록으로 기록한다.
        WriteBundleList(AssetBundleListFilename);
    }

    [MenuItem("Assets/Build One AssetBundle")]
    static void BuildOneAssetBundle()
    {
        //폴더가 있는지 확인.
        CheckDirectory(path);

        //빌드할 bundle name을 설정후 메뉴에서 실행해 주세요.
        string bundleName = "guitar2";

        /*********************************************************************** 
         * 이름 : BuildPipeLine.BuildAssetBundles()
         * 용도 : BuildPipeLine 클래스의 함수 BuildAssetBundles()는 에셋번들을 만들어줍니다.
         * 매개변수에는 String 값을 넘기게 되며, 빌드된 에셋 번들을 저장할 경로입니다.
         * 예를 들어 Assets 하위 폴더에 저장하려면 "Assets/AssetBundles"로 입력해야합니다.
         * Manifest File
         *      애셋번들을 Export 하면 Manifest도 함께 생성된다.
         *     이 파일은 애셋번들의 해시정보와 CRC 정보를 포함한다.
         *     CRC는 해당 애셋번들이 서버에 올라가고 클라이언트에 다운로드 되면서
         *     변조되지 않았는지 체크할 때 사용된다. 
         ***********************************************************************/
        AssetBundleBuild[] builds = new AssetBundleBuild[1];
        builds[0].assetBundleName = bundleName;
        builds[0].assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);


        BuildAssetBundleOptions option = BuildAssetBundleOptions.CollectDependencies    //의존관계의 리소스를 함께 압축한다.
            | BuildAssetBundleOptions.CompleteAssets
            | BuildAssetBundleOptions.DeterministicAssetBundle //리빌드시의 해시 변경을 막을 수 있었습니다.
            | BuildAssetBundleOptions.UncompressedAssetBundle;

        //빌드시 대상 platform 선택
        BuildPipeline.BuildAssetBundles(path, builds, option, BuildTarget.Android);

        //WriteBundleList을 여기에서는 호출하지 않습니다
        //생성된 AssetBundle이 리스트에 없는 경우에는 직접입력해 줍니다.
    }


    //==============================================
    //  selection개체를 사용해서 AssetBundle 생성하면
    //  manifest 안생기고, 번들명 지정해고, 등등
    //  번거러워서 폐쇄.
    //==============================================
    //[MenuItem("Assets/ExportResource")]
    //static void ExportResource()
    //{
    //    // 저장할 에셋번들 이름.
    //    // 경로가 없을 경우 최상위(Assets상위)에 저장된다.
    //    // 현재 저장경로는 Assets/myAssetBundle 이다.
    //    path += "/myAssetBundle";
    //    Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

    //    // 현재 선택된 항목
    //    // 게임오브젝트만 해당.
    //    // 만약 씬에 있는 항목이라면, activeObject대신 activeTransform 사용권장.
    //    if (Selection.activeObject != null)
    //        Debug.Log("Current : " + Selection.activeObject.name);
    //    // 선택된 모든(하위 포함) 항목들
    //    foreach (Object sel in selection)
    //    {
    //        Debug.Log("Path ; " + sel.ToString() + ", path :" + AssetDatabase.GetAssetPath(sel));
    //    }

    //    // Selection.activeObject가 들어간 곳이, mainAsset이 된다.
    //    // 세번째 인자는 에셋번들을 저장할 경로(파일명 포함)
    //    // 현재 테스트로 Android 플랫폼으로 하도록 한다.
    //    BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path,
    //                                   BuildAssetBundleOptions.CollectDependencies
    //                                 | BuildAssetBundleOptions.CompleteAssets, BuildTarget.Android);
    //}

    #endregion
}

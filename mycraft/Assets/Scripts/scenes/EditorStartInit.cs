using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

#if UNITY_EDITOR
[InitializeOnLoad]
public class EditorStartInit
{
    static EditorStartInit()
    {
        //int num = 0;
        var pathOfFirstScene = EditorBuildSettings.scenes[0].path; // 씬 번호를 넣어주자.
        var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathOfFirstScene);
        EditorSceneManager.playModeStartScene = sceneAsset;
        Debug.Log($"[{pathOfFirstScene}] 씬이 에디터 플레이 모드 시작 씬으로 지정됨");
    }
}
#endif
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
        var pathOfFirstScene = EditorBuildSettings.scenes[0].path; // �� ��ȣ�� �־�����.
        var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathOfFirstScene);
        EditorSceneManager.playModeStartScene = sceneAsset;
        Debug.Log($"[{pathOfFirstScene}] ���� ������ �÷��� ��� ���� ������ ������");
    }
}
#endif
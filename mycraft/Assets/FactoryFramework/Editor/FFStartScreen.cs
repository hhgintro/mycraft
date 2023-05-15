using System;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

using FactoryFramework;

[InitializeOnLoad()]
public class FFStartup
{
    static FFStartup()
    {
        var ChangeLogGUID = AssetDatabase.FindAssets("FF_ChangeLog")[0];
        var changelog = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(ChangeLogGUID));
        if (changelog != null && changelog.text.StartsWith("~"))
        {
            FFStartScreen.Init();
            string newText = changelog.text.Substring(1);
            File.WriteAllText(AssetDatabase.GUIDToAssetPath(ChangeLogGUID), newText);
            EditorUtility.SetDirty(changelog);
        } 

    }
}

public class FFStartScreen : EditorWindow
{
    [MenuItem("Window/FactoryFramework/Start Screen", false, 1997)]
    public static void Init()
    {
        // code just used to find guids\
        ChangeLogGUID = AssetDatabase.FindAssets("FF_ChangeLog")[0];
        //URPPackageGUID = AssetDatabase.FindAssets("FF_URP_demo")[0];
        //URPPackageGUID = AssetDatabase.FindAssets("FF_HDRP_demo")[0];

        FFStartScreen window = GetWindow<FFStartScreen>("Factory Framework");
        window.minSize = new Vector2(300, 250);
        window.maxSize = new Vector2(300, 250);
        window.Show(true);
    }

    private static string ChangeLogGUID;
    private static string URPPackageGUID;
    private static string HDRPPackageGUID;

    private static readonly string OnlineDocumentationURL = "https://docs.google.com/document/d/1U2zVDWSDcqG6s1sXl5qliNWZ3GclAbTwtYuxOxIfalg/edit?usp=sharing";
    private static readonly string AssetStoreURL = "https://u3d.as/2NNm";
    private static readonly string TwitterURL = "https://twitter.com/CtrlAltBees";
    private static readonly string DiscordURL = "https://discord.gg/9tnKg9XPpV";

  
    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/FactoryFramework/Editor/FFStartScreen.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        // handle Render Pipeline Imports
        var urpButton = root.Q<Button>("urp");
        //urpButton.RegisterCallback<ClickEvent>((evt) =>
        //{
        //    ImportSample(urpButton.text, URPPackageGUID);
        //});
        urpButton.parent.Remove(urpButton);
        var hdrpButton = root.Q<Button>("hdrp");
        //hdrpButton.RegisterCallback<ClickEvent>((evt) =>
        //{
        //    ImportSample(hdrpButton.text, HDRPPackageGUID);
        //});
        hdrpButton.parent.Remove(hdrpButton);

        // online documentation link(s)
        var onlineDocsButton = root.Q<Button>("docs");
        onlineDocsButton.RegisterCallback<ClickEvent>((evt) => Application.OpenURL(OnlineDocumentationURL));

            //settings open
        var settingsButton = root.Q<Button>("settings");
        settingsButton.RegisterCallback<ClickEvent>((evt) => SettingsService.OpenProjectSettings("Project/Factory Framework"));

        // community links
        var twitterLink = root.Q<Button>("twitter");
        twitterLink.RegisterCallback<ClickEvent>((evt) => Application.OpenURL(TwitterURL));
        var discordLink = root.Q<Button>("discord");
        discordLink.RegisterCallback<ClickEvent>((evt) => Application.OpenURL(DiscordURL));

        // get current version
        var changelog = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(ChangeLogGUID));
        Regex r = new Regex(@"v(\d+\.\d+\.\d)", RegexOptions.IgnoreCase);
        Match m = r.Match(changelog.text);
        Label currentVersion = root.Q<Label>("current-version");
        currentVersion.text = m.Value.ToString();

        // download version
        Label assetStoreLink = root.Q<Label>("asset-store-link");
        assetStoreLink.RegisterCallback<ClickEvent>((evt) =>
            {
                Application.OpenURL(AssetStoreURL);
            });
    }

    void ImportSample(string pipeline, string guid)
    {
        if (EditorUtility.DisplayDialog("Import Compatible Render Pipeline Assets", "Import the samples for " + pipeline + ", make sure the pipeline is properly installed before importing the samples.\n\nContinue?", "Yes", "No"))
        {
            AssetDatabase.ImportPackage(AssetDatabase.GUIDToAssetPath(guid), false);
        }
    }
}

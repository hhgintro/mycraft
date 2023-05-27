using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
#endif

namespace FactoryFramework
{
	/// <summary>
	/// ScriptableObject with a string reference to the AssetDatabase GUID of the object.
	/// </summary>
	/// <remarks>Using the string GUID allows for easier serialization operations</remarks>
    public class SerializeableScriptableObject : ScriptableObject
    {
		[SerializeField, HideInInspector] private string _guid;
		public string Guid => _guid;
		[SerializeField, HideInInspector]
		public string resourcesPath = "";


#if UNITY_EDITOR
    static Regex rx = new Regex(@".*Resources/(.*)\.asset");
        void Register()
        {
            // find the path from resources to this obj
            MatchCollection matches = rx.Matches(AssetDatabase.GetAssetPath(this));
            if (matches.Count > 0)
            {
                resourcesPath = matches[0].Groups[1].Value;
            }

            var path = AssetDatabase.GetAssetPath(this);
            _guid = AssetDatabase.AssetPathToGUID(path);
        }
        void OnValidate()
		{
            Register();
		}
        void Awake()
        {
            Register();
        }
#endif
	}

}
using System;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using ScriptableObjectsFromSheets.Utils;
using UnityEditor;
using UnityEngine;

namespace ScriptableObjectsFromSheets.APIIntegration
{
    public class AuthWindow : EditorWindow
    {
        private string _credentialsPath;
        
        [MenuItem("SOFromSheets/Authentication")]
        public static void ShowWindow()
        {
            var window = GetWindow<AuthWindow>("Scriptable Object Manager");
            window.minSize = new Vector2(640, 480);
        }

        private void OnEnable()
        {
            _credentialsPath = PlayerPrefs.GetString(AccountParameters.PlayerPrefsKey, "");
        }

        public void OnGUI() 
        {
            UIUtils.DrawHeaderBar("Authentication");
            
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.LabelField("Credentials File", GUILayout.Width(EditorGUIUtility.labelWidth));
            _credentialsPath = EditorGUILayout.TextField(_credentialsPath);
            if (GUILayout.Button("Browse", UIUtils.ClassSetupButtonLayout))
            {
                string selectedPath = EditorUtility.OpenFilePanel("Select the downloaded JSON file with the Service Account crdentials", _credentialsPath, "json");
                if (!string.IsNullOrEmpty(selectedPath)) _credentialsPath = selectedPath;
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            
            if (string.IsNullOrEmpty(_credentialsPath)) GUI.enabled = false;
            if (GUILayout.Button("Test Credentials", UIUtils.ClassSetupButtonLayout))
            {
                TestCredentials();
            }
            if (GUILayout.Button("Save file path", UIUtils.ClassSetupButtonLayout))
            {
                AccountParameters.UpdateServiceAccountJsonPath(_credentialsPath);
                EditorUtility.DisplayDialog("Success", "Credential file path successfully saved.\n\nRemember to update file path through this screen if you change file location.", "OK");
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }
        
        private void TestCredentials()
        {
            try
            {
                ServiceAccountCredential credential;
                using (var stream =
                       new FileStream(_credentialsPath, FileMode.Open, FileAccess.Read))
                {
                    credential = (ServiceAccountCredential)GoogleCredential.FromStream(stream)
                        .CreateScoped(SheetsService.Scope.Spreadsheets)
                        .UnderlyingCredential;
                }

                BaseClientService.Initializer baseServiceInitializer = new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "ScriptableObjectsFromSheets"
                };

                var service = new SheetsService(baseServiceInitializer);
                
                EditorUtility.DisplayDialog("Success", "Credentials are valid", "OK");
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Error", $"Failed to validate credentials\n\n{e.Message}", "OK");
                Debug.LogError($"Error intializing sheet service with provided credentials: {e.Message}");
            }
        }
    }
}
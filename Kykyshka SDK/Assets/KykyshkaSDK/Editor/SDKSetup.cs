using System;
using System.IO;
using KykyshkaSDK.Core;
using KykyshkaSDK.Models;
using UnityEditor;
using UnityEngine;

namespace KykyshkaSDK.Editor
{
    /// <summary>
    /// SDK Setup Window
    /// </summary>
    internal class SDKSetup : EditorWindow
    {
        // Window Scroll
        private Vector2 _scrollPosition = Vector2.zero;
        private static EditorWindow _sdkWindow;
        
        // States
        private bool _isOptionsLoaded = false;
        private SDKOptions _options = new SDKOptions();
        private bool _isToggle = false;
        private string _userId = "";
        private string _appKey = "";
        
        /// <summary>
        /// Show SDK Setup Window
        /// </summary>
        [MenuItem("Kykyshka SDK/Setup Wizard", false, 0)]
        internal static void ShowWindow()
        {
            _sdkWindow = GetWindow<SDKSetup>(false, "Kykyshka SDK Setup Wizard", true);
            _sdkWindow.minSize = new Vector2(500, 200);
            _sdkWindow.maxSize = new Vector2(500, 800);
        }
        
        /// <summary>
        /// On Lost Window Focus
        /// </summary>
        private void OnLostFocus()
        {
            SaveSDKSetup();
        }

        /// <summary>
        /// Load SDK Setup
        /// </summary>
        private void OnFocus()
        {
            LoadSDKSetup();
        }

        /// <summary>
        /// On Draw Window GUI
        /// </summary>
        private void OnGUI()
        {
            if(!_isOptionsLoaded)
                LoadSDKSetup();
            
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true, GUILayout.Width(500));

            // Draw Logo
            GUILayout.Box(Resources.Load("Logos/Editor") as Texture2D, GetLogoStyle());
            
            // Draw Space
            if (GUILayout.Button("Open Documentation", GetButtonStyle())){
                Application.OpenURL(Constants.SDKGit);
            }
            if (GUILayout.Button("Open GitHub", GetButtonStyle())){
                Application.OpenURL(Constants.SDKDocumentation);
            }
            
            DrawLine(1);
            
            GUILayout.Label("General SDK Setup", GetHeadlineStyle());
            _isToggle = EditorGUILayout.Toggle("Debug Mode:", _isToggle);
            _appKey = EditorGUILayout.TextField("App Key: ", _appKey);
            _userId = EditorGUILayout.TextField("User ID: ", _userId);

            DrawLine(1);
            
            if (GUILayout.Button("Save Settings", GetButtonStyle()))
            {
                SaveSDKSetup();
            }

            GUILayout.EndScrollView();
        }

        /// <summary>
        /// Load SDK Options
        /// </summary>
        private void LoadSDKSetup()
        {
            // Load Asset from Resources
            TextAsset loadedData = Resources.Load<TextAsset>(Constants.ConfigResourcePath);
            if (loadedData == null || string.IsNullOrEmpty(loadedData.text))
                return;
            
            // Convert from JSON
            _options = JsonUtility.FromJson<SDKOptions>(loadedData.text);

            // Not Specified Already
            if (_options == null)
                return;

            _isToggle = _options.DebugMode;
            _userId = _options.UserID;
            _appKey = _options.AppKey;
            _isOptionsLoaded = true;
        }

        /// <summary>
        /// Save SDK Setup
        /// </summary>
        private void SaveSDKSetup()
        {
            // Setup Options
            _options.DebugMode = _isToggle;
            _options.UserID = _userId;
            _options.AppKey = _appKey;
            
            // Setup File Data
            string fileData = JsonUtility.ToJson(_options);
            
            // Save to File
            string path = $"Assets/KykyshkaSDK/Resources/{Constants.ConfigResourcePath}.json";
            string str = fileData;
            using (FileStream fs = new FileStream(path, FileMode.Create)){
                using (StreamWriter writer = new StreamWriter(fs)){
                    writer.Write(str);
                }
            }
            
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh ();
            #endif
        }

        /// <summary>
        /// Get Logo Style
        /// </summary>
        /// <returns></returns>
        private GUIStyle GetLogoStyle()
        {
            GUIStyle logoStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                border = new RectOffset(0, 0, 0, 0),
                fixedWidth = 487
            };
            logoStyle.margin = new RectOffset(0, 0, 0, 0);
            logoStyle.overflow = new RectOffset(0, 0, 0, 0);
            logoStyle.padding = new RectOffset(0, 0, 0, 0);

            return logoStyle;
        }
        
        /// <summary>
        /// Get Button Style
        /// </summary>
        /// <returns></returns>
        private GUIStyle GetButtonStyle()
        {
            GUIStyle buttonStyle = new GUIStyle()
            {
                fixedHeight = 40,
                alignment = TextAnchor.MiddleCenter,
                border = new RectOffset(1,1,1,1),
                fontSize = 16,
                fontStyle = FontStyle.Bold
            };
            buttonStyle.normal.textColor = Color.white;
            buttonStyle.normal.background = Resources.Load("Logos/Button") as Texture2D;
            buttonStyle.margin = new RectOffset(15, 15, 15, 15);
            
            buttonStyle.active.textColor = Color.white;
            buttonStyle.active.background = Resources.Load("Logos/ButtonHover") as Texture2D;

            return buttonStyle;
        }

        /// <summary>
        /// Get Headline Style
        /// </summary>
        /// <returns></returns>
        private GUIStyle GetHeadlineStyle()
        {
            GUIStyle headlineStyle = new GUIStyle()
            {
                fixedHeight = 40,
                alignment = TextAnchor.MiddleCenter,
                fontSize = 20,
                fontStyle = FontStyle.Bold
            };
            headlineStyle.normal.textColor = Color.white;

            return headlineStyle;
        }

        /// <summary>
        /// Draw Separator Line
        /// </summary>
        /// <param name="height"></param>
        private void DrawLine(int height)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, height );
            rect.height = height;
            EditorGUI.DrawRect(rect, new Color ( 0.5f,0.5f,0.5f, 0.5f ) );
        }
    }
}
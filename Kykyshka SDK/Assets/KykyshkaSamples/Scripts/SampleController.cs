using System;
using KykyshkaSDK;
using KykyshkaSDK.Models;
using UnityEngine;
using UnityEngine.UI;

namespace KykyshkaSamples.Scripts
{
    /// <summary>
    /// Sample SDK Controller
    /// </summary>
    internal class SampleController : MonoBehaviour
    {
        [Header("Example Setup")] 
        [SerializeField] private InputField _userId;
        [SerializeField] private InputField _appKey;

        [Header("SDK Buttons")] 
        [SerializeField] private Button _preloadSurvey;
        [SerializeField] private Button _showSurvey;
        
        [Header("SDK Status")] 
        [SerializeField] private Text _sdkStatus;

        // SDK Instance
        private Kykyshka _sdkInstance;
        
        /// <summary>
        /// On Game Started
        /// </summary>
        private void Start()
        {
            SetStatus("SDK Initialization...");

            // Initialize SDK
            //SimpleInitialization();                 // For Setup Wizard Initialization
            CustomInitialization();               // For By-Code Initialization

            // Write SDK Settings to game UI
            SetAppKey(_sdkInstance.AppKey);
            SetUserId(_sdkInstance.UserId);
            SetStatus("SDK is Initialized!");
            
            // Add Survey Callbacks
            _sdkInstance.OnSurveyStart = () =>              // On Survey Started
            {
                SetStatus("Survey Started!");
            };
            _sdkInstance.OnSuccess = hq =>              // On Survey Complete
            {
                SetStatus("Survey Complete!");
            };
            _sdkInstance.OnFail = data =>         // On Survey Failed
            {
                SetStatus($"Survey Error", true);
            };
            _sdkInstance.OnLoadFail = () =>                 // On Loading Failed
            {
                SetStatus($"Survey Loading Error", true);
            };
            
            // Add Preloading Callbacks
            _sdkInstance.OnSurveyAvailable = () =>          // On Survey Available
            {
                SetStatus($"Survey Preloading Complete");
            };
            _sdkInstance.OnSurveyUnavailable = () =>        // On Survey Unavailable
            {
                SetStatus($"Survey Preloading Error", true);
            };
            
            // Add UI Button Listeners
            _preloadSurvey.onClick.AddListener(() =>
            {
                _sdkInstance.HasSurvey();
                SetStatus("Preloading started...");
            });
            _showSurvey.onClick.AddListener(() =>
            {
                _sdkInstance.ShowSurvey();
                SetStatus("Starting showing survey");
            });
            
            // Change User ID and AppKey at Runtime
            _userId.onValueChanged.AddListener(value =>
            {
                _sdkInstance.SetUserID(value);
            });
            _appKey.onValueChanged.AddListener(value =>
            {
                _sdkInstance.SetPublicKey(value);
            });
        }

        /// <summary>
        /// Simple SDK Initialization using Setup Wizard
        /// </summary>
        private void SimpleInitialization()
        {
            _sdkInstance = new Kykyshka();              // Use this way if you want to use settings from resources
            //_sdkInstance.SetUserID(your_game_user_id);         // Setup user ID from your game or will be automatically generated
            SetAppKey(_sdkInstance.UserId);
            SetUserId(_sdkInstance.AppKey);
        }

        /// <summary>
        /// Custom Initialization by code
        /// </summary>
        private void CustomInitialization()
        {
            SetAppKey("gamedemo");
            SetUserId("gamedemo");
            
            _sdkInstance = new Kykyshka(new SDKOptions
            {
                DebugMode = false,                  // Debug Mode
                AppKey = _appKey.text,              // Application Key
                UserID = _userId.text               // User ID
            });
        }

        /// <summary>
        /// On Destroy
        /// </summary>
        private void OnDestroy()
        {
            _preloadSurvey.onClick.RemoveAllListeners();
            _showSurvey.onClick.RemoveAllListeners();
            _userId.onValueChanged.RemoveAllListeners();
            _appKey.onValueChanged.RemoveAllListeners();
        }

        /// <summary>
        /// Setup SDK Status
        /// </summary>
        /// <param name="status"></param>
        /// <param name="isError"></param>
        private void SetStatus(string status, bool isError = false)
        {
            _sdkStatus.text = status;
            _sdkStatus.color = (isError) ? Color.red : new Color(0.4f, 0.4f, 0.4f, 1f);
        }

        /// <summary>
        /// Set App Key
        /// </summary>
        /// <param name="appKey"></param>
        private void SetAppKey(string appKey)
        {
            _appKey.text = appKey;
        }

        /// <summary>
        /// Set User Id
        /// </summary>
        /// <param name="userId"></param>
        private void SetUserId(string userId)
        {
            _userId.text = userId;
        }
    }
}
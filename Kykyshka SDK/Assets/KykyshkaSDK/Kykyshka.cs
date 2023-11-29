using System;
using KykyshkaSDK.Core;
using KykyshkaSDK.Core.Encryption;
using KykyshkaSDK.Models;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KykyshkaSDK
{
    /// <summary>
    /// Kykyshka SDK General Class
    /// </summary>
    public class Kykyshka
    {
        // SDK Setup
        private bool _isInitialized = false;            // Is SDK Initialized
        private SDKOptions _currentSetup = null;        // Current SDK Options
        private string _additionalUrlParams = "";       // Additional Request Params

        // Platform Setup
        private Platform _currentPlatfrom;              // Current SDK Platform

        // Public SDK Properties
        public bool Initialized => _isInitialized;              // Is SDK Initialized
        public Platform CurrentPlatform => _currentPlatfrom;    // Get SDK Platfrom
        public string UserId => _currentSetup.UserID;           // SDK User ID
        public string AppKey => _currentSetup.AppKey;           // SDK Application Key
        public bool IsDebug => _currentSetup.DebugMode;         // SDK Debug Mode

        
        // Survey Options
        private string _lastSurvey = "";                      // Last Survey ID

        // Survey Timer
        private bool _isTimerStarted = false;               // Is Survey Timer Started
        private float _surveyTimer = 0f;                    // Survey Timer
        private float _checkTimer = 1f;                     // Checking Timer

        // Survey Timeout
        private bool _preloadTimeout = false;               // Is Preload timeout started
        private float _currentTimeout = 0f;                 // Current Timeout
        private float _maxTimeout = 10f;                    // Maximal Timeout
        
        // Preloaded Survey Timeout
        private float _preloadedTimeout = 600f;             // Preloaded Survey Timeout

        // Current Survey Parameters
        private bool _hasSurveyCalled = false;              // Is Survey Called
        private bool _hasSurveyResult = false;              // Has Survey Result
        private bool _isSurveyShowing = false;              // Is Survey Showing

        // SDK Callbacks
        public Action OnInitialized;                        // On SDK Initialized Callback
        public Action<string> OnInitializationError;        // On SDK Initialization Error Callback
        public Action<string> OnError;                      // On General SDK Errors Callback
        
        public Action OnSurveyAvailable;                    // On Survey Available Callback
        public Action OnSurveyUnavailable;                  // On Survey Unavailable Callback
        public Action OnSurveyStart;                        // On Survey Start Callback
        public Action<int?> OnSuccess;                      // On Survey Success Callback (with IsQualified param)
        public Action<SurveyResult> OnFail;                 // On Survey Fail Callback
        public Action OnLoadFail;                           // On Loading Fail Callback
        
        // Wrapper Object
        private KykyshkaWrapper _wrapper;                   // Wrapper Object

        /// <summary>
        /// Initialize SDK
        /// </summary>
        /// <param name="options"></param>
        public Kykyshka(SDKOptions options = null, Action onInitialized = null, Action<string> initializationError = null)
        {
            if (onInitialized != null)
            {
                OnInitialized = onInitialized;
            }
            if (initializationError != null)
            {
                OnInitializationError = initializationError;
            }
            // Initialize SDK Options
            if (options == null)
                InitializeFromResources();
            else
                _currentSetup = options;

            if (_currentSetup.DebugMode && string.IsNullOrEmpty(_currentSetup.AppKey))
                _currentSetup.AppKey = "gamedemo";
            
            // Get Platform
            _currentPlatfrom = GetPlatfrom();
            
            // Validate Application Key
            if (!ValidationUtil.ValidateAppKey(_currentSetup.AppKey))
            {
                if(_currentSetup.DebugMode)
                    throw new Exception(SDKCodes.WrongAppKey);
                else
                {
                    OnInitializationError?.Invoke(SDKCodes.WrongAppKey);
                    return;
                }
            }

            // Validate User ID
            if(string.IsNullOrEmpty(_currentSetup.UserID))
                _currentSetup.UserID = PlayerPrefs.GetString(Constants.UserIDStorageKey, "");
            
            _lastSurvey = PlayerPrefs.GetString(Constants.LastSurveyKey, "");
            if (!ValidationUtil.ValidateUserID(_currentSetup.UserID))
            {
                if (_currentSetup.DebugMode)
                    Debug.LogWarning(SDKCodes.UserIdNotSpecified);

                _currentSetup.UserID = GetGeneratedUserID();
            }
            else
                SetUserID(_currentSetup.UserID);
            
            if(_currentSetup.DebugMode)
                Debug.Log(string.Format(SDKCodes.SDKInitializationMessage, _currentSetup.UserID, _currentSetup.AppKey));

            // Set Initialized Flag
            PreloadSDK();
        }

        /// <summary>
        /// SDK Destructor
        /// </summary>
        ~Kykyshka()
        {
            // Cleanup Wrapper
            if (_wrapper != null)
            {
                _wrapper.Dispose();
                Object.Destroy(_wrapper.gameObject);
                _wrapper = null;
            }
        }

        /// <summary>
        /// Clear User Storage
        /// </summary>
        public void ClearStorage()
        {
            PlayerPrefs.SetString(Constants.UserIDStorageKey, "");
            PlayerPrefs.SetString(Constants.LastSurveyKey, "");
        }

        /// <summary>
        /// Set User ID
        /// </summary>
        /// <param name="userId"></param>
        public void SetUserID(string userId)
        {
            // Validate User ID
            if (!ValidationUtil.ValidateUserID(userId))
            {
                OnError?.Invoke(SDKCodes.WrongUserId);
                return;
            }
            
            _currentSetup.UserID = userId;
            PlayerPrefs.SetString(Constants.UserIDStorageKey, userId);
        }

        /// <summary>
        /// Set Public Key
        /// </summary>
        /// <param name="appKey"></param>
        public void SetPublicKey(string appKey)
        {
            // Validate User ID
            if (!ValidationUtil.ValidateAppKey(appKey))
            {
                OnError?.Invoke(SDKCodes.WrongAppKey);
                return;
            }
            
            _currentSetup.AppKey = appKey;
        }

        /// <summary>
        /// Set Last Survey ID
        /// </summary>
        /// <param name="surveyId"></param>
        public void SetLastSurvey(string surveyId)
        {
            _lastSurvey = surveyId;
            PlayerPrefs.SetString(Constants.LastSurveyKey, _lastSurvey);
        }

        /// <summary>
        /// Show Survey
        /// </summary>
        public void ShowSurvey()
        {
            if (!_wrapper.IsWebViewShown && !_isSurveyShowing)
            {
                if (!_hasSurveyResult)
                    PrepareWebview();
                else
                {
                    _wrapper.PostMessage("getTime", "*");
                    _wrapper.ShowWebView(true);
                    ResetSurveyTimer();
                    StartSurveyTimer(true);
                    ResetTimeout();
                    StartTimeout(false);
                    ResetPreloadedTimeout();
                    OnSurveyStart?.Invoke();
                }

                _hasSurveyCalled = false;
                _hasSurveyResult = false;
                _isSurveyShowing = true;
            }
            else
            {
                if(_currentSetup.DebugMode)
                    Debug.LogWarning(SDKCodes.IsSurveyAlreadyOpened);
            }
        }

        /// <summary>
        /// Prepare WebView
        /// </summary>
        private void PrepareWebview()
        {
            _wrapper.SetURL(Constants.SurveyURL + GetURLString());
            ResetTimeout();
            StartTimeout(true);
        }

        /// <summary>
        /// Preload Survey
        /// </summary>
        public void HasSurvey()
        {
            if (!_hasSurveyCalled)
            {
                if (!_wrapper.IsWebViewShown)
                {
                    _hasSurveyCalled = true;
                    ResetPreloadedTimeout();
                    PrepareWebview();
                }
                else
                {
                    if(_currentSetup.DebugMode)
                        Debug.LogWarning(SDKCodes.IsSurveyAlreadyOpened);
                }
            }
            else
            {
                if(_currentSetup.DebugMode)
                    Debug.LogWarning(SDKCodes.IsSurveyAlreadyPreloading);
            }
        }

        /// <summary>
        /// Set Additional URL Parameters
        /// </summary>
        /// <param name="additionalParameters"></param>
        public void SetURLParams(string additionalParameters)
        {
            _additionalUrlParams = additionalParameters;
        }

        /// <summary>
        /// Get URL String
        /// </summary>
        /// <returns></returns>
        private string GetURLString()
        {
            string urlString = $"?isWebView=1&send_from_page=1&platform={(int)_currentPlatfrom}&gid={AppKey}&uid={UserId}&version={Constants.SurveyVersion}&lastSurvey={_lastSurvey}&surveyUrlParams={_additionalUrlParams}";
            return urlString;
        }
        
        /// <summary>
        /// Trying to Initialize from Resources
        /// </summary>
        private void InitializeFromResources()
        {
            if(_currentSetup is {DebugMode: true})
                return;
            
            // Load Asset from Resources
            TextAsset loadedData = Resources.Load<TextAsset>(Constants.ConfigResourcePath);
            if (loadedData == null || string.IsNullOrEmpty(loadedData.text))
                throw new Exception(
                    SDKCodes.FailedToLoadSDKSettings);
            
            // Convert from JSON
            _currentSetup = JsonUtility.FromJson<SDKOptions>(loadedData.text);

            // Not Specified Already
            if (_currentSetup == null)
                throw new Exception(
                    SDKCodes.SDKNotSetup);
        }

        /// <summary>
        /// Get Generated User ID
        /// </summary>
        /// <returns></returns>
        private string GetGeneratedUserID()
        {
            string userId = PlayerPrefs.GetString(Constants.UserIDStorageKey, "");
            if (!ValidationUtil.ValidateUserID(userId))
            {
                userId = (GetTimestamp() * 1000).ToString();
                userId = new MD5().EncodeString(userId);
                PlayerPrefs.SetString(Constants.UserIDStorageKey, userId);
            }

            return userId;
        } 

        /// <summary>
        /// Initialize All SDK Modules
        /// </summary>
        private void PreloadSDK()
        {
            // Initialize Kykyshka Wrapper
            if (_wrapper == null)
            {
                GameObject wrapperObject = new GameObject(Constants.MainWrapperName);
                wrapperObject.AddComponent<KykyshkaWrapper>();
                _wrapper = wrapperObject.GetComponent<KykyshkaWrapper>();
                _wrapper.OnUpdate = OnUpdate;
                _wrapper.OnMessage = OnMessage;
            }

            // SDK Is Initialized
            _isInitialized = true;
            OnInitialized?.Invoke();
        }
        
        /// <summary>
        /// On Update
        /// </summary>
        /// <param name="deltaTime"></param>
        private void OnUpdate(float deltaTime)
        {
            if(_preloadTimeout) PreloadTimeout(deltaTime);
            if (_isTimerStarted) SurveyTimerTick(deltaTime);
            if(_hasSurveyCalled && !_wrapper.IsWebViewShown) PreloadedTimeoutTick(deltaTime);
        }

        /// <summary>
        /// Reset Timeout
        /// </summary>
        private void ResetTimeout()
        {
            _currentTimeout = _maxTimeout;
            _preloadTimeout = false;
        }

        /// <summary>
        /// Reset preloaded Timeout
        /// </summary>
        private void ResetPreloadedTimeout()
        {
            _preloadedTimeout = 600f;
        }

        /// <summary>
        /// Start Timeout
        /// </summary>
        private void StartTimeout(bool isStarted)
        {
            _preloadTimeout = isStarted;
        }
        
        /// <summary>
        /// Preload Timeout
        /// </summary>
        private void PreloadTimeout(float deltaTime)
        {
            if (_currentTimeout <= 0f)
            {
                StartTimeout(false);
                ResetTimeout();
                OnLoadFail?.Invoke();
            }
            else
            {
                _currentTimeout -= deltaTime;
            }
        }

        /// <summary>
        /// Preloaded Timeout Tick
        /// </summary>
        /// <param name="deltaTime"></param>
        private void PreloadedTimeoutTick(float deltaTime)
        {
            if (_preloadedTimeout <= 0f)
            {
                _hasSurveyCalled = false;
                _wrapper.ShowWebView(false);
                _isSurveyShowing = false;
                _hasSurveyCalled = false;
                _hasSurveyResult = false;
                ResetPreloadedTimeout();
            }
            else
            {
                _preloadedTimeout -= deltaTime;
            }
        }

        /// <summary>
        /// Start Survey Timer
        /// </summary>
        /// <param name="isStarted"></param>
        private void StartSurveyTimer(bool isStarted)
        {
            _isTimerStarted = isStarted;
        }
        
        /// <summary>
        /// Reset Survey Timer
        /// </summary>
        private void ResetSurveyTimer()
        {
            _surveyTimer = 0f;
            _checkTimer = 1f;
        }
        
        /// <summary>
        /// Survey Timer Tick
        /// </summary>
        /// <param name="deltaTime"></param>
        private void SurveyTimerTick(float deltaTime)
        {
            if (_checkTimer <= 0f)
            {
                _checkTimer = 1f;
                _surveyTimer += 1f;
            }
            else
            {
                _checkTimer -= deltaTime;
            }
        }

        /// <summary>
        /// On Message Received
        /// </summary>
        /// <param name="message"></param>
        private void OnMessage(string message)
        {
            // Received Error
            if (message == Constants.MessageError)
            {
                //if(_currentSetup.DebugMode)
                    Debug.LogError(SDKCodes.FailedToParseMessage);
                OnFail?.Invoke(null);
                return;
            }
            
            // Trying to parse response
            try
            {
                SurveyResult data = JsonUtility.FromJson<SurveyResult>(message);
                if (data == null || (data.customData == null && data.surveyMaster == null))
                {
                    //if(_currentSetup.DebugMode)
                        Debug.LogError(SDKCodes.EmptyDataReceived);
                    OnFail?.Invoke(null);
                    return;
                }

                if (data.customData != null && data.customData.data != null &&
                    !string.IsNullOrEmpty(data.customData.data.link))
                {
                    Application.OpenURL(data.customData.data.link);
                    return;
                }

                if(!string.IsNullOrEmpty(data.surveyMaster.@event))
                    ProcessCallback(data);
                else
                {
                    if(_currentSetup.DebugMode)
                        Debug.LogError(SDKCodes.FailedToProcessCallback);
                }
            }
            catch
            {
                if(_currentSetup.DebugMode)
                    Debug.LogError(SDKCodes.FailedToParseMessage);
                //OnFail?.Invoke(null);
            }
        }

        /// <summary>
        /// Process Survey callback
        /// </summary>
        /// <param name="data"></param>
        private void ProcessCallback(SurveyResult data)
        {
            switch (data.surveyMaster.@event)
            {
                case "onAnswersReady":
                    onAnswersReady(data);
                    break;
                case "onSuccess":
                    onSuccess(data);
                    break;
                case "onFail":
                    onFail(data);
                    break;
                case "onLoadFail":
                    onLoadFail(data);
                    break;
                case "onPageReady":
                    onPageReady(data);
                    break;
                case "onSurveyAvailable":
                    onSurveyAvailable(data);
                    break;
                case "onSurveyUnavailable":
                    onSurveyUnavailable(data);
                    break;
                default:
                    OnUnknownEvent(data);
                    break;
            }
        }

        /// <summary>
        /// On Answers Ready
        /// </summary>
        /// <param name="data"></param>
        private void onAnswersReady(SurveyResult data)
        {
            StartSurveyTimer(false);
            data.surveyMaster.data.body.user_id = UserId;
            data.surveyMaster.data.body.app_key = AppKey;
            data.surveyMaster.data.body.time_spent = Mathf.RoundToInt(_surveyTimer);
            
            ResetSurveyTimer();
            SetLastSurvey(data.surveyMaster.data.body.survey_key);

            // Send Request to Server
            /*AnswerRequest reqData = new AnswerRequest
            {
                body = data.surveyMaster.data.body
            };
            _wrapper.SendRequest(Constants.CallbackURL + "answer", JsonUtility.ToJson(reqData), res =>
            {
                if(_currentSetup.DebugMode)
                    Debug.Log($"Survey Response: {res}");
            }, error =>
            {
                if(_currentSetup.DebugMode)
                    Debug.Log($"Survey Error: {error}");
            });*/
        }
        
        /// <summary>
        /// On Survey Callback
        /// </summary>
        /// <param name="data"></param>
        private void onSuccess(SurveyResult data)
        {
            _wrapper.ShowWebView(false);
            _isSurveyShowing = false;
            _hasSurveyCalled = false;
            _hasSurveyResult = false;

            // Callback
            if (data.surveyMaster != null && data.surveyMaster.data.body.nq != null)
                OnSuccess?.Invoke(data.surveyMaster.data.body.nq);
            else
                OnSuccess?.Invoke(null);
        }

        /// <summary>
        /// On Survey Fail
        /// </summary>
        /// <param name="data"></param>
        private void onFail(SurveyResult data)
        {
            _wrapper.ShowWebView(false);
            _isSurveyShowing = false;
            OnFail?.Invoke(data);
        }
        
        /// <summary>
        /// On Loading Fail
        /// </summary>
        /// <param name="data"></param>
        private void onLoadFail(SurveyResult data)
        {
            if(_currentSetup.DebugMode)
                Debug.LogWarning(SDKCodes.SurveyLoadingFail);
            _wrapper.ShowWebView(false);
            _isSurveyShowing = false;
            _hasSurveyCalled = false;
            _hasSurveyResult = false;
            _wrapper.SetURL("");
            OnLoadFail?.Invoke();
        }

        /// <summary>
        /// On Page Ready
        /// </summary>
        /// <param name="data"></param>
        private void onPageReady(SurveyResult data)
        {
            ResetTimeout();
            StartTimeout(false);
            
            if (_hasSurveyCalled)
            {
                onSurveyAvailable(null);
            }
            else
            {
                _wrapper.PostMessage("getTime", "*");
                _wrapper.ShowWebView(true);
                ResetSurveyTimer();
                StartSurveyTimer(true);
                OnSurveyStart?.Invoke();
            }
        }
        
        /// <summary>
        /// On Survey Available
        /// </summary>
        /// <param name="data"></param>
        private void onSurveyAvailable(SurveyResult data)
        {
            if (_hasSurveyCalled && !_hasSurveyResult)
            {
                _hasSurveyResult = true;
                _wrapper.ShowWebView(false);
                OnSurveyAvailable?.Invoke();
            }
        }
        
        /// <summary>
        /// On Survey Unavailable
        /// </summary>
        /// <param name="data"></param>
        private void onSurveyUnavailable(SurveyResult data)
        {
            ResetTimeout();
            StartTimeout(false);
            _hasSurveyResult = false;
            _hasSurveyCalled = false;
            _wrapper.ShowWebView(false);
            _isSurveyShowing = false;
            OnSurveyUnavailable?.Invoke();
        }

        /// <summary>
        /// On Unknown Event
        /// </summary>
        private void OnUnknownEvent(SurveyResult data)
        {
            if(_currentSetup.DebugMode)
                Debug.LogWarning($"Failed to Process callback. Event name \"{data.surveyMaster.@event}\" is unknown.");
        }
        
        /// <summary>
        /// Get Platform
        /// </summary>
        /// <returns></returns>
        private Platform GetPlatfrom()
        {
            if (Application.platform == RuntimePlatform.Android)
                return Platform.Android;
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
                return Platform.iOS;
            else if (Application.platform == RuntimePlatform.tvOS ||
                     Application.platform == RuntimePlatform.OSXEditor ||
                     Application.platform == RuntimePlatform.OSXPlayer)
                return Platform.Mac;
            else if (Application.platform == RuntimePlatform.LinuxEditor ||
                     Application.platform == RuntimePlatform.LinuxPlayer ||
                     Application.platform == RuntimePlatform.Stadia)
                return Platform.Linux;
            else if (Application.platform == RuntimePlatform.WindowsEditor ||
                     Application.platform == RuntimePlatform.WindowsPlayer ||
                     Application.platform == RuntimePlatform.WSAPlayerX64 || 
                     Application.platform == RuntimePlatform.WSAPlayerX86 || 
                     Application.platform == RuntimePlatform.WSAPlayerARM)
                return Platform.Windows;

            return Platform.Unknown;
        }

        /// <summary>
        /// Get UnixTimestamp
        /// </summary>
        /// <returns></returns>
        private long GetTimestamp()
        {
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long currentTime = (long)(DateTime.UtcNow - epochStart).TotalSeconds;
            return currentTime;
        }
    }
}

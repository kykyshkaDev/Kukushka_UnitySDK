using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace KykyshkaSDK
{
    /// <summary>
    /// Kykyshka SDK Util Mono Wrapper
    /// </summary>
    internal class KykyshkaWrapper : MonoBehaviour
    {
        // Webview Object
        private WebViewObject webViewObject;            // WebView Object
        private bool _isWebViewShown = false;           // Is Currently WebView Shown
        
        // Wrapper Callbacks
        public Action<string> OnMessage;                // On WebView Message
        public Action<string> OnError;                  // On General Error
        public Action OnLoadStarted;                    // On Load Started
        public Action OnLoadComplete;                   // On Load Completed
        public Action<float> OnUpdate;                  // On GameLoop Update

        // Public Wrapper Parameters
        public bool IsWebViewShown => _isWebViewShown;
        
        // Latest Screen Orientation
        private ScreenOrientation _latestOrientation;

        /// <summary>
        /// On Wrapper Started
        /// </summary>
        private IEnumerator Start()
        {
            // Create WebView
            webViewObject = (new GameObject("_KYKYSHKA_WEBVIEW_")).AddComponent<WebViewObject>();
            webViewObject.Init(
                cb: (msg) =>
                {
                    Debug.Log("SDK WebView Message: " + msg);
                    OnMessage?.Invoke(msg);
                },
                err: (msg) =>
                {
                    OnError?.Invoke(msg);
                },
                httpErr: (msg) =>
                {
                    OnError?.Invoke(msg);
                },
                started: (msg) =>
                {
                    OnLoadStarted?.Invoke();
                    webViewObject.EvaluateJS(@"
                    window.addEventListener('message',(event) => {
                      try {
                        if (typeof event.data==='string')
                        {
                          Unity.call(event.data);
                        }else{
                           Unity.call(JSON.stringify(event.data));
                        }
                      }
                      catch(e) {
                        Unity.call('MESSAGE_ERROR');
                        return;
                      }
                  }, false);
                ");
                },
                hooked: (msg) =>
                {
                    Debug.Log(string.Format("CallOnHooked[{0}]", msg));
                },
                ld: (msg) =>
                {
                    OnLoadComplete?.Invoke();
                },
                enableWKWebView: true
            );
            
            #if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            webViewObject.bitmapRefreshCycle = 1;
            #endif
            
            // Disable Scrollbars
            webViewObject.SetScrollbarsVisibility(false);
            
            // Setup Fullscreen
            webViewObject.SetMargins(0, 0, 0, 0);
            webViewObject.SetTextZoom(100);
            ShowWebView(false);

            // Do not Destroy this wrapper
            DontDestroyOnLoad(this.gameObject);
            DontDestroyOnLoad(webViewObject.gameObject);
            yield break;
        }

        /// <summary>
        /// Post Message to WebView
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void PostMessage(string type, string data)
        {
            webViewObject.EvaluateJS("window.postMessage(\""+type+"\",\""+data+"\");");
        }
        
        /// <summary>
        /// Show Webview Object
        /// </summary>
        public void ShowWebView(bool isShown)
        {
            if (isShown)
            {
                _latestOrientation = Screen.orientation;
                Screen.orientation = ScreenOrientation.Portrait;
            }
            else
            {
                Screen.orientation = _latestOrientation;
            }

            _isWebViewShown = isShown;
            webViewObject.SetVisibility(isShown);
        }

        /// <summary>
        /// Set Webview URL
        /// </summary>
        /// <param name="url"></param>
        public void SetURL(string url)
        {
            webViewObject.LoadURL(url);
        }

        /// <summary>
        /// Dispose WebView
        /// </summary>
        public void Dispose()
        {
            webViewObject.SetVisibility(false);
            Destroy(webViewObject);
        }

        /// <summary>
        /// Send Request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="onComplete"></param>
        /// <param name="onError"></param>
        public void SendRequest(string url, string data, Action<string> onComplete = null, Action<string> onError = null)
        {
            StartCoroutine(SendWebRequest(url, data, onComplete, onError));
        }

        /// <summary>
        /// Send Web Request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="onComplete"></param>
        /// <param name="onError"></param>
        /// <returns></returns>
        private IEnumerator SendWebRequest(string url, string data, Action<string> onComplete = null, Action<string> onError = null)
        {
            var req = new UnityWebRequest(url, "POST");
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(data);
            req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            //Send the request then wait here until it returns
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success && req.result != UnityWebRequest.Result.InProgress)
            {
                onError?.Invoke(req.error);
            }
            else if(req.result == UnityWebRequest.Result.Success)
            {
                onComplete?.Invoke(req.downloadHandler.text);
            }
        }

        /// <summary>
        /// On Wrapper Update
        /// </summary>
        private void Update()
        {
            OnUpdate?.Invoke(Time.deltaTime);
        }
    }
}
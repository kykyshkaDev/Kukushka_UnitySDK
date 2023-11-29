using System.Collections;
using KykyshkaSDK.Models;
using UnityEngine;

namespace KykyshkaSDK
{
    public class KukushkaSample : MonoBehaviour
    {
        private Kykyshka sdkInstance;

        private void Awake()
        {
            sdkInstance = new Kykyshka(new SDKOptions
            {
                DebugMode = false, // Debug Mode — when active will always display a demo survey regardless of AppKey, UserID, your geo etc. Useful for testing
                AppKey = "gamedemo", // Application Key
                UserID = "demouserid" // User ID
            });

            // Add Survey Callbacks
            sdkInstance.OnSurveyStart = () => { Debug.Log("Started"); };
            sdkInstance.OnSuccess = hq => { Debug.Log("Success"); };
            sdkInstance.OnFail = data => { Debug.Log("Failed"); };
            sdkInstance.OnLoadFail = () => { Debug.Log("On Load Fail"); };

            // Add Preloading Callbacks
            sdkInstance.OnSurveyAvailable = () => { Debug.Log("Available"); };
            sdkInstance.OnSurveyUnavailable = () => { Debug.Log("Unavailable"); };

            // Error
            sdkInstance.OnError = message => { Debug.Log($"Error: {message}"); };
        }

        private void Start()
        {
            StartCoroutine(StartSurvey());
        }

        private IEnumerator StartSurvey()
        {
            yield return new WaitForSeconds(1);
            Debug.Log("HasSurvey");
            sdkInstance.HasSurvey();
            yield return new WaitForSeconds(1);
            Debug.Log("ShowSurvey");
            sdkInstance.ShowSurvey();
        }
    }
}
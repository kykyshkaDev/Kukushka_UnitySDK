﻿namespace KykyshkaSDK.Models
{
    /// <summary>
    /// Kykyshka SDK Options
    /// </summary>
    [System.Serializable]
    public class SDKOptions
    {
        public bool DebugMode = false;                  // Debug Mode
        public bool InnerMode = false;                  // Inner Mode
        public string UserID = "";                      // User ID
        public string AppKey = "";                      // Application Key
    }
}
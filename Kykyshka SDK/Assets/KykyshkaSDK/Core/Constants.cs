namespace KykyshkaSDK.Core
{
    /// <summary>
    /// SDK Constants
    /// </summary>
    public static class Constants
    {
        // SDK General Settings
        public const string SurveyURL = "https://survey.askgames.io/"; // https://survey.askgames.io/
        public const string CallbackURL = "https://api.kykyshka.ru/v1/service/survey/"; // https://api.kykyshka.ru/v1/service/survey/
        public const string SurveyVersion = "1.0.0";
        
        // SDK Wizard Constants
        public const string SDKGit = "https://github.com/TinyPlay/Kykyshka_UnitySDK";
        public const string SDKDocumentation = "https://github.com/TinyPlay/Kykyshka_UnitySDK/blob/main/README.md";
        
        // Configuration Paths
        public const string ConfigResourcePath = "KykyshkaSDK";
        public const string UserIDStorageKey = "KykyshkaUserID";
        public const string LastSurveyKey = "KykyshkaLastSurvey";

        // Message Error
        public const string MessageError = "MESSAGE_ERROR";
        
        // Wrapper Constants
        public const string MainWrapperName = "_KYKYSHKA_WRAPPER_";
    }
}
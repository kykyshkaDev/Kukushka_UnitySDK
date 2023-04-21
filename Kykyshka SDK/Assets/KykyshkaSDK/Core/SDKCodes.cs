namespace KykyshkaSDK.Core
{
    public class SDKCodes
    {
        public const string WrongAppKey = "Kykyshka SDK: Failed to initialize SDK. The Application Key is not specified or has wrong format.";
        public const string UserIdNotSpecified = "Kykyshka SDK: The User ID is not specified and will be used automatically generated";
        public const string WrongUserId = "Kykyshka SDK: Failed to setup User ID. Please, check user ID format and try again.";
        public const string FailedToLoadSDKSettings = "Kykyshka SDK: Failed to load SDK Settings from Resources. Please, Run SDK Master from Editor.";
        public const string SDKNotSetup = "Kykyshka SDK: Failed to Initialize SDK. Please, specify SDK Options or Run SDK Master from Editor.";

        public const string FailedToParseMessage = "Kykyshka SDK: Failed to initialize Survey. Failed to parse survey message";
        public const string EmptyDataReceived = "Kykyshka SDK: Failed to read Survey message. Empty server response received.";

        public const string IsSurveyAlreadyOpened = "Kykyshka SDK: Is Survey Window already shown";
        public const string IsSurveyAlreadyPreloading = "Kykyshka SDK: Is Survey preloading already started";
        public const string SurveyLoadingFail = "Kykyshka SDK: Survey Loading Failed";
        public const string FailedToProcessCallback = "Kykyshka SDK: Failed to Process callback. Event name is empty.";
        public const string SDKInitializationMessage = "Kykyshka SDK Setup: Current User ID is {0}, Application Key: {1}";
    }
}
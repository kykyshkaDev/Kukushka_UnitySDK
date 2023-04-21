namespace KykyshkaSDK.Core
{
    /// <summary>
    /// SDK Validation Util
    /// </summary>
    public static class ValidationUtil
    {
        /// <summary>
        /// Check Application Key
        /// </summary>
        /// <param name="appKey"></param>
        /// <returns></returns>
        public static bool ValidateAppKey(string appKey)
        {
            if (string.IsNullOrEmpty(appKey))
                return false;

            return true;
        }

        /// <summary>
        /// Validate User ID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool ValidateUserID(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return false;

            return true;
        }
    }
}
namespace KykyshkaSDK.Models
{
    /// <summary>
    /// Survey Data Model
    /// </summary>
    [System.Serializable]
    public class SurveyResult
    {
        public SurveyMaster surveyMaster = new SurveyMaster();
        public SurveyCustomData customData = new SurveyCustomData();
    }

    [System.Serializable]
    public class SurveyMaster
    {
        public string @event = "";
        public SurveyData data = new SurveyData();
    }

    [System.Serializable]
    public class SurveyData
    {
        public SurveyBody body = new SurveyBody();
    }

    [System.Serializable]
    public class SurveyBody
    {
        public int nq = 0;
        public int start_time = 0;
        public string token = "";
        public string user_id = "";
        public string app_key = "";
        public int time_spent = 0;
        public string survey_key = "";
    }

    [System.Serializable]
    public class SurveyCustomData
    {
        public SurveyCustomDataSubData data = new SurveyCustomDataSubData();
    }

    [System.Serializable]
    public class SurveyCustomDataSubData
    {
        public string link = "";
    }
}
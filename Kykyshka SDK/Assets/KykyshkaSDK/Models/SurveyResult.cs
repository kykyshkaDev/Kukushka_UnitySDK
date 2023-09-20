using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace KykyshkaSDK.Models
{
    /// <summary>
    /// Survey Data Model
    /// </summary>
    [System.Serializable]
    public class SurveyResult
    {
        public SurveyMaster surveyMaster = new SurveyMaster();
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
    public class SurveyAnswerObj
    {
        public string question_id;
#nullable enable
        [JsonProperty("array_of_uints",NullValueHandling = NullValueHandling.Ignore)]
        public ArrayOfUints? array_of_uints;
        [JsonProperty("array_of_strings",NullValueHandling = NullValueHandling.Ignore)]
        public ArrayOfStrings? array_of_strings;
#nullable disable
        public int targetId;
    }

    [System.Serializable]
    public class SurveyAnswerArray
    {
        public List<SurveyAnswerObj> answers = new List<SurveyAnswerObj>();
    }

    [System.Serializable]
    public class ArrayOfUints
    {
        public List<int> array = new List<int>();
#nullable enable
        [JsonProperty("own_answer",NullValueHandling = NullValueHandling.Ignore)]
        public string? own_answer;
#nullable disable
    }

    [System.Serializable]
    public class ArrayOfStrings
    {
        public List<string> array = new List<string>();
    }

    [System.Serializable]
    public class Clickmap
    {
        public double x;
        public double y;
    }

    [System.Serializable]
    public class SurveyBody
    {
        public int? nq;
        public int start_time = 0;
        public string token = "";
        public string user_id = "";
        public string app_key = "";
        public int time_spent = 0;
        public string survey_key = "";
        public SurveyAnswerArray answers;
        public List<Clickmap> clickmap = new List<Clickmap>();
    }
}
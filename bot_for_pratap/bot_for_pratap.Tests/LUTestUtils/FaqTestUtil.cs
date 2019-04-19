﻿using bot_for_pratap.Tests.Mocks;
using bot_for_pratap.Tests.Utterances;
using Microsoft.Bot.Builder.AI.QnA;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace bot_for_pratap.Tests.LUTestUtils
{
    public class FaqTestUtil
    {
        private static Dictionary<string, QueryResult[]> _utterances = new Dictionary<string, QueryResult[]>
        {
            { FaqUtterances.Overview, CreateAnswer(@"Resources\faq_overview.json") },
        };

        public static MockQnAMaker CreateRecognizer()
        {
            var recognizer = new MockQnAMaker(defaultAnswer: CreateAnswer(@"Resources\faq_default.json"));
            recognizer.RegisterAnswers(_utterances);
            return recognizer;
        }

        public static QueryResult[] CreateAnswer(string jsonPath)
        {
            var content = File.ReadAllText(jsonPath);
            dynamic result = JsonConvert.DeserializeObject(content);
            return result.answers.ToObject<QueryResult[]>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Pressitter.Dtos;

namespace Pressitter.Services
{
    public static class AzureServices
    {
        public static void GetKeyWords(List<NewsArticle> articles, Dictionary<string, List<NewsTopic>> topics, Dictionary<string, List<NewsDay>> dayTopics, ILogger log) 
        {
            string payload = "{\"documents\": ["; 

            for (int i = 0; i < articles.Count; i++) 
            {
                if (i>0) payload += ",";
                payload += $"{{ \"language\": \"en\", \"id\": \"{i+1}\", \"text\": \"{Utilities.RemoveInvalidChars(articles[i].TitleEnglish)}\"}}";  
            }
            payload += "]}";

            using (HttpClient client = new HttpClient()) 
            { 
                string key = System.Environment.GetEnvironmentVariable("TextAnalyticsKey");
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key); 
                var content = new StringContent(payload, Encoding.UTF8, "application/json"); 
    
                HttpResponseMessage msg = client.PostAsync("https://westeurope.api.cognitive.microsoft.com/text/analytics/v2.0/keyPhrases", content).Result; 

                if (msg.IsSuccessStatusCode) 
                { 
                    var JsonDataResponse = msg.Content.ReadAsStringAsync().Result; 
                    Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(JsonDataResponse); 
                    if (obj != null) 
                    { 
                        foreach (JObject docObject in obj["documents"]) 
                        { 
                            int index = Convert.ToInt16(docObject["id"].ToString());
                            
                            articles[index-1].KeyWords = docObject["keyPhrases"].ToString().Replace("\n", "");;

                            foreach (JToken phraseObject in docObject["keyPhrases"]) 
                            {
                                string pKey = phraseObject.ToString().ToUpper();
                                foreach (string str in new string[] {"'", "?", "\\", "/", "#", "."})
                                {
                                    pKey = pKey.Replace(str, "");
                                }
                                
                                string rKey = pKey + "." + articles[index-1].PartitionKey + "." + articles[index-1].RowKey;
                                NewsTopic topic = AutoMapper.Mapper.Map<NewsTopic>(articles[index-1]);
                                NewsDay day = AutoMapper.Mapper.Map<NewsDay>(articles[index-1]);
                                day.PartitionKey = System.DateTime.Now.ToString("dd.MM.yyyy");
                                day.RowKey = rKey;
                                day.tempPartitionKey = System.DateTime.Now.ToString("dd.MM.yyyy");
                                day.tempRowKey = rKey;

                                string dateString = articles[index-1].PublishDate.ToString("dd.MM.yyyy");
                                if (!dayTopics.ContainsKey(dateString)) dayTopics.Add(dateString, new List<NewsDay>());
                                dayTopics[dateString].Add(day);

                                topic.PartitionKey = pKey;
                                topic.RowKey = rKey;
                                topic.tempPartitionKey = pKey;
                                topic.tempRowKey = rKey;

                                if (!topics.ContainsKey(topic.PartitionKey)) topics.Add(topic.PartitionKey, new List<NewsTopic>());
                                topics[topic.PartitionKey].Add(topic);                              
                            }
                        } 
                    } 
                }  
                else {
                    // Error with request
                    log.LogError("Bad Azure TextAnalytics response code received: " + msg.StatusCode.ToString());
                }
            }
        }
    
        public static void GetTranslations(Translations translateCommand)
        {
            foreach (string lang in translateCommand.Articles.Keys)
            {
                int counter = 0;

                while (counter < translateCommand.Articles[lang].Stories.Count) 
                {
                    int innerCounter = counter;
                    Newtonsoft.Json.Linq.JArray requestTexts = new Newtonsoft.Json.Linq.JArray();

                    while ((innerCounter - counter) <= 24 && innerCounter < translateCommand.Articles[lang].Stories.Count)
                    {
                        JObject text = new JObject();
                        text.Add("Text", translateCommand.Articles[lang].Stories[innerCounter].Title);
                        requestTexts.Add(text);
                        innerCounter++;
                    }

                    List<string> translations = GetTranslation(requestTexts, lang, translateCommand.DisplayLanguage).Result;

                    if (translations.Count > 0)
                    {
                        for (int i = 0; i < translations.Count; i++)
                        {
                            translateCommand.Articles[lang].Stories[counter + i].RowKey = RssUtilities.GetArticleId(translations[i]);
                            translateCommand.Articles[lang].Stories[counter + i].TitleEnglish = translations[i];
                        }
                    }

                    counter = innerCounter;
                }
            }
        }

        public static async Task<List<string>> GetTranslation(JArray texts, string sourceLanguage, string destinationLanguage)
        {
            List<string> results = new List<string>();
            string from = sourceLanguage;
            string to = destinationLanguage;
            string uri = "https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&from=" + from + "&to=" + to;
            string subKey = System.Environment.GetEnvironmentVariable("TranslatorKey", EnvironmentVariableTarget.Process); //System.Configuration.ConfigurationManager.AppSettings["TranslatorKey"];

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subKey);
                var content = new StringContent(texts.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage msg = client.PostAsync(uri, content).Result;
                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = msg.Content.ReadAsStringAsync().Result;
                    Newtonsoft.Json.Linq.JArray obj = Newtonsoft.Json.Linq.JArray.Parse(JsonDataResponse);
                    if (obj != null)
                    {
                        foreach (JObject translationObject in obj)
                        {
                            int counter = 0;
                            JArray translations = (JArray)translationObject["translations"];
                            while (counter < translations.Count)
                            {
                                results.Add(translations[counter]["text"].ToString());
                                counter++;
                            }
                        }
                    }
                }
            }

            return results;
        }    
    }
}
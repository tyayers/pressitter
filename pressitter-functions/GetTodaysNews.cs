using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Pressitter.Dtos;
using System.Linq;

namespace Pressitter.Functions
{
    public static class GetTodaysNews
    {
        [FunctionName("GetTodaysNews")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            Pressitter.Services.NewsRepository repo = new Services.NewsRepository();
            string date = System.DateTime.Now.ToString("dd.MM.yyyy");
            List<NewsDay> articles = repo.GetNewsForDay(date);
            if (articles.Count == 0) 
            {
                date = System.DateTime.Now.AddDays(-1).ToString("dd.MM.yyyy");
                articles = repo.GetNewsForDay(date);
            }
           
            NewsDayTopics dayByTopic = new NewsDayTopics() {Date = date};
            Dictionary<string, List<NewsDay>> tempNewsTopics = new Dictionary<string, List<NewsDay>>();            

            if (articles != null) 
            {
                foreach (NewsDay day in articles) 
                {
                    string[] pieces = day.RowKey.Split(".");
                    if (!tempNewsTopics.ContainsKey(pieces[0]))
                        tempNewsTopics.Add(pieces[0], new List<NewsDay>());

                    tempNewsTopics[pieces[0]].Add(day);
                }

                var keys = (from topic in tempNewsTopics orderby topic.Value.Count descending select topic.Key);

                foreach (string topic in keys) {
                    dayByTopic.Topics.Add(new NewsDayTopic(){Topic = topic, News = tempNewsTopics[topic]});
                }
            }

            return dayByTopic.Topics.Count > 0
                ? (ActionResult)new OkObjectResult(Newtonsoft.Json.JsonConvert.SerializeObject(dayByTopic))
                : new BadRequestObjectResult("No recent articles found!");
        }
    }
}

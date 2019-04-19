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
using Pressitter.Services;
using AutoMapper;

namespace Pressitter.Functions
{
    public static class NewsUpdate
    {
        static NewsUpdate() {
            Mapper.Initialize(cfg => {
                cfg.CreateMap<NewsArticle, NewsTopic>();
                cfg.CreateMap<NewsArticle, NewsDay>();
            });
        }

        [FunctionName("NewsUpdate")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            Pressitter.Services.NewsRepository repo = new Services.NewsRepository();

            List<NewsSource> sources = repo.GetNewsSources();
            List<NewsArticle> allNewArticles = new List<NewsArticle>();
            Dictionary<string, List<NewsTopic>> allNewTopics = new Dictionary<string, List<NewsTopic>>();
            Dictionary<string, List<NewsDay>> allNewsDays = new Dictionary<string, List<NewsDay>>();

            foreach (NewsSource source in sources) 
            {
                if (source.Active) 
                {
                    List<NewsArticle> stories = RssUtilities.GetRSSUpdates(source, log);
                    if (stories.Count > 0)
                    {
                        if (source.Language != "en") 
                        {
                            Translations translate = new Translations() { DisplayLanguage = "en" };
                            translate.AddStoriesSameLanguage(source.Language, stories);
                            AzureServices.GetTranslations(translate);
                        }

                        allNewArticles.AddRange(stories);

                        AzureServices.GetKeyWords(stories, allNewTopics, allNewsDays, log);
                        log.LogInformation($"Found {stories.Count} new stories from {source.RowKey}");

                        // We have updates
                        // foreach (NewsArticle article in stories) 
                        // {
                        //     repo.SaveNewsArticle(article, log);    
                        // }

                        repo.SaveNewsArticles(stories, log);
                        repo.UpdateNewsSource(source);
                    }  
                }              
            }

            //NewsDay day = repo.GetNewsDay(System.DateTime.Now.ToString("dd.MM.yyyy"));

            foreach (string name in allNewTopics.Keys) 
            {
                //if (!day.Topics.Contains(name)) day.Topics += "," + name;

                List<NewsTopic> topics = allNewTopics[name];
                // foreach (NewsTopic topic in topics)
                // {
                //     repo.SaveNewsTopic(topic, log);
                // }
                repo.SaveNewsTopics(topics, log);
            }

            foreach (string date in allNewsDays.Keys) 
            {
                //if (!day.Topics.Contains(name)) day.Topics += "," + name;

                List<NewsDay> day = allNewsDays[date];
                // foreach (NewsTopic topic in topics)
                // {
                //     repo.SaveNewsTopic(topic, log);
                // }
                repo.SaveNewsDays(day, log);
            }            

            //repo.SaveNewsDay(day, log);

            return (ActionResult) new OkObjectResult($"Updated {allNewArticles.Count} news articles.");
        }
    }
}

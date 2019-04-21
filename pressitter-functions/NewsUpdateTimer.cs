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
    public static class NewsUpdateTimer
    {
        static NewsUpdateTimer() {
            Mapper.Initialize(cfg => {
                cfg.CreateMap<NewsArticle, NewsTopic>();
                cfg.CreateMap<NewsArticle, NewsDay>();
            });
        }

        [FunctionName("NewsUpdateTimer")]
        public static void Run([TimerTrigger("0 30 9 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

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

                        repo.SaveNewsArticles(stories, log);
                        repo.UpdateNewsSource(source);
                    }  
                }              
            }

            foreach (string name in allNewTopics.Keys) 
            {
                List<NewsTopic> topics = allNewTopics[name];
                repo.SaveNewsTopics(topics, log);
            }

            foreach (string date in allNewsDays.Keys) 
            {
                List<NewsDay> day = allNewsDays[date];
                repo.SaveNewsDays(day, log);
            }             
        }
    }
}

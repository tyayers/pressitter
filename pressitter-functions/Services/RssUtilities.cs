

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Pressitter.Dtos;

namespace Pressitter.Services
{
    public static class RssUtilities
    {
        public static List<NewsArticle> GetRSSUpdates(NewsSource provider, ILogger log)
        {
            List<NewsArticle> results = new List<NewsArticle>();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string rssContent = client.GetStringAsync(provider.RssUrl).Result;

                    if (!String.IsNullOrEmpty(rssContent))
                    {

                        XDocument doc = XDocument.Parse(rssContent);

                        string newUpdate = GetPublishDate(doc.Element("rss").Element("channel"));
                        DateTime newUpdateTime = DateTime.Parse(newUpdate);
                        //DateTime lastUpdateTime = DateTime.Parse(provider.LastUpdate);
                        if (newUpdateTime > provider.LastUpdate)
                        {
                            //provider.LastUpdated = newUpdateTime;
                            IEnumerable<XElement> items = doc.Descendants("item");

                            foreach (XElement item in items)
                            {
                                if (!GetIfBlacklisted(item.Element("title").Value, provider.BlacklistedTerms)) 
                                {
                                    NewsArticle newsItem = new NewsArticle
                                    {
                                        Title = item.Element("title").Value,
                                        Description = item.Element("description").Value,
                                        Language = provider.Language,
                                        Region = provider.Region,
                                        Url = item.Element("link").Value,
                                        ImageUrl = GetImageUrl(item),
                                        LogoUrl = provider.LogoUrl,
                                        PartitionKey = provider.RowKey,
                                        RowKey = GetArticleId(item.Element("title").Value),
                                        LastUpdate = DateTime.Now,
                                        PublishDate = DateTime.Parse(item.Element("pubDate").Value),
                                        Source = provider.RowKey
                                    };

                                    if (provider.Language == "en") 
                                    {
                                        newsItem.TitleEnglish = newsItem.Title;
                                    }

                                    if (newsItem.PublishDate > provider.LastPublished)
                                    {
                                        // This came in after the last update, so add
                                        results.Add(newsItem);
                                    }
                                }
                            }

                            provider.LastPublished = newUpdateTime;
                            provider.LastUpdate = DateTime.Now;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Error processing news provider {provider.RowKey}, error: {ex.Message}");
            }

            return results;
        }

        public static string GetImageUrl(XElement element)
        {
            XNamespace media = "http://search.yahoo.com/mrss/";

            string result = "";

            XElement mediaElem = element.Element(media + "content");
            if (mediaElem != null)
            {
                result = mediaElem.Attribute("url").Value;
            }
            else
            {
                // Get it like spiegel
                mediaElem = element.Element("enclosure");
                if (mediaElem != null)
                {
                    result = mediaElem.Attribute("url").Value;
                }
            }

            return result;
        }

        public static string GetArticleId(string title) {
            string result = title;

            if (result.Length > 400)
                result = result.Substring(0, 400);

            foreach (string str in new string[] {"'", "?", "\\", "/", "#", "."})
            {
                result = result.Replace(str, "");
            }

            return result;
        }

        public static string GetPublishDate(XElement channel)
        {
            string result = "";

            XElement dateElem = channel.Element("lastBuildDate");
            if (dateElem != null)
            {
                result = dateElem.Value;
            }
            else
            {
                dateElem = channel.Element("date");
                if (dateElem != null)
                    result = dateElem.Value;
                else
                {
                    dateElem = channel.Element("pubDate");
                    if (dateElem != null)
                        result = dateElem.Value;
                    else
                    {
                        dateElem = channel.Descendants("pubDate").First();
                        if (dateElem != null)
                            result = dateElem.Value;
                    }
                }
            }

            return result;
        }

        public static bool GetIfBlacklisted(string Title, string BlacklistTerms) 
        {
            bool result = false;
            if (!String.IsNullOrEmpty(BlacklistTerms))
            {
                foreach (string blacklist in BlacklistTerms.Split(","))
                {
                    if (Title.ToUpper().Contains(blacklist.ToUpper())) result = true;
                }
            }
            return result;
        }
    }
}
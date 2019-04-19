using System.Collections.Generic;

namespace Pressitter.Dtos
{
    public class Translations
    {
        public string DisplayLanguage { get; set; }
        public Dictionary<string, Translate> Articles { get; set; } = new Dictionary<string, Translate>();

        public void AddStory(NewsArticle story)
        {
            if (!Articles.ContainsKey(story.Language))
            {
                Articles.Add(story.Language, new Translate() { LangaugeFrom = story.Language, LanguageTo = DisplayLanguage });                
            }

            Articles[story.Language].Stories.Add(story);
        }

        public void AddStoriesSameLanguage(string Language, List<NewsArticle> stories) 
        {
            if (!Articles.ContainsKey(Language))
            {
                Articles.Add(Language, new Translate() { LangaugeFrom = Language, LanguageTo = DisplayLanguage }); 
            }

            Articles[Language].Stories.AddRange(stories);              
        }
    }

    public class Translate
    {
        public string LangaugeFrom { get; set; }
        public string LanguageTo { get; set; }
        public List<NewsArticle> Stories { get; set; } = new List<NewsArticle>();
    }
}
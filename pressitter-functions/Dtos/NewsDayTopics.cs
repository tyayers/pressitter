using System.Collections.Generic;

namespace Pressitter.Dtos 
{
    public class NewsDayTopics
    {
        public string Date {get;set;}

        public List<NewsDayTopic> Topics {get;set;} = new List<NewsDayTopic>();

        //public Dictionary<string, List<NewsDay>> News {get;set;} = new Dictionary<string, List<NewsDay>> ();
    }
}
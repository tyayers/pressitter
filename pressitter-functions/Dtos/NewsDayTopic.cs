using System.Collections.Generic;

namespace Pressitter.Dtos 
{
    public class NewsDayTopic
    {
        public string Topic {get;set;}

        public List<NewsDay> News {get; set;} = new List<NewsDay> ();
    }
}
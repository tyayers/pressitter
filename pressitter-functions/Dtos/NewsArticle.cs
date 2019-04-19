using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pressitter.Dtos
{
    public class NewsArticle : TableEntity
    {
        public NewsArticle(string partitionKey, string rowKey)
            : base(partitionKey, rowKey) {
        }

        public NewsArticle() { }    

        public string Language {get;set;}
        public string Region {get;set;}
        public DateTime LastUpdate {get;set;}
        public DateTime PublishDate {get;set;}
        public string Source {get;set;}
        public string Title {get;set;}
        public string TitleEnglish {get;set;}
        public string Description {get;set;}
        public string Url {get;set;}
        public string ImageUrl {get;set;}        
        public string LogoUrl {get;set;}
        public string KeyWords {get;set;}
    }
}
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pressitter.Dtos
{
    public class NewsSource : TableEntity
    {
        public NewsSource(string partitionKey, string rowKey)
            : base(partitionKey, rowKey) {
        }

        public NewsSource() { }    

        public string Language {get;set;}
        public string RssUrl {get;set;}
        public string LogoUrl {get;set;}
        public string Region {get;set;}
        public string BlacklistedTerms {get;set;}
        public DateTime LastUpdate {get;set;}
        public DateTime LastPublished {get;set;}
        public bool Active {get;set;} = true;
    }
}
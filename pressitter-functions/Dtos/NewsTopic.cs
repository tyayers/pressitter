using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pressitter.Dtos
{
    public class NewsTopic : TableEntity
    {
        public NewsTopic(string partitionKey, string rowKey)
            : base(partitionKey, rowKey) {
        }

        public NewsTopic() { }    

        public string Language {get;set;}
        public string Region {get;set;}
        public DateTime PublishDate {get;set;}
        public string Title {get;set;}
        public string Description {get;set;}
        public string Source {get;set;}
        public string Url {get;set;}
        public string ImageUrl {get;set;}        
        public string LogoUrl {get;set;}
        public string KeyWords {get;set;}
        public string tempRowKey {get;set;}
        public string tempPartitionKey {get;set;}
    }
}
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Pressitter.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pressitter.Services
{
    public class NewsRepository 
    {
        public List<NewsSource> GetNewsSources() 
        {
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(System.Environment.GetEnvironmentVariable("StorageConnectionString"));

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("Sources");
            table.CreateIfNotExistsAsync().Wait();

            TableContinuationToken token = null;
            var entities = new List<NewsSource>();
            do
            {
                var queryResult = table.ExecuteQuerySegmentedAsync(new TableQuery<NewsSource>(), token).Result;
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);

            return entities;
        }

        public void UpdateNewsSource(NewsSource source) {
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(System.Environment.GetEnvironmentVariable("StorageConnectionString"));

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("Sources");
            table.CreateIfNotExistsAsync().Wait();

            // Create the TableOperation object that inserts the customer entity.
            TableOperation replaceOperation = TableOperation.Replace(source);

            // Execute the insert operation.
            var result = table.ExecuteAsync(replaceOperation).Result;            
        }
        public void SaveNewsArticle(NewsArticle article, ILogger log) 
        {
            try
            {
                // Retrieve the storage account from the connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(System.Environment.GetEnvironmentVariable("StorageConnectionString"));

                // Create the table client.
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                // Create the CloudTable object that represents the "people" table.
                CloudTable table = tableClient.GetTableReference("Articles");
                table.CreateIfNotExistsAsync().Wait();

                // Create the TableOperation object that inserts the customer entity.
                TableOperation insertOperation = TableOperation.InsertOrReplace(article);

                // Execute the insert operation.
                var result = table.ExecuteAsync(insertOperation).Result;
            }
            catch (Exception ex) {
                log.LogError(ex.ToString());
                throw ex;
            }

        }

        public void SaveNewsArticles(List<NewsArticle> articles, ILogger log) 
        {
            try {
                // Retrieve the storage account from the connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(System.Environment.GetEnvironmentVariable("StorageConnectionString"));

                // Create the table client.
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                // Create the CloudTable object that represents the "people" table.
                CloudTable table = tableClient.GetTableReference("Articles");
                table.CreateIfNotExistsAsync().Wait();

                int counter = 0;

                while (counter < articles.Count) 
                {
                    TableBatchOperation batchOperation = new TableBatchOperation();
                    int batchCounter = 0;

                    while (counter < articles.Count && batchCounter < 100) 
                    {
                        NewsArticle article = articles[counter];
                        batchOperation.InsertOrReplace(article);
                        counter++;
                        batchCounter++;
                    }

                    var result = table.ExecuteBatchAsync(batchOperation).Result;
                }
            }
            catch (Exception ex) {
                log.LogError(ex.ToString());
                throw ex;
            }
        }        

        public void SaveNewsTopic(NewsTopic topic, ILogger log) 
        {
            try
            {
                // Retrieve the storage account from the connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(System.Environment.GetEnvironmentVariable("StorageConnectionString"));

                // Create the table client.
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                // Create the CloudTable object that represents the "people" table.
                CloudTable table = tableClient.GetTableReference("Topics");
                table.CreateIfNotExistsAsync().Wait();

                // Create the TableOperation object that inserts the customer entity.
                TableOperation insertOperation = TableOperation.InsertOrReplace(topic);

                // Execute the insert operation.
                var result = table.ExecuteAsync(insertOperation).Result;
            }
            catch (Exception ex) {
                log.LogError(ex.ToString());
                //throw ex;
            }
        }

        public void SaveNewsTopics(List<NewsTopic> topics, ILogger log) 
        {
            try {
                // Retrieve the storage account from the connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(System.Environment.GetEnvironmentVariable("StorageConnectionString"));

                // Create the table client.
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                // Create the CloudTable object that represents the "people" table.
                CloudTable table = tableClient.GetTableReference("Topics");
                table.CreateIfNotExistsAsync().Wait();

                int counter = 0;

                while (counter < topics.Count) 
                {
                    TableBatchOperation batchOperation = new TableBatchOperation();
                    int batchCounter = 0;

                    while (counter < topics.Count && batchCounter < 100) 
                    {
                        NewsTopic topic = topics[counter];
                        batchOperation.InsertOrReplace(topic);
                        counter++;
                        batchCounter++;
                    }

                    var result = table.ExecuteBatchAsync(batchOperation).Result;
                }
            }
            catch (Exception ex) {
                log.LogError(ex.ToString());
                throw ex;
            }            
        }

        public List<NewsDay> GetNewsForDay(string date) 
        {
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(System.Environment.GetEnvironmentVariable("StorageConnectionString"));

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("Calendar");
            table.CreateIfNotExistsAsync().Wait();

            TableContinuationToken token = null;
            var entities = new List<NewsDay>();
            do
            {
                var queryResult = table.ExecuteQuerySegmentedAsync(new TableQuery<NewsDay>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, date)), token).Result;
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);

            return entities;         
        }         

        public void SaveNewsDay(NewsDay day, ILogger log) 
        {
            try
            {
                // Retrieve the storage account from the connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(System.Environment.GetEnvironmentVariable("StorageConnectionString"));

                // Create the table client.
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                // Create the CloudTable object that represents the "people" table.
                CloudTable table = tableClient.GetTableReference("Calendar");
                table.CreateIfNotExistsAsync().Wait();

                // Create the TableOperation object that inserts the customer entity.
                TableOperation insertOperation = TableOperation.InsertOrReplace(day);

                // Execute the insert operation.
                var result = table.ExecuteAsync(insertOperation).Result;
            }
            catch (Exception ex) {
                log.LogError(ex.ToString());
                //throw ex;
            }
        }      
        
        public void SaveNewsDays(List<NewsDay> days, ILogger log) 
        {
            try {
                // Retrieve the storage account from the connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(System.Environment.GetEnvironmentVariable("StorageConnectionString"));

                // Create the table client.
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                // Create the CloudTable object that represents the "people" table.
                CloudTable table = tableClient.GetTableReference("Calendar");
                table.CreateIfNotExistsAsync().Wait();

                int counter = 0;

                while (counter < days.Count) 
                {
                    TableBatchOperation batchOperation = new TableBatchOperation();
                    int batchCounter = 0;

                    while (counter < days.Count && batchCounter < 100) 
                    {
                        NewsDay day = days[counter];
                        batchOperation.InsertOrReplace(day);
                        counter++;
                        batchCounter++;
                    }

                    var result = table.ExecuteBatchAsync(batchOperation).Result;
                }
            }
            catch (Exception ex) {
                log.LogError(ex.ToString());
                throw ex;
            }            
        }                    
    }
}
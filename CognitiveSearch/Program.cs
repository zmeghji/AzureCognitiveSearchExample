using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using System;
using System.Collections.Generic;

namespace CognitiveSearch
{
    class Program
    {
        private static string queryKey = "";
        private static string adminKey = "";
        private static Uri endpoint = new Uri("");
        private static string indexName = "ninjas";
        static void Main(string[] args)
        {
            //CreateIndex();
            //AddDocuments();
            SearchDocuments();
        }
        private static void SearchDocuments()
        {
            var credential = new AzureKeyCredential(queryKey);
            var client = new SearchClient(endpoint, indexName, credential);
            var results = client.Search<Ninja>("naruto").Value.GetResults();
            foreach(var result in results)
            {
                Console.WriteLine($"Name: {result.Document.Name}");
            }
        }
        private static void AddDocuments()
        {
            var credential = new AzureKeyCredential(adminKey);
            var client = new SearchClient(endpoint, indexName, credential);
            var action1 = IndexDocumentsAction.Upload(new Ninja
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Naruto Uzumaki"
            });
            var action2 = IndexDocumentsAction.Upload(new Ninja
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Sasuke Uchiha"
            });
            var batch = IndexDocumentsBatch.Create(action1, action2);
            try
            {
                IndexDocumentsResult result = client.IndexDocuments(batch);
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to index some of the documents: {0}");
            }
        }
        private static void CreateIndex()
        {
            var credential = new AzureKeyCredential(adminKey);
            var client = new SearchIndexClient(endpoint, credential);
            var fields = new List<SearchField>
            {
                new SearchField("Id", SearchFieldDataType.String)
                {
                    IsSearchable = false,
                    IsKey = true
                },
                new SearchField("Name", SearchFieldDataType.String)
                {
                    IsSearchable = true,
                    IsKey = false
                }
            };
            client.CreateIndex(new SearchIndex(indexName, fields));
        }
        public class Ninja
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}

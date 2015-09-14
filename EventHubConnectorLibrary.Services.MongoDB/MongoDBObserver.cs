using EventHubConnectorLibrary.Contracts;
using EventHubConnectorLibrary.Core;
using Microsoft.ServiceBus.Messaging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace EventHubConnectorLibrary.Services.MySql
{
    public class MongoDBObserver : IObserver<EventHubMessage>, IFilter
    {
        private readonly ILog _log;
        private readonly string _connectionString;
        private readonly string _databaseName;
        private readonly string _collection;
        private MongoClient _connection;
        private IMongoDatabase _database;
        private IMongoCollection<BsonDocument> _documentCollection;

        public MongoDBObserver(ILog log, string connectionString, string databaseName, string collection)
        {
            _log = log;
            _connectionString = connectionString;
            _databaseName = databaseName;
            _collection = collection;

            _connection = new MongoClient(_connectionString);
            _database = _connection.GetDatabase(databaseName);
            _documentCollection = _database.GetCollection<BsonDocument>(collection);
        }

        public async void OnNext(EventHubMessage value)
        {
            //var doc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(value.Body);
            var msg = System.Text.Encoding.UTF8.GetString(value.Body);

            if (Filter != null && Filter.Length > 0)
            {
                foreach (var f in Filter)
                {
                    //YOU SHALL NOT PASS
                    if (!msg.Contains(f)) return;
                }
            }

            var doc = BsonDocument.Parse(msg);
            if (value.Properties != null)
            {
                foreach (var p in value.Properties)
                {
                    if (!doc.Contains(p.Key)) doc.Add(new BsonElement(p.Key, p.Value.ToString()));
                }
            }
            if (value.SystemProperties != null)
            {
                foreach (var p in value.SystemProperties)
                {
                    if (!doc.Contains(p.Key)) doc.Add(new BsonElement(p.Key, p.Value.ToString()));
                }
            }
            bool success = false;
            int count = 0;
            while (!success || count > 5)
            {
                try
                {
                    await _documentCollection.InsertOneAsync(doc);
                    success = true;
                }
                catch (Exception)
                {
                    await Task.Delay(500);
                    count++;
                }
            }
        }

        public async void OnError(Exception error)
        {
            await _log.Error(error, "MqSqlObserver received an error");
        }

        public void OnCompleted()
        {
        }

        public string[] Filter { get; set; }
    }
}
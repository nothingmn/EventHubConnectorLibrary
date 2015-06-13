using EventHubConnectorLibrary.Contracts;
using EventHubConnectorLibrary.Core;
using Microsoft.ServiceBus.Messaging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace EventHubConnectorLibrary.Services.MySql
{
    public class MongoDBObserver : IObserver<EventHubMessage>
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

        public void OnNext(EventHubMessage value)
        {
            //var doc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(value.Body);
            var doc = BsonDocument.Parse(System.Text.Encoding.UTF8.GetString(value.Body));
            _documentCollection.InsertOneAsync(doc).Wait();
        }

        public async void OnError(Exception error)
        {
            await _log.Error(error, "MqSqlObserver received an error");
        }

        public void OnCompleted()
        {
        }
    }
}
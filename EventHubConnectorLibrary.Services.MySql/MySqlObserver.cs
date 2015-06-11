using EventHubConnectorLibrary.Contracts;
using EventHubConnectorLibrary.Core;
using Microsoft.ServiceBus.Messaging;
using MySql.Data.MySqlClient;
using System;

namespace EventHubConnectorLibrary.Services.MySql
{
    public class MySqlObserver : IObserver<EventHubMessage>
    {
        private readonly ILog _log;
        private readonly string _connectionString;
        private MySqlConnection _connection;

        public Func<EventHubMessage, string> SqlCommandAction { get; set; }

        public MySqlObserver(ILog log, string connectionString)
        {
            _log = log;
            _connectionString = connectionString;

            _connection = new MySqlConnection(_connectionString);
            _connection.Open();
        }

        public void OnNext(EventHubMessage value)
        {
            if (SqlCommandAction != null)
            {
                var sql = SqlCommandAction(value);

                if (!string.IsNullOrEmpty(sql))
                {
                    try
                    {
                        var cmd = new MySqlCommand(sql, _connection);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        _log.Error(e, "MqSqlObserver received an error, when calling OnNext").Wait();
                    }
                }
            }

            //client.Publish(topic, body);
        }

        public async void OnError(Exception error)
        {
            await _log.Error(error, "MqSqlObserver received an error");
        }

        public void OnCompleted()
        {
            _connection.Close();
        }
    }
}
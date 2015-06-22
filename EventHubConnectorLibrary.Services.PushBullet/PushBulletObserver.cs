using EventHubConnectorLibrary.Contracts;
using EventHubConnectorLibrary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace EventHubConnectorLibrary.Services.PushBullet
{
    public class PushBulletObserver : IObserver<EventHubMessage>, IFilter
    {
        private readonly ILog _log;
        private readonly string _apiKey;

        public PushBulletObserver(ILog log, string apiKey)
        {
            _log = log;
            _apiKey = apiKey;
        }

        public Func<string, PushBulletNote> HubToNoteFunc { get; set; }

        public async void OnNext(EventHubMessage value)
        {
            var body = value.Body;
            var content = System.Text.Encoding.UTF8.GetString(body);

            if (!string.IsNullOrEmpty(Filter))
            {
                if (!content.Contains(Filter)) return;
            }
            var note = HubToNoteFunc(content);
            if (note != null)
            {
                var http = await PushNote(note);
                var success = http.IsSuccessStatusCode;
            }
        }

        public async void OnError(Exception error)
        {
            await _log.Error(error, "MqSqlObserver received an error");
        }

        public void OnCompleted()
        {
        }

        private async Task<HttpResponseMessage> PushNote(PushBulletNote note)
        {
            // Push a note to all devices.
            String type = "note";
            byte[] data =
                Encoding.ASCII.GetBytes(String.Format("{{ \"type\": \"{0}\", \"title\": \"{1}\", \"body\": \"{2}\" }}", type, note.Title, note.Body));

            var handler = new HttpClientHandler();
            handler.Credentials = new System.Net.NetworkCredential(_apiKey, "");

            HttpClient c = new HttpClient(handler);
            var content = new ByteArrayContent(data);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return await c.PostAsync("https://api.pushbullet.com/v2/pushes", content);
        }

        public string Filter { get; set; }
    }
}
using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Text;
using ILN.API;
using ILN.Core;
using Newtonsoft.Json;

namespace ILN.GRPC.Service.Plugin.SplunkForwarder;

public class MessageProcessor : IMessageProcessor
{
    private string                _endpoint        = null!;
    private string                _index           = null!;
    private HttpClient            _httpClient      = null!;
    private Fields                _baseFields      = null!;
    private Logger                _logger          = null!;
    private MessageConsolePrinter _fallbackPrinter = null!;

    private Task<Task> _worker;

    private readonly ConcurrentQueue<IMessage> _eventQueue = new();

    //

    public string ID => "ILN.GRPC.Service.Plugin.SplunkForwarder";

    public void Initialize(IReadOnlyDictionary<string, object> parameters)
    {
        _endpoint = (string) parameters["Endpoint"];
        _index    = (string) parameters["Index"];

        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Splunk", (string) parameters["Token"]);

        _baseFields = new Fields
        {
            {"Endpoint", _endpoint},
            {"Index", _index},
        };

        _logger          = MessageConsolePrinter.New("ILN.GRPC.Service.Plugin.SplunkForwarder", "ILN");
        _fallbackPrinter = new MessageConsolePrinter(true, true);

        //

        _worker = Task.Factory.StartNew(async () => await Worker());
    }

    private async Task Worker()
    {
        while (true)
        {
            if (_eventQueue.TryDequeue(out IMessage? message))
            {
                SplunkEvent splunkEvent;

                try
                {
                    splunkEvent = Convert(message);
                }
                catch (Exception e)
                {
                    _logger.Error
                    (
                        "Failed to convert IMessage to a SplunkEvent; will not retry.", e,
                        _baseFields + new Fields
                        {
                            {"Message", new Fields(message)},
                        }
                    );
                    continue;
                }

                string content = JsonConvert.SerializeObject(splunkEvent);
                Console.WriteLine(content);

                try
                {
                    var request = new HttpRequestMessage
                    (
                        HttpMethod.Post,
                        _endpoint
                    );
                    request.Content = new StringContent(content, Encoding.UTF8, "application/json");

                    HttpResponseMessage result = await _httpClient.SendAsync(request);

                    if (!result.IsSuccessStatusCode)
                    {
                        _logger.Error
                        (
                            "Failed to send message to Splunk; server returned non-success status code.", null,
                            _baseFields + new Fields
                            {
                                {"Message", new Fields(message)},
                                {"StatusCode", result.StatusCode},
                                {"Response", await result.Content.ReadAsStringAsync()},
                            }
                        );
                        
                        _fallbackPrinter.Handle(message);
                    }
                }
                catch (Exception e)
                {
                    _logger.Error
                    (
                        "Failed to send message to Splunk; unhandled exception occured.", e,
                        _baseFields + new Fields
                        {
                            {"Message", new Fields(message)},
                        }
                    );

                    _fallbackPrinter.Handle(message);
                }
            }
            else
            {
                Thread.Sleep(50);
            }
        }
    }

    public async Task Process(IMessage message, CancellationToken cancellationToken)
    {
        Console.WriteLine($"FORWARD {message.Text}");

        _eventQueue.Enqueue(message);
    }

    private SplunkEvent Convert(IMessage message) => new()
    {
        Event = new Fields
        {
            {"Time", message.Time},
            {"Level", message.Level.ToString()},
            {"Text", message.Text},
            {"Fields", message.Fields},
            {"Exception", new Fields(message.Exception)},
            {"MemberName", message.MemberName},
            {"SourceFilePath", message.SourceFilePath},
            {"SourceLineNumber", message.SourceFileLine},
        },
        Host       = message.Host,
        Index      = _index,
        Source     = message.ApplicationID,
        SourceType = "_json",
    };
}

public struct SplunkEvent
{
    [JsonProperty("event")]
    public Fields Event { get; set; }

    [JsonProperty("host")]
    public string Host { get; set; }

    [JsonProperty("index")]
    public string Index { get; set; }

    [JsonProperty("source")]
    public string Source { get; set; }

    [JsonProperty("sourcetype")]
    public string SourceType { get; set; }

    [JsonProperty("time", NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? Time { get; set; }
}
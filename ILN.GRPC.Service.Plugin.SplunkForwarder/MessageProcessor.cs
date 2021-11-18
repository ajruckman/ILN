using System.Net.Http.Headers;
using System.Text;
using ILN.API;
using Newtonsoft.Json;

namespace ILN.GRPC.Service.Plugin.SplunkForwarder;

public class MessageProcessor : IMessageProcessor
{
    private string     _endpoint   = null!;
    private string     _index      = null!;
    private HttpClient _httpClient = null!;

    //

    public string ID => "ILN.GRPC.Service.Plugin.SplunkForwarder";

    public void Initialize(IReadOnlyDictionary<string, object> parameters)
    {
        _endpoint = (string) parameters["Endpoint"];
        _index    = (string) parameters["Index"];

        _httpClient = new HttpClient();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Splunk", (string) parameters["Token"]);
    }

    public async Task Process(IMessage message, CancellationToken cancellationToken)
    {
        Console.WriteLine($"FORWARD {message.Text}");

        SplunkEvent splunkEvent = new SplunkEvent
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

        //

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

            HttpResponseMessage result = await _httpClient.SendAsync(request, cancellationToken);

            if (!result.IsSuccessStatusCode)
                Console.WriteLine("Failed to send message to Splunk: " +
                                  result.Content.ReadAsStringAsync(cancellationToken).Result);

            var resultContent = await result.Content.ReadAsStringAsync();
            Console.WriteLine(resultContent);
            
            // Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
        catch (Exception e)
        {
            Console.WriteLine("-->");
            Console.WriteLine(e);
        }
    }
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
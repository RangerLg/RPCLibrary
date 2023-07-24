using System.Net.Sockets;
using System.Text;

using Newtonsoft.Json;

namespace RpcConnection;

/// <summary>
///   Helper for read/write operation to network stream.
/// </summary>
public class ReadWriteHelper : IReadWriteHelper
{

    /// <inheritdoc/>
    public void SendMessageToStream(NetworkStream networkStream, Message command)
    {
        {
            if (command == null)
                throw new ArgumentException("Command is null", nameof(command));

            if (networkStream == null)
                throw new ArgumentException($"{nameof(networkStream)} is null", nameof(networkStream));
        }

        var json = JsonConvert.SerializeObject(command, Formatting.Indented, jsonSettings);
        var bytesToSend = GetBytes(json);

        networkStream.Write(bytesToSend, 0, bytesToSend.Length);
    }

    /// <inheritdoc/>
    public Message ReadMessage(NetworkStream networkStream)
    {
        {
            if (networkStream == null)
                throw new ArgumentException($"{nameof(networkStream)} is null", nameof(networkStream));
        }

        using var streamReader = new StreamReader(networkStream, encoding: encodingType, leaveOpen: true);
        using var jsonReader = new JsonTextReader(streamReader);

        return JsonSerializer.Create(jsonSettings).Deserialize<Message>(jsonReader);
    }

    private JsonSerializerSettings jsonSettings = new()
    {
        TypeNameHandling = TypeNameHandling.Auto
    };

    private static readonly Encoding encodingType = Encoding.ASCII;
    private static byte[] GetBytes(string message) => encodingType.GetBytes(message);
    private static string GetString(byte[] message) => encodingType.GetString(message);
}
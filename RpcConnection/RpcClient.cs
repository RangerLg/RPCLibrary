using System.Net.Sockets;

namespace RpcConnection;

/// <summary>
///   Rpc connection with some server by TCP.
/// </summary>
public class RpcClient
{
    /// <summary>
    ///   Define server ip and server port.
    /// </summary>
    public RpcClient(string serverIp, int serverPort)
    {
        {
            if (string.IsNullOrWhiteSpace(serverIp))
                throw new ArgumentException("", nameof(serverIp));

            if (serverPort <= 0)
                throw new ArgumentException("server port cannot be negative or zero", nameof(serverPort));
        }

        this.serverIp = serverIp;
        this.serverPort = serverPort;
        readWriteHelper = new ReadWriteHelper();
    }

    /// <summary>
    ///   Invoke server method without any parameters.
    /// </summary>
    /// <returns>Message from server.</returns>
    /// <exception cref="ArgumentException">Method name is empty.</exception>
    /// <exception cref="Exception">Server side exception.</exception>
    public Message CallMethod(string methodName)
    {
        CheckMethodName(methodName);

        var parameters = new Dictionary<string, object>();

        return SendCommand(methodName, parameters);
    }

    /// <summary>
    ///   Invoke async server method without any parameters.
    /// </summary>
    /// <returns>Message from server.</returns>
    /// <exception cref="ArgumentException">Method name is empty.</exception>
    /// <exception cref="Exception">Server side exception.</exception>
    public async Task<Message> CallMethodAsync(string methodName)
    {
        CheckMethodName(methodName);

        var parameters = new Dictionary<string, object>();

        return await SendCommandAsync(methodName, parameters);
    }

    /// <summary>
    ///   Invoke async server method.
    /// </summary>
    /// <param name="parameters">Collection of parameters by name.</param>
    /// <returns>Message from server.</returns>
    /// <exception cref="ArgumentException">Method name is empty.</exception>
    /// <exception cref="Exception">Server side exception.</exception>
    public async Task<Message> CallMethodAsync(string methodName, Dictionary<string, object> parameters)
    {
        CheckMethodName(methodName);
        CheckParameters(parameters);

        return await SendCommandAsync(methodName, parameters);
    }

    /// <summary>
    ///   Invoke server method.
    /// </summary>
    /// <param name="parameters">Collection of parameters by name.</param>
    /// <returns>Message from server.</returns>
    /// <exception cref="ArgumentException">Method name is empty.</exception>
    /// <exception cref="Exception">Server side exception.</exception>
    public Message CallMethod(string methodName, Dictionary<string, object> parameters)
    {
        CheckMethodName(methodName);
        CheckParameters(parameters);

        return SendCommand(methodName, parameters);
    }

    private IReadWriteHelper readWriteHelper;
    private readonly string serverIp;
    private readonly int serverPort;

    private static void CheckMethodName(string methodName)
    {
        if (string.IsNullOrEmpty(methodName))
            throw new ArgumentException("Method name cannot be empty", nameof(methodName));
    }

    private static void CheckParameters(Dictionary<string, object> parameters)
    {
        if (parameters == null || !parameters.Any())
            throw new ArgumentException("Parameters collection cannot be empty", nameof(parameters));
    }

    private Message SendCommand(string methodName, Dictionary<string, object> messageParameters)
    {
        var client = new TcpClient(serverIp, serverPort);
        var networkStream = client.GetStream();

        try
        {
            var message = Invoke(methodName, messageParameters, networkStream);

            return message;
        }
        finally
        {
            client.Close();
        }
    }

    private Task<Message> SendCommandAsync(string methodName, Dictionary<string, object> messageParameters)
    {
        var client = new TcpClient(serverIp, serverPort);
        var networkStream = client.GetStream();

        try
        {
            var message = Invoke(methodName, messageParameters, networkStream);

            return Task.FromResult(message);
        }
        finally
        {
            client.Close();
        }
    }

    private Message Invoke(string methodName, Dictionary<string, object> messageParameters, NetworkStream networkStream)
    {
        var commandParameters = new Message(methodName, messageParameters);

        readWriteHelper.SendMessageToStream(networkStream, commandParameters);

        var message = readWriteHelper.ReadMessage(networkStream);

        if (message.Exception != null) throw message.Exception!;

        return message;
    }
}
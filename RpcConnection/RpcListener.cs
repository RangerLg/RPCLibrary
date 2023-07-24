using System.Net;
using System.Net.Sockets;

using InvokeHelper;

namespace RpcConnection;

/// <summary>
///   Open rpc listener for some class.
/// </summary>
/// <typeparam name="T">Server type.</typeparam>
public class RpcListener<T>
{
    /// <summary>
    ///   Get rpc listener for some server
    /// </summary>
    /// <param name="server">Instants of server.</param>
    public RpcListener(string serverIp, int serverPort, T server)
    {
        {
            if (string.IsNullOrWhiteSpace(serverIp))
                throw new ArgumentException($"{nameof(serverIp)} cannot be empty", nameof(serverIp));

            if (serverPort<=0)
                throw new ArgumentException($"{nameof(serverPort)} cannot be not positive", nameof(serverPort));

            if (server == null)
                throw new ArgumentException($"{nameof(server)} cannot be null", nameof(server));
        }

        var localAdd = IPAddress.Parse(serverIp);
        tcpListener = new TcpListener(localAdd, serverPort);
        readWriteHelper = new ReadWriteHelper();
        cancellationTokenSource = new CancellationTokenSource();
        invokeHelper = new InvokeHelper<T>(server);
    }

    /// <summary>
    ///   Open rpc connection.
    /// </summary>
    public async void OpenConnection()
    {
        tcpListener.Start();
        while (true)
        {
            try
            {
                var client = await tcpListener.AcceptTcpClientAsync(cancellationTokenSource.Token);

                var clientThread = new Thread(c => HandleClientCommunication((TcpClient)c));
                clientThread.Start(client);
            }
            catch (OperationCanceledException e)
            {
                break;
            }
        }
        tcpListener.Stop();
    }

    /// <summary>
    ///   Close connection.
    /// </summary>
    public void CloseConnection()
    {
        cancellationTokenSource.Cancel();
        tcpListener.Stop();
    }

    
    private readonly TcpListener tcpListener;
    private IReadWriteHelper readWriteHelper;
    private CancellationTokenSource cancellationTokenSource;
    private InvokeHelper<T> invokeHelper;

    private void HandleClientCommunication(TcpClient tcpClient)
    {
        var nwStream = tcpClient.GetStream();

        var command = new Message();
        try
        {
            command = readWriteHelper.ReadMessage(nwStream);
            var methodName = command.MethodName;

            var result = invokeHelper.InvokeMethodByName(methodName, command.parametersByName);

            command.SetResponse(result);
        }
        catch (Exception e)
        {
            command.Exception = e;
        }

        try
        {
            readWriteHelper.SendMessageToStream(nwStream, command);
        }
        finally
        {
            tcpClient.Close();
            tcpClient.Dispose();
        }
    }
}
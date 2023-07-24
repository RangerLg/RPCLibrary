using System.Net.Sockets;

namespace RpcConnection;

public interface IReadWriteHelper
{
    /// <summary>
    ///   Send some message to stream.
    /// </summary>
    /// <param name="networkStream">Stream to send.</param>
    /// <param name="command">Message that we need to send.</param>
    void SendMessageToStream(NetworkStream networkStream, Message command);

    /// <summary>
    ///   Read message from stream.
    /// </summary>
    /// <param name="networkStream">Stream to read.</param>
    /// <returns>Message from stream.</returns>
    Message ReadMessage(NetworkStream networkStream);
}
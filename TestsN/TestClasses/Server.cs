using RpcConnection;

using Tests;

namespace TestsN.TestClasses;

public class Server: IDisposable
{
    private RpcListener<Server> rpc;

    public Server(string serverIp, int serverPort)
    {
        rpc = new RpcListener<Server>(serverIp, serverPort, this);
    }

    /// <summary>
    ///   Start rpc server.
    /// </summary>
    public void StartServer()
    {
        rpc.OpenConnection();
    }

    /// <summary>
    ///   Stop server command.
    /// </summary>
    public void StopServer()
    {
        rpc.CloseConnection();
    }

    /// <summary>
    ///   Some void method.
    /// </summary>
    public void VoidMethod()
    {
        return;
    }

    /// <summary>
    /// To lower method override.
    /// </summary>
    /// <param name="message">message</param>
    /// <returns>Message in lower format.</returns>
    public string ToLower(string message, string nextMessage)
    {
        return message.ToLower() + nextMessage;
    }

    /// <summary>
    /// To lower method.
    /// </summary>
    /// <param name="message">message</param>
    /// <returns>Message in lower format.</returns>
    public string ToLower(string message)
    {
        return message.ToLower();
    }
    /// <summary>
    /// Method with class parameter.
    /// </summary>
    /// <param name="tempClass">Some class</param>
    /// <returns>Some message</returns>
    public string CheckTempClass(TempClass tempClass)
    {
        return tempClass.firstField + tempClass.secondField;
    }

    /// <summary>
    ///   Merge method.
    /// </summary>
    /// <param name="message">Message.</param>
    /// <param name="secondMessage">Second message.</param>
    /// <returns></returns>
    public string Merge(string message, string secondMessage)
    {
        return message + message;
    }

    public void Dispose()
    {
        rpc.CloseConnection();
    }
}
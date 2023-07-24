using System.Text;

using RpcConnection;
using Tests;

using TestsN.TestClasses;

namespace TestsN;

public class Tests
{
    private Server server;
    /// <summary>
    ///   Start server.
    /// </summary>
    [SetUp]
    public void CheckServerExampleUsage()
    {
        // Get instance server.
        // In constructor must be initialized RPC listener.
        if(server == null)
            server = new Server(Constants.serverIp, Constants.serverPort);

        // Start rpc server.
        server.StartServer();
    }

    /// <summary>
    ///   Check response from CallMethod.
    /// </summary>
    [Test]
    public void TestsClientExamplesUsage()
    {
        // Chose method name.
        const string methodName = "ToLower";

        // Set parameters collection.
        var parameters = new Dictionary<string, object> {
            { "message", Constants.testMessage }
        };

        //Get instance of NetworkConnection for server with some ip and port.
        var connection = new RpcClient(Constants.serverIp, Constants.serverPort);

        //Call method from server.
        var result = connection.CallMethod(methodName, parameters);
    }

    /// <summary>
    ///   Check response from CallMethod.
    /// </summary>
    [Test]
    public void CallMethod_MethodExists_VerifyMessage()
    {
        const string methodName = "ToLower";
        var parameters = new Dictionary<string, object> {
            { "message", Constants.testMessage }
        };
        var connection = new RpcClient(Constants.serverIp, Constants.serverPort);

        var result = connection.CallMethod(methodName, parameters);

        Assert.That(result.GetResponse<string>(), Is.EqualTo(Constants.testMessage.ToLower()));
    }

    /// <summary>
    ///   Check response from CallMethod override.
    /// </summary>
    [Test]
    public void CallMethodOverride_MethodExists_VerifyMessage()
    {
        const string methodName = "ToLower";
        var parameters = new Dictionary<string, object> {
            { "message", Constants.testMessage },
            { "nextMessage", Constants.testMessage }
        };
        var connection = new RpcClient(Constants.serverIp, Constants.serverPort);

        var result = connection.CallMethod(methodName, parameters);

        Assert.That(result.GetResponse<string>(), Is.EqualTo(Constants.testMessage.ToLower()+Constants.testMessage));
    }

    /// <summary>
    ///   Check response from several parameters method.
    /// </summary>
    [Test]
    public void CallMethod_SeveralParameters_VerifyMessage()
    {
        const string methodName = "Merge";
        var parameters = new Dictionary<string, object> {
            { "message", Constants.testMessage },
            { "secondMessage", Constants.testMessage }
        };
        var connection = new RpcClient(Constants.serverIp, Constants.serverPort);

        var result = connection.CallMethod(methodName, parameters);

        Assert.That(result.GetResponse<string>(), Is.EqualTo(Constants.testMessage + Constants.testMessage));
    }

    /// <summary>
    ///   Check response from several parameters method.
    /// </summary>
    [Test]
    public async Task CallMethodAsync_SeveralParameters_VerifyMessage()
    {
        const string methodName = "Merge";
        var parameters = new Dictionary<string, object> {
            { "message", Constants.testMessage },
            { "secondMessage", Constants.testMessage }
        };
        var connection = new RpcClient(Constants.serverIp, Constants.serverPort);

        var result = await connection.CallMethodAsync(methodName, parameters);

        Assert.That(result.GetResponse<string>(), Is.EqualTo(Constants.testMessage + Constants.testMessage));
    }

    /// <summary>
    ///   Check response from void method.
    /// </summary>
    [Test]
    public void CallMethod_MethodVoid_VerifyEmptyResponse()
    {
        const string methodName = "VoidMethod";
        var connection = new RpcClient(Constants.serverIp, Constants.serverPort);

        var result = connection.CallMethod(methodName);

        Assert.That(result.GetResponse<object>(), Is.Null);
    }

    /// <summary>
    ///   Check response from void method.
    /// </summary>
    [Test]
    public async Task CallMethodAsync_MethodVoid_VerifyEmptyResponse()
    {
        const string methodName = "VoidMethod";
        var connection = new RpcClient(Constants.serverIp, Constants.serverPort);

        var result = await connection.CallMethodAsync(methodName);

        Assert.That(result.GetResponse<object>(), Is.Null);
    }

    /// <summary>
    ///   Check response from void method.
    /// </summary>
    [Test]
    public void CallMethod_MethodWithClass_VerifyEmptyResponse()
    {
        const string methodName = "CheckTempClass";
        var tempClass = new TempClass {
            firstField = "123", secondField = 123
        };
        var parameters = new Dictionary<string, object> {
            { "tempClass", tempClass }
        };
        var connection = new RpcClient(Constants.serverIp, Constants.serverPort);

        var result = connection.CallMethod(methodName, parameters);

        Assert.That(result.GetResponse<string>(), Is.EqualTo(tempClass.firstField+tempClass.secondField));
    }

    /// <summary>
    ///   Check response from several parameters method.
    /// </summary>
    [Test]
    public void CallMethod_OneIncorrectParameters_VerifyMessage()
    {
        const string methodName = "Merge";
        var parameters = new Dictionary<string, object> {
            { "message", Constants.testMessage },
            { "secondMessageTest", Constants.testMessage }
        };
        var connection = new RpcClient(Constants.serverIp, Constants.serverPort);

        Assert.Throws<KeyNotFoundException>(() => connection.CallMethod(methodName, parameters));
    }

    /// <summary>
    ///   Check empty name exceptions.
    /// </summary>
    [Test]
    public void CallMethod_MethodNameEmpty_ThrowError()
    {
        var methodName = string.Empty;
        var parameters = new Dictionary<string, object> {
            { "message", Constants.testMessage }
        };
        var connection = new RpcClient(Constants.serverIp, Constants.serverPort);

        var ex = Assert.Throws<ArgumentException>(() => connection.CallMethod(methodName, parameters));

        Assert.That(ex.ParamName, Is.EqualTo(nameof(methodName)));
    }

    /// <summary>
    ///   Check error name exceptions.
    /// </summary>
    [Test]
    public void CallMethod_MethodNotExists_ThrowError()
    {
        const string methodName = "ErrorName";
        var parameters = new Dictionary<string, object>
        {
            { "message", Constants.testMessage }
        };
        var connection = new RpcClient(Constants.serverIp, Constants.serverPort);

        var ex = Assert.Throws<ArgumentException>(() => connection.CallMethod(methodName, parameters));

        Assert.That(ex.Message.Contains(methodName), Is.True);
    }

    /// <summary>
    ///   Check error name exceptions.
    /// </summary>
    [Test]
    public void CallMethod_ParameterIsMissed_ThrowError()
    {
        const string methodName = "Merge";
        var parameters = new Dictionary<string, object>
        {
            { "message", Constants.testMessage }
        };
        var connection = new RpcClient(Constants.serverIp, Constants.serverPort);

        var ex = Assert.Throws<ArgumentException>(() => connection.CallMethod(methodName, parameters));
    }

    /// <summary>
    ///   Check error name exceptions.
    /// </summary>
    [Test]
    public void CallMethodAsync_ParameterIsMissed_ThrowError()
    {
        const string methodName = "Merge";
        var parameters = new Dictionary<string, object>
        {
            { "message", Constants.testMessage }
        };
        var connection = new RpcClient(Constants.serverIp, Constants.serverPort);

        var ex = Assert.ThrowsAsync<ArgumentException>(async () => await connection.CallMethodAsync(methodName, parameters));
    }

    /// <summary>
    ///   Check parameter type incorrect exceptions.
    /// </summary>
    [Test]
    public void CallMethod_ParameterTypeIncorrect_ThrowError()
    {
        const string methodName = "ToLower";
        var tempClass = new TempClass {
            firstField = "123", secondField = 123
        };
        var parameters = new Dictionary<string, object>
        {
            { "message", tempClass }
        };
        var connection = new RpcClient(Constants.serverIp, Constants.serverPort);

        var ex = Assert.Throws<ArgumentException>(() => connection.CallMethod(methodName, parameters));
    }

    /// <summary>
    ///   Check parameter type incorrect exceptions LongRunning.
    /// </summary>
    [Test]
    public void CallMethod_RandomizeTest_CorectFormatAllStrings()
    {
        const string methodName = "ToLower";
        var count = 0;
        while (count < 1000)
        {
            var randomString = GetRandomString();
            var parameters = new Dictionary<string, object>
            {
                { "message", randomString }
            };
            var connection = new RpcClient(Constants.serverIp, Constants.serverPort);

            var result = connection.CallMethod(methodName, parameters);

            Assert.That(result.GetResponse<string>(), Is.EqualTo(randomString.ToLower()));
            count++;
        }
    }

    private static string GetRandomString()
    {
        var length = 7;
        var strBuild = new StringBuilder();
        var random = new Random();
        char letter;

        for (var i = 0; i < length; i++)
        {
            var flt = random.NextDouble();
            var shift = Convert.ToInt32(Math.Floor(25 * flt));
            letter = Convert.ToChar(shift + 65);
            strBuild.Append(letter);
        }

        return strBuild.ToString();
    }
}
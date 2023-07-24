using Newtonsoft.Json;

namespace RpcConnection
{
    /// <summary>
    ///   Type of message to server.
    /// </summary>
    public class Message
    {
        [JsonProperty]
        public Exception? Exception { get; set; }
        [JsonProperty]
        public object Response { get; private set; }
        [JsonProperty]
        public string MethodName { get; private set; }
        
        /// <summary>Parameters for server.</summary>
        public readonly Dictionary<string,object> parametersByName;

        /// <summary>Create message with empty fields.</summary>
        public Message()
        {
            parametersByName = new Dictionary<string, object>();
        }

        /// <summary>
        ///   Get instance of message by bytes array.
        /// </summary>
        public Message(string methodName, Dictionary<string,object> resultFromStream)
        {
            {
                if (string.IsNullOrWhiteSpace(methodName))
                    throw new ArgumentException("methodName cannot be empty", methodName);

                if (resultFromStream == null)
                    throw new ArgumentException("ResultFromStream is null", nameof(resultFromStream));
            }

            MethodName = methodName;
            parametersByName = resultFromStream;
        }

        /// <summary>
        ///   Serialize response from server to specific type.
        /// </summary>
        public T GetResponse<T>()
        {
            return (T)Response;
        }

        /// <summary>
        ///   Set response from server
        /// </summary>
        public void SetResponse(object response)
        {
            Response = response;
        }
    }
}

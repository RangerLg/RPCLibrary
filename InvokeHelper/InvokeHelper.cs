using System.Reflection;

namespace InvokeHelper;

/// <summary>
///   Helper for invoking server method
/// </summary>
/// <typeparam name="T">Server type</typeparam>
public class InvokeHelper<T>
{
    /// <summary>
    ///   Get instance of <see cref="InvokeHelper{T}"/> for some server
    /// </summary>
    /// <param name="objectToInvoke">Class whose methods will be called.</param>
    public InvokeHelper(T objectToInvoke)
    {
        this.objectToInvoke = objectToInvoke;
    }

    /// <summary>
    ///   Invoke specific method by name and with some parameters.
    /// </summary>
    /// <param name="parameters">Parameters by name.</param>
    /// <returns>Response from server</returns>
    /// <remarks>Order parameters must be the same as the method signature.</remarks>
    public object InvokeMethodByName(string methodName, Dictionary<string, object> parameters)
    {
        {
            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentException($"{nameof(methodName)} cannot be empty", nameof(methodName));

            if (parameters == null)
                throw new ArgumentException($"{nameof(parameters)} cannot be null", nameof(parameters));
        }
        var types = parameters.Select(parameter => parameter.Value.GetType()).ToArray();
        var action = GetMethod(methodName, types);

        var methodParameters = TryGetParameters(action.GetParameters(), parameters).ToArray();

        return action.Invoke(objectToInvoke, methodParameters);
    }

    
    private T objectToInvoke;

    private static MethodInfo? GetMethod(string methodName, Type[] types)
    {
        var action = typeof(T).GetMethod(methodName, types);
        if (action == null)
            throw new ArgumentException($"Method {GetSignatureString(methodName, types)} doesnt exist");

        return action;
    }

    private List<object> TryGetParameters(
        ParameterInfo[] methodParameters, Dictionary<string, object> parametersFromServer)
    {
        return methodParameters.Select(p => parametersFromServer[p.Name]).ToList();
    }

    private static string GetSignatureString(string methodName, Type[] types)
    {
        var signature = $"{methodName}(";
        signature = types.Aggregate(signature, (current, type) => current + $"{type},");

        return signature.TrimEnd(',') + ")";
    }
}
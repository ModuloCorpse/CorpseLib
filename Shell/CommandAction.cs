using CorpseLib.Actions;
using CorpseLib.Serialize;
using static CorpseLib.Actions.ActionDefinition;

namespace CorpseLib.Shell
{
    public class CommandAction(AAction action) : Command(action.ActionName)
    {
        private static readonly Dictionary<Type, AStringSerializer> ms_RegisteredSerializers = [];
        private static readonly StringNativeSerializer ms_Serializer = new();

        public static void RegisterSerializer<T>(AStringSerializer<T> serializer) => ms_RegisteredSerializers[typeof(T)] = serializer;
        public static void UnregisterSerializer<T>() => ms_RegisteredSerializers.Remove(typeof(T));

        private readonly AAction m_Action = action;

        protected override OperationResult<string> Execute(string[] args)
        {
            StringSerializer serializer = new();
            foreach (AStringSerializer registeredSerializer in ms_RegisteredSerializers.Values)
                serializer.Register(registeredSerializer);
            ArgumentDefinition[] arguments = m_Action.ActionArguments;
            if (arguments.Length < args.Length)
                return new("Bad arguments", "Too much arguments");
            string?[] realArgs = new string[arguments.Length];
            for (int i = 0; i < realArgs.Length; i++)
                realArgs[i] = (i < args.Length) ? args[i] : null;

            for (int i = (arguments.Length - 1);  i >= 0; i--)
            {
                if (arguments[i].Required && realArgs[i] == null)
                {
                    int u = i;
                    while (u >= 0 && realArgs[u] == null)
                        u--;
                    if (u < 0)
                        return new("Bad arguments", "Not enough argument");
                    realArgs[i] = realArgs[u];
                    realArgs[u] = null;
                }
            }

            object?[] actionArgs = new object[arguments.Length];
            for (int i = 0; i < arguments.Length; i++)
            {
                if (realArgs[i] == null)
                    actionArgs[i] = arguments[i].Default;
                else
                {
                    if (ms_Serializer.IsNative(arguments[i].Type))
                        actionArgs[i] = ms_Serializer.Deserialize(realArgs[i]!, arguments[i].Type);
                    else
                    {
                        OperationResult<object?> result = serializer.Deserialize(realArgs[i]!, arguments[i].Type);
                        if (result)
                            actionArgs[i] = result.Result;
                        else
                            return new(result.Error, result.Description);
                    }
                }

                if (actionArgs[i] == null && arguments[i].Required)
                    return new("Bad arguments", string.Format("Invalid argument at pos {0}", i));
            }

            object?[] ret = m_Action.Call(actionArgs);
            if (ret.Length == 0 || ret[0] == null)
                return new(string.Empty);
            return new(ret[0]!.ToString());
        }
    }
}

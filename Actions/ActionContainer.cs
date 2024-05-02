using System.Collections;
using static CorpseLib.Actions.ActionDefinition;

namespace CorpseLib.Actions
{
    public class ActionContainer : IEnumerable<AAction>
    {
        private readonly Dictionary<string, AAction> m_Actions = [];

        public void Add(AAction action) => m_Actions[action.ActionName] = action;

        public object?[] Call(string actionName, params object?[] args)
        {
            if (m_Actions.TryGetValue(actionName, out AAction? action))
                return action.Call(args);
            return [];
        }

        public object?[] SafeCall(string actionName, params object?[] args)
        {
            if (m_Actions.TryGetValue(actionName, out AAction? action))
            {
                ArgumentDefinition[] arguments = action.ActionArguments;
                if (arguments.Length < args.Length)
                    return [];
                object?[] actionArgs = new object[arguments.Length];
                for (int i = 0; i < actionArgs.Length; i++)
                    actionArgs[i] = (i < args.Length) ? args[i] : null;

                for (int i = (arguments.Length - 1); i >= 0; i--)
                {
                    if (arguments[i].Required && actionArgs[i] == null)
                    {
                        int u = i;
                        while (u >= 0 && actionArgs[u] == null)
                            u--;
                        if (u < 0)
                            return [];
                        actionArgs[i] = actionArgs[u];
                        actionArgs[u] = null;
                    }
                }

                for (int i = 0; i < arguments.Length; i++)
                {
                    if (actionArgs[i] == null)
                        actionArgs[i] = arguments[i].Default;
                    else
                    {
                        try
                        {
                            actionArgs[i] = Helper.Cast(actionArgs[i], arguments[i].Type);
                            if (arguments[i].Required && actionArgs[i] == null)
                                return [];
                        }
                        catch
                        {
                            return [];
                        }
                    }
                }

                return action.Call(actionArgs);
            }
            return [];
        }

        public object?[] Call(string actionName, Dictionary<string, object?> args)
        {
            if (m_Actions.TryGetValue(actionName, out AAction? action))
            {
                ArgumentDefinition[] arguments = action.ActionArguments;
                object?[] actionArgs = new object[arguments.Length];

                for (int i = 0; i < actionArgs.Length; i++)
                {
                    ArgumentDefinition definition = arguments[i];
                    if (args.TryGetValue(definition.Name, out object? obj))
                    {
                        if (obj == null)
                        {
                            if (definition.Required)
                                return [];
                            else
                                actionArgs[i] = definition.Default;
                        }
                        else
                        {
                            try
                            {
                                actionArgs[i] = Helper.Cast(obj, definition.Type);
                                if (definition.Required && actionArgs[i] == null)
                                    return [];
                            }
                            catch
                            {
                                return [];
                            }
                        }
                    }
                    else
                    {
                        if (definition.Required)
                            return [];
                        else
                            actionArgs[i] = definition.Default;
                    }
                }

                return action.Call(actionArgs);
            }
            return [];
        }

        public IEnumerator<AAction> GetEnumerator() => ((IEnumerable<AAction>)m_Actions.Values).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)m_Actions.Values).GetEnumerator();
    }
}

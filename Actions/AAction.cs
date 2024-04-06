using static CorpseLib.Actions.ActionDefinition;

namespace CorpseLib.Actions
{
    public abstract class AAction(ActionDefinition actionDefinition)
    {
        private readonly ActionDefinition m_ActionDefinition = actionDefinition;

        public ArgumentDefinition[] ActionArguments => m_ActionDefinition.Arguments;
        public string ActionName => m_ActionDefinition.Name;

        public abstract object? Call(object?[] args);
    }
}

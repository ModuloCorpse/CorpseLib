namespace CorpseLib
{
    public static class Helper
    {
        public static T? Cast<T>(object value) => (T?)Cast(value, typeof(T));

        public static object? Cast(object value, Type type)
        {
            if (value.GetType().IsAssignableTo(type))
                return value;
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                    return default;
                type = Nullable.GetUnderlyingType(type)!;
            }
            if (value is string str)
            {
                if (type == typeof(Guid))
                    return Guid.Parse(str);
            }
            if (type.IsEnum)
            {
                int enumValue = (int)Convert.ChangeType(value, typeof(int));
                return Enum.ToObject(type, enumValue);
            }
            return Convert.ChangeType(value, type);
        }
    }
}

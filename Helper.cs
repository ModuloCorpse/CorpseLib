namespace CorpseLib
{
    public static class Helper
    {
        public static T? Cast<T>(object value) => (T?)Cast(value, typeof(T));
        public static object? Cast(object value, Type type)
        {
            if (value.GetType() == type)
                return value;
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                    return default;
                type = Nullable.GetUnderlyingType(type)!;
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

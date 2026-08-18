namespace MyTodo.Extensions
{
    public static class EnumParsingExtensions
    {
        public static bool TryParseOrLogWarning<TEnum>(this ILogger logger, string? value, string context, out TEnum result) where TEnum : struct, Enum
        {
            if (Enum.TryParse(value, out result))
            {
                return true;
            }

            logger.LogWarning("Invalid {EnumType} value {Value} when {Context}", typeof(TEnum).Name, value, context);
            return false;
        }
    }
}

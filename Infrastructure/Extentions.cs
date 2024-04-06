namespace Infrastructure;

public static class Extentions
{
    public static IEnumerable<Type> InheritancePathUpTo(this Type type, Type baseType)
    {
        if (type.IsInterface || baseType.IsInterface || !type.IsAssignableTo(baseType))
            throw new ArgumentException();

        while (type != baseType)
        {
            yield return type;
            type = type.BaseType!;
        }
    }
    public static IEnumerable<Type> InheritancePathUpTo<BaseType>(this Type type)
        => type.InheritancePathUpTo(typeof(BaseType));

    public static TValue GetOrInsert<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value) where TKey : notnull
    {
        if (dictionary.TryGetValue(key, out TValue? result))
            return result;
        dictionary.Add(key, value);
        return value;
    }
}

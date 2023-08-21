using System.Collections.Concurrent;

using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

public static class TypeReflectionCache
{
     private static readonly ConcurrentDictionary<Type, Delegate> CompiledExpressions = new();
    private static readonly ConcurrentDictionary<Type, List<(string name, Type type)>> PropertyDetails = new();

    public static Func<T, object[]> GetOrCreateObjectArrayConverter<T>()
    {
        if (CompiledExpressions.TryGetValue(typeof(T), out var del))
            return del as Func<T, object[]>;

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var parameterExpression = Expression.Parameter(typeof(T), "entity");
        var newArrayExpression = Expression.NewArrayInit(
            typeof(object),
            properties.Select(p => Expression.Convert(Expression.Property(parameterExpression, p), typeof(object))).ToArray()
        );
        var lambda = Expression.Lambda<Func<T, object[]>>(newArrayExpression, parameterExpression).Compile();

        CompiledExpressions.TryAdd(typeof(T), lambda);
        return lambda;
    }

    public static List<(string name, Type type)> GetOrCreatePropertyDetails<T>()
    {
        if (PropertyDetails.TryGetValue(typeof(T), out var details))
            return details;

        var propDetails = new List<(string name, Type type)>();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .Where(p => p.CanRead)
                                .ToList();

        // Create columns from property names
        foreach (var prop in properties)
        {
            propDetails.Add((prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType));
        }

        PropertyDetails.TryAdd(typeof(T), propDetails);
        return propDetails;
    }
}

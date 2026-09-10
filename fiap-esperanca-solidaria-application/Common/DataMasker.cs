using System.Reflection;

namespace FiapEsperancaSolidaria.Usuario.Application.Common;

public static class DataMasker
{
    public static IDictionary<string, object> Mask(object obj)
    {
        if (obj == null)
            return new Dictionary<string, object>();

        var result = new Dictionary<string, object>();
        var type = obj.GetType();

        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!prop.CanRead) continue;

            var value = prop.GetValue(obj);

            if (prop.GetCustomAttribute<SensitiveDataAttribute>() != null)
            {
                result[prop.Name] = "***MASKED***";
            }
            else
            {
                result[prop.Name] = NormalizeValue(value);
            }
        }

        return result;
    }

    private static object NormalizeValue(object value)
    {
        if (value is null)
            return null;

        return value switch
        {
            string s => s,
            int or long or double or decimal or bool => value,
            Guid g => g.ToString(),
            Enum e => e.ToString(),
            DateTime dt => dt.ToUniversalTime().ToString("o"),
            DateTimeOffset dto => dto.ToUniversalTime().ToString("o"),

            _ => value.ToString()
        };
    }
}
using System.Linq.Expressions;
using System.Reflection;

namespace EnumTranslatorExpression;

public static class EnumTranslatorExpressionExtensions
{
    public static Expression<Func<TSource, string>> TranslateExpression<TSource, TEnum>(
        this Expression<Func<TSource, TEnum>> enumExpression,
        Type enumTranslationsType
    )
        where TEnum : Enum
    {
        // Ensure the enum expression is a member access expression
        var enumProperty = enumExpression.Body as MemberExpression;
        if (enumProperty == null)
            throw new ArgumentException("The enumExpression must be a member expression.");

        var enumValues = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray().Reverse();

        Expression ternaryExpression = Expression.Constant("Unknown");
        // Loop through all enum values and create the ternary condition expressions
        foreach (var enumValue in enumValues)
        {
            var resourceKey = $"{typeof(TEnum).Name}_{enumValue.ToString()}";
            var propertyInfo = enumTranslationsType.GetProperty(
                resourceKey,
                BindingFlags.NonPublic | BindingFlags.Static
            );

            var staticPropertyAccess = Expression.Property(null, propertyInfo);
            var enumValueExpression = Expression.Equal(
                enumExpression.Body,
                Expression.Constant(enumValue)
            );

            ternaryExpression = Expression.Condition(
                enumValueExpression,
                staticPropertyAccess,
                ternaryExpression
            );
        }

        return Expression.Lambda<Func<TSource, string>>(
            ternaryExpression,
            enumExpression.Parameters
        );
    }
}

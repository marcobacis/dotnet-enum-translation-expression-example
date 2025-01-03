using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;

namespace EnumTranslator;

public static class EnumProjectionTranslationExtensions
{
    public static void MapFromTranslatedEnum<TSource, TDestination, TSourceMember>(
        this IProjectionMemberConfiguration<TSource, TDestination, string> mapOptions,
        Expression<Func<TSource, TSourceMember>> mapExpression)
        where TSourceMember : Enum
    {
        mapOptions.MapFrom(EnumTranslateExpression<TSource, TSourceMember, EnumTranslations>(mapExpression));
    }
    
    public static Expression<Func<TSource, string>> EnumTranslateExpression<TSource, TEnum, TTranslationsResources>(
        Expression<Func<TSource, TEnum>> enumExpression)
        where TEnum : Enum
    {
        // Ensure the enum expression is a member access expression
        var enumProperty = enumExpression.Body as MemberExpression;
        if (enumProperty == null)
            throw new ArgumentException("The enumExpression must be a member expression.");

        var enumTranslationsType = typeof(TTranslationsResources);
        
        var enumValues = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray();

        Expression ternaryExpression = Expression.Constant("Unknown");
        // Loop through all enum values and create the ternary condition expressions
        foreach (var enumValue in enumValues)
        {
            var resourceKey = $"{typeof(TEnum).Name}_{enumValue.ToString()}";

            // Access the static property (getter) via reflection
            var propertyInfo = enumTranslationsType.GetProperty(resourceKey, BindingFlags.NonPublic | BindingFlags.Static);
            var staticPropertyAccess = Expression.Property(null, propertyInfo);

            // Create the condition for the current enum value
            var enumValueExpression = Expression.Equal(enumExpression.Body, Expression.Constant(enumValue));
            var currentTernary = Expression.Condition(enumValueExpression, staticPropertyAccess, ternaryExpression);

            // Set the ternary expression to the current one, creating a chain
            ternaryExpression = currentTernary;
        }
        
        return Expression.Lambda<Func<TSource, string>>(ternaryExpression, enumExpression.Parameters);
    }
}
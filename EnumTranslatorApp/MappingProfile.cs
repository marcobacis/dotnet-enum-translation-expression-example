using System.Linq.Expressions;
using AutoMapper;
using EnumTranslatorExpression;

namespace EnumTranslator;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<MyEntity, ExampleDto>()
            .ForMember(src => src.TypeStr, opt => opt.MapFromTranslatedEnum(src => src.Type));
    }
}

public static class EnumMappingExtensions
{
    public static void MapFromTranslatedEnum<TSource, TDestination, TSourceMember>(
        this IProjectionMemberConfiguration<TSource, TDestination, string> mapOptions,
        Expression<Func<TSource, TSourceMember>> mapExpression
    )
        where TSourceMember : Enum
    {
        mapOptions.MapFrom(mapExpression.TranslateExpression(typeof(EnumTranslations)));
    }
}

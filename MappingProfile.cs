using AutoMapper;

namespace EnumTranslator;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<MyEntity, ExampleDto>()
            .ForMember(src => src.TypeStr,
                opt => 
                    opt.MapFromTranslatedEnum(src => src.Type));
    }
}
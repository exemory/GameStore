using AutoMapper;
using Business.DataTransferObjects;
using Data.Entities;

namespace Business;

public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        CreateMap<Game, GameWithGenresDto>()
            .ForMember(g => g.Genres, o => o.MapFrom(gm => gm.Genres.Select(g => g.Name)));
        CreateMap<Game, GameWithDetailsDto>()
            .ForMember(g => g.Genres, o => o.MapFrom(gm => gm.Genres.Select(g => g.Name)))
            .ForMember(g => g.PlatformTypes, o => o.MapFrom(g => g.PlatformTypes.Select(pt => pt.Type)));
        CreateMap<GameCreationDto, Game>(MemberList.Source);
        CreateMap<GameUpdateDto, Game>(MemberList.Source);

        CreateMap<Comment, CommentDto>();
        CreateMap<CommentCreationDto, Comment>(MemberList.Source);

        CreateMap<Genre, GenreDto>();

        CreateMap<PlatformType, PlatformTypeDto>();
    }
}
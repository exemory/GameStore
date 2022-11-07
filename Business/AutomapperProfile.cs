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
        CreateMap<GameCreationDto, Game>(MemberList.Source)
            .ForSourceMember(g => g.GenreIds, o => o.DoNotValidate())
            .ForSourceMember(g => g.PlatformTypeIds, o => o.DoNotValidate());
        CreateMap<GameUpdateDto, Game>(MemberList.Source)
            .ForSourceMember(g => g.GenreIds, o => o.DoNotValidate())
            .ForSourceMember(g => g.PlatformTypeIds, o => o.DoNotValidate())
            .ForMember(g => g.ImageFileName,
                o => o.Condition((d, g, v) => v != null));

        CreateMap<Comment, CommentDto>()
            .ForMember(c => c.UserInfo, o => o.MapFrom(c => c.User));
        CreateMap<CommentCreationDto, Comment>(MemberList.Source);
        CreateMap<CommentUpdateDto, Comment>(MemberList.Source);

        CreateMap<Genre, GenreDto>();

        CreateMap<PlatformType, PlatformTypeDto>();

        CreateMap<User, UserInfoDto>()
            .ForMember(u => u.HasAvatar, o => o.MapFrom(u => u.Avatar != null))
            .ForMember(u => u.UserRoles, o => o.Ignore());

        CreateMap<User, CommentUserInfoDto>()
            .ForMember(u => u.HasAvatar, o => o.MapFrom(u => u.Avatar != null));

        CreateMap<SignUpDto, User>(MemberList.Source)
            .ForSourceMember(d => d.Password, o => o.DoNotValidate());
    }
}
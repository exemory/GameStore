using AutoMapper;

namespace Business.Tests;

public static class UnitTestHelper
{
    public static MapperConfiguration CreateMapperConfiguration()
    {
        var profile = new AutomapperProfile();
        return new MapperConfiguration(cfg => cfg.AddProfile(profile));
    }

    public static IMapper CreateMapper()
    {
        var configuration = CreateMapperConfiguration();

        return new Mapper(configuration);
    }
}
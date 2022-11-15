using Xunit;

namespace Business.Tests
{
    public class AutomapperTests : TestsBase
    {
        [Fact]
        public void ValidateConfiguration()
        {
            var configuration = UnitTestHelper.CreateMapperConfiguration();

            configuration.AssertConfigurationIsValid();
        }
    }
}
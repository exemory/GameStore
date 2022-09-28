using Xunit;

namespace Business.Tests
{
    public class AutomapperTests
    {
        [Fact]
        public void ValidateConfiguration()
        {
            var configuration = UnitTestHelper.CreateMapperConfiguration();

            configuration.AssertConfigurationIsValid();
        }
    }
}
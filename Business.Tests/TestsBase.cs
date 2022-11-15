using AutoFixture;

namespace Business.Tests;

public abstract class TestsBase
{
    protected readonly IFixture Fixture = UnitTestHelper.CreateFixture();
}
using NetArchTest.Rules;

namespace Architecture.Tests;

public class ProjectReferenceTests
{
    [Fact]
    public void Domain_Should_Not_HaveDependencyOnOtherProjects()
    {
        var domainAssembly =  typeof(Sample.Domain.PlaceHolder).Assembly;
        
        var result = Types.InAssembly(domainAssembly)
            .Should()
            .NotHaveDependencyOn("Sample.Application")
            .And()
            .NotHaveDependencyOn("Sample.Front")
            .And()
            .NotHaveDependencyOn("Sample.Infrastructure")
            .And()
            .NotHaveDependencyOn("Sample.Api")
            .GetResult();
        
        Assert.True(result.IsSuccessful);
    }
}
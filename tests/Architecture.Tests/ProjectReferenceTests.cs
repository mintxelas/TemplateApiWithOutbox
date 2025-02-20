using NetArchTest.Rules;

namespace Architecture.Tests;

public class ProjectReferenceTests
{
    [Fact]
    public void Domain_Should_Not_HaveDependencyOnOtherProjects()
    {
        var assembly =  typeof(Sample.Domain.AssemblyReference).Assembly;
        
        var result = Types.InAssembly(assembly)
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
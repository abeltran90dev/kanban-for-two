using System.Reflection;
using NetArchTest.Rules;
using ArchTestResult = NetArchTest.Rules.TestResult;

namespace KanbanForTwo.ArchitectureTests;

public sealed class LayerDependencyTests
{
    private const string ApplicationNamespace = "KanbanForTwo.Application";
    private const string InfrastructureNamespace = "KanbanForTwo.Infrastructure";
    private const string ApiNamespace = "KanbanForTwo.Api";

    private static readonly Assembly _domainAssembly = typeof(Domain.AssemblyMarker).Assembly;
    private static readonly Assembly _applicationAssembly = typeof(Application.AssemblyMarker).Assembly;
    private static readonly Assembly _infrastructureAssembly = typeof(Infrastructure.AssemblyMarker).Assembly;

    [Fact]
    public void Domain_ShouldNotDependOnOtherLayers()
    {
        var result = Types.InAssembly(_domainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(ApplicationNamespace, InfrastructureNamespace, ApiNamespace)
            .GetResult();

        AssertSuccessful(result);

    }

    [Fact]
    public void Domain_ShouldNotDependOnFrameworks()
    {
        var result = Types.InAssembly(_domainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Microsoft.EntityFrameworkCore",
                "Microsoft.AspNetCore",
                "System.ComponentModel.DataAnnotations")
            .GetResult();

        AssertSuccessful(result);
    }

    [Fact]
    public void Application_ShouldNotDependOnOuterLayers()
    {
        var result = Types.InAssembly(_applicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(InfrastructureNamespace, ApiNamespace)
            .GetResult();

        AssertSuccessful(result);
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOnApi()
    {
        var result = Types.InAssembly(_infrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        AssertSuccessful(result);
    }

    private static void AssertSuccessful(ArchTestResult result)
    {
        var fallingTypes = string.Join(", ", result.FailingTypeNames ?? []);
        Assert.True(result.IsSuccessful, $"Rule violated by: {fallingTypes}");
    }



}

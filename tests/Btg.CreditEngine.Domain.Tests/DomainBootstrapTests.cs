using Btg.CreditEngine.Domain;

namespace Btg.CreditEngine.Domain.Tests;

public sealed class DomainBootstrapTests
{
    [Fact]
    public void DomainAssemblyIsLoadableWithoutApiTypes()
    {
        Assert.NotNull(typeof(DomainAssembly).Assembly);
        Assert.DoesNotContain(
            "Microsoft.AspNetCore",
            typeof(DomainAssembly).Assembly.GetReferencedAssemblies().Select(assembly => assembly.Name));
    }
}

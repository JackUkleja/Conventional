using System.Reflection;

namespace Conventional.Conventions.Solution
{
    public interface IAssemblyConventionSpecification
    {
        ConventionResult IsSatisfiedBy(Assembly assembly);
    }
}
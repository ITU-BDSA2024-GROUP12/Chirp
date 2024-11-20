using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.TestsOrder;
using Xunit.Sdk;

// file for setting up Testorderer for executing test in order.

namespace TestOrderer;

public class TestOrderer : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase
    {
        // Example: Order tests alphabetically by method name
        return testCases.OrderBy(tc => tc.TestMethod.Method.Name);
    }
}
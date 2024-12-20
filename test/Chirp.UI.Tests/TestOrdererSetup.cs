﻿using Xunit.Abstractions;
using Xunit.Sdk;

namespace TestOrderer;

/// <summary>
/// This testOrderer is meant to be used for Playwright tests but can be used elsewhere as well
/// It allows Xunit to order test by alphabetical order
/// remember to state both namespace and assembly name when using.
/// </summary>
public class AlphabeticalOrderer  : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(
        IEnumerable<TTestCase> testCases) where TTestCase : ITestCase =>
        testCases.OrderBy(testCase => testCase.TestMethod.Method.Name);
}

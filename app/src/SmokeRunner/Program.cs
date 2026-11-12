using System.Reflection;
using HeritageTrail.Tests;
using Xunit;

// Fallback runner: executes the same [Fact] integration tests without the vstest testhost
// (this machine blocks testhost's parent-process OpenProcess call).
Console.WriteLine("STEP factory init...");
var factory = new TestAppFactory();
await factory.InitializeAsync();
Console.WriteLine("STEP factory init OK");

var specType = typeof(SpecTests);
var methods = specType.GetMethods()
    .Where(m => m.GetCustomAttribute<FactAttribute>() is not null)
    .OrderBy(m => m.Name)
    .ToList();

int pass = 0, fail = 0;
foreach (var m in methods)
{
    Console.WriteLine($"RUN {m.Name}");
    // Fresh SpecTests per test = fresh client/cookies per test.
    var instance = Activator.CreateInstance(specType, factory)!;
    try
    {
        var task = (Task)m.Invoke(instance, null)!;
        await task;
        pass++;
        Console.WriteLine($"PASS {m.Name}");
    }
    catch (Exception ex)
    {
        fail++;
        var inner = ex.InnerException ?? ex;
        Console.WriteLine($"FAIL {m.Name}: {inner.GetType().Name}: {inner.Message}");
    }
}

Console.WriteLine("STEP dispose...");
await factory.DisposeAsync();
Console.WriteLine($"RESULT: {pass} passed, {fail} failed");
return fail == 0 ? 0 : 1;

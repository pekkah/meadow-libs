using System.Runtime.CompilerServices;
using VerifyTests;

namespace Chibi.Ui.SourceGenerators.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Enable();
    }
}
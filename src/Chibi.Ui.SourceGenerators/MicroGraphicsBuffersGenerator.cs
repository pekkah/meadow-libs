using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Chibi.Ui.SourceGenerators.Helpers;
using Meadow.Foundation.Graphics.Buffers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Chibi.Ui.SourceGenerators
{
    [Generator(LanguageNames.CSharp)]
    public class MicroGraphicsBuffersGenerator : IIncrementalGenerator
    {
        public static string AttributeSource =
            $@"namespace {Namespace}
{{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class {Attribute} : System.Attribute
    {{
        public string RelativeSourcePath {{ get; set; }}

        public string BufferType {{ get; set; }}
    }}
}}";

        private const string Namespace = "Chibi.Ui.MicroGraphics";
        private const string Attribute = "MicroGraphicsBuffersAttribute";
        private static readonly string FullAttributeName = $"{Namespace}.{Attribute}";


        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                //Debugger.Launch();
            }
#endif

            context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
                "MicroGraphicsBuffersAttribute.cs",
                SourceText.From(AttributeSource, Encoding.UTF8)));

            IncrementalValuesProvider<ClassDeclarationSyntax> classDeclarationsWithAttributes = context.SyntaxProvider
                .CreateSyntaxProvider(
                    static (s, _) =>
                    {
                        if (s is ClassDeclarationSyntax syntax)
                            return IsTarget(syntax);

                        return false;
                    },
                    static (ctx, _) => GetSemanticTargetForGeneration(ctx))
                .Where(syntax => syntax != null)
                .Where(syntax => syntax!.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))!;


            IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> compilationAndClasses
                = context.CompilationProvider.Combine(classDeclarationsWithAttributes.Collect());

            context.RegisterSourceOutput(compilationAndClasses,
                static (spc, source) => Execute(source.Item1, source.Item2, spc));
        }

        private static bool IsTarget(ClassDeclarationSyntax classDeclarationSyntax)
        {
            foreach (AttributeListSyntax attributeListSyntax in classDeclarationSyntax.AttributeLists)
            {
                foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
                {
                    if (Attribute.StartsWith(attributeSyntax.Name.ToFullString()))
                        return true;
                }
            }

            return false;
        }

        private static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
        {
            return (ClassDeclarationSyntax)context.Node;
        }

        private static void Execute(
            Compilation compilation,
            ImmutableArray<ClassDeclarationSyntax> classes,
            SourceProductionContext context)
        {
            if (classes.IsDefaultOrEmpty)
                // nothing to do yet
                return;

            var classDeclarationSyntaxes = classes.Distinct();
            var atlasDefinitions = GetTypesToGenerate(compilation, classDeclarationSyntaxes, context);

            StringBuilder sb = default;
            bool found = false;

            foreach (var atlasDefinition in atlasDefinitions)
            {
                if (sb is null)
                    sb = new StringBuilder();
                else
                    sb.Clear();
                try
                {
                    var result = GenerateAtlas(atlasDefinition, context);

                    if (string.IsNullOrEmpty(result))
                        continue;

                    context.AddSource(atlasDefinition.Name + ".Buffers.cs", SourceText.From(result, Encoding.UTF8));

                    found = true;
                }
                catch (Exception x)
                {
                    context.ReportDiagnostic(CreateDiagnostic(null, $"CHIBI0000", $"Error {x}", DiagnosticSeverity.Error));
                }
            }

            if (!found)
            {
                context.ReportDiagnostic(CreateDiagnostic(null, $"CHIBI0010", $"No {Attribute} found on partial classes", DiagnosticSeverity.Info));
            }
        }

        private static string GenerateAtlas(BuffersClassDefinition buffersClassDefinition, SourceProductionContext context)
        {
            var builder = new IndentedStringBuilder();

            builder.AppendLine($"namespace {buffersClassDefinition.Namespace}");
            builder.AppendLine("{");
            using (var classScope = builder.Indent())
            {
                builder.AppendLine("using Meadow.Foundation.Graphics.Buffers;");
                builder.AppendLine();

                builder.AppendLine($"public partial class {buffersClassDefinition.Name}");
                builder.AppendLine("{");
                using (var membersScope = builder.Indent())
                {
                    // combine images
                    var directory = Path.GetFullPath(Path.Combine(
                        Path.GetDirectoryName(buffersClassDefinition.FilePath) ?? string.Empty,
                        buffersClassDefinition.RelativeSourcePath));

                    buffersClassDefinition.BufferSources = ReadParts(directory);

                    foreach (var part in buffersClassDefinition.BufferSources)
                    {
                        using var memberScope = builder.Indent();

                        var name = $"{Path.GetFileNameWithoutExtension(part.FileName)}{part.Width}x{part.Height}";
                        name = $"{char.ToUpper(name[0])}{name.Substring(1)}";
                        builder.AppendLine(
                            $"public {buffersClassDefinition.BufferType} {name} {{ get; }} = new {buffersClassDefinition.BufferType}({part.Width}, {part.Height}, new byte[]");
                        builder.AppendLine("{");

                        using var sourceImage = Image.Load<Rgba32>(part.FileName);
                        if (TryGetBytes(sourceImage, buffersClassDefinition.BufferType, context, out var buffer))
                        {
                            using var bytesScope = builder.Indent();
                            var bytes = buffer!.Buffer;

                            for (var i = 0; i < bytes.Length; i++)
                            {
                                builder.Append(bytes[i].ToString());

                                if (i < bytes.Length - 1)
                                    builder.Append(",");
                            }
                        }

                        builder.AppendLine();
                        builder.AppendLine("});");
                        builder.AppendLine();
                    }
                }

                builder.AppendLine("}");
            }

            builder.AppendLine("}");
            return builder.ToString();
        }

        private static bool TryGetBytes(Image<Rgba32> imageRgb8888, string bufferType, SourceProductionContext context,
            out IDisplayBuffer? buffer)
        {
            if (imageRgb8888.TryGetSinglePixelSpan(out var data))
            {
                var rgba32Bytes = MemoryMarshal.AsBytes(data);

                buffer = CreateBuffer(imageRgb8888.Width, imageRgb8888.Height, rgba32Bytes, bufferType, context);
                return true;
            }

            buffer = null;
            return false;
        }

        private static IDisplayBuffer? CreateBuffer(int width, int height, Span<byte> rgba32Bytes, string bufferType,
            SourceProductionContext context)
        {
            // use the matching buffer to do the conversion to bufferType
            var buffer8888 = new BufferRgb8888(width, height, rgba32Bytes.ToArray());

            IDisplayBuffer? targetBuffer = bufferType switch
            {
                "Buffer1" => new Buffer1bpp(width, height),
                nameof(Buffer1bpp) => new Buffer1bpp(width, height),
                nameof(BufferGray4) => new BufferGray4(width, height),
                nameof(BufferGray8) => new BufferGray8(width, height),
                nameof(BufferRgb332) => new BufferRgb332(width, height),
                nameof(BufferRgb444) => new BufferRgb444(width, height),
                nameof(BufferRgb565) => new BufferRgb565(width, height),
                nameof(BufferRgb888) => new BufferRgb888(width, height),
                nameof(BufferRgb8888) => buffer8888,
                _ => null
            };

            if (targetBuffer is null)
            {
                context.ReportDiagnostic(CreateDiagnostic(null, "CHIBI0003", $"Could not create buffer for '{bufferType}",
                    DiagnosticSeverity.Error));
                return null;
            }

            if (targetBuffer is BufferRgb8888)
                return buffer8888;

            targetBuffer.WriteBuffer(0, 0, buffer8888);

            return targetBuffer;
        }

        private static IEnumerable<BufferSourceDefinition> ReadParts(string directory)
        {
            var pngs = Directory.GetFiles(directory, "*.png", SearchOption.AllDirectories);

            if (pngs.Length == 0)
                return Enumerable.Empty<BufferSourceDefinition>();

            var parts = new List<BufferSourceDefinition>();
            foreach (var png in pngs)
            {
                var part = Image.Identify(Configuration.Default, png, out _);
                parts.Add(new BufferSourceDefinition
                {
                    FileName = png,
                    Height = part.Height,
                    Width = part.Width
                });
            }

            return parts;
        }

        private static IEnumerable<BuffersClassDefinition> GetTypesToGenerate(
            Compilation compilation,
            IEnumerable<ClassDeclarationSyntax> classes,
            SourceProductionContext context)
        {
            var buffersAttribute = compilation.GetTypeByMetadataName(FullAttributeName);
            if (buffersAttribute == null) yield break;

            foreach (var classDeclaration in classes)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                SemanticModel semanticModel = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
                if (semanticModel.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol namedTypeSymbol)
                {
                    // report diagnostic, something went wrong
                    continue;
                }

                var name = namedTypeSymbol.Name;
                var nameSpace = namedTypeSymbol.ContainingNamespace.IsGlobalNamespace
                    ? string.Empty
                    : namedTypeSymbol.ContainingNamespace.ToString();

                string? relativeSourcePath = default;
                string? sourcePixelType = default;
                string? targetBufferType = default;
                foreach (var attributeData in namedTypeSymbol.GetAttributes())
                {
                    if (!buffersAttribute.Equals(attributeData.AttributeClass, SymbolEqualityComparer.Default)) continue;

                    foreach (var namedArgument in attributeData.NamedArguments)
                    {
                        if (namedArgument.Key == "RelativeSourcePath"
                            && namedArgument.Value.Value?.ToString() is { } relativeSourcePathArg)
                            relativeSourcePath = relativeSourcePathArg;

                        if (namedArgument.Key == "PixelType"
                            && namedArgument.Value.Value?.ToString() is { } sourcePixelTypeArg)
                            sourcePixelType = sourcePixelTypeArg;

                        if (namedArgument.Key == "BufferType"
                            && namedArgument.Value.Value?.ToString() is { } targetBufferTypeArg)
                            targetBufferType = targetBufferTypeArg;
                    }
                }

                if (relativeSourcePath is not { Length: > 0 })
                {
                    context.ReportDiagnostic(CreateDiagnostic(null, "CHIBI0001",
                        "MicroGraphicsBuffersAttribute usage missing RelativeSourcePath", DiagnosticSeverity.Error));
                    yield break;
                }

                var fullyQualifiedName = namedTypeSymbol.ToString();

                yield return new BuffersClassDefinition
                {
                    Name = name,
                    FullyQualifiedName = fullyQualifiedName,
                    Namespace = nameSpace,
                    RelativeSourcePath = relativeSourcePath,
                    FilePath = namedTypeSymbol.DeclaringSyntaxReferences.First().SyntaxTree.FilePath,
                    BufferType = targetBufferType ?? string.Empty
                };
            }
        }

        private static Diagnostic CreateDiagnostic(ClassDeclarationSyntax syntax, string id, string title,
            DiagnosticSeverity severity)
        {
            var descriptor = new DiagnosticDescriptor(
                id,
                title,
                title,
                "Build",
                severity,
                true,
                description: title);

            return Diagnostic.Create(descriptor, syntax?.GetLocation());
        }
    }
}
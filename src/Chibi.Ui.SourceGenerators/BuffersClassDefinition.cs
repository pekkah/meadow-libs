using System;
using System.Collections.Generic;

namespace Chibi.Ui.SourceGenerators
{
    public class BuffersClassDefinition
    {
        public string FilePath { get; set; } = null!;

        public string FullyQualifiedName { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Namespace { get; set; } = null!;

        public IEnumerable<BufferSourceDefinition> BufferSources { get; set; } = Array.Empty<BufferSourceDefinition>();

        public string RelativeSourcePath { get; set; } = null!;

        public string BufferType { get; set; } = null!;
    }
}
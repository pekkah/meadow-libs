using System;
using System.Collections.Generic;

namespace Chibi.Ui.SourceGenerators
{
    public class AtlasDefinition
    {
        public string FilePath { get; set; }
        public string FullyQualifiedName { get; set; }

        public string Name { get; set; }

        public string NameSpace { get; set; }

        public IEnumerable<AtlasPartDefinition> Parts { get; set; } = Array.Empty<AtlasPartDefinition>();

        public string RelativeSourcePath { get; set; } = null!;

        //public string PixelType { get; set; }

        public string BufferType { get; set; }
    }
}
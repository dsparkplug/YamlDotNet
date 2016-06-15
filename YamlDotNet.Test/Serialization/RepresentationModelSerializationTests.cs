using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace YamlDotNet.Test.Serialization
{
    public class RepresentationModelSerializationTests
    {
        [Fact]
        public void X()
        {
            var deserializer = new Deserializer();
            var scalar = deserializer.Deserialize<YamlScalarNode>(new StringReader("!!int 123"));
            var mapping = deserializer.Deserialize<YamlMappingNode>(new StringReader("{ a: b, c: d }"));
            var sequence = deserializer.Deserialize<YamlSequenceNode>(new StringReader("[ 1, { 2: 2.5 }, 3 ]"));

            var serializer = new Serializer();

            var buffer = new StringWriter();
            serializer.Serialize(buffer, new
            {
                Scalar = new YamlScalarNode("hello") { Tag = "!!int",  },
                Scalar2 = scalar,
                Mapping = new YamlMappingNode(
                    "Id", "1",
                    "Name", "hello"
                ),
                Mapping2 = mapping,
                Sequence = new YamlSequenceNode("a", "b", "c"),
                Sequence2 = sequence,
            });

            var yaml = buffer.ToString();
        }
    }
}

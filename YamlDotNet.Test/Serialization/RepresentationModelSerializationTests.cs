using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using YamlDotNet.Core;
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
            deserializer.RegisterTagMapping("tag:yaml.org,2002:binary", typeof(byte[]));
            deserializer.RegisterTypeConverter(new ByteArrayConverter());

            var scalar = deserializer.Deserialize<YamlScalarNode>(new StringReader("!!int 123"));
            var mapping = deserializer.Deserialize<YamlMappingNode>(new StringReader("{ a: b, c: d }"));
            var sequence = deserializer.Deserialize<YamlSequenceNode>(new StringReader("[ 1, { 2: 2.5 }, 3 ]"));

            var bytes = deserializer.Deserialize<byte[]>(new StringReader("!!binary R0lG"));

            var serializer = new Serializer();
            serializer.RegisterTypeConverter(new ByteArrayConverter());

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

                binary = new byte[] { 1, 2, 3 }
            });

            var yaml = buffer.ToString();
        }
    }

    public class ByteArrayConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(byte[]);
        }

        public object ReadYaml(IParser parser, Type type)
        {
            var scalar = (YamlDotNet.Core.Events.Scalar)parser.Current;
            var bytes = Convert.FromBase64String(scalar.Value);
            parser.MoveNext();
            return bytes;
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            var bytes = (byte[])value;
            emitter.Emit(new YamlDotNet.Core.Events.Scalar(
                null,
                "tag:yaml.org,2002:binary",
                Convert.ToBase64String(bytes),
                ScalarStyle.Plain,
                false,
                false
            ));
        }
    }
}

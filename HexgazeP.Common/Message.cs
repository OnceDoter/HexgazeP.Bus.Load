using System.Diagnostics.CodeAnalysis;

namespace HexgazeP.Common;

#pragma warning disable CS8618
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class Message
{
    private static readonly Random Random = new();
    
    public int IntProperty { get; set; }
    public string StringProperty { get; set; }
    public double DoubleProperty { get; set; }
    public bool BoolProperty { get; set; }
    public DateTime DateTimeProperty { get; set; }
    public Guid GuidProperty { get; set; }
    public decimal DecimalProperty { get; set; }
    public char CharProperty { get; set; }
    public long LongProperty { get; set; }
    public float FloatProperty { get; set; }
    public byte ByteProperty { get; set; }
    public short ShortProperty { get; set; }
    public ushort UShortProperty { get; set; }
    public uint UIntProperty { get; set; }
    public ulong ULongProperty { get; set; }
    public byte[] ByteArrayProperty { get; set; }
    public TimeSpan TimeSpanProperty { get; set; }
    public Uri UriProperty { get; set; }
    public Version VersionProperty { get; set; }
    public object ObjectProperty { get; set; }
    
    public NestedClass NestedClassProperty { get; set; }

    public class NestedClass
    {
        public string NestedStringProperty { get; set; }
        public int NestedIntProperty { get; set; }
    }

    public static Message InitializeRandomValues()
    {
        var message = new Message
        {
            IntProperty = Random.Next(),
            StringProperty = Guid.NewGuid().ToString(),
            DoubleProperty = Random.NextDouble(),
            BoolProperty = Random.Next(2) == 1,
            DateTimeProperty = DateTime.Now.AddSeconds(Random.Next(3600)),
            GuidProperty = Guid.NewGuid(),
            DecimalProperty = (decimal)Random.NextDouble(),
            CharProperty = (char)('a' + Random.Next(26)),
            LongProperty = Random.Next(),
            FloatProperty = (float)Random.NextDouble(),
            ByteProperty = (byte)Random.Next(256),
            ShortProperty = (short)Random.Next(),
            UShortProperty = (ushort)Random.Next(65536),
            UIntProperty = (uint)Random.Next(),
            ULongProperty = (ulong)Random.Next(),
            ByteArrayProperty = new byte[8],
            TimeSpanProperty = TimeSpan.FromSeconds(Random.Next(3600)),
            UriProperty = new Uri("http://example.com"),
            VersionProperty = new Version(Random.Next(10), Random.Next(10), Random.Next(10)),
            ObjectProperty = new object(),
            NestedClassProperty = new NestedClass
            {
                NestedStringProperty = Guid.NewGuid().ToString(),
                NestedIntProperty = Random.Next()
            },
        };
        Random.NextBytes(message.ByteArrayProperty);
        return message;
    }
}
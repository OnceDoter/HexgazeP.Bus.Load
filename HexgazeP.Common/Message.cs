namespace HexgazeP.Common;

public class Message
{
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
        Random random = new Random();
        var message = new Message()
        {
            IntProperty = random.Next(),
            StringProperty = Guid.NewGuid().ToString(),
            DoubleProperty = random.NextDouble(),
            BoolProperty = random.Next(2) == 1,
            DateTimeProperty = DateTime.Now.AddSeconds(random.Next(3600)),
            GuidProperty = Guid.NewGuid(),
            DecimalProperty = (decimal)random.NextDouble(),
            CharProperty = (char)('a' + random.Next(26)),
            LongProperty = random.Next(),
            FloatProperty = (float)random.NextDouble(),
            ByteProperty = (byte)random.Next(256),
            ShortProperty = (short)random.Next(),
            UShortProperty = (ushort)random.Next(65536),
            UIntProperty = (uint)random.Next(),
            ULongProperty = (ulong)random.Next(),
            ByteArrayProperty = new byte[8],
            TimeSpanProperty = TimeSpan.FromSeconds(random.Next(3600)),
            UriProperty = new Uri("http://example.com"),
            VersionProperty = new Version(random.Next(10), random.Next(10), random.Next(10)),
            ObjectProperty = new object(),
            NestedClassProperty = new NestedClass
            {
                NestedStringProperty = Guid.NewGuid().ToString(),
                NestedIntProperty = random.Next()
            },
        };
        random.NextBytes(message.ByteArrayProperty);
        return message;
    }
}
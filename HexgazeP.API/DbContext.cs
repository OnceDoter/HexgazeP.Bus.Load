using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace HexgazeP.API;

public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbContext(DbContextOptions<DbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Message> Messages { get; set; }
    
    
    public class Message
    {
        [Key]
        public Guid GuidProperty { get; set; }
        public int IntProperty { get; set; }
        public string StringProperty { get; set; }
        public double DoubleProperty { get; set; }
        public bool BoolProperty { get; set; }
        public DateTime DateTimeProperty { get; set; }
        public decimal DecimalProperty { get; set; }
        public char CharProperty { get; set; }
        public long LongProperty { get; set; }
        public float FloatProperty { get; set; }
        public byte ByteProperty { get; set; }
        public short ShortProperty { get; set; }
        public ushort UShortProperty { get; set; }
        public uint UIntProperty { get; set; }
        public ulong ULongProperty { get; set; }
        public TimeSpan TimeSpanProperty { get; set; }
        public string NestedClassProperty { get; set; }
    }
}
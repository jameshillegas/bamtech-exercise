using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace StargateAPI.Business.Data;

[Table("Log")]
public class Log
{
    public int Id { get; set; }
    public string? GroupIdentifier { get; set; }
    public required string Message { get; set; }
    public string? Data { get; set; }
    public bool IsException { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class LogConfiguration : IEntityTypeConfiguration<Log>
{
    public void Configure(EntityTypeBuilder<Log> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Message).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
    }
}


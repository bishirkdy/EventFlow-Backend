using System.Text.Json;
using EventFlow.Registration.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EventFlow.Registration.Infrastructure.Persistence.Configurations;

public sealed class CertificateSettingsConfiguration : IEntityTypeConfiguration<CertificateSettings>
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public void Configure(EntityTypeBuilder<CertificateSettings> b)
    {
        b.ToTable("CertificateSettings");
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.EventId).IsUnique();
        b.Property(x => x.Title).HasMaxLength(200).IsRequired();
        b.Property(x => x.Subtitle).HasMaxLength(500).IsRequired();
        b.Property(x => x.SignatoryName).HasMaxLength(200);
        b.Property(x => x.SignatoryTitle).HasMaxLength(200);
        b.Property(x => x.ThemeColor).HasMaxLength(9).IsRequired();
        b.Property(x => x.BackgroundImageUrl).HasMaxLength(1000);
        b.Property(x => x.BackgroundImagePublicId).HasMaxLength(300);
        b.Property(x => x.TemplateFields)
            .HasColumnType("jsonb")
            .HasConversion(
                new ValueConverter<List<CertificateTemplateField>, string>(
                    fields => JsonSerializer.Serialize(fields, SerializerOptions),
                    json => string.IsNullOrWhiteSpace(json)
                        ? new List<CertificateTemplateField>()
                        : JsonSerializer.Deserialize<List<CertificateTemplateField>>(json, SerializerOptions) ?? new List<CertificateTemplateField>()))
            .Metadata.SetValueComparer(JsonValueComparer);
    }

    private static ValueComparer<List<CertificateTemplateField>> JsonValueComparer =>
        new(
            (a, b) => a != null && b != null && a.Count == b.Count && a.SequenceEqual(b),
            fields => fields.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
            fields => fields.Select(CloneField).ToList());

    private static CertificateTemplateField CloneField(CertificateTemplateField item) => new()
    {
        Key = item.Key,
        Text = item.Text,
        XPercent = item.XPercent,
        YPercent = item.YPercent,
        WidthPercent = item.WidthPercent,
        FontSizePt = item.FontSizePt,
        FontColor = item.FontColor,
        Bold = item.Bold,
        Align = item.Align,
        FontFamily = item.FontFamily
    };
}

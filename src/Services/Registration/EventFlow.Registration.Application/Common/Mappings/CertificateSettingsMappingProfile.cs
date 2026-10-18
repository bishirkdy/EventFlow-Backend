using AutoMapper;
using EventFlow.Registration.Application.Contracts.Certificates;
using EventFlow.Registration.Domain.Entities;
using EventFlow.Registration.Domain.Enums;

namespace EventFlow.Registration.Application.Common.Mappings;

public sealed class CertificateSettingsMappingProfile : Profile
{
    public CertificateSettingsMappingProfile()
    {
        CreateMap<CertificateSettings, CertificateSettingsDto>()
            .ForMember(
                destination => destination.TemplateMode,
                options => options.MapFrom(source => source.TemplateMode.ToString()))
            .ForMember(
                destination => destination.UpdatedAtUtc,
                options => options.MapFrom(source => source.UpdatedAt));

        CreateMap<CertificateTemplateField, CertificateTemplateFieldDto>()
            .ForMember(
                destination => destination.Align,
                options => options.MapFrom(source => source.Align.ToString()));

        CreateMap<CertificateTemplateFieldDto, CertificateTemplateField>()
            .ForMember(
                destination => destination.Key,
                options => options.MapFrom(source => source.Key.Trim()))
            .ForMember(
                destination => destination.Text,
                options => options.MapFrom(
                    source => string.IsNullOrWhiteSpace(source.Text)
                        ? null
                        : source.Text.Trim()))
            .ForMember(
                destination => destination.FontColor,
                options => options.MapFrom(
                    source => string.IsNullOrWhiteSpace(source.FontColor)
                        ? "#111827"
                        : source.FontColor.Trim()))
            .ForMember(
                destination => destination.Align,
                options => options.MapFrom(source => ParseAlign(source.Align)))
            .ForMember(
                destination => destination.FontFamily,
                options => options.MapFrom(
                    source => string.IsNullOrWhiteSpace(source.FontFamily)
                        ? null
                        : source.FontFamily.Trim()));

        CreateMap<string, CertificateTemplateMode>()
            .ConvertUsing(value => ParseTemplateMode(value));
    }

    public static CertificateTemplateMode ParseTemplateMode(string? value) =>
        Enum.TryParse<CertificateTemplateMode>(value, ignoreCase: true, out var mode)
            ? mode
            : CertificateTemplateMode.Classic;

    private static CertificateFieldAlign ParseAlign(string? value) =>
        Enum.TryParse<CertificateFieldAlign>(value, ignoreCase: true, out var align)
            ? align
            : CertificateFieldAlign.Center;
}

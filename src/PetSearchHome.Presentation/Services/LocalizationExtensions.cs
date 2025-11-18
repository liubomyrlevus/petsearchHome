using System;
using PetSearchHome.BLL.Domain.Enums;

namespace PetSearchHome.Presentation.Services;

public static class LocalizationExtensions
{
    public static string ToUkrainianLabel(this AnimalType value) => value switch
    {
        AnimalType.dog => "Собака",
        AnimalType.cat => "Кіт / Кішка",
        AnimalType.bird => "Птах",
        AnimalType.rodent => "Гризун",
        AnimalType.reptile => "Рептилія",
        AnimalType.other => "Інший вид",
        _ => "Невідомо"
    };

    public static string ToUkrainianLabel(this AnimalSex? value) => value switch
    {
        AnimalSex.male => "Самець",
        AnimalSex.female => "Самка",
        _ => "Невідомо"
    };

    public static string ToUkrainianLabel(this AnimalSize? value) => value switch
    {
        AnimalSize.small => "Малий",
        AnimalSize.medium => "Середній",
        AnimalSize.large => "Великий",
        AnimalSize.giant => "Дуже великий",
        _ => "Невідомо"
    };

    public static string ToUkrainianLabel(this ListingStatus value) => value switch
    {
        ListingStatus.active => "Опубліковано",
        ListingStatus.pending => "На модерації",
        ListingStatus.draft => "Чернетка",
        ListingStatus.rejected => "Відхилено",
        ListingStatus.archived => "Архів",
        _ => value.ToString()
    };

    public static string ToUkrainianLabel(this string? animalType) =>
        Enum.TryParse(animalType, true, out AnimalType parsed)
            ? parsed.ToUkrainianLabel()
            : string.IsNullOrWhiteSpace(animalType)
                ? "Невідомо"
                : animalType;
}

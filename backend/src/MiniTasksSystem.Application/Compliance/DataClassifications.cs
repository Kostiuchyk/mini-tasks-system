using Microsoft.Extensions.Compliance.Classification;

namespace MiniTasksSystem.Application.Compliance;

public static class DataTaxonomy
{
    public static string TaxonomyName { get; } = typeof(DataTaxonomy).FullName!;

    public static DataClassification PiiData { get; } = new(TaxonomyName, nameof(PiiData));
    public static DataClassification SensitiveData { get; } = new(TaxonomyName, nameof(SensitiveData));

}

public sealed class PersonalDataAttribute : DataClassificationAttribute
{
    public PersonalDataAttribute() : base(DataTaxonomy.PiiData)
    {
    }
}

public sealed class SensitiveDataAttribute : DataClassificationAttribute
{
    public SensitiveDataAttribute() : base(DataTaxonomy.SensitiveData)
    {
    }
}

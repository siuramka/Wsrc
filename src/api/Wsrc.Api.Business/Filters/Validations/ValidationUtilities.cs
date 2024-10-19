namespace Wsrc.Api.Business.Filters.Validations;

public class ValidationUtilities
{
    public string? Required(string value, string fieldName)
    {
        return string.IsNullOrWhiteSpace(value)
            ? string.Format(ValidationConstants.RequiredFieldMessage, fieldName)
            : null;
    }

    public string? MaxLength(string value, string fieldName, int maxLength)
    {
        return value.Length > maxLength
            ? string.Format(ValidationConstants.MaxNameLengthMessage, fieldName, maxLength)
            : null;
    }

    public string GetErrors(List<string?> errors)
    {
        return errors
            .Where(e => e is not null)
            .Select(e => e + "\n")
            .Aggregate(string.Empty, (acc, e) => acc + e);
    }
}
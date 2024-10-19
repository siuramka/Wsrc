namespace Wsrc.Api.Business.Filters.Validations;

public static class ValidationConstants
{
    public const int MaxNameLength = 256;

    public const string MaxNameLengthMessage = "{0} cannot be longer than {1} characters.";
    
    public const string RequiredFieldMessage = "{0} is required.";
}
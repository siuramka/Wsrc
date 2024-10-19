namespace Wsrc.Api.Business.Interfaces;

public interface IParametersValidationFilter<T>
{
    public string GetValidationErrors(T entity);
}
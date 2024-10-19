namespace Wsrc.Api.Business.Interfaces;

public interface IParameterFilter<T>
{
    public string GetValidationErrors(T entity);
}
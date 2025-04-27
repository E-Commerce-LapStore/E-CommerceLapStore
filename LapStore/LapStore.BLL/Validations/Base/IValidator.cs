namespace LapStore.BLL.Validations.Base
{
    public interface IValidator<T>
    {
        (bool IsValid, List<string> Errors) Validate(T entity);
    }
} 
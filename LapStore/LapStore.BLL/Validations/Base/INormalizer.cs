namespace LapStore.BLL.Validations.Base
{
    public interface INormalizer<T>
    {
        T Normalize(T entity);
    }
} 
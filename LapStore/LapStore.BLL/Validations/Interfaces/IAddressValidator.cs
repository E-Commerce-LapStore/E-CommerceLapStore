using LapStore.BLL.Validations.Base;
using LapStore.BLL.ViewModels;

namespace LapStore.BLL.Validations.Interfaces
{
    public interface IAddressValidator : IValidator<AddressVM>, INormalizer<AddressVM>
    {

    }
}

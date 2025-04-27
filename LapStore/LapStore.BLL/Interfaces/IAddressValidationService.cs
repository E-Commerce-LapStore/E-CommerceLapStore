using LapStore.BLL.ViewModels;

namespace LapStore.BLL.Interfaces
{
    public interface IAddressValidationService
    {
        (bool IsValid, List<string> Errors) ValidateAddress(AddressVM address);
        AddressVM NormalizeAddress(AddressVM address);
    }
} 
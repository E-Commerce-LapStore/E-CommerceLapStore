using System.Text.RegularExpressions;
using LapStore.BLL.Validations.Interfaces;
using LapStore.BLL.ViewModels;

namespace LapStore.BLL.Validations.Services
{
    public class AddressValidator : IAddressValidator
    {
        public (bool IsValid, List<string> Errors) Validate(AddressVM address)
        {
            var errors = new List<string>();

            ValidateStreet(address.Street, errors);
            ValidateCity(address.City, errors);
            ValidateGovernorate(address.Governorate, errors);
            ValidateZipCode(address.ZipCode, errors);
            ValidateCountry(address.Country, errors);

            return (errors.Count == 0, errors);
        }

        public AddressVM Normalize(AddressVM address)
        {
            return new AddressVM
            {
                Id = address.Id,
                Street = NormalizeStreet(address.Street),
                City = NormalizeCity(address.City),
                Governorate = NormalizeGovernorate(address.Governorate),
                ZipCode = NormalizeZipCode(address.ZipCode),
                Country = NormalizeCountry(address.Country)
            };
        }

        private void ValidateStreet(string street, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(street))
            {
                errors.Add("Street is required");
            }
            else if (street.Length > 100)
            {
                errors.Add("Street cannot exceed 100 characters");
            }
        }

        private void ValidateCity(string city, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                errors.Add("City is required");
            }
            else if (city.Length > 50)
            {
                errors.Add("City cannot exceed 50 characters");
            }
        }

        private void ValidateGovernorate(string governorate, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(governorate))
            {
                errors.Add("State/Governorate is required");
            }
            else if (governorate.Length > 50)
            {
                errors.Add("State/Governorate cannot exceed 50 characters");
            }
        }

        private void ValidateZipCode(string zipCode, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(zipCode))
            {
                errors.Add("ZIP Code is required");
            }
            else if (!Regex.IsMatch(zipCode, @"^\d{5}(-\d{4})?$"))
            {
                errors.Add("Invalid ZIP Code format");
            }
        }

        private void ValidateCountry(string country, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(country))
            {
                errors.Add("Country is required");
            }
            else if (country.Length > 50)
            {
                errors.Add("Country cannot exceed 50 characters");
            }
        }

        private string NormalizeStreet(string street)
        {
            if (string.IsNullOrWhiteSpace(street)) return string.Empty;

            street = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(street.ToLower());

            street = street.Replace(" St ", " Street ")
                         .Replace(" Ave ", " Avenue ")
                         .Replace(" Rd ", " Road ")
                         .Replace(" Blvd ", " Boulevard ");

            return Regex.Replace(street, @"\s+", " ").Trim();
        }

        private string NormalizeCity(string city)
        {
            if (string.IsNullOrWhiteSpace(city)) return string.Empty;

            city = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(city.ToLower());
            return Regex.Replace(city, @"\s+", " ").Trim();
        }

        private string NormalizeGovernorate(string governorate)
        {
            if (string.IsNullOrWhiteSpace(governorate)) return string.Empty;

            governorate = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(governorate.ToLower());
            return Regex.Replace(governorate, @"\s+", " ").Trim();
        }

        private string NormalizeZipCode(string zipCode)
        {
            if (string.IsNullOrWhiteSpace(zipCode)) return string.Empty;

            zipCode = Regex.Replace(zipCode, @"[^\d-]", "");

            if (Regex.IsMatch(zipCode, @"^\d{5}$"))
            {
                return zipCode;
            }
            else if (Regex.IsMatch(zipCode, @"^\d{5}-\d{4}$"))
            {
                return zipCode;
            }
            else if (Regex.IsMatch(zipCode, @"^\d{9}$"))
            {
                return zipCode.Insert(5, "-");
            }

            return zipCode;
        }

        private string NormalizeCountry(string country)
        {
            if (string.IsNullOrWhiteSpace(country)) return string.Empty;

            country = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(country.ToLower());
            return Regex.Replace(country, @"\s+", " ").Trim();
        }
    }
} 
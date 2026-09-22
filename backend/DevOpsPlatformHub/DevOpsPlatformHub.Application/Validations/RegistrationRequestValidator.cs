using System.ComponentModel.DataAnnotations;
using DevOpsPlatformHub.Application.Dtos.Authentication;
using DevOpsPlatformHub.Core.Exceptions;

namespace DevOpsPlatformHub.Application.Authentication.Validations;

public static class RegistrationRequestValidator
{
    public static void Validate(RegistrationRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors[nameof(RegistrationRequest.Name)] = ["Name is required."];
        }
        else if (request.Name.Trim().Length > 100)
        {
            errors[nameof(RegistrationRequest.Name)] = ["Name must not exceed 100 characters"];
        }

        if (string.IsNullOrWhiteSpace(request.Username))
        {
            errors[nameof(RegistrationRequest.Username)] = ["Username is required."];
        }
        else if (request.Username.Trim().Length > 100)
        {
            errors[nameof(RegistrationRequest.Username)] = ["Username must not exceed 100 characters"];
        }
        else if (request.Username.Contains('@', StringComparison.Ordinal))
        {
            errors[nameof(RegistrationRequest.Username)] = ["Username must not contain '@'"];
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            errors[nameof(RegistrationRequest.Email)] = ["Email is required."];
        }
        else if (request.Email.Trim().Length > 254 || !new EmailAddressAttribute().IsValid(request.Email.Trim()))
        {
            errors[nameof(RegistrationRequest.Email)] = ["Email must be a valid email address of 254 characters or fewer."];
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
        {
            errors[nameof(RegistrationRequest.Password)] = ["Password must contain at least 8 characters."];
        }

        if (errors.Count > 0)
        {
            throw new ValidationFailureException(errors);
        }
    }
}

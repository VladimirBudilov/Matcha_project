using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using DAL.Helpers;
using Web_API.DTOs;
using Web_API.DTOs.Request;
using SixLabors.ImageSharp;

namespace Web_API.Helpers;

public class DtoValidator
{
    public void CheckUserAuth(long id, IEnumerable<Claim> claims)
    {
        if (claims == null)
        {
            throw new NotAuthorizedRequestException();
        }

        var claimId = claims.FirstOrDefault(c => c.Type == "Id");
        if (claimId == null)
        {
            throw new NotAuthorizedRequestException();
        }

        var authorised = int.TryParse(claimId.Value, out var userId);
        if (!authorised || id != userId)
        {
            throw new ForbiddenRequestException();
        }
    }

    public void UserDto(UserDto userDto)
    {
        if (userDto == null)
        {
            throw new DataValidationException("UserDto is null.");
        }

        ValidateEmail(userDto.Email);
        ValidateName(userDto.UserName);
        ValidateName(userDto.FirstName);
        ValidateName(userDto.LastName);
    }

    public void UserRegistrationDto(UserRegistrationDto userRegistrationDto)
    {
        if (userRegistrationDto == null)
        {
            throw new DataValidationException("UserRegestrationDto is null.");
        }

        ValidateEmail(userRegistrationDto.Email);
        ValidateName(userRegistrationDto.UserName);
        ValidateName(userRegistrationDto.FirstName);
        ValidateName(userRegistrationDto.LastName);
        ValidatePassword(userRegistrationDto.Password);
    }

    public void UserAuthRequestDto(UserAuthRequestDto userAuthRequestDto)
    {
        if (userAuthRequestDto == null)
        {
            throw new DataValidationException("UserAuthRequestDto is null.");
        }

        ValidateName(userAuthRequestDto.UserName);
        ValidatePassword(userAuthRequestDto.Password);
    }

    public void ProfileRequestDto(ProfileDto profileCreationResponseDto)
    {
        if (profileCreationResponseDto == null)
        {
            throw new DataValidationException("ProfileDto is null.");
        }

        ValidateGender(profileCreationResponseDto.Gender);
        ValidateSexualPreference(profileCreationResponseDto.SexualPreferences);
        ValidateBio(profileCreationResponseDto.Biography);
        ValidateLocation(profileCreationResponseDto.Latitude, profileCreationResponseDto.Longitude);
        ValidateAge(profileCreationResponseDto.Age);
        CheckTags(profileCreationResponseDto.Interests);
    }

    public void CheckId(int id)
    {
        if (id <= 0)
        {
            throw new DataValidationException("Invalid id format.");
        }
    }

    public void CheckPositiveNumber(int number)
    {
        if (number < 0)
        {
            throw new DataValidationException("Invalid number format.");
        }
    }

    #region

    private static void ValidateEmail(string email)
    {
        if (!new EmailAddressAttribute().IsValid(email))
        {
            throw new DataValidationException("Invalid email format.");
        }
    }

    public void ValidatePassword(string password)
    {
        if (!(password.Length >= 8 && password.Length <= 20 &&
              password.Any(char.IsDigit) && password.Any(char.IsUpper)))
        {
            throw new DataValidationException(
                "Invalid password format. Must be between 8 and 20 characters long, contain at least one digit and one uppercase letter.");
        }
    }

    public void ValidateName(string username)
    {
        if (!(username.Length >= 3 && username.Length <= 20 &&
              username.All(char.IsLetterOrDigit)))
        {
            throw new DataValidationException("Invalid username format. Must be between 3 and 20 characters long.");
        }
    }

    private void ValidateGender(string gender)
    {
        if (!(gender is "male" or "female"))
        {
            throw new DataValidationException("Invalid gender format. Must be male or female.");
        }
    }

    private void ValidateSexualPreference(string sexualPreference)
    {
        if (!(sexualPreference is "male" or "female" or "both" or null))
        {
            throw new DataValidationException("Invalid sexual preference format.Must be male, female or both.");
        }
    }

    private void ValidateAge(int age)
    {
        if (!(age >= 18 && age <= 100))
        {
            throw new DataValidationException("Invalid age format.");
        }
    }

    private void ValidateBio(string bio)
    {
        var isValid = bio.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || char.IsPunctuation(c));
        if (!(isValid && bio.Length <= 500))
        {
            throw new DataValidationException("Invalid bio format.");
        }
    }

    private void ValidateTag(string tag)
    {
        if(tag[0] != '#') throw new DataValidationException("Invalid tag format.");
        tag = tag.Substring(1);
        if (!(tag.Length is >= 3 and <= 50 && tag.All(x => char.IsLetterOrDigit(x) || x == '_' ||  x == '-')))
        {
            throw new DataValidationException("Invalid tag format.");
        }
    }

    private void ValidateLocation(double? latitude, double? longitude)
    {
        if (latitude is null || latitude < -90 || latitude > 90)
        {
            throw new DataValidationException("Invalid latitude format.");
        }

        if (longitude is null || longitude < -180 || longitude > 180)
        {
            throw new DataValidationException("Invalid longitude format.");
        }
    }

    public void ValidatePhoto(IFormFile photo)
    {
        long _maxFileSize = 5 * 1024 * 1024;
        string[] _permittedExtensions = new[] { ".jpg", ".jpeg", ".png" };

        var isValid = Validate(photo, out var message);

        if (!isValid)
        {
            throw new DataValidationException(message);
        }

        bool Validate(IFormFile file, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (file.Length > _maxFileSize)
            {
                errorMessage = $"File size exceeds {_maxFileSize / 1024 / 1024} MB.";
                return false;
            }

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !_permittedExtensions.Contains(ext))
            {
                errorMessage = "Invalid file type.";
                return false;
            }

            if (!IsImage(file))
            {
                errorMessage = "Invalid image file.";
                return false;
            }

            return true;
        }

        bool IsImage(IFormFile file)
        {
            try
            {
                using var img = Image.Load(file.OpenReadStream());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    #endregion

    public void EmailAndToken(string email, string token)
    {
        ValidateEmail(email);
        if (string.IsNullOrEmpty(token))
        {
            throw new DataValidationException("Token is null or empty.");
        }
    }

    public void CheckSearchParameters(SearchParameters search)
    {
        ExecuteIfNotNull(CheckDistance, search.MaxDistance);
        CheckFilteringRage(search.MinFameRating, search.MaxFameRating);
        CheckFilteringRage(search.MinAge, search.MaxAge);
        CheckFilteringRage(search.MinFameRating, search.MaxFameRating);
        ExecuteIfNotNull(ValidateSexualPreference, search.SexualPreferences);
        CheckTags(search.CommonTags);
    }

    private void CheckTags(List<string> commonTags)
    {
        foreach (var tag in commonTags)
        {
            ValidateTag(tag);
        }
    }

    private void CheckFilteringRage(int? min, int? max)
    {
        if (min is null && max is null) return;
        if (min is null || max is null) throw new DataValidationException("Invalid filtering rage format.");
        if (min > max) throw new DataValidationException("Invalid filtering rage format.");
        CheckPositiveNumber(min.Value);
        CheckPositiveNumber(max.Value);
    }


    private void CheckDistance(int? obj)
    {
        if (obj < 0)
        {
            throw new DataValidationException("Invalid distance format.");
        }
    }

    public void CheckSortParameters(SortParameters sort)
    {
        var sortList = sort.ToList();
        foreach (var sortParameter in sortList)
        {
            if (sortParameter != "ASC" && sortParameter != "DESC")
            {
                throw new DataValidationException("Invalid sort parameter format.");
            }
        }

        if (sort.SortingMainParameter < 0 || sort.SortingMainParameter > 3)
            throw new DataValidationException("Invalid sorting main parameter format.");
    }

    public void CheckPaginationParameters(PaginationParameters pagination)
    {
        if (pagination.PageNumber <= 0 || pagination.PageSize <= 0)
        {
            throw new DataValidationException("Invalid pagination parameters format.");
        }
    }

    public void ExecuteIfNotNull<T>(Action<T> action, T value)
    {
        if (value != null)
        {
            action(value);
        }
    }
}
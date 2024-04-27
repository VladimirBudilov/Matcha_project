using System.ComponentModel.DataAnnotations;
using Web_API.DTOs;

namespace Web_API.Helpers;

public class DtoValidator
{
    public void UserDto(UserDto userDto)
    {
        if (userDto == null)
        {
            throw new DataValidationException("UserDto is null.");
        }
        ValidateEmail(userDto.Email);
        ValidatePassword(userDto.Password);
        ValidateUsername(userDto.UserName);
        ValidateName(userDto.FirstName);
        ValidateSurname(userDto.LastName);
    }
    
    public void UserAuthRequestDto(UserAuthRequestDto userAuthRequestDto)
    {
        if (userAuthRequestDto == null)
        {
            throw new DataValidationException("UserAuthRequestDto is null.");
        }
        ValidateUsername(userAuthRequestDto.UserName);
        ValidatePassword(userAuthRequestDto.Password);
    }

    public void ProfileRequestDto(ProfileDto profileDto)
    {
        if (profileDto == null)
        {
            throw new DataValidationException("ProfileDto is null.");
        }
        ValidateUsername(profileDto.UserName);
        ValidateName(profileDto.FirstName);
        ValidateSurname(profileDto.LastName);
        ValidateAge(profileDto.Age);
    }
    
    public void CheckId(int id)
    {
        if (id <= 0)
        {
            throw new DataValidationException("Invalid id format.");
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

    private static void ValidatePassword(string password)
    {
        if (!(password.Length >= 8 && password.Length <= 20 &&
              password.Any(char.IsDigit) && password.Any(char.IsUpper)))
        {
            throw new DataValidationException("Invalid password format. Must be between 8 and 20 characters long, contain at least one digit and one uppercase letter.");
        }
    }

    public void ValidateUsername(string username)
    {
        if (!(username.Length >= 3 && username.Length <= 20 &&
              username.All(char.IsLetter)))
        {
            throw new DataValidationException("Invalid username format. Must be between 3 and 20 characters long.");
        }
    }

    private void ValidateName(string name)
    {
        if (!(name.Length >= 3 && name.Length <= 20 &&
              name.All(char.IsLetter)))
        {
            throw new DataValidationException("Invalid name format.");
        }
    }

    private void ValidateSurname(string surname)
    {
        if (!(surname.Length >= 3 && surname.Length <= 20 &&
              surname.All(char.IsLetter)))
        {
            throw new DataValidationException("Invalid surname format.");
        }
    }

    private void ValidatePhoneNumber(string phoneNumber)
    {
        if (!(phoneNumber.Length == 9 && phoneNumber.All(char.IsDigit)))
        {
            throw new DataValidationException("Invalid phone number format.");
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
        var isValid = bio.All(char.IsLetterOrDigit) || bio.All(char.IsPunctuation);
        if (!(isValid && bio.Length <= 500))
        {
            throw new DataValidationException("Invalid bio format.");
        }
    }

    private void ValidateTag(string tag)
    {
        if (!(tag.Length >= 3 && tag.Length <= 20 && tag.All(char.IsLetter)))
        {
            throw new DataValidationException("Invalid tag format.");
        }
    }

    private void ValidateTagList(string tagList)
    {
        var tags = tagList.Split(',');
        if (!tags.All(tag =>
            {
                try
                {
                    ValidateTag(tag);
                    return true;
                }
                catch
                {
                    return false;
                }
            }))
        {
            throw new DataValidationException("Invalid tag list format.");
        }
    }

    private void ValidateLocation(string location)
    {
        // Add your validation logic here
        // If validation fails, throw exception
        // throw new DataValidationException("Invalid location format.");
    }

    private void ValidatePhoto(string photo)
    {
        // Add your validation logic here
        // If validation fails, throw exception
        // throw new DataValidationException("Invalid photo format.");
    }
    #endregion

}
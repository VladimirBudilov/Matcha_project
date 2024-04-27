using System.ComponentModel.DataAnnotations;
using Web_API.DTOs;

namespace Web_API.Helpers;

public class DtoValidator
{
    public void UserDtoValidator(UserDto userDto)
    {
        ValidateEmail(userDto.Email);
        ValidatePassword(userDto.Password);
        ValidateUsername(userDto.UserName);
        ValidateName(userDto.FirstName);
        ValidateSurname(userDto.LastName);
    }
    
    public void UserAuthRequestDtoValidator(UserAuthRequestDto userAuthRequestDto)
    {
        ValidateUsername(userAuthRequestDto.UserName);
        ValidatePassword(userAuthRequestDto.Password);
    }

    public void ProfileDtoValidator(ProfileDto profileDto)
    {
        ValidateUsername(profileDto.UserName);
        ValidateName(profileDto.FirstName);
        ValidateSurname(profileDto.LastName);
        ValidateAge(profileDto.Age);
    }


    #region 
    public void ValidateEmail(string email)
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

    public void ValidateName(string name)
    {
        if (!(name.Length >= 3 && name.Length <= 20 &&
              name.All(char.IsLetter)))
        {
            throw new DataValidationException("Invalid name format.");
        }
    }

    public void ValidateSurname(string surname)
    {
        if (!(surname.Length >= 3 && surname.Length <= 20 &&
              surname.All(char.IsLetter)))
        {
            throw new DataValidationException("Invalid surname format.");
        }
    }

    public void ValidatePhoneNumber(string phoneNumber)
    {
        if (!(phoneNumber.Length == 9 && phoneNumber.All(char.IsDigit)))
        {
            throw new DataValidationException("Invalid phone number format.");
        }
    }

    public void ValidateGender(string gender)
    {
        if (!(gender is "male" or "female"))
        {
            throw new DataValidationException("Invalid gender format. Must be male or female.");
        }
    }

    public void ValidateSexualPreference(string sexualPreference)
    {
        if (!(sexualPreference is "male" or "female" or "both" or null))
        {
            throw new DataValidationException("Invalid sexual preference format.Must be male, female or both.");
        }
    }

    public void ValidateAge(int age)
    {
        if (!(age >= 18 && age <= 100))
        {
            throw new DataValidationException("Invalid age format.");
        }
    }

    public void ValidateBio(string bio)
    {
        var isValid = bio.All(char.IsLetterOrDigit) || bio.All(char.IsPunctuation);
        if (!(isValid && bio.Length <= 500))
        {
            throw new DataValidationException("Invalid bio format.");
        }
    }

    public void ValidateTag(string tag)
    {
        if (!(tag.Length >= 3 && tag.Length <= 20 && tag.All(char.IsLetter)))
        {
            throw new DataValidationException("Invalid tag format.");
        }
    }

    public void ValidateTagList(string tagList)
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

    public void ValidateLocation(string location)
    {
        // Add your validation logic here
        // If validation fails, throw exception
        // throw new DataValidationException("Invalid location format.");
    }

    public void ValidatePhoto(string photo)
    {
        // Add your validation logic here
        // If validation fails, throw exception
        // throw new DataValidationException("Invalid photo format.");
    }
    #endregion

}
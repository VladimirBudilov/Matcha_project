using System.ComponentModel.DataAnnotations;

namespace Web_API.Helpers;

public class DtoValidator
{
    public bool ValidateEmail(string email)
    {
        return new EmailAddressAttribute().IsValid(email);
    }

    public bool ValidatePassword(string password)
    {
        return password.Length >= 8 && password.Length <= 20 &&
               password.Any(char.IsDigit) && password.Any(char.IsUpper);
    }

    public bool ValidateUsername(string username)
    {
        return username.Length >= 3 && username.Length <= 20 &&
               username.All(char.IsLetter);
    }

    public bool ValidateName(string name)
    {
        return name.Length >= 3 && name.Length <= 20 &&
               name.All(char.IsLetter);
    }

    public bool ValidateSurname(string surname)
    {
        return surname.Length >= 3 && surname.Length <= 20 &&
               surname.All(char.IsLetter);
    }

    public bool ValidatePhoneNumber(string phoneNumber)
    {
        return phoneNumber.Length == 9 && phoneNumber.All(char.IsDigit);
    }

    public bool ValidateGender(string gender)
    {
        return gender is "male" or "female";
    }

    public bool ValidateSexualPreference(string sexualPreference)
    {
        return sexualPreference is "male" or "female" or "both" or null;
    }
    
    public bool ValidateAge(int age)
    {
        return age >= 18 && age <= 100;
    }
    
    public bool ValidateBio(string bio)
    {
        var isValid = bio.All(char.IsLetterOrDigit) || bio.All(char.IsPunctuation);
        return isValid && bio.Length <= 500;
    }
    
    public bool ValidateTag(string tag)
    {
        return tag.Length >= 3 && tag.Length <= 20 && tag.All(char.IsLetter);
    }
    
    public bool ValidateTagList(string tagList)
    {
        var tags = tagList.Split(',');
        return tags.All(ValidateTag);
    }
    
    public bool ValidateLocation(string location)
    {
        return true;
        //return location.Length >= 3 && location.Length <= 50;
    }  
    
    public bool ValidatePhoto(string photo)
    {
        return true;
        //return photo.Length >= 3 && photo.Length <= 50;
    }
    
    
}
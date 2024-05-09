using System.ComponentModel.DataAnnotations;
using Web_API.Helpers;

namespace Web_API.Test
{
    public class DtoValidatorTests
    {
        [Fact]
        public void DtoValidator_ValidatePassword_ValidPassword()
        {
            //Arange
            var validator = new DtoValidator();
            validator.ValidatePassword("12345");
            //Assign
            
            //Assert
        }
    }
}
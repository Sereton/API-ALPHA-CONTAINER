using System;

using System.ComponentModel.DataAnnotations;


namespace Entities.DTO
{
    public abstract class EmployeeForManipulationDTO
    {
      
        
            [Required(ErrorMessage = "The employee must have a name")]
            [MaxLength(30, ErrorMessage = "Maximum length of name is 30 characters")]
            public string Name { get; set; }
            [Required(ErrorMessage = "The employee must have an age recorded.")]
            [Range(18, 200, ErrorMessage = "The employee must be at least 18, the age must be an integer")]
            public int Age { get; set; }
            [Required(ErrorMessage = "The employee must have a position")]
            [MaxLength(30, ErrorMessage = "Maximum length of position is 30 characters")]
            public string Position { get; set; }
        
    }
}

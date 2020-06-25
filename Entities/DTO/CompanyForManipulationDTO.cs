using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.DTO
{
    public abstract class CompanyForManipulationDTO
    {
        [Required(ErrorMessage = "The company must have a name")]
        [MaxLength(30, ErrorMessage = "Maximum length of company is 30 characters")]
        public string Name { get; set; }
        [Required(ErrorMessage = "The Address must be included")]
        [MaxLength(30, ErrorMessage = "Maximum length of company is 30 characters")]
        public string Address { get; set; }
        [Required(ErrorMessage = "The Country must be included")]
        [MaxLength(30, ErrorMessage = "Maximum length of country is 30 characters")]
        public string Country { get; set; }

        public IEnumerable<EmployeeForCreationDTO> Employees { get; set; }
    }
}

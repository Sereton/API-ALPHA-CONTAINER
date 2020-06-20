using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Models
{
    public class Company
    {
        [Column("CompanyId")]
        public Guid Id { get; set; }

        [Required(ErrorMessage ="Company name is a required field")]
        [MaxLength(69, ErrorMessage ="Maximum length for the name is 69 characters")]

        public string Name { get; set; }

        [Required(ErrorMessage = "Company name is a required field")]
        [MaxLength(69, ErrorMessage = "Maximum length for the Address is 69 characters")]

        public string Address { get; set; }

        public string Country { get; set; }

        public ICollection<Employee> Employees { get; set; }


    }
}
 
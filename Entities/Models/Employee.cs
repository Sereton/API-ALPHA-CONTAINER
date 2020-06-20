using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Models
{
    public class Employee
    {
        [Column("EmployeeId")]
        public Guid Id { get; set; }

        [Required(ErrorMessage ="A employee name must be provided")]
        [MaxLength(30, ErrorMessage ="Max length for the name is 30 characters")]

        public string Name { get; set; }

        [Required(ErrorMessage = "A employee age must be provided")]
        public int Age { get; set; }

        [Required(ErrorMessage = "A employee position must be provided")]
        [MaxLength(30, ErrorMessage = "Max length for the position is 30 characters")]

        public string Position { get; set; }

        [ForeignKey(nameof(Company))]
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }


    }
}

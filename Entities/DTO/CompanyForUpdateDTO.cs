﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DTO
{
    public class CompanyForUpdateDTO
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }

        public IEnumerable<EmployeeForCreationDTO> Employees { get; set; }
    }
}

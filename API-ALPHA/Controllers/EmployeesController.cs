﻿using AutoMapper;
using Contracts;
using Entities.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_ALPHA.Controllers
{
    [Route("/api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public EmployeesController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        
        public IActionResult GetEmployeesForCompany(Guid companyId)
        {
            var company = _repository.Company.GetCompany(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
            return NotFound();
            }
            var employeesFromDb = _repository.Employee.GetEmployees(companyId,
            trackChanges: false);

            var employeesDto = _mapper.Map<IEnumerable<EmployeeDTO>>(employeesFromDb);
            return Ok(employeesDto);
        }

        [HttpGet("{employeeId}")]

        public IActionResult GetEmployeeForCompany(Guid companyId, Guid employeeId)
        {
             var company = _repository.Company.GetCompany(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId}  doesn't exist in the database.");
                return NotFound();
            }

            var employee = _repository.Employee.GetEmployee(companyId, employeeId, trackChanges: false);
            if (employee == null)
            {
                _logger.LogInfo($"Employee with id: {employeeId}  doesn't exist in the database.");
                return NotFound();
            }

            else
            {
                var employeeDto = _mapper.Map<EmployeeDTO>(employee);
                return Ok(employeeDto);
            }
        }
    }
}

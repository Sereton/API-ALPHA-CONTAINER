using API_ALPHA.ActionFilters;
using API_ALPHA.ModelBinders;
using AutoMapper;
using Contracts;
using Entities.DTO;
using Entities.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        [ServiceFilter(typeof(ValidateCompanyExistsAttribute))]
        
        public async Task<IActionResult> GetEmployeesForCompany(Guid companyId)
        {
            var company = HttpContext.Items["company"] as Company;
            var employeesFromDb = await _repository.Employee.GetEmployeesAsync(company.Id,
            trackChanges: false);

            var employeesDto = _mapper.Map<IEnumerable<EmployeeDTO>>(employeesFromDb);
            return Ok(employeesDto);
        }

        [HttpGet("{id}", Name="GetEmployeeForCompany")]
        
        [ServiceFilter(typeof(ValidateEmployeeForCompanyExistsAttribute))]

        public  IActionResult GetEmployeeForCompany(Guid companyId, Guid id)
        {
            
            var employee = HttpContext.Items["employee"] as Employee;



            

            
             var employeeDto = _mapper.Map<EmployeeDTO>(employee);
             return Ok(employeeDto);
            
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateCompanyExistsAttribute))]
        public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDTO employee)
        {

          

            
            var employeeEntity = _mapper.Map<Employee>(employee);
            _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
            await _repository.SaveAsync();

            var employeeToReturn = _mapper.Map<EmployeeDTO>(employeeEntity);
            return CreatedAtRoute("GetEmployeeForCompany",new{ companyId,id = employeeToReturn.Id},
                employeeToReturn);
        }

        [HttpDelete("{id}")]
        
        [ServiceFilter(typeof(ValidateEmployeeForCompanyExistsAttribute))]


        public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid id)
        {

            var employeeForCompany = HttpContext.Items["employee"] as Employee;
           

            _repository.Employee.DeleteEmployee(employeeForCompany);
            await _repository.SaveAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        
        [ServiceFilter(typeof(ValidateEmployeeForCompanyExistsAttribute))]

        public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] EmployeeForUpdateDTO
            employee)
        {
            
       
            //trackChanges allows to "record" the modifications made to that item and eventually save them to the DB
            var employeeEntity =  HttpContext.Items["employee"] as Employee;

            

            _mapper.Map(employee, employeeEntity);//I copy employee(my DTO to be updated) to employeeEntity(in the DB)
            await _repository.SaveAsync();
            return NoContent();



        }


        [HttpPatch("{id}")]
        
        [ServiceFilter(typeof(ValidateEmployeeForCompanyExistsAttribute))]

        public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(
                Guid companyId, Guid id, [FromBody] JsonPatchDocument<EmployeeForUpdateDTO> jsonPatchDocument
            )
        {
            if (jsonPatchDocument == null)
            {
                _logger.LogError("Bad patch document");
                return BadRequest();
            }

            var employeeEntity = HttpContext.Items["employee"] as Employee;
         
            var employeeToPatch = _mapper.Map<EmployeeForUpdateDTO>(employeeEntity);
            jsonPatchDocument.ApplyTo(employeeToPatch, ModelState);
            TryValidateModel(employeeToPatch);
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the patch document");
                return UnprocessableEntity(ModelState);
            }
            _mapper.Map(employeeToPatch, employeeEntity);
            await _repository.SaveAsync();

            return NoContent();
        }
    }
}

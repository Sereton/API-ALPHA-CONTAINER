using AutoMapper;
using Contracts;
using Entities.DTO;
using Entities.Models;
using Microsoft.AspNetCore.JsonPatch;
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
        
        public async Task<IActionResult> GetEmployeesForCompany(Guid companyId)
        {
            var company =  await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
            return NotFound();
            }
            var employeesFromDb = await _repository.Employee.GetEmployeesAsync(companyId,
            trackChanges: false);

            var employeesDto = _mapper.Map<IEnumerable<EmployeeDTO>>(employeesFromDb);
            return Ok(employeesDto);
        }

        [HttpGet("{id}", Name="GetEmployeeForCompany")]

        public  async Task<IActionResult> GetEmployeeForCompany(Guid companyId, Guid id)
        {
             var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId}  doesn't exist in the database.");
                return NotFound();
            }

            var employee = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges: false);
            if (employee == null)
            {
                _logger.LogInfo($"Employee with id: {id}  doesn't exist in the database.");
                return NotFound();
            }

            else
            {
                var employeeDto = _mapper.Map<EmployeeDTO>(employee);
                return Ok(employeeDto);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDTO employee)
        {
            if (employee == null)
            {
                _logger.LogError("EmployeeForCreationDto object sent from client is null.");
                return BadRequest("EmployeeForCreationDto object is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the employee DTO");
                return UnprocessableEntity(ModelState);
            }
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
            return NotFound();
            }
            var employeeEntity = _mapper.Map<Employee>(employee);
            _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
            await _repository.SaveAsync();

            var employeeToReturn = _mapper.Map<EmployeeDTO>(employeeEntity);
            return CreatedAtRoute("GetEmployeeForCompany",new{ companyId,id = employeeToReturn.Id},
                employeeToReturn);
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid id)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }
            var employeeForCompany = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges: false);
            if (employeeForCompany == null)
            {
                _logger.LogInfo($"The employee with id: {id} could not be found.");
                return NotFound();
            }

            _repository.Employee.DeleteEmployee(employeeForCompany);
            await _repository.SaveAsync();

            return NoContent();
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] EmployeeForUpdateDTO
            employee)
        {
            if (employee == null)
            {
                _logger.LogError($"The employee submitted is incorrect or has wrong format");
                return BadRequest("The employee is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the employee for update DTO");
                return UnprocessableEntity(ModelState);
            }
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges:false);
            if (company == null)
            {
                _logger.LogInfo("The company submitted was not found");
                return NotFound();
            }
            //trackChanges allows to "record" the modifications made to that item and eventually save them to the DB
            var employeeEntity = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges: true);

            if (employeeEntity == null)
            {
                _logger.LogInfo("The employee submitted was not found");
                return NotFound();
            }

            _mapper.Map(employee, employeeEntity);//I copy employee(my DTO to be updated) to employeeEntity(in the DB)
            await _repository.SaveAsync();
            return NoContent();



        }


        [HttpPatch("{id}")]

        public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(
                Guid companyId, Guid id, [FromBody] JsonPatchDocument<EmployeeForUpdateDTO> jsonPatchDocument
            )
        {
            if (jsonPatchDocument == null)
            {
                _logger.LogError("Bad patch document");
                return BadRequest();
            }
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }
            var employeeEntity = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges: true);
            if (employeeEntity == null)
            {
                _logger.LogInfo($"The employee with id: {id} could not be found.");
                return NotFound();
            }

            var employeeToPatch = _mapper.Map<EmployeeForUpdateDTO>(employeeEntity);
            jsonPatchDocument.ApplyTo(employeeToPatch, ModelState);
           /* TryValidateModel(employeeToPatch);*/
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

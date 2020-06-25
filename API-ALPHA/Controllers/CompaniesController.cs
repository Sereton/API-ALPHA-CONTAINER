using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_ALPHA.ActionFilters;
using API_ALPHA.ModelBinders;
using AutoMapper;
using Contracts;
using Entities.DTO;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_ALPHA.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public CompaniesController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]

        public async Task <IActionResult> GetCompanies()
        {
            
           
                var companies = await _repository.Company.GetAllCompaniesAsync(trackChanges: false);
                var companiesDTO = _mapper.Map<IEnumerable<CompanyDTO>>(companies);
         
            return Ok(companiesDTO);
            
          
        }

        [HttpGet("{id}", Name ="CompanyById")]

        public async Task<IActionResult> GetCompany(Guid id)
        {
            var company = await _repository.Company.GetCompanyAsync(id, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with the id of {company.Id} does not exist in the DB");
                return NotFound();
            }

            else
            {
                var companyDto = _mapper.Map<CompanyDTO>(company);
                return Ok(companyDto);
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDTO company)
        {
            

            var companyEntity = _mapper.Map<Company>(company);

                _repository.Company.CreateCompany(companyEntity);
               await  _repository.SaveAsync();

                var companyToReturn = _mapper.Map<CompanyDTO>(companyEntity);

                return CreatedAtRoute("CompanyById", new {id= companyToReturn.Id }, companyToReturn);
           
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            var companyToRemove = await _repository.Company.GetCompanyAsync(id, trackChanges: false);
            if (companyToRemove == null)
            {
                _logger.LogInfo($"The company with id: {id} could not be found.");
                    return NotFound();
            }
            _repository.Company.DeleteCompany(companyToRemove);
            await _repository.SaveAsync();
            return NoContent();

        }
        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]

        public  async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyForUpdateDTO company)
        {
            
            var companyToUpdate = await _repository.Company.GetCompanyAsync(id, trackChanges: true);
            if (companyToUpdate == null)
            {
                _logger.LogInfo($"The company with id: {id} could not be found.");
                return NotFound();
            }

            _mapper.Map(company, companyToUpdate);
            await _repository.SaveAsync();
            return NoContent();

        }

        [HttpGet("collection/({ids})", Name = "CompanyCollection")]

        public async Task<IActionResult> GetCompanyCollection([ModelBinder(BinderType = 
            typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                _logger.LogError("The ids of the companies are incorrect");
                return BadRequest("Ids are null");
            }

            var companyEntities = await _repository.Company.GetByIdAsync(ids, trackChanges: false);

            if(ids.Count() != companyEntities.Count())
            {
                _logger.LogError("Some companies have wrong ids");
                return NotFound();
            }
            var companiesToReturn = _mapper.Map<IEnumerable<CompanyDTO>>(companyEntities);
            return Ok(companiesToReturn);
        }

        [HttpPost("collection")]

        public async Task<IActionResult> CreateCompanyCollection([FromBody]
        IEnumerable<CompanyForCreationDTO> companyCollection
            )
        {
            if(companyCollection == null)
            {
                _logger.LogError("Company collection sent from client is null.");
                return BadRequest("Company collection is null");
            }

            var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
            foreach(var company in companyEntities)
            {
                _repository.Company.CreateCompany(company);
            }

            await  _repository.SaveAsync();

            var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDTO>>(companyEntities);
            var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));

            return CreatedAtRoute("CompanyCollection", new { ids }, companyCollectionToReturn);
        }


    }
}

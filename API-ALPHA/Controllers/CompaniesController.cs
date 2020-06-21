using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public IActionResult GetCompanies()
        {
            
           
                var companies = _repository.Company.GetAllCompanies(trackChanges: false);
                var companiesDTO = _mapper.Map<IEnumerable<CompanyDTO>>(companies);
         
            return Ok(companiesDTO);
            
          
        }

        [HttpGet("{id}", Name ="CompanyById")]

        public IActionResult GetCompany(Guid id)
        {
            var company = _repository.Company.GetCompany(id, trackChanges: false);
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
        public IActionResult CreateCompany([FromBody] CompanyForCreationDTO company)
        {
            if (company == null)
            {
                _logger.LogError($"CompanyDTO sent by client was null");
                return BadRequest("CompantyDTO sent is null");
            }
          
            
                var companyEntity = _mapper.Map<Company>(company);

                _repository.Company.CreateCompany(companyEntity);
                _repository.Save();

                var companyToReturn = _mapper.Map<CompanyDTO>(companyEntity);

                return CreatedAtRoute("CompanyById", new {id= companyToReturn.Id }, companyToReturn);
           
        }

        [HttpDelete("{id}")]

        public IActionResult DeleteCompany(Guid id)
        {
            var companyToRemove = _repository.Company.GetCompany(id, trackChanges: false);
            if (companyToRemove == null)
            {
                _logger.LogInfo($"The company with id: {id} could not be found.");
                    return NotFound();
            }
            _repository.Company.DeleteCompany(companyToRemove);
            _repository.Save();
            return NoContent();

        }
        [HttpPut("{id}")]

        public IActionResult UpdateCompany(Guid id, [FromBody] CompanyForUpdateDTO company)
        {
            if (company == null)
            {
                _logger.LogError("CompanyForUpdateDto object sent from client is null.");
                return BadRequest("CompanyForUpdateDto object is null");
            }
            var companyToUpdate = _repository.Company.GetCompany(id, trackChanges: true);
            if (companyToUpdate == null)
            {
                _logger.LogInfo($"The company with id: {id} could not be found.");
                return NotFound();
            }

            _mapper.Map(company, companyToUpdate);
            _repository.Save();
            return NoContent();

        }

        [HttpGet("collection/({ids})", Name = "CompanyCollection")]

        public IActionResult GetCompanyCollection([ModelBinder(BinderType = 
            typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                _logger.LogError("The ids of the companies are incorrect");
                return BadRequest("Ids are null");
            }

            var companyEntities = _repository.Company.GetByIds(ids, trackChanges: false);

            if(ids.Count() != companyEntities.Count())
            {
                _logger.LogError("Some companies have wrong ids");
                return NotFound();
            }
            var companiesToReturn = _mapper.Map<IEnumerable<CompanyDTO>>(companyEntities);
            return Ok(companiesToReturn);
        }

        [HttpPost("collection")]

        public IActionResult CreateCompanyCollection([FromBody]
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

            _repository.Save();

            var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDTO>>(companyEntities);
            var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));

            return CreatedAtRoute("CompanyCollection", new { ids }, companyCollectionToReturn);
        }


    }
}

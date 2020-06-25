using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_ALPHA.ActionFilters
{
    public class ValidateEmployeeForCompanyExistsAttribute : IAsyncActionFilter
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        public ValidateEmployeeForCompanyExistsAttribute(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var method = context.HttpContext.Request.Method;
            var trackChanges = (method.Equals("PUT")|| method.Equals("PATCH")) ? true :false;
            var companyId = (Guid)context.ActionArguments["companyId"];
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
            if (company == null)
            {
                _logger.LogInfo($"A company with the id: {companyId} does not exist.");
                context.Result = new NotFoundResult();
                return;
            }
            var id = (Guid)context.ActionArguments["id"];
            var employee = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);
            if (employee == null)
            {
                _logger.LogInfo($"An employee with the id: {id} does not exist in company{company.Name}.");
                context.Result = new NotFoundResult();
                return;
            }

            else
            {
                context.HttpContext.Items.Add("employee", employee);
                await next();
            }


        }
    }
}

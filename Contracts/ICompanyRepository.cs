using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICompanyRepository
    {
        Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges);
        Task<IEnumerable<Company>> GetByIdAsync(IEnumerable<Guid> ids, bool trackChanges);
        Task<Company> GetCompanyAsync(Guid companyID, bool trackChanges);

        void CreateCompany(Company company);
        /*void CreateCompanies(IEnumerable<Company> companies);*/

        void DeleteCompany(Company company);
    }
}

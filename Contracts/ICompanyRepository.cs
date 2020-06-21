using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public interface ICompanyRepository
    {
        IEnumerable<Company> GetAllCompanies(bool trackChanges);
        IEnumerable<Company> GetByIds(IEnumerable<Guid> ids, bool trackChanges);
        Company GetCompany(Guid companyID, bool trackChanges);

        void CreateCompany(Company company);
        /*void CreateCompanies(IEnumerable<Company> companies);*/

        void DeleteCompany(Company company);
    }
}

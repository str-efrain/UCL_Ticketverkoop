using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domains.Data;
using Domains.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services
{
    public class SeizoenService: ISeizoenService
    {
        private readonly  ISeizoenDAO _seizoenDAO;

        public SeizoenService(ISeizoenDAO seizoenDAO)
        {
            _seizoenDAO = seizoenDAO;
        }

        public Task AddAsync(Seizoen entity)
        {
            return _seizoenDAO.AddAsync(entity);
        }

        public Task DeleteAsync(Seizoen entity)
        {
            return _seizoenDAO.DeleteAsync(entity);
        }

        public Task<Seizoen?> FindByIdAsync(int Id)
        {
            return _seizoenDAO.FindByIdAsync(Id);
        }

        public Task<IEnumerable<Seizoen>?> GetAllAsync()
        {
            return _seizoenDAO.GetAllAsync();
        }

        public Task<Seizoen> GetCurrentSeasonAsync()
        {
            return _seizoenDAO.GetCurrentSeasonAsync();
        }

        public Task UpdateAsync(Seizoen entity)
        {
            return _seizoenDAO.UpdateAsync(entity);
        }
    }       
}
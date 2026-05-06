using Domains.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ISeizoenService : IService<Domains.Entities.Seizoen>
    {
        Task<Seizoen> GetCurrentSeasonAsync();
    }
}

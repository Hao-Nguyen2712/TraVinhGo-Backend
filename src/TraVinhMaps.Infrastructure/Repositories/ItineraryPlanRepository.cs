using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Infrastructure.Db;

namespace TraVinhMaps.Infrastructure.CustomRepositories
{
    public class ItineraryPlanRepository : BaseRepository<ItineraryPlan>, IItineraryPlanRepository
    {
        public ItineraryPlanRepository(IDbContext context) : base(context)
        {
        }
    }
}

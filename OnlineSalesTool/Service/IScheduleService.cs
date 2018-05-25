using OnlineSalesTool.Logic;
using OnlineSalesTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesTool.Service
{
    public interface IScheduleService
    {
        Task<ShiftAssignerViewModel> Get();
        Task Create(ScheduleContainer schedule);
    }
}

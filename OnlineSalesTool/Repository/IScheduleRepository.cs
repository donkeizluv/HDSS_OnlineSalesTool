using OnlineSalesTool.Logic;
using OnlineSalesTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesTool.Repository
{
    public interface IScheduleRepository : IDisposable
    {
        Task<ShiftAssignerViewModel> CreateAssignerVM();
        Task SaveSchedule(ScheduleContainer schedule);
    }
}

using OnlineSalesTool.Logic;
using OnlineSalesTool.DTO;
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
        Task<IEnumerable<ScheduleDetailDTO>> GetDetail(int posScheduleId);
        Task<int> Create(ScheduleContainer schedule);
    }
}

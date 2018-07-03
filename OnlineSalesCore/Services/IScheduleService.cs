using OnlineSalesCore.DTO;
using OnlineSalesCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineSalesCore.Services
{
    public interface IScheduleService : IDisposable
    {
        Task<ShiftAssignerVM> Get();
        Task<IEnumerable<ScheduleDetailDTO>> GetDetail(int posScheduleId);
        Task<int> Create(ScheduleContainer schedule);
        bool AllowCreate(DateTime current);
    }
}

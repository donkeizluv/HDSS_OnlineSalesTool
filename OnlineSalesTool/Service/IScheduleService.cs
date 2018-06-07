﻿using OnlineSalesTool.Const;
using OnlineSalesTool.DTO;
using OnlineSalesTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesTool.Const
{
    public interface IScheduleService : IDisposable
    {
        Task<ShiftAssignerViewModel> Get();
        Task<IEnumerable<ScheduleDetailDTO>> GetDetail(int posScheduleId);
        Task<int> Create(ScheduleContainer schedule);
    }
}

﻿using OnlineSalesTool.Logic;
using OnlineSalesTool.DTO;
using System;
using System.Collections.Generic;

namespace OnlineSalesTool.ViewModels
{
    /// <summary>
    /// Shift assigner special VM
    /// </summary>
    public class ShiftAssignerViewModel
    {
        private DateTime _systemMonthYear;
        //All POSs under mangement
        public IEnumerable<PosDTO> POSs { get; set; }
        //All users under management
        public IEnumerable<AppUserDTO> Users { get; set; }
        //Current system MonthYear
        public DateTime SystemMonthYear { get => _systemMonthYear; set => _systemMonthYear = value.Date; }
        //Total days of current system date
        public int TotalDaysOfMonth { get => DateTime.DaysInMonth(SystemMonthYear.Year, SystemMonthYear.Month); }
    }
}

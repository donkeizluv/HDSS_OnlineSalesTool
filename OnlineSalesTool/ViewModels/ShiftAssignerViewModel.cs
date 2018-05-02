﻿using OnlineSalesTool.Logic;
using OnlineSalesTool.POCO;
using System;
using System.Collections.Generic;

namespace OnlineSalesTool.ViewModels
{
    public class ShiftAssignerViewModel
    {
        private DateTime _systemMonthYear;
        //All POSs under mangement
        public IEnumerable<PosPOCO> POSs { get; set; }
        //All users under management
        public IEnumerable<AppUserPOCO> Users { get; set; }
        //Current system MonthYear
        public DateTime SystemMonthYear { get => _systemMonthYear; set => _systemMonthYear = value.Date; }
        public string SystemMonthYearDisplay => SystemMonthYear.ToString("MM-yyyy");
        //Total days of current system date
        public int TotalDaysOfMonth { get => DateTime.DaysInMonth(SystemMonthYear.Year, SystemMonthYear.Month); }
    }
}

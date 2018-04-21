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
    }
}

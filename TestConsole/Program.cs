using NLog;
using OnlineSalesTool.EFModel;
using OnlineSalesTool.Logic;
using OnlineSalesTool.Logic.Impl;
using OnlineSalesTool.POCO;
using OnlineSalesTool.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public static IEnumerable<DateTime> GetDaysInMonthRange(DateTime lastDate, DateTime firstDate)
        {
            return Enumerable.Range(0, (int)(lastDate - firstDate).TotalDays + 1)
                  .Select(i => firstDate.AddDays(i));
        }

        static void Main(string[] args)
        {
            var context = new OnlineSalesContext("Server=(LocalDb)\\local;Database=OnlineSales;");
            //TestScheduleMatcher(context);
            //SchedulerRepo_Check(context).GetAwaiter().GetResult();
            //TestSchedulerRepo_CreateVM(context).GetAwaiter().GetResult();
            //Wtf();
            Console.WriteLine("Done!");
            Console.ReadLine();
        }
        private static void Wtf()
        {
            var list1 = new int[] { 1, 2 };
            var list2 = new int[] { 2 };
            Console.WriteLine("list1 except list2");
            foreach (var item in list1.Except(list2))
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("list2 except list1");
            foreach (var item in list2.Except(list1))
            {
                Console.WriteLine(item);
            }
        }
        //private static async Task TestSchedulerRepo_CreateVM(OnlineSalesContext context)
        //{
        //    var repo = new ScheduleRepository(context, new ClaimsPrincipal(new[] { new ClaimsIdentity(new[] { new Claim(CustomClaims.UserId.ToString(), "1") }) }));
        //    var vm = await repo.CreateAssignerVM();
        //    //print test rs
        //    foreach (var pos in vm.POSs)
        //    {
        //        Console.WriteLine($"{pos.PosName} {pos.PosId}");
        //        Console.WriteLine($"Current month schedule: {pos.HasCurrentMonthSchedule}");
        //        foreach (var shift in pos.Shifts)
        //        {
        //            Console.WriteLine($"{shift.Name}");
        //            Console.WriteLine("Details:");
        //            foreach (var detail in shift.ShiftDetails)
        //            {
        //                Console.WriteLine($"{detail.StartAt.ToString()} - {detail.EndAt.ToString()}");
        //            }
        //        }
        //        Console.WriteLine($"Prev schedule count: {pos.PreviousMonthSchedule.Count()}");
        //        Console.WriteLine("Prev schedule detail:");
        //        foreach (var prev in pos.PreviousMonthSchedule)
        //        {
        //            Console.WriteLine($"PosId: {prev.TargetPos} M/Y: {prev.MonthYear.ToString("MM/yyyy")}");
        //            Console.WriteLine("Shift details:");
        //            foreach (var shift in prev.ShiftSchedulePOCOs)
        //            {
        //                Console.WriteLine($"{shift.ShiftDate.ToShortDateString()}-user: {shift.UserId}-shift: {shift.ShiftId}");
        //            }
        //        }
        //    }
            
        //}

        //private static async Task SchedulerRepo_Check(OnlineSalesContext context)
        //{
        //    var repo = new ScheduleRepository(context, new ClaimsPrincipal(new[] { new ClaimsIdentity(new[] { new Claim(CustomClaims.UserId.ToString(), "1") }) }));
        //    var details = new List<ShiftSchedulePOCO>();
        //    foreach (var day in GetDaysInMonthRange(new DateTime(2018, 1, 31), new DateTime(2018, 1, 1)))
        //    {
        //        details.Add(new ShiftSchedulePOCO()
        //        {
        //            ShiftDate = day,
        //            ShiftId = 2,
        //            UserId = 3
        //        });
        //        details.Add(new ShiftSchedulePOCO()
        //        {
        //            ShiftDate = day,
        //            ShiftId = 3,
        //            UserId = 4
        //        });
        //    }
            
        //    var schedule = new MonthSchedule(2, new DateTime(2018, 1, 1), details);
        //    await repo.SaveSchedule(schedule);
        //}

        private static void TestScheduleMatcher(OnlineSalesContext context)
        {
            var matcher = new SimpleScheduleMatcher(context);
            var testList = new List<string>() { "POS12345", "POS12346" };

            foreach (var item in testList)
            {
                if (matcher.GetUserMatchedSchedule(item, DateTime.Now , out var ids, out string reason))
                {
                    foreach (var id in ids)
                    {
                        _logger.Trace($"match user id: {id}");
                    }
                }
                else
                {
                    _logger.Trace(reason);
                }
            }
            
        }
    }
}

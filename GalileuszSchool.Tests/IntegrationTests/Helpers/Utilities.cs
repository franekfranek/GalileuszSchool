using GalileuszSchool.Infrastructure;
using GalileuszSchool.Models.ModelsForNormalUsers.Calendar;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalileuszSchool.Tests.IntegrationTests.Helpers
{
    public static class Utilities
    {
        #region snippet1
        public static void InitializeDbForTests(GalileuszSchoolContext db)
        {
            db.CalendarEvents.AddRange(GetSeedingCalendarEvents());
            db.SaveChanges();
        }

        public static void ReinitializeDbForTests(GalileuszSchoolContext db)
        {
            db.CalendarEvents.RemoveRange(db.CalendarEvents);
            InitializeDbForTests(db);
        }

        public static List<CalendarEvent> GetSeedingCalendarEvents()
        {
            return new List<CalendarEvent>()
            {
                new CalendarEvent(){ Title = "TEST RECORD: You're standing on my scarf.",
                                        Start = DateTime.Now.AddDays(1), End= DateTime.Now.AddDays(2)},
                new CalendarEvent(){ Title = "TEST RECORD: Would you like a jelly baby?",
                                        Start = DateTime.Now.AddDays(3), End= DateTime.Now.AddDays(4)},
                new CalendarEvent(){ Title = "TEST RECORD: To the rational mind, " +
                    "nothing is inexplicable; only unexplained.",
                                        Start = DateTime.Now.AddDays(5), End= DateTime.Now.AddDays(6)}
            };
        }
        #endregion
    }
}

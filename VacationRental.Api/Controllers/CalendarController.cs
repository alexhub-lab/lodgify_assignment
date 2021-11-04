using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Data.Contracts;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/calendar")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly ICalendarService _calendarProvider;

        public CalendarController(ICalendarService calendarProvider)
        {
            _calendarProvider = calendarProvider ?? throw new ArgumentNullException(nameof(calendarProvider));
        }

        [HttpGet]
        public CalendarViewModel Get(int rentalId, DateTime start, int nights)
        {
            var result = _calendarProvider.Get(rentalId, start, nights);
            return result;
        }
    }
}

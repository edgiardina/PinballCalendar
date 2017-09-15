using Ical.Net;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Ical.Net.Serialization.iCalendar.Serializers;
using PinballApi;
using PinballApi.Models.WPPR.Calendar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace PinballCalendar.Controllers
{
    public class CalendarController : ApiController
    {
        [Route("api/calendar/{address}/{distance}")]
        public async Task<HttpResponseMessage> Get(string address, int distance, bool showLeagues = true)
        {
            var apiKey = ConfigurationSettings.AppSettings["WPPRKey"];
            var rankingApi = new PinballRankingApi(apiKey);

            var pinballEvents = await rankingApi.GetCalendarSearch(address, distance, DistanceUnit.Miles);

            var calendarEvents = pinballEvents.Calendar                
                .Select(n =>
                new Event
                {
                    Description = n.Details,
                    Summary = n.TournamentName,
                    DtStart = new CalDateTime(n.StartDate),
                    DtEnd = new CalDateTime(n.EndDate),
                    Url = new Uri(n.Website),
                    GeographicLocation = new GeographicLocation(n.Latitude, n.Longitude),
                    Location = $"{n.City}, {n.State} {n.CountryName}",
                    //Can't show an organizer if you want this to work on MacOS
                    //https://apple.stackexchange.com/questions/47484/why-does-ical-often-say-the-server-responded-403-to-operation-caldavsetpropert
                    //Organizer = new Organizer(n.DirectorName),                    
                    Uid = $"tournament-{n.TournamentId}@ifpapinball.com"
            });

            //hide leagues since they often clutter the calendar
            if (!showLeagues)
            {
                calendarEvents = calendarEvents.Where(n => !n.Summary.ToLower().Contains("league") && (n.DtEnd.Subtract(n.DtStart).TotalDays <= 3));
            }
            
            var calendar = new Calendar();
            
            foreach(var calEvent in calendarEvents)
                calendar.Events.Add(calEvent);

            var serializer = new CalendarSerializer(new SerializationContext());
            var serializedCalendar = serializer.SerializeToString(calendar);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(serializedCalendar, Encoding.UTF8, "text/calendar");

            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "calendar.ics"
            };

            return response;
        }
    }
}

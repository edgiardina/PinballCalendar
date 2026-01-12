using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Microsoft.AspNetCore.Mvc;
using PinballApi.Interfaces;
using PinballApi.Models.WPPR;
using PinballApi.Models.WPPR.Universal.Tournaments.Search;
using PinballCalendar.Services;
using System.Text;

namespace PinballCalendar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalendarController(IPinballRankingApi PinballRankingApi, IGeocodingService GeocodingService) : ControllerBase
    {

        [HttpGet("{address}/{distance}")]
        public async Task<IActionResult> Get(string address, int distance, bool showLeagues = true)
        {
            TournamentSearch pinballEvents = null;

            try
            {
                var geocodedAddress = await GeocodingService.GeocodeAsync(address);

                if (showLeagues)
                {
                    pinballEvents = await PinballRankingApi.TournamentSearch(geocodedAddress.Latitude,
                                                                                 geocodedAddress.Longitude, distance,
                                                                                 DistanceType.Miles,
                                                                                 startDate: DateTime.Now,
                                                                                 endDate: DateTime.Now.AddYears(1),
                                                                                 totalReturn: 500);
                }
                else
                {
                    pinballEvents = await PinballRankingApi.TournamentSearch(geocodedAddress.Latitude,
                                                                                 geocodedAddress.Longitude, distance,
                                                                                 DistanceType.Miles,                                                                                 
                                                                                 startDate: DateTime.Now,
                                                                                 endDate: DateTime.Now.AddYears(1),
                                                                                 tournamentEventType: TournamentEventType.Tournament,
                                                                                 totalReturn: 500);
                }

                var calendarEvents = pinballEvents.Tournaments
                    .Select(n =>
                    new CalendarEvent
                    {
                        Description = n.Details,
                        Summary = n.TournamentName,
                        DtStart = new CalDateTime(n.EventStartDate.DateTime),
                        DtEnd = n.EventEndDate.HasValue ? new CalDateTime(n.EventEndDate.Value.DateTime) : null,
                        Url = n.Website,
                        GeographicLocation = new GeographicLocation(n.Latitude, n.Longitude),
                        Location = $"{n.City}, {n.Stateprov} {n.CountryName}",
                        //Can't show an organizer if you want this to work on MacOS
                        //https://apple.stackexchange.com/questions/47484/why-does-ical-often-say-the-server-responded-403-to-operation-caldavsetpropert
                        //Organizer = new Organizer(n.DirectorName),                    
                        Uid = $"tournament-{n.TournamentId}@ifpapinball.com"
                    });

                var calendar = new Calendar();

                foreach (var calEvent in calendarEvents)
                    calendar.Events.Add(calEvent);

                var serializer = new CalendarSerializer();
                var serializedCalendar = serializer.SerializeToString(calendar);

                var result = new FileContentResult(Encoding.UTF8.GetBytes(serializedCalendar ?? string.Empty), "text/calendar")
                {
                    FileDownloadName = "calendar.ics"
                };

                return result;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching calendar data: {ex.Message}");
            }
        }
    }
}

using Domains.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Interfaces;
using System.Net;
using System.Text.Json;
using UCL_Ticketverkoop.ViewModels;

namespace UCL_Ticketverkoop.Controllers
{
    public class HotelController : Controller
    {
        private readonly IMatchService _matchService;
        private readonly IHttpClientFactory _httpClientFactory;
        private const int MaxHotels = 5;

        public HotelController(IMatchService matchService, IHttpClientFactory httpClientFactory)
        {
            _matchService = matchService;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? city)
        {
            HotelSearchVM viewModel = new()
            {
                SelectedCity = city,
                HasSearched = !string.IsNullOrWhiteSpace(city)
            };

            var cities = await GetMatchCitiesAsync();
            viewModel.Cities = cities
                .Select(c => new SelectListItem
                {
                    Text = c,
                    Value = c,
                    Selected = string.Equals(c, city, StringComparison.OrdinalIgnoreCase)
                })
                .ToList();

            if (viewModel.Cities.Count == 0)
            {
                viewModel.ErrorMessage = "No match cities are available yet.";
                return View(viewModel);
            }

            if (string.IsNullOrWhiteSpace(city))
            {
                return View(viewModel);
            }

            if (!cities.Contains(city, StringComparer.OrdinalIgnoreCase))
            {
                viewModel.ErrorMessage = "Please choose one of the available match cities.";
                return View(viewModel);
            }

            try
            {
                viewModel.Hotels = await GetHotelsForCityAsync(city);
                if (viewModel.Hotels.Count == 0)
                {
                    viewModel.ErrorMessage = $"No hotel recommendations were found for {city}.";
                }
            }
            catch
            {
                viewModel.ErrorMessage = "Hotel recommendations are temporarily unavailable. Please try again later.";
            }

            return View(viewModel);
        }

        private async Task<List<string>> GetMatchCitiesAsync()
        {
            IEnumerable<Match> matches = await _matchService.GetAllAsync() ?? Enumerable.Empty<Match>();

            return matches
                .Select(m => m.HomeClub?.Stadium?.City)
                .Where(city => !string.IsNullOrWhiteSpace(city))
                .Select(city => city!.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(city => city)
                .ToList();
        }

        private async Task<List<HotelRecommendationVM>> GetHotelsForCityAsync(string city)
        {
            string overpassQuery = BuildHotelQuery(city);
            var hotels = await ExecuteHotelQueryAsync(overpassQuery, city);

            if (hotels.Count == 0 && TryGetCityCoordinates(city, out double latitude, out double longitude))
            {
                hotels = await ExecuteHotelQueryAsync(BuildHotelRadiusQuery(latitude, longitude), city);
            }

            foreach (var hotel in hotels.Where(hotel => string.IsNullOrWhiteSpace(hotel.PhotoUrl)))
            {
                hotel.PhotoUrl = await FindWikimediaImageAsync(hotel.Name, city);
            }

            return hotels;
        }

        private async Task<List<HotelRecommendationVM>> ExecuteHotelQueryAsync(string overpassQuery, string city)
        {
            using var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["data"] = overpassQuery
            });

            using HttpClient client = _httpClientFactory.CreateClient("OverpassApi");
            using HttpResponseMessage response = await client.PostAsync("interpreter", content);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();
            using JsonDocument document = await JsonDocument.ParseAsync(stream);

            if (!document.RootElement.TryGetProperty("elements", out JsonElement elements))
            {
                return new List<HotelRecommendationVM>();
            }

            var hotels = elements
                .EnumerateArray()
                .Select(element => ParseHotel(element, city))
                .Where(hotel => !string.IsNullOrWhiteSpace(hotel.Name))
                .GroupBy(hotel => hotel.Name, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .Take(MaxHotels)
                .ToList();

            return hotels;
        }

        private static string BuildHotelQuery(string city)
        {
            string cityPattern = BuildCityPattern(city);

            return $"""
                [out:json][timeout:25];
                area["name"~"^({cityPattern})$","i"]["boundary"="administrative"]->.searchArea;
                (
                  nwr["tourism"="hotel"](area.searchArea);
                  nwr["tourism"="guest_house"](area.searchArea);
                  nwr["tourism"="hostel"](area.searchArea);
                );
                out tags center 25;
                """;
        }

        private static string BuildHotelRadiusQuery(double latitude, double longitude)
        {
            string lat = latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string lon = longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);

            return $"""
                [out:json][timeout:25];
                (
                  nwr["tourism"="hotel"](around:8000,{lat},{lon});
                  nwr["tourism"="guest_house"](around:8000,{lat},{lon});
                  nwr["tourism"="hostel"](around:8000,{lat},{lon});
                );
                out tags center 25;
                """;
        }

        private static bool TryGetCityCoordinates(string city, out double latitude, out double longitude)
        {
            string normalizedCity = city.Trim();
            if (IsMunich(normalizedCity))
            {
                latitude = 48.137154;
                longitude = 11.576124;
                return true;
            }

            latitude = 0;
            longitude = 0;
            return false;
        }

        private static string BuildCityPattern(string city)
        {
            var cityAliases = GetCityAliases(city)
                .Select(EscapeOverpassRegex)
                .Distinct(StringComparer.OrdinalIgnoreCase);

            return string.Join("|", cityAliases);
        }

        private static IEnumerable<string> GetCityAliases(string city)
        {
            string trimmedCity = city.Trim();
            yield return trimmedCity;

            if (trimmedCity.Equals("München", StringComparison.OrdinalIgnoreCase) ||
                trimmedCity.Equals("Munchen", StringComparison.OrdinalIgnoreCase) ||
                trimmedCity.Equals("Muenchen", StringComparison.OrdinalIgnoreCase) ||
                trimmedCity.Equals("Munich", StringComparison.OrdinalIgnoreCase))
            {
                yield return "München";
                yield return "Munchen";
                yield return "Muenchen";
                yield return "Munich";
            }
        }

        private static string EscapeOverpassRegex(string value)
        {
            return value
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("(", "\\(")
                .Replace(")", "\\)")
                .Replace("[", "\\[")
                .Replace("]", "\\]")
                .Replace(".", "\\.")
                .Replace("+", "\\+")
                .Replace("*", "\\*")
                .Replace("?", "\\?")
                .Replace("|", "\\|")
                .Replace("^", "\\^")
                .Replace("$", "\\$");
        }

        private static bool IsMunich(string city)
        {
            return city.Equals("M\u00FCnchen", StringComparison.OrdinalIgnoreCase) ||
                   city.Equals("Munchen", StringComparison.OrdinalIgnoreCase) ||
                   city.Equals("Muenchen", StringComparison.OrdinalIgnoreCase) ||
                   city.Equals("Munich", StringComparison.OrdinalIgnoreCase);
        }

        private static HotelRecommendationVM ParseHotel(JsonElement element, string city)
        {
            JsonElement tags = element.TryGetProperty("tags", out JsonElement tagsElement)
                ? tagsElement
                : default;

            string hotelName = ReadTag(tags, "name") ?? "Hotel";
            string? photoUrl = ReadImageUrl(tags);

            return new HotelRecommendationVM
            {
                Name = hotelName,
                Address = BuildAddress(tags),
                Stars = ReadStars(tags),
                PhotoUrl = photoUrl,
                BookingUrl = BuildBookingUrl(hotelName, city)
            };
        }

        private async Task<string?> FindWikimediaImageAsync(string hotelName, string city)
        {
            string query = $"{hotelName} {city} hotel";
            string requestUrl = "w/api.php?action=query&generator=search&gsrnamespace=6&gsrlimit=1" +
                                $"&gsrsearch={WebUtility.UrlEncode(query)}&prop=imageinfo&iiprop=url&format=json";

            try
            {
                using HttpClient client = _httpClientFactory.CreateClient("WikimediaApi");
                using HttpResponseMessage response = await client.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                await using var stream = await response.Content.ReadAsStreamAsync();
                using JsonDocument document = await JsonDocument.ParseAsync(stream);

                if (!document.RootElement.TryGetProperty("query", out JsonElement queryElement) ||
                    !queryElement.TryGetProperty("pages", out JsonElement pages))
                {
                    return null;
                }

                foreach (JsonProperty page in pages.EnumerateObject())
                {
                    if (page.Value.TryGetProperty("imageinfo", out JsonElement imageInfo) &&
                        imageInfo.ValueKind == JsonValueKind.Array &&
                        imageInfo.GetArrayLength() > 0 &&
                        imageInfo[0].TryGetProperty("url", out JsonElement url) &&
                        url.ValueKind == JsonValueKind.String)
                    {
                        return url.GetString();
                    }
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        private static string? BuildAddress(JsonElement tags)
        {
            string? street = ReadTag(tags, "addr:street");
            string? houseNumber = ReadTag(tags, "addr:housenumber");
            string? postcode = ReadTag(tags, "addr:postcode");
            string? city = ReadTag(tags, "addr:city");

            string streetLine = string.Join(" ", new[] { street, houseNumber }.Where(v => !string.IsNullOrWhiteSpace(v)));
            string address = string.Join(", ", new[] { streetLine, postcode, city }.Where(v => !string.IsNullOrWhiteSpace(v)));

            return string.IsNullOrWhiteSpace(address) ? null : address;
        }

        private static string? ReadStars(JsonElement tags)
        {
            string? rating = new[]
                {
                    "stars",
                    "hotel:stars",
                    "stars:hotel",
                    "official_rating",
                    "official_rating:hotel",
                    "tourism:stars",
                    "rating"
                }
                .Select(key => ReadTag(tags, key))
                .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));

            if (string.IsNullOrWhiteSpace(rating))
            {
                return null;
            }

            string normalizedRating = rating.Trim();
            if (normalizedRating.All(character => character == '*'))
            {
                return normalizedRating.Length.ToString();
            }

            string digits = new(normalizedRating.TakeWhile(character => char.IsDigit(character)).ToArray());
            return string.IsNullOrWhiteSpace(digits) ? normalizedRating : digits;
        }

        private static string BuildBookingUrl(string hotelName, string city)
        {
            return $"https://www.booking.com/searchresults.html?ss={WebUtility.UrlEncode($"{hotelName} {city}")}";
        }

        private static string? ReadImageUrl(JsonElement tags)
        {
            string? image = ReadTag(tags, "image");
            if (!string.IsNullOrWhiteSpace(image) && Uri.TryCreate(image, UriKind.Absolute, out _))
            {
                return image;
            }

            string? wikimediaCommons = ReadTag(tags, "wikimedia_commons");
            if (string.IsNullOrWhiteSpace(wikimediaCommons))
            {
                return null;
            }

            string fileName = wikimediaCommons.StartsWith("File:", StringComparison.OrdinalIgnoreCase)
                ? wikimediaCommons
                : $"File:{wikimediaCommons}";

            return $"https://commons.wikimedia.org/wiki/Special:Redirect/file/{WebUtility.UrlEncode(fileName)}";
        }

        private static string? ReadTag(JsonElement tags, string key)
        {
            return tags.ValueKind == JsonValueKind.Object &&
                   tags.TryGetProperty(key, out JsonElement value) &&
                   value.ValueKind == JsonValueKind.String
                ? value.GetString()
                : null;
        }

    }
}

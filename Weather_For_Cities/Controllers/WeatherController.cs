using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class WeatherController : ControllerBase
{
    private readonly string apiKey = "be04d31f2a30f7ef43a9121889c1ed9a\n";
    private readonly string apiUrl = "http://api.openweathermap.org/data/2.5/weather";

    [HttpGet]
    public async Task<IActionResult> GetWeather([FromQuery] string city1, [FromQuery] string city2)
    {
        if (string.IsNullOrEmpty(city1) || string.IsNullOrEmpty(city2))
        {
            return BadRequest("Lütfen iki þehir ismi girin.");
        }

        try
        {
            string city1Url = $"{apiUrl}?q={city1}&appid={apiKey}";
            string city2Url = $"{apiUrl}?q={city2}&appid={apiKey}";

            var temperature1 = await GetTemperature(city1Url);
            var temperature2 = await GetTemperature(city2Url);

            var temperatureDifference = temperature1 - temperature2;

            return Ok($"Sýcaklýk Farký: {temperatureDifference} °C");
        }
        catch (Exception ex)
        {
            return BadRequest($"Hata: {ex.Message}");
        }
    }

    private async Task<double> GetTemperature(string apiUrl)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<WeatherApiResponse>(json);
                double temperatureInCelsius = data.Main.Temp - 273.15;
                return temperatureInCelsius;
            }
            else
            {
                throw new HttpRequestException($"Hata kodu: {response.StatusCode}");
            }
        }
    }
}

public class WeatherApiResponse
{
    public MainData Main { get; set; }
}

public class MainData
{
    public double Temp { get; set; }
}
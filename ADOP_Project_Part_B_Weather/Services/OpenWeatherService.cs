using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

using ADOP_Project_Part_B_Weather.Models;
using System.Collections.Concurrent;
using static System.Net.WebRequestMethods;

namespace ADOP_Project_Part_B_Weather.Services
{
    public class OpenWeatherService
    {
        HttpClient httpClient = new HttpClient();

        readonly string apiKey = "ef1d88b7c404add114633c5469439c95";

        public async Task<Forecast> GetForecastAsync(string City)
        {
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?q={City}&units=metric&lang={language}&appid={apiKey}";

            Forecast forecast = await ReadWebApiAsync(uri);
            return forecast;
        }

        private async Task<Forecast> ReadWebApiAsync(string uri)
        {
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            WeatherApiData wd = await response.Content.ReadFromJsonAsync<WeatherApiData>();
            return WeatherApiDataToForecast(wd);
        }

        private Forecast WeatherApiDataToForecast(WeatherApiData wData)
        {
            var forecast = new Forecast()
            {
                City = wData.city.name,
                Items = wData.list.Select(item => new ForecastItem
                {
                    DateTime = UnixTimeStampToDateTime(item.dt),
                    Temperature = item.main.temp,
                    WindSpeed = item.wind.speed,
                    Information = $"Temperatur: {item.main.temp}degC Vind: {item.wind.speed}m/s",
                    Description = item.weather.FirstOrDefault().description,
                    Icon = $"http://openweathermap.org/img/w/{item.weather.First().icon}.png"
                }).ToList()
            };

            return forecast;
        }

        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }
}

using System.Net;
using System.Text.Json;
using MauiAppTempoAgora.Models;

namespace MauiAppTempoAgora.Services
{
    public class DataService
    {
        private static readonly HttpClient client = new HttpClient();
        private const string ApiKey = "dadcbf315d5fa56968ca453c21945c04"; // Coloque sua chave do OpenWeather

        public static async Task<Tempo?> GetPrevisao(string cidade)
        {
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={cidade}&units=metric&appid={ApiKey}&lang=pt_br";

            HttpResponseMessage response = await client.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new Exception("ChaveInvalida");
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new Exception("CidadeNaoEncontrada");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Erro na requisição: {response.ReasonPhrase}");
            }

            string json = await response.Content.ReadAsStringAsync();

            // Mapeando diretamente do JSON para evitar incompatibilidade de tipos
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                var root = doc.RootElement;

                Tempo tempo = new Tempo
                {
                    lat = root.GetProperty("coord").GetProperty("lat").GetDouble(),
                    lon = root.GetProperty("coord").GetProperty("lon").GetDouble(),
                    description = root.GetProperty("weather")[0].GetProperty("description").GetString(),
                    temp_min = root.GetProperty("main").GetProperty("temp_min").GetDouble(),
                    temp_max = root.GetProperty("main").GetProperty("temp_max").GetDouble(),
                    speed = root.GetProperty("wind").GetProperty("speed").GetDouble(),
                    visibility = root.GetProperty("visibility").GetInt32(),

                    // Converte Unix Timestamp para Hora/Minuto legível
                    sunrise = DateTimeOffset.FromUnixTimeSeconds(root.GetProperty("sys").GetProperty("sunrise").GetInt64()).ToLocalTime().ToString("HH:mm"),
                    sunset = DateTimeOffset.FromUnixTimeSeconds(root.GetProperty("sys").GetProperty("sunset").GetInt64()).ToLocalTime().ToString("HH:mm")
                };

                return tempo;
            }
        }
    }
}
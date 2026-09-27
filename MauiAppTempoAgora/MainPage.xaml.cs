using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;
using System;

namespace MauiAppTempoAgora
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txt_cidade.Text))
                {
                    lbl_res.Text = "Por favor, digite o nome de uma cidade.";
                    return;
                }

                Tempo? t = await DataService.GetPrevisao(txt_cidade.Text);

                if (t != null)
                {
                    string visibilidadeKm = t.visibility.HasValue
                        ? $"{t.visibility.Value / 1000.0:F1} km"
                        : "N/A";

                    string dados_previsao =
                        $"Clima: {t.description}\n" +
                        $"Temp Máx: {t.temp_max}°C\n" +
                        $"Temp Min: {t.temp_min}°C\n" +
                        $"Vento: {t.speed} m/s\n" +
                        $"Visibilidade: {visibilidadeKm}\n\n" +
                        $"Nascer do Sol: {t.sunrise}\n" +
                        $"Pôr do Sol: {t.sunset}\n" +
                        $"Coordenadas: {t.lat}, {t.lon}";

                    lbl_res.Text = dados_previsao;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "ChaveInvalida")
                {
                    await DisplayAlert("Chave de API Inválida", "A chave de API configurada não é válida ou ainda não foi ativada. Verifique as credenciais no DataService.", "OK");
                    lbl_res.Text = "Erro de autenticação da API.";
                }
                else if (ex.Message == "CidadeNaoEncontrada")
                {
                    await DisplayAlert("Cidade Não Encontrada", "A cidade informada não foi localizada. Verifique a ortografia e tente novamente.", "OK");
                    lbl_res.Text = "Cidade não encontrada.";
                }
                else
                {
                    await DisplayAlert("Erro", $"Ocorreu um erro: {ex.Message}", "OK");
                    lbl_res.Text = "Erro ao buscar dados do tempo.";
                }
            }
        }
    }
}
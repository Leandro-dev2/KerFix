using System;
using System.Collections.Generic;
using System.Text;

using System.Net.Http;
using System.Net.Http.Json;
using KerfixDesktop.Models;

namespace KerfixDesktop.Services
{
    public class RegistroApiService
    {
        private readonly HttpClient _httpClient;

        public RegistroApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Registro> CrearRegistroAsync(Registro registro)
        {
            var respuesta = await _httpClient.PostAsJsonAsync(
                "api/registros",
                registro
            );

            respuesta.EnsureSuccessStatusCode();

            return await respuesta.Content
                .ReadFromJsonAsync<Registro>()
                ?? throw new InvalidOperationException(
                    "La API no devolvió el registro creado.");
        }
    }

}

using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Json;
using KerfixDesktop.Models;

namespace KerfixDesktop.Services
{
    public class ClienteApiService
    {
        private readonly HttpClient _httpClient;

        public ClienteApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Cliente>> ObtenerClientesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Cliente>>(
                "api/clientes") ?? new List<Cliente>();
        }

        public async Task ActualizarClienteAsync(int id, Cliente cliente)
        {
            var respuesta = await _httpClient.PutAsJsonAsync(
                $"api/clientes/{id}", cliente);

            respuesta.EnsureSuccessStatusCode();
        }

        public async Task<Cliente> CrearClienteAsync(Cliente cliente)
        {
            var respuesta = await _httpClient.PostAsJsonAsync(
                "api/clientes",
                cliente
            );

            respuesta.EnsureSuccessStatusCode();

            var clienteCreado =
                await respuesta.Content.ReadFromJsonAsync<Cliente>();

            return clienteCreado
                ?? throw new InvalidOperationException(
                    "La API no devolvió el cliente creado.");
        }
    }
}

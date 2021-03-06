﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using JohnsonControls.Net.Http;
using Newtonsoft.Json.Linq;

namespace lesson3
{
    public class Program
    {

        public static async Task Main(string[] args)
        {
            var username = args[0];
            var password = args[1];
            var hostname = args[2];

            var handler = new HttpClientRedirectHandler();

            using (var client = new HttpClient(handler) {BaseAddress = new Uri($"https://{hostname}/api/v1")})
            {
                // Login - constructing a payload that looks like { "username": "thename', "password": "thepassword" }
                var loginMessage = $"{{ 'username': '{username}', 'password': '{password}' }}";

                var loginContent = new StringContent(loginMessage,
                    Encoding.UTF8,
                    "application/json");

                var loginResponseMessage = await client.PostAsync("login", loginContent);

                // Read the results
                var loginResult = await loginResponseMessage.Content.ReadAsStringAsync();

                // Get the access token
                // We could use string manipulation methods, but using JSON.NET is much easier
                var accessToken = JToken.Parse(loginResult)["accessToken"].Value<string>();

                // Add an authorization header to the list of default headers for our client
                client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {accessToken}");


                // Let's attempt to retrieve the first page of alarms
                var alarmsResponse = await client.GetAsync("alarms");

                // Parse the response using Newtonsoft and print the first one.
                var alarmsObject = JObject.Parse(await alarmsResponse.Content.ReadAsStringAsync());
                var alarms = alarmsObject["items"];
                Console.WriteLine($"First alarm: {alarms?[0]}");
            }



        }

    }
}

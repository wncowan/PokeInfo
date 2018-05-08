using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PokeInfo
{
    public class WebRequest
    {
        // The second parameter is a function that returns a Dictionary of string keys to object values.
        // If an API returned an array as its top level collection the parameter type would be "Action>"
        public static async Task GetPokemonDataAsync(int PokeId, Action<Pokemon> Callback)
        {
            // Create a temporary HttpClient connection.
            using (var Client = new HttpClient())
            {
                try
                {
                    Client.BaseAddress = new Uri($"http://pokeapi.co/api/v2/pokemon/{PokeId}");
                    HttpResponseMessage Response = await Client.GetAsync(""); 
                    Response.EnsureSuccessStatusCode();
                    string StringResponse = await Response.Content.ReadAsStringAsync();
                    // Console.WriteLine(StringResponse);
                    // Dictionary<string, object> JsonResponse = JsonConvert.DeserializeObject<Dictionary<string,object>>(StringResponse);                    
                    // Console.WriteLine("JsonResponse: ");
                    // Console.WriteLine(JsonResponse);
                    // Console.WriteLine(JsonResponse["types"]);

                    JObject PokeObject = JsonConvert.DeserializeObject<JObject>(StringResponse);
                    JArray PokeTypes = PokeObject["types"].Value<JArray>();
                    List<string> Types = new List<string>();

                    foreach(JObject TypeObject in PokeTypes)
                    {
                        Types.Add(TypeObject["type"]["name"].Value<string>());
                    }

                    Pokemon JsonResponse = new Pokemon{
                        Name = PokeObject["name"].Value<string>(),
                        Types = Types,
                        Weight = PokeObject["weight"].Value<long>(),
                        Height = PokeObject["height"].Value<long>(),
                    };

                    Callback(JsonResponse);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request exception: {e.Message}");
                }
            }
        }
    }
}
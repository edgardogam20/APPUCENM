using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace APIRESTFULL.Controllers
{
    public static class PersonasController
    {
        // POST - Crear persona
        public async static Task<Models.Msg> CreatePerson(Models.Personas persona)
        {
            Models.Msg msg = new Models.Msg();

            String jsonObject = JsonConvert.SerializeObject(persona);
            StringContent contenido = new StringContent(jsonObject, Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsync("http://192.168.20.26/crud-php/PostPersons.php", contenido);

                if (response != null && response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadAsStringAsync();
                    msg = JsonConvert.DeserializeObject<Models.Msg>(resultado);
                }
            }
            return msg;
        }

        // GET - Leer personas
        public async static Task<List<Models.Personas>> ReadPersons()
        {
            List<Models.Personas> personas = new List<Models.Personas>();

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync("http://192.168.20.26/crud-php/GetPersons.php");

                if (response != null && response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadAsStringAsync();
                    personas = JsonConvert.DeserializeObject<List<Models.Personas>>(resultado);
                }
            }
            return personas;
        }

        // PUT - Actualizar persona
        public async static Task<Models.Msg> UpdatePerson(Models.Personas persona)
        {
            Models.Msg msg = new Models.Msg();

            String jsonObject = JsonConvert.SerializeObject(persona);
            StringContent contenido = new StringContent(jsonObject, Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PutAsync("http://192.168.20.26/crud-php/UpdatePersons.php", contenido);

                if (response != null && response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadAsStringAsync();
                    msg = JsonConvert.DeserializeObject<Models.Msg>(resultado);
                }
            }
            return msg;
        }

        // DELETE - Eliminar persona
        public async static Task<Models.Msg> DeletePerson(int id)
        {
            Models.Msg msg = new Models.Msg();

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.DeleteAsync($"http://192.168.20.26/crud-php/DeletePersons.php?id={id}");

                if (response != null && response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadAsStringAsync();
                    msg = JsonConvert.DeserializeObject<Models.Msg>(resultado);
                }
            }
            return msg;
        }
    }
}
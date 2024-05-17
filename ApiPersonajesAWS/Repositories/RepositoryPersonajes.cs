using ApiPersonajesAWS.Data;
using ApiPersonajesAWS.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace ApiPersonajesAWS.Repositories
{
    public class RepositoryPersonajes
    {
        private PersonajesContext context;

        public RepositoryPersonajes(PersonajesContext context)
        {
            this.context = context;
        }

        public async Task<List<Personaje>>
            GetPersonajesAsync()
        {
            return await this.context.Personajes.ToListAsync();
        }

        public async Task<Personaje> FindPersonajeAsync
            (int id)
        {
            return await this.context.Personajes
                .FirstOrDefaultAsync(x => x.IdPersonaje == id);
        }

        private async Task<int> GetMaxIdPersonajeAsync()
        {
            return await this.context.Personajes
                .MaxAsync(x => x.IdPersonaje) + 1;
        }

        public async Task CreatePersonajeAsync
            (string nombre, string imagen)
        {
            Personaje personaje = new Personaje
            {
                IdPersonaje = await this.GetMaxIdPersonajeAsync(),
                Nombre = nombre,
                Imagen = imagen
            };
            this.context.Personajes.Add(personaje);
            await this.context.SaveChangesAsync();
        }

        public async Task<Personaje?> UpdatePersonajeAsync(int id, string nombre, string imagen)
        {
            Personaje? personaje = null;
            using (MySqlConnection conn = new MySqlConnection(this.context.Database.GetConnectionString()))
            {
                conn.Open();
                using (MySqlCommand com = new MySqlCommand("Update_Personaje", conn))
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@idpersonaje", id);
                    com.Parameters.AddWithValue("@nombre", nombre);
                    com.Parameters.AddWithValue("@imagen", imagen);

                    using (MySqlDataReader reader = com.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            personaje = new Personaje
                            {
                                IdPersonaje = int.Parse(reader["IDPERSONAJE"].ToString()!),
                                Nombre = reader["PERSONAJE"].ToString()!,
                                Imagen = reader["IMAGEN"].ToString()!,
                            };
                        }
                    }
                }
            }
            return personaje;
        }
    }
}



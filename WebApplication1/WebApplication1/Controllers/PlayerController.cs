using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        //configurando el entorno para usar la cadena de coneccion , _config es la llave para usar la cadena de conexion
        private IConfiguration _Config;

        public PlayerController(IConfiguration Config)
        {
            _Config = Config;
        }

        [HttpGet]
        public async Task<ActionResult<List<Player>>> GetAllLPlayer()
        {
            using var conexion = new SqlConnection(_Config.GetConnectionString("DefaultConnection"));
            conexion.Open();
            var oLibros = conexion.Query<Player>("GetPlayer", commandType: System.Data.CommandType.StoredProcedure);
            return Ok(oLibros);
        }

        [HttpGet("{PlayerID}")]
        public async Task<ActionResult<List<Player>>> GetPlayerbyID(int PlayerID)
        {
            using var conexion = new SqlConnection(_Config.GetConnectionString("DefaultConnection"));
            conexion.Open();
            var parametro = new DynamicParameters();
            parametro.Add("@PlayerID", PlayerID);
            var oLibros = conexion.Query<Player>("GetPlayerByID", parametro, commandType: System.Data.CommandType.StoredProcedure);
            return Ok(oLibros);
        }

        [HttpPost]
        // obteniendo el objeto de usuario de la informacion proporcionada por Swagger
        public async Task<ActionResult<Player>> CreatePlayer(Player pl)
        {
            try
            {
                using var conexion = new SqlConnection(_Config.GetConnectionString("DefaultConnection"));
                conexion.Open();
                var parametro = new DynamicParameters();
                parametro.Add("@FirstName", pl.FirstName);
                parametro.Add("@LastName", pl.LastName);
                parametro.Add("@Team", pl.Team);
                parametro.Add("@Position", pl.Position);

                var oLibro = conexion.Query<Player>("InsertPlayer", parametro, commandType: System.Data.CommandType.StoredProcedure);

                // Verificar si la operación fue exitosa (por ejemplo, si oLibro no es nulo)
                if (oLibro != null)
                {

                    var mensaje = "Jugador creado exitosamente.";
                    return Ok(mensaje);
                }
                else
                {

                    var mensaje = "No se pudo crear el nuevo jugador.";
                    return BadRequest(new { mensaje });
                }
            }
            catch (Exception ex)
            {

                var mensaje = "Se produjo un error al crear el jugador: " + ex.Message;
                return StatusCode(500, new { mensaje });
            }
        }

        [HttpDelete("{PlayerID}")]
        // obteniendo id del libro a eliminar (este id es de la clase Libros)
        public async Task<ActionResult> DeletePlayer(int PlayerID)
        {   
            //manejo de errores con trycach
            try
            {
                using (var conexion = new SqlConnection(_Config.GetConnectionString("DefaultConnection")))
                {
                    await conexion.OpenAsync();

                    var parametro = new DynamicParameters();
                    parametro.Add("@PlayerID", PlayerID);
                    await conexion.ExecuteAsync("DeletePlayer", parametro, commandType: CommandType.StoredProcedure);

                    return Ok("Jugador eliminado correctamente.");
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Error al eliminar el Jugador: {ex.Message}");
            }
        }
    }
}

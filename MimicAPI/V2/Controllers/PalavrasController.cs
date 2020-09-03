using Microsoft.AspNetCore.Mvc;

namespace MimicAPI.V2.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class PalavrasController
    {
        /// <summary>
        /// Recupera todas as palavras que existentes
        /// </summary>
        /// <param name="palavraUrlQuery">Filtros de pesquisa</param>
        /// <returns>Lista de palavras</returns>
        [HttpGet("", Name = "ObterTodas")]
        public string FindAll()
        {
            return "API 2.0";
        }
    }
}

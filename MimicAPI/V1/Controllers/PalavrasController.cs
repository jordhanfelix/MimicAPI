using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MimicAPI.Helpers;
using MimicAPI.V1.Models;
using MimicAPI.V1.Models.DTO;
using MimicAPI.V1.Repositories.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MimicAPI.V1.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class PalavrasController : ControllerBase
    {
        private readonly IPalavraRepository _repository;
        private readonly IMapper _mapper;

        public PalavrasController(IPalavraRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Recupera todas as palavras existentes
        /// </summary>
        /// <param name="palavraUrlQuery">Filtros de pesquisa</param>
        /// <returns>Lista de palavras</returns>
        [HttpGet("", Name = "ObterTodas")]
        public ActionResult FindAll([FromQuery] PalavraUrlQuery palavraUrlQuery)
        {
            var item = _repository.ObterPalavras(palavraUrlQuery);


            if (item.Resultados.Count == 0)
                return NotFound();

            PaginacaoLista<PalavraDTO> palavraLista = CriarLinksPalavras(palavraUrlQuery, item);

            return Ok(palavraLista);
        }
        // parei na aula 25

        /// <summary>
        /// Obtem apenas uma palavra da base de dados
        /// </summary>
        /// <param name="id">Identificador da palavra no banco de dados</param>
        /// <returns> Um objeto palavra</returns>
        [HttpGet("{id}", Name = "ObterPalavra")]
        public ActionResult FindById(int id)
        {


            var obj = _repository.Obter(id);

            if (obj == null)
                return NotFound();


            PalavraDTO palavraDto = _mapper.Map<Palavra, PalavraDTO>(obj);
            palavraDto.Links.Add(

                new LinkDTO("self", Url.Link("ObterPalavra", new { id = palavraDto.Id }), "GET")
                );
            palavraDto.Links.Add(

                new LinkDTO("Update", Url.Link("ObterPalavra", new { id = palavraDto.Id }), "PUT")
                );
            palavraDto.Links.Add(

                new LinkDTO("Delete", Url.Link("ObterPalavra", new { id = palavraDto.Id }), "DELETE")
                );

            return Ok(palavraDto);

        }

        /// <summary>
        /// Cria um novo Objeto na base de dados
        /// </summary>
        /// <param name="palavra"> Onjeto a ser inserido no banco</param>
        /// <returns>Objeto palavra com ID criado no banco</returns>
        [Route("")]
        [HttpPost]
        public ActionResult Create([FromBody] Palavra palavra)
        {

            try
            {

                if (palavra == null)
                    return BadRequest();

                if (!ModelState.IsValid)
                    return UnprocessableEntity(ModelState);

                _repository.Cadastrar(palavra);
                palavra.Ativo = true;
                palavra.Criado = DateTime.Now;

                PalavraDTO palavraDto = _mapper.Map<Palavra, PalavraDTO>(palavra);
                palavraDto.Links.Add(

                    new LinkDTO("self", Url.Link("ObterPalavra", new { id = palavraDto.Id }), "GET")
                    );


                return Created($"api/words/{palavra.Id}", palavraDto);
            }
            catch (Exception)
            {

                return new JsonResult("ERROR");
            }

        }

        /// <summary>
        /// Atualiza informações do Objeto Palavra
        /// </summary>
        /// <param name="id">Identificador do objeto a ser alterado</param>
        /// <param name="palavra">Objeto </param>
        /// <returns></returns>
        [HttpPut("{id}", Name = "AtualizarPalavra")]
        public ActionResult Update(int id, [FromBody] Palavra palavra)
        {

            try
            {
                var obj = _repository.Obter(id);
                if (obj == null)
                    return NotFound();

                if (palavra == null)
                    return BadRequest();

                if (!ModelState.IsValid)
                    return UnprocessableEntity(ModelState);


                palavra.Id = id;
                palavra.Ativo = obj.Ativo;
                palavra.Criado = obj.Criado;
                palavra.Atualizado = DateTime.Now;

                _repository.Atualizar(palavra);
                PalavraDTO palavraDto = _mapper.Map<Palavra, PalavraDTO>(palavra);
                palavraDto.Links.Add(

                    new LinkDTO("self", Url.Link("ObterPalavra", new { id = palavraDto.Id }), "GET")
                    );


                return Ok();
            }
            catch (Exception)
            {

                return new JsonResult("ERROR");
            }

        }

        /// <summary>
        /// Operação que desativa uma palavra do sistema.
        /// </summary>
        /// <param name="id">Identificador do Objeto a ser desativado</param>
        /// <returns></returns>

        [HttpDelete("{id}", Name = "ExcluirPalavra")]
        public ActionResult Delete(int id)
        {

            try
            {
                var palavra = _repository.Obter(id);

                if (palavra == null)
                    return NotFound();

                _repository.Deletar(id);

                return NoContent();
            }
            catch (Exception)
            {

                return new JsonResult("ERROR");
            }

        }

        #region cria links da paginação

        /// <summary>
        /// CriarLinksPalavras Cria o link de paginação no obter todas
        /// </summary> 
        /// <param name="palavraUrlQuery"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private PaginacaoLista<PalavraDTO> CriarLinksPalavras(PalavraUrlQuery palavraUrlQuery, PaginacaoLista<Palavra> item)
        {
            var palavraLista = _mapper.Map<PaginacaoLista<Palavra>, PaginacaoLista<PalavraDTO>>(item);
            foreach (var palavra in palavraLista.Resultados)
            {
                palavra.Links = new List<LinkDTO>();
                palavra.Links.Add(new LinkDTO("self", Url.Link("ObterPalavra", new { id = palavra.Id }), "GET"));

            }
            palavraLista.Links.Add(new LinkDTO("self", Url.Link("ObterTodas", palavraUrlQuery), "GET"));

            if (item.Paginacao != null)
            {
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(item.Paginacao));

                if (palavraUrlQuery.PaginaNumero + 1 <= item.Paginacao.TotalDePagina)
                {
                    var queryString = new PalavraUrlQuery() { PaginaNumero = palavraUrlQuery.PaginaNumero + 1, RegistroPagina = palavraUrlQuery.RegistroPagina, Data = palavraUrlQuery.Data };
                    palavraLista.Links.Add(new LinkDTO("prox", Url.Link("ObterTodas", queryString), "GET"));
                }

                if (palavraUrlQuery.PaginaNumero - 1 > 0)
                {
                    var queryString = new PalavraUrlQuery() { PaginaNumero = palavraUrlQuery.PaginaNumero - 1, RegistroPagina = palavraUrlQuery.RegistroPagina, Data = palavraUrlQuery.Data };
                    palavraLista.Links.Add(new LinkDTO("ant", Url.Link("ObterTodas", queryString), "GET"));
                }

            }

            return palavraLista;
        }
        #endregion

    }
}

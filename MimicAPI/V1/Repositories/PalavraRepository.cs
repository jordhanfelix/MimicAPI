using Microsoft.EntityFrameworkCore;
using MimicAPI.DataBase;
using MimicAPI.Helpers;
using MimicAPI.V1.Models;
using MimicAPI.V1.Repositories.Contracts;
using System;
using System.Linq;

namespace MimicAPI.V1.Repositories
{
    public class PalavraRepository : IPalavraRepository
    {
        private readonly MimicContext _banco;
        public PalavraRepository(MimicContext banco)
        {
            _banco = banco;
        }
        public Palavra Obter(int id)
        {
            return _banco.Palavras.AsNoTracking().FirstOrDefault(a => a.Id == id);
        }
        public PaginacaoLista<Palavra> ObterPalavras(PalavraUrlQuery palavraUrlQuery)
        {

            var lista = new PaginacaoLista<Palavra>();
            var item = _banco.Palavras.AsNoTracking().AsQueryable();
            if (palavraUrlQuery.Data.HasValue)
            {
                item = item.Where(a => a.Criado > palavraUrlQuery.Data.Value || a.Atualizado > palavraUrlQuery.Data.Value);
            }
            if (palavraUrlQuery.PaginaNumero.HasValue)
            {
                var totalDeRegistro = item.Count();
                item = item.Skip((palavraUrlQuery.PaginaNumero.Value - 1) * palavraUrlQuery.RegistroPagina.Value).Take(palavraUrlQuery.RegistroPagina.Value);
                var pagina = new Paginacao();

                pagina.NumeroPagina = palavraUrlQuery.PaginaNumero.Value;
                pagina.RegistroPagina = palavraUrlQuery.RegistroPagina.Value;
                pagina.TotalDeRegistro = totalDeRegistro;
                pagina.TotalDePagina = (int)Math.Ceiling((double)totalDeRegistro / palavraUrlQuery.RegistroPagina.Value);
                lista.Paginacao = pagina;
            }

            lista.Resultados.AddRange(item.ToList());

            return lista;

        }
        public void Atualizar(Palavra palavra)
        {
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
        }

        public void Cadastrar(Palavra palavra)
        {
            _banco.Palavras.Add(palavra);
            _banco.SaveChanges();
        }

        public void Deletar(int id)
        {
            var palavra = Obter(id);
            palavra.Ativo = false;
            _banco.Palavras.Update(palavra);

            _banco.SaveChanges();


        }


    }
}

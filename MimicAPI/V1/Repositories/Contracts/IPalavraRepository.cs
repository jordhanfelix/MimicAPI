using MimicAPI.Helpers;
using MimicAPI.V1.Models;

namespace MimicAPI.V1.Repositories.Contracts
{
    public interface IPalavraRepository
    {
        PaginacaoLista<Palavra> ObterPalavras(PalavraUrlQuery palavraUrlQuery);
        Palavra Obter(int id);
        void Cadastrar(Palavra palavra);
        void Atualizar(Palavra palavra);
        void Deletar(int id);

    }
}

using AutoMapper;
using MimicAPI.V1.Models;
using MimicAPI.V1.Models.DTO;

namespace MimicAPI.Helpers
{
    public class DTOMapperProfile : Profile
    {
        public DTOMapperProfile()
        {
            CreateMap<Palavra, PalavraDTO>();
            CreateMap<PaginacaoLista<Palavra>, PaginacaoLista<PalavraDTO>>();
        }
    }
}

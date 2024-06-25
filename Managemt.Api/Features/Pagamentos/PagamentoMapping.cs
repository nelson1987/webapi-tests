using AutoMapper;

namespace Managemt.Api.Features.Pagamentos;

public class PagamentoMapping : Profile
{
    public PagamentoMapping()
    {
        CreateMap<PagamentoCommand, Pagamento>()
            .ForCtorParam("numeroContrato", opt => opt.MapFrom(src => src.NumeroContrato))
            .ForCtorParam("preco", opt => opt.MapFrom(src => src.Preco))
            .ForCtorParam("quantidade", opt => opt.MapFrom(src => src.Quantidade))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IdEstoque, opt => opt.Ignore());
    }
}

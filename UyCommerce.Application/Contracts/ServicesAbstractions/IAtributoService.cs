using UyCommerce.Application.DTOs.Atributos;

namespace UyCommerce.Application.Contracts.ServicesAbstractions
{
    public interface IAtributoService
    {
        Task<GetAtributosResponse> GetAtributos();
        Task AddAtributoValor(AddAtributoValorRequest atributoDto);
        Task AddAtributo(string nombre);
    }
}

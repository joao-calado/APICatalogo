using System.ComponentModel.DataAnnotations;

namespace APICatalogo.DTOs;

public class ProdutoDTOUpdateRequest : IValidatableObject
{
    [Range(0, 9999, ErrorMessage = "O estoque deve estar entre 1 e 9999")]
    public float Estoque { get; set; }

    public DateTime DataCadastro { get; set; }
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DataCadastro.Date <= DateTime.Now.Date)
        {
            yield return new ValidationResult("A data deve ser menor, ou igual, que a data atual",
                new[] { nameof(this.DataCadastro) });
        }
    }
}

namespace Dual.Shared.Models.Empresa;

public class Tutor
{
    public int Id { get; set; }

    public string Correu { get; set; } = null!;

    public string Nom { get; set; } = null!;

    public int IdCicle { get; set; }

    public string? Observacions { get; set; }

    public virtual Cicle IdCicleNavigation { get; set; } = null!;
}

namespace Dual.Shared.Models;

public partial class TTutor
{
    public int Id { get; set; }

    public string Correu { get; set; } = null!;

    public string Nom { get; set; } = null!;

    public int IdCicle { get; set; }

    public string? Observacions { get; set; }

    public virtual TCicle IdCicleNavigation { get; set; } = null!;
}

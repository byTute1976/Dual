namespace Dual.Shared.Models.Empresa;

public class Cicle
{
    public int Id { get; set; }

    public string Codi { get; set; } = null!;

    public string Nom { get; set; } = null!;

    public string? Observacions { get; set; }

    public virtual ICollection<Tutor> Tutors { get; set; } = new List<Tutor>();
}

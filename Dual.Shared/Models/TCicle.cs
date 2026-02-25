namespace Dual.Shared.Models;

public partial class TCicle
{
    public int Id { get; set; }

    public string Codi { get; set; } = null!;

    public string Nom { get; set; } = null!;

    public string? Observacions { get; set; }

    public virtual ICollection<TTutor> TTutors { get; set; } = new List<TTutor>();
}

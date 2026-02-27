namespace Dual.Api.Data
{
    public static class Sql
    {
        public const string GetCicles = @"
            SELECT
                c.Id,
                c.Codi,
                c.Nom,
                c.Observacions,
                t.Id AS TutorId,
                t.Correu AS TutorCorreu,
                t.Nom AS TutorNom,
                t.IdCicle AS TutorIdCicle,
                t.Observacions AS TutorObservacions
            FROM t_Cicles c
            LEFT OUTER JOIN TTutors t ON c.Id = t.IdCicle;
        ";

        public const string GetCicleById = @"
            SELECT
                c.Id,
                c.Codi,
                c.Nom,
                c.Observacions,
                t.Id AS TutorId,
                t.Correu AS TutorCorreu,
                t.Nom AS TutorNom,
                t.IdCicle AS TutorIdCicle,
                t.Observacions AS TutorObservacions
            FROM t_Cicles c
            LEFT JOIN Tutors t ON c.Id = t.IdCicle
            WHERE c.Id = @Id;
        ";
    }
}

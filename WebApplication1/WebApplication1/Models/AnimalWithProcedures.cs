namespace WebApplication1.Models;

public class AnimalWithProcedures
{
    public string Name { get; set; }
    public string Type { get; set; }
    public DateTime admissionDate { get; set; }
    public int OwnerID { get; set; }
    public List<NewProcedure> Procedures { get; set; }
    
}

public class NewProcedure
{
    public int ProcedureID { get; set; }
    public DateTime Date { get; set; }
}
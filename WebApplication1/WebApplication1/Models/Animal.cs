namespace WebApplication1.Models;

public class Animal
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public DateTime AdmissionDate { get; set; }
    public Owner owner { get; set; }
    public List<Procedure> Procedures { get; set; }
}

public class Owner
{
    public int ID { get; set; }
    public string FirstName { get; set; }
    public string LastNAme { get; set; }
}
public class Procedure
{
    //public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
}

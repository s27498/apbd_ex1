
using Microsoft.Data.SqlClient;
using WebApplication1.Models;

namespace WebApplication1.Repositories;

public class AnimalRepository
{
    // obyczaj
    private readonly IConfiguration _configuration;

    public AnimalRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    // obyczaj
    public async Task<bool> doesAnimalExist(int id)
    {
        var query = "SELECT 1 FROM ANIMAL WHERE @id = ID";
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@id", id);
        await connection.OpenAsync();
        var reader = await command.ExecuteScalarAsync();
        if (reader == null)
        {
            return false;
        }

        return true;
    }
    public async Task<bool> doesOwnerExist(int id)
    {
        var query = "Select 1 from Owner where id = @id";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@id", id);

        await connection.OpenAsync();

        var result = await command.ExecuteScalarAsync();

        return result is not null;
    }
    public async Task<bool> doesProcedureExist(int id)
    {
        var query = "Select 1 from [Procedure] where id = @id";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@id", id);

        await connection.OpenAsync();

        var result = await command.ExecuteScalarAsync();

        return result is not null;
    }

    public async Task<Animal> getAnimal(int id)
    {
        // obyczaj
        
        var query = "SELECT ANIMAL.ID, ANIMAL.NAME, Type, AdmissionDate, Owner_ID, FirstName, LastName, P.Name, P.Description, DATE FROM Animal JOIN dbo.Owner O on Animal.Owner_ID = O.ID JOIN dbo.Procedure_Animal PA on Animal.ID = PA.Animal_ID JOIN dbo.[Procedure] P on Animal.ID = @id";
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
   
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        // parametr @ (kinda obyczaj)
        command.Parameters.AddWithValue("@id", id);
        await connection.OpenAsync();

        // obyczaj
        var reader = await command.ExecuteReaderAsync();
        var animalIdOrdinal = reader.GetOrdinal("ID");
        var animalNameOrdinal = reader.GetOrdinal("NAME");
        var animalTypeOrdinal = reader.GetOrdinal("Type");
        var admissionDateOrdinal = reader.GetOrdinal("AdmissionDate");
        var ownerIdOrdinal = reader.GetOrdinal("Owner_ID");
        var firstNameOrdinal = reader.GetOrdinal("FirstName");
        var lastNameOrdinal = reader.GetOrdinal("LastName");
        var procedureNameOrdinal = reader.GetOrdinal("Name");
        var procedureDescriptionOrdinal = reader.GetOrdinal("Description");
        var dateOrdinal = reader.GetOrdinal("Date");

        Animal animal = null;
        while (await reader.ReadAsync())
        {
            if (animal == null)
            {
                animal = new Animal()
                {
                    ID = reader.GetInt32(animalIdOrdinal),
                    Name = reader.GetString(animalNameOrdinal),
                    Type = reader.GetString(animalTypeOrdinal),
                    AdmissionDate = reader.GetDateTime(admissionDateOrdinal),
                    owner = new Owner()
                    {
                        ID = reader.GetInt32(ownerIdOrdinal),
                        FirstName = reader.GetString(firstNameOrdinal),
                        LastNAme = reader.GetString(lastNameOrdinal)
                    },
                    Procedures = new List<Procedure>()
                    {
                        new Procedure()
                        {
                            Name = reader.GetString(procedureNameOrdinal),
                            Description = reader.GetString(procedureDescriptionOrdinal),
                            Date = reader.GetDateTime(dateOrdinal)
                        }
                    }

                };
            }
            else
            {
                animal.Procedures.Add(new Procedure()
                {
                    Name = reader.GetString(procedureNameOrdinal),
                    Description = reader.GetString(procedureDescriptionOrdinal),
                    Date = reader.GetDateTime(dateOrdinal)
                });
            }
            
        }

        return animal;
    }

    public async Task addAnimalWithProcedure(AnimalWithProcedures animalWithProcedures)
    {
        var insert = @"INSERT INTO Animal VALUES(@Name, @Type, @AdmissionDate, @OwnerId);
					   SELECT @@IDENTITY AS ID;";
	    
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
	    
        command.Connection = connection;
        command.CommandText = insert;
	    
        command.Parameters.AddWithValue("@Name", animalWithProcedures.Name);
        command.Parameters.AddWithValue("@Type", animalWithProcedures.Type);
        command.Parameters.AddWithValue("@AdmissionDate", animalWithProcedures.admissionDate);
        command.Parameters.AddWithValue("@OwnerId", animalWithProcedures.OwnerID);
	    
        await connection.OpenAsync();

        var transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;
	    
        try
        {
            var id = await command.ExecuteScalarAsync();
    
            foreach (var procedure in animalWithProcedures.Procedures)
            {
                command.Parameters.Clear();
                command.CommandText = "INSERT INTO Procedure_Animal VALUES(@ProcedureId, @AnimalId, @Date)";
                command.Parameters.AddWithValue("@ProcedureId", procedure.ProcedureID);
                command.Parameters.AddWithValue("@AnimalId", id);
                command.Parameters.AddWithValue("@Date", procedure.Date);

                await command.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
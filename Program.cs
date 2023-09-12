using Npgsql;
using HoneyRaesAPI.Models;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;

var connectionString = "Host=localhost;Port=5432;Username=postgres;Password=INSERTPASSWORDHERE;Database=HoneyRaes";

// importing the fully-qualified namespace of program data models; prior to the "using" directive, above
// List<HoneyRaesAPI.Models.Customer> customers = new List<HoneyRaesAPI.Models.Customer> {};
// List<HoneyRaesAPI.Models.Employee> employees = new List<HoneyRaesAPI.Models.Employee> {};
// List<HoneyRaesAPI.Models.ServiceTickets> serviceTickets = new List<HoneyRaesAPI.Models.ServiceTickets> {};

List<Customer> customers = new List<Customer>
{
    new Customer () {Id = 1, Name = "Robert", Address = "123 Street St"},
    new Customer () {Id = 2, Name = "Marley", Address = "321 Street St"},
    new Customer () {Id = 3, Name = "Richard", Address = "456 Street St"}
};

List<Employee> employees = new List<Employee>
{
    new Employee () {Id = 1, Name = "George", Specialty = "777 Street St"},
    new Employee () {Id = 2, Name = "Ronald", Specialty = "888 Street St"},
};

List<ServiceTicket> serviceTickets = new List<ServiceTicket>
{
    new ServiceTicket () {Id = 1, CustomerId = 1, EmployeeId = 1, Description = "dirty laundry", Emergency = true, DateCompleted = new DateTime(2023, 8, 10)},
    new ServiceTicket () {Id = 2, CustomerId = 2, Description = "cat litter", Emergency = true},
    new ServiceTicket () {Id = 3, CustomerId = 3, EmployeeId = 1, Description = "need to get rid of a dead body", Emergency = false, DateCompleted = new DateTime(2023, 7, 29)},
    new ServiceTicket () {Id = 4, CustomerId = 1, EmployeeId = 2, Description = "worrying and foreboding machinations", Emergency = false},
    new ServiceTicket () {Id = 5, CustomerId = 2, Description = "drive me to school", Emergency = true},
};

var builder = WebApplication.CreateBuilder(args);

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// endpoints

// original "get employees" endpoint with C#-based non-persistent database
// app.MapGet("/employees", () =>
// {
//     return employees;
// });

app.MapGet("/employees", () =>
{
    // create an empty list of employees to add to. 
    List<Employee> employees = new List<Employee>();
    //make a connection to the PostgreSQL database using the connection string
    using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
    //open the connection
    connection.Open();
    // create a sql command to send to the database
    using NpgsqlCommand command = connection.CreateCommand();
    command.CommandText = "SELECT * FROM Employee";
    //send the command. 
    using NpgsqlDataReader reader = command.ExecuteReader();
    //read the results of the command row by row
    while (reader.Read()) // reader.Read() returns a boolean, to say whether there is a row or not, it also advances down to that row if it's there. 
    {
        //This code adds a new C# employee object with the data in the current row of the data reader 
        employees.Add(new Employee
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")), //find what position the Id column is in, then get the integer stored at that position
            Name = reader.GetString(reader.GetOrdinal("Name")),
            Specialty = reader.GetString(reader.GetOrdinal("Specialty"))
        });
    }
    //once all the rows have been read, send the list of employees back to the client as JSON
    return employees;
});

// original "get employee by id" endpoint with C#-based non-persistent database
// app.MapGet("/employees/{id}", (int id) =>
// {
//     Employee employee = employees.FirstOrDefault(e => e.Id == id);
//     if (employee == null)
//     {
//         return Results.NotFound();
//     }
//     employee.ServiceTickets = serviceTickets.Where(st => st.EmployeeId == id).ToList();
//     return Results.Ok(employee);
// });

app.MapGet("/employees/{id}", (int id) =>
{
    Employee employee = null;
    using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
    connection.Open();
    using NpgsqlCommand command = connection.CreateCommand();
    command.CommandText = @"
        SELECT 
            e.Id,
            e.Name, 
            e.Specialty, 
            st.Id AS serviceTicketId, 
            st.CustomerId,
            st.Description,
            st.Emergency,
            st.DateCompleted 
        FROM Employee e
        LEFT JOIN ServiceTicket st ON st.EmployeeId = e.Id
        WHERE e.Id = @id";

    // use command parameters to add the specific Id we are looking for to the query
    command.Parameters.AddWithValue("@id", id);
    using NpgsqlDataReader reader = command.ExecuteReader();

    // For if we are only expecting one row back (and therefore don't need a loop):
    // if (reader.Read())
    // {
    //     employee = new Employee
    //     {
    //         Id = reader.GetInt32(reader.GetOrdinal("Id")),
    //         Name = reader.GetString(reader.GetOrdinal("Name")),
    //         Specialty = reader.GetString(reader.GetOrdinal("Specialty"))
    //     };
    // }

    // For if we are expecting multiple rows back:
    while (reader.Read())
    {
        // this "if" condition catches if we have already set employee data or not (i.e., is this the first row or not)
        if (employee == null)
        {
            employee = new Employee
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Specialty = reader.GetString(reader.GetOrdinal("Specialty")),
                ServiceTickets = new List<ServiceTicket>() //empty List to add service tickets to
            };
        }

        // reader.IsDBNull checks if a column in a particular position is null
        if (!reader.IsDBNull(reader.GetOrdinal("serviceTicketId")))
        {
            employee.ServiceTickets.Add(new ServiceTicket
            {
                Id = reader.GetInt32(reader.GetOrdinal("serviceTicketId")),
                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                //we don't need to get this from the database, we already know it
                EmployeeId = id,
                Description = reader.GetString(reader.GetOrdinal("Description")),
                Emergency = reader.GetBoolean(reader.GetOrdinal("Emergency")),
                // Npgsql can't automatically convert NULL in the database to C# null, so we have to check whether it's null before trying to get it
                DateCompleted = reader.IsDBNull(reader.GetOrdinal("DateCompleted")) ? null : reader.GetDateTime(reader.GetOrdinal("DateCompleted"))
            });
        }
    }
    // Return 404 if the employee is never set (meaning, that reader.Read() immediately returned false because the id did not match an employee)
    // otherwise 200 with the employee data
    return employee == null ? Results.NotFound() : Results.Ok(employee);
});

// POST new employee
app.MapPost("/employees", (Employee employee) =>
{
    using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
    connection.Open();
    using NpgsqlCommand command = connection.CreateCommand();
    command.CommandText = @"
        INSERT INTO Employee (Name, Specialty)
        VALUES (@name, @specialty)
        RETURNING Id
    ";
    command.Parameters.AddWithValue("@name", employee.Name);
    command.Parameters.AddWithValue("@specialty", employee.Specialty);

    //the database will return the new Id for the employee, add it to the C# object
    employee.Id = (int)command.ExecuteScalar();

    return employee;
});

// EDIT existing employee at :id
app.MapPut("/employees/{id}", (int id, Employee employee) =>
{
    if (id != employee.Id)
    {
        return Results.BadRequest();
    }
    using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
    connection.Open();
    using NpgsqlCommand command = connection.CreateCommand();
    command.CommandText = @"
        UPDATE Employee 
        SET Name = @name,
            Specialty = @specialty
        WHERE Id = @id
    ";
    command.Parameters.AddWithValue("@name", employee.Name);
    command.Parameters.AddWithValue("@specialty", employee.Specialty);
    command.Parameters.AddWithValue("@id", id);

    command.ExecuteNonQuery();
    return Results.NoContent();
});

// DELETE existing employee at id

app.MapDelete("/employees/{id}", (int id) =>
{
    using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
    connection.Open();
    using NpgsqlCommand command = connection.CreateCommand();
    command.CommandText = @"
        DELETE FROM Employee WHERE Id=@id
    ";
    command.Parameters.AddWithValue("@id", id);
    command.ExecuteNonQuery();
    return Results.NoContent();
});

app.MapGet("/customers", () =>
{
    return customers;
});

app.MapGet("/customers/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(c => c.Id == id);
    if (customer == null)
    {
        return Results.NotFound();
    }
    customer.ServiceTickets = serviceTickets.Where(st => st.CustomerId == id).ToList();
    return Results.Ok(customer);
});

app.MapGet("/serviceTickets", () =>
{
    return serviceTickets;
});

app.MapGet("/serviceTickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (serviceTicket == null)
    {
        return Results.NotFound();
    }
    serviceTicket.Customer = customers.FirstOrDefault(c => c.Id == serviceTicket.CustomerId);
    serviceTicket.Employee = employees.FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);
    return Results.Ok(serviceTicket);
});

app.MapPost("/serviceTickets", (ServiceTicket serviceTicket) =>
{
    // creates a new ID (later: our SQL database will do this for us)
    serviceTicket.Id = serviceTickets.Count > 0 ? serviceTickets.Max(st => st.Id) + 1 : 1;
    serviceTickets.Add(serviceTicket);
    return serviceTicket;
});

app.MapDelete("/serviceTickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (serviceTicket == null)
    {
        return Results.NotFound();
    }
    serviceTickets.RemoveAt(id - 1);
    return Results.Ok();
});

app.MapPut("/serviceTickets/{id}", (int id, ServiceTicket serviceTicket) =>
{
    ServiceTicket ticketToUpdate = serviceTickets.FirstOrDefault(st => st.Id == id);
    int ticketIndex = serviceTickets.IndexOf(ticketToUpdate);
    if (ticketToUpdate == null)
    {
        return Results.NotFound();
    }
    if (id != serviceTicket.Id)
    {
        return Results.BadRequest();
    }
    serviceTickets[ticketIndex] = serviceTicket;
    return Results.Ok();
});

app.Run();
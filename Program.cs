using HoneyRaesAPI.Models;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;

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

app.MapGet("/employees", () =>
{
    return employees;
});

app.MapGet("/employees/{id}", (int id) =>
{
    Employee employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    employee.ServiceTickets = serviceTickets.Where(st => st.EmployeeId == id).ToList();
    return Results.Ok(employee);
});

app.MapGet("/customers", () =>
{
    return  customers;
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
    serviceTicket.Id = serviceTickets.Count > 0 ?serviceTickets.Max(st => st.Id) + 1 : 1;
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
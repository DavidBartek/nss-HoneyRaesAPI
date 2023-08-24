# Book 2 Terms Reference

`app.MapGet("/customers", () =>
{
    return  customers;
});`

- **endpoint** - a specific url at which clients can access resources provided by the API.
    - endpoints are each individually set up in the API program.
    - constituted by a **route** and a **handler**

- **route** - the URL for a specific endpoint. Above: `"/customers`

- **handler** - function that is called when a request is made to a particular endpoint's URL. Above: `() => { return customers }`
    - can accept data from HTTP request as arguments, and returns data for HTTP response.
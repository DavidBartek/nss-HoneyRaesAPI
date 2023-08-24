# Book 2 Terms Reference

## API Terms

- **API** - "application programming interface"

- **REST** - *representational state transfer* - an architectural approach to designing web services. Most common REST API implementations use HTTP as the application protocol.

- **resource** - any kind of object, data, or service. REST APIs are designed around these. They are accessed by the client.

- **identifier** - a URI that uniquely identifies a given resource. e.g., `example.com/customers/1`

- **representations** - (*RE*ST) - clients interact with a service by obtaining *representations* of resources. JSON is one of the most common exchange formats.

- **uniform interface** - for REST APIs built on HTTP, uniform interface includes using standard HTTP verbs to perform CRUD operations on resources: GET, POST, PUT, PATCH, DELETE

- **stateless request model** - (RE*S*T) - HTTP requests are to be independent, can occur in any order, each as its own "atomic" operation.

- **hypermedia links** - what REST APIs are to be driven by. Contained in the representation.

`{
    "orderID":3,
    "productID":2,
    "quantity":4,
    "orderValue":16.60,
    "links": [
        {"rel":"product","href":"https://adventure-works.com/customers/3", "action":"GET" },
        {"rel":"product","href":"https://adventure-works.com/customers/3", "action":"PUT" }
    ]
}`

*note: not all REST APIs meet this criteria; most terminate their RESTfulness on conformity to the HTTP methods

## Endpoint Anatomy

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


## Organizing an API's design

- APIs are client-facing; they should not expose any internal implementation, but rather should model entities and the operations an app can perform on those entities.
- **focus on the business entities the web API exposes.** e.g., customers, orders.
- **prefer nouns over verbs.** e.g. `https://example.com/orders` *not* `https://example.com/create-order`
- **If present URIs should represent *collections* of entities.** .../orders... vs. /order-1/
- **max depth of complexity:** `collection/item/collection`. Avoid requiring resource URIs more complex than this.

## HTTP Semantics - Response Codes for Each Type of HTTP Method

- **GET**
    - successful: 200 (OK)
    - not found: 404 (Not Found)
    - fulfilled but no response body included in HTTP response: 204 (No Content)
- **POST**
    - successful resource creation: 201 (Created)
        - URI of new resource is included in the location header of response
        - body contains a resource representation
    - Processing occurs but no new resource created: 200 (OK)
        - include result of operation in the response body
    - OR: no result to return: 204 (No Content)
        - include no response body
- **PUT**
    - successful resource creation: 201 (Created)
    - successful update to existing resource: 200 (OK) or 204 (No Content)
    - if not possible to update an existing resource: 409 (Conflict)
- **PATCH** - 2 JSON-based patch formats:
    - **JSON Patch**
    - **JSON Merge Patch** - simpler, with examples below.
        - Patch document format not supported: 415 (unsupported media type)
        - Malformed patch document: 400 (Bad Request)
        - Patch doc is valid, but changes cannot be applied to resource in its current state: 409 (Conflict)

Original resource:

`{
    "name":"gizmo",
    "category":"widgets",
    "color":"blue",
    "price":10
}`

Possible JSON merge patch:

`{
    "price":12,
    "color":null,
    "size":"small"
}`

*updates price, deletes color, adds size, name and category not modified.*
Merge Patch not suitable if the original resource cannot contain explicit **null** values.

- **DELETE**
    - successful deletion: 204 (No Content)
        - response body contains no further info
    - resource doesn't exist: 404 (Not Found)
# Running Questions for Instructors

1. Project opens 2 ports: 1. https...7052 2. http...5178
   **Why?**

1. When I run dotnet watch run from terminal: properly-rendered page opens in browser (port 7052)
1. When I run VSCode debugger: "404" page opens in browser (port 7052)
   **Why?**

1. When entities are embedded into other entities as properties, "null" values result if that nested entity itself contains nested entities.
   **Is this expected?**

^^ many of the above pending an answer as of 8/24/23

1. I could use an explanatory walkthrough of all things CORS...

- the restrictions are set up by API, yet enforced by the user's browser?
- why does the API read the proxy server as the same origin as the client when another (arbitrary) domain & port is specified altogether?
  - why is a **forwarded** request from a different origin allowable when a **regular** request from a different origin not?
- how this would all be modified (or not) when deploying an app?

1. Asynchronous operations

- async/await vs. then - why
- if async/await is better, why ever use .then

1. DeShawn's - handling the many-to-many relationship between cities and walkers

- in Program.cs: see endpoint, MapGet("/filteredWalkers/{cityId}") & in Walkers.js: corresponding useEffect. This is where we are filtering the list of walkers by a given city id.
   - why (and...how to) embed a "cities" list in the "Walkers" class (and vice versa), when you can set up an endpoint like this?
   - how was it *envisioned* for us to do this? / what is best practice?

1. DeShawn's - Console error quirk - in Walkers.js

- Configured an "offCanvas" element to display assignable dogs
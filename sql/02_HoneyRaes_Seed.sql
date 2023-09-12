\c HoneyRaes

INSERT INTO Customer (Name, Address) VALUES ('Robert', '123 Street St');
INSERT INTO Customer (Name, Address) VALUES ('Marley', '321 Street St');
INSERT INTO Customer (Name, Address) VALUES ('Richard', '456 Street St');

INSERT INTO Employee (Name, Specialty) VALUES ('George', 'scheming');
INSERT INTO Employee (Name, Specialty) VALUES ('Ronald', 'snooping');

INSERT INTO ServiceTicket (CustomerId, EmployeeId, Description, Emergency, DateCompleted) VALUES (1, 1, 'My laundry is dirty', true, '2023-08-10');
INSERT INTO ServiceTicket (CustomerId, EmployeeId, Description, Emergency, DateCompleted) VALUES (2, NULL, '3-week-uncleaned cat litter', true, NULL);
INSERT INTO ServiceTicket (CustomerId, EmployeeId, Description, Emergency, DateCompleted) VALUES (3, 1, 'Need to get rid of a dead body', false, '2023-07-29');
INSERT INTO ServiceTicket (CustomerId, EmployeeId, Description, Emergency, DateCompleted) VALUES (1, 2, 'Worrysome, forboding machinations', false, NULL);
INSERT INTO ServiceTicket (CustomerId, EmployeeId, Description, Emergency, DateCompleted) VALUES (2, NULL, 'I missed the bus drive me to school please', true, NULL);
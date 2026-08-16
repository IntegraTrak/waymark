# Waymark EF Core sample

This sample uses EF Core's native migrations and model snapshot tooling.

The generated migrations under `Migrations/` provide the controlled states:

- `Initial` is the earlier model.
- `AddCustomerEmail` is the later model and adds `Customer.Email`.

The design-time factory makes the sample discoverable by `dotnet ef` without connecting to a database during migration generation.

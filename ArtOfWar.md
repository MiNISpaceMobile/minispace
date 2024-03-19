# Art Of War

This document contains guidelines for the developers.
It defines the division of responsibility between .NET projects in this solution.

## Api

This is the startup project.

All configuration, which is not security-critical (NO CONNECTION STRINGS / API KEYS) should be put in appsettings.json or its specialized versions.

All interface implementations should be registered into dependency injection in Program.cs.
-> Look for IPingResponder registration inside as an example.

All api endpoints should be implemented as Controller methods.
However, they should only use dependency-injected logic from other projects.
Controllers do not implement business or integration logic!
-> See PingController as an example.

## Domain

This is where the data model and business logic is implemented.

Also all interfaces (database, email sender, etc.) are defined here.
-> See IPingResponder as an example

## Infrastructure

This project contains implementations (and fakes) of all interfaces defined in Domain.
-> See PongPingResponder as an example

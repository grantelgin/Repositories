# Repositories
Repository Pattern implementations for various persistence formats

In *Domain Driven Design: Tackling Complexity in the Heart of Software*, Eric Evans states that a repository "represents all objects of a certain type as a conceptual set (usually emulated)". He also says that for every object that is an Aggregate (a related group of entities), create a repository for the object and give it the look and feel of an in-memory collection of that type of object. 

The main point of repositories is to keep the developer focused on the domain model logic, and hide the plumbing of data access behind a well-known repository interface.

This project includes an IRepository interface that has worked well for my projects as well as the persistence specific implementations (e.g. JSON, SQL, XML), along with example "Contexts" that use the repositories. 




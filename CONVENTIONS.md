# C# and .NET Coding Conventions

## General Guidelines
- Use C# 10.0 or later features where applicable
- Target .NET 6.0 or later
- Follow SOLID principles
- Implement Clean Architecture
- Use async/await for asynchronous operations
- Prefer immutability where possible

## Code Style
- Use PascalCase for class names, method names, and public members
- Use camelCase for local variables and private fields
- Prefix private fields with an underscore (_)
- Use verb-noun naming for methods (e.g., GetUser, CalculateTotal)
- Use meaningful and descriptive names for variables, methods, and classes

## Architecture and Design Patterns
- Prefer Microsoft.Extensions.DependencyInjection for dependency injection
- Use the Strategy pattern for different delimiter strategies:
    - This allows for flexible parsing of input strings with different delimiter rules.
    - Each strategy (e.g., DefaultDelimiterStrategy, CustomDelimiterStrategy) encapsulates a specific parsing algorithm.
- Use the Command pattern for handling different calculation operations:
    - This encapsulates each operation (Add, Subtract, Multiply, Divide) as a separate object.
    - Enables features like operation history, undo/redo functionality, or queuing of operations.
- Use the Factory pattern for object creation when appropriate
- Use the Options pattern for strongly-typed configuration

## Dependency Injection
- Use constructor injection as the preferred method for dependency injection
- Register dependencies in a centralized location (e.g., Startup.cs or Program.cs)

## Project Structure
- Organize solution into multiple projects: Core, Application, Infrastructure, and Presentation
- Keep the Core project free of external dependencies
- Use feature folders within projects when appropriate

## File Organization
- One class per file, unless dealing with small, closely related classes
- Organize files in a logical folder structure that mirrors the namespace structure

## Nullable Reference Types
- Enable nullable reference types in all projects
- Use the null-coalescing operator ?? and null-conditional operator ?. where appropriate

## LINQ
- Use LINQ whenever possible if doing so provides equal or better code readability
- Prefer method syntax over query syntax for consistency
- Use meaningful names for lambda parameters (avoid single-letter names except for very short lambdas)

## Performance Considerations
- Use StringBuilder for string concatenation in loops
- Prefer for loops over foreach when dealing with arrays or lists if the index is needed
- Use struct for small, immutable value types

## Error Handling and Logging
- Use custom exceptions for domain-specific errors
- Use specific exception types rather than generic exceptions
- Implement global exception handling
- Avoid empty catch blocks. Always handle or rethrow exceptions
- Use exception filters where appropriate
- Use a logging framework like Serilog for structured logging

## Testing
- Write unit tests using xUnit
- Use Moq for mocking in unit tests
- Aim for high test coverage, especially in the Core and Application layers
- Implement integration tests for critical paths
- Write unit tests for all public methods and complex private methods
- Use descriptive test method names that explain the scenario and expected outcome
- Follow the Arrange-Act-Assert (AAA) pattern in unit tests
- Use mock objects for external dependencies in unit tests
- Consider using FluentAssertions for more readable test assertions

## Performance and Optimization
- Use `IEnumerable<T>` for method parameters that only need to be enumerated once
- Use `IReadOnlyList<T>` or `IReadOnlyCollection<T>` for read-only collections
- Consider using `Span<T>` and `Memory<T>` for high-performance scenarios

## Security
- Use the latest security best practices from OWASP
- Implement proper input validation and sanitization
- Use secure coding practices to prevent common vulnerabilities (e.g., SQL injection, XSS)

## Code Analysis and Quality
- Maintain a high level of code quality and consistency
- Regularly refactor and improve existing code

## Documentation
- Use XML comments for public APIs and important internal methods
- Keep README files up-to-date with project setup and usage instructions

## Package Management
- Use NuGet for package management
- Keep packages updated to their latest stable versions

## Version Control
- Write meaningful commit messages

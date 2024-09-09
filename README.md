# Calc - String Calculator Challenge

## Project Overview

Calc is a .NET-based string calculator application developed as part of a coding challenge. The project aims to demonstrate clean code practices, test-driven development (TDD), and the application of various design patterns in a real-world scenario.

## Features

The calculator supports the following operations:

1. Addition of numbers provided as a formatted string
2. Custom delimiters
3. Handling of invalid inputs
4. Negative number detection
5. Upper bound limit on input numbers

Stretch goals include:
- Subtraction, multiplication, and division operations
- Display of the calculation formula
- Command-line argument support for configuration
- Continuous input processing

## Project Structure

The solution is divided into three main projects:

1. `Calc.Core`: Contains the core domain logic and interfaces
2. `Calc.Infrastructure`: Implements the core interfaces and provides concrete implementations
3. `Calc.ConsoleApp`: Provides the user interface for interacting with the calculator

Each project has a corresponding test project to ensure code quality and functionality.

## Getting Started

### Prerequisites

- .NET 6.0 SDK or later

### Building the Project

1. Clone the repository
2. Navigate to the project root directory
3. Run `dotnet build` to build the solution

### Running Tests

To run the tests, use the following command in the project root directory:

```
dotnet test
```

### Running the Application

To run the console application:

1. Navigate to the `src/Calc.ConsoleApp` directory
2. Run `dotnet run`

## Design Patterns Used

- Strategy Pattern: For handling different delimiter strategies
- Command Pattern: For encapsulating calculator operations
- Factory Pattern: For creating delimiter strategies and operations
- Options Pattern: For handling configuration

## Contributing

This project is part of a coding challenge and is not currently open for contributions. However, feedback and suggestions are welcome.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- This project was developed as part of a coding challenge to demonstrate clean code practices and test-driven development.
- Special thanks to the creators of the challenge for providing an opportunity to showcase software development skills.
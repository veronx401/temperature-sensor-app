# Temperature Sensor Simulation

[![.NET Tests](https://github.com/veronx401/temperature-sensor-app/actions/workflows/dotnet-test.yml/badge.svg)](https://github.com/veronx401/temperature-sensor-app/actions/workflows/dotnet-test.yml)

A .NET console application that simulates a virtual temperature sensor for data center monitoring.


## Prerequisites
- .NET 6.0 or later


## Features

***Core Features***

- **Sensor Initialisation**: `InitialiseSensor()` - reads the configuration from the YAML file and validates the input.
- **Data Simulation**: `SimulateData()` - Generates temperature readings with random noise (±0.5°C).
- **Data Validation**: `ValidateData()` - Checks if the temperature is within the 22–24 °C range.
- **Data Logging**: `LogData()` - Saves data with a timestamp to the console and a file.
- **Anomaly Detection**: `DetectAnomaly()` - flags readings that deviate from the recent average.
- **Sensor Control**: `ShutdownSensor()` - stops the sensor and clears the history.
- **Data Storage**: `StoreData()` – Maintains history in JSON format.
- **Advanced Features** - Data smoothing, anomaly detection, fault injection
- **Unit Testing** - Comprehensive test coverage (71.5%)

***Advanced Features***

- **Data smoothing**: applies a moving average to reduce noise.
- **Fault injection**: simulates sensor failures for testing.
- **Threshold alerts**: custom temperature limits.


## Setup
1. Clone repo: `git clone https://github.com/veronx401/temperature-sensor-app.git`
2. `cd temperature-sensor-app`
3. `dotnet restore`
4. Create `config.txt` file:
```yaml
name: "DataCentre-Sensor1"
location: "Server Room A"
minTemp: 22
maxTemp: 24
```
5. `dotnet run`


## Testing
Run: `dotnet test`
- **Test Coverage**: 71.5%
- **Tests include**: Sensor initialization, data validation, anomaly detection, edge cases
- **Framework**: xUnit



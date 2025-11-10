using System;
using System.Threading;

namespace Sensors
{
    internal static class Program
    {
        private static readonly Random Random = new();

        private static void Main(string[] args)
        {
            try
            {
                // TODO: needs to initialize name from file ← IMPLEMENT INI!
                Sensor sensor = Sensor.InitializeSensor();

                Console.WriteLine($"Simulation started for sensor : {sensor.Name}");
                Console.WriteLine($"Location: {sensor.Location}");
                Console.WriteLine($"Temperature Range: {sensor.MinTemp}-{sensor.MaxTemp}°C");
                Console.WriteLine("-----------------------------------");

                //simulation loop
                while (true)
                {
                    double tempC = sensor.SimulateData();
                    bool isValid = sensor.ValidateData(tempC);

                    //display validation status
                    string status = isValid ? "VALID" : "INVALID";
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss}  Temperature: {tempC:F1}°C [{status}]");

                    Thread.Sleep(1000);
                    var reading = new Reading();
                    reading.SensorName = sensor.Name;
                    reading.DateTime = DateTime.Now;
                    reading.Value = tempC;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        //DELETE THIS METHOD (because it has been replaced with Sensor.SimulateData())
        // private static double GenerateTemperature()
        // {
        //     const double mean = 22.0;
        //     const double variation = 5.0;
        //     return mean + (Random.NextDouble() * 2 - 1) * variation;
        // }
    }
}


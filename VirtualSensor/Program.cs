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
                //initialize sensor form file config
                Sensor sensor = Sensor.InitializeSensor();

                Console.WriteLine("=== TEMPERATURE SENSOR SIMULATION ===");
                Console.WriteLine($"Sensor: {sensor.Name}");
                Console.WriteLine($"Location: {sensor.Location}");
                Console.WriteLine($"Temperature Range: {sensor.MinTemp}-{sensor.MaxTemp}°C");
                Console.WriteLine("Logging to: sensor_log.txt");
                Console.WriteLine("Data storage: sensor_history.json");
                Console.WriteLine("=====================================");
                Console.WriteLine();

                //main simulation loop
                while (true)
                {
                    //inject fault with probability 15%
                    bool injectFault = new Random().Next(100) < 15; // 15% probability
                    double tempC;

                    if (injectFault && !sensor.IsFaultInjected)
                    {
                        tempC = sensor.InjectFault();
                        Console.WriteLine($"[FAULT INJECTED] Temperature: {tempC}°C");
                    }
                    else
                    {
                        tempC = sensor.SimulateData(); //normal operation
                    }

                    bool isValid = sensor.ValidateData(tempC);
                    string status = isValid ? "VALID" : "INVALID";

                    //fitur advanced - smoothing, anomaly detection, threshold checking
                    double smoothed = sensor.SmoothData();
                    bool isAnomaly = sensor.DetectAnomaly(tempC);
                    bool thresholdAlert = sensor.CheckThreshold(tempC);

                    //display with flag status
                    string anomalyFlag = isAnomaly ? " [ANOMALY]" : "";
                    string thresholdFlag = thresholdAlert ? " [HIGH]" : "";
                    string faultFlag = sensor.IsFaultInjected ? " [FAULT]" : "";

                    Console.WriteLine($"{DateTime.Now:HH:mm:ss}  Temperature: {tempC:F1}°C [{status}]{faultFlag}{anomalyFlag}{thresholdFlag}");
                    Console.WriteLine($"   Smoothed: {smoothed}°C");

                    //log & save data reading
                    var reading = new Reading
                    {
                        SensorName = sensor.Name,
                        Value = tempC,
                        DateTime = DateTime.Now,
                        IsValid = isValid
                    };

                    sensor.LogData(reading);
                    sensor.StoreData(reading);

                    //reset fault after 1 reading
                    if (sensor.IsFaultInjected)
                    {
                        sensor.ResetFault();
                    }

                    Thread.Sleep(1000); //delay 1 second
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        //DELETE THIS METHOD (because it has been replaced with Sensor.SimulateData())
        // private static double GenerateTemperature()
        // {`
        //     const double mean = 22.0;
        //     const double variation = 5.0;
        //     return mean + (Random.NextDouble() * 2 - 1) * variation;
        // }
    }
}


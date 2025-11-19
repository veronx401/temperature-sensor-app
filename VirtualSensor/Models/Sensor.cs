using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Sensors
{
    public class Sensor
    {
        //basic sensor properties 
        public string Name { get; set; } = "";
        public string Location { get; set; } = "";
        public double MinTemp { get; set; }
        public double MaxTemp { get; set; }
        private Random random = new Random();

        public Sensor()
        {
        }

        public Sensor(string name, string location, double minTemp, double maxTemp)
        {
            Name = name;
            Location = location;
            MinTemp = minTemp;
            MaxTemp = maxTemp;
        }


        //method for initialising sensors from a file
        public static Sensor InitializeSensor()
        {
            try
            {
                string configPath = "config.txt";

                if (!File.Exists(configPath))
                {
                    throw new Exception($"File not found: {Path.GetFullPath(configPath)}");
                }

                string[] configLines = File.ReadAllLines(configPath);


                var sensor = new Sensor
                {
                    Name = configLines[0],
                    Location = configLines[1],
                    MinTemp = double.Parse(configLines[2]),
                    MaxTemp = double.Parse(configLines[3])
                };

                return sensor;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to read config file: " + ex.Message);
            }
        }

        //simulated temperature data from noise
        public double SimulateData()
        {
            //generate a random temperature between MinTemp and MaxTemp
            double temperature = MinTemp + (random.NextDouble() * (MaxTemp - MinTemp));

            //add noise (±0.3 degrees)
            double noise = (random.NextDouble() - 0.5) * 0.6;
            temperature += noise;

            return Math.Round(temperature, 1);
        }

        //validate if temperature within the specified range
        public bool ValidateData(double temperature)
        {
            if (temperature >= MinTemp && temperature <= MaxTemp)
                return true;
            else
                return false;
        }

        //log data to file teks
        public void LogData(Reading reading)
        {
            try
            {
                string logEntry = $"{reading.DateTime:yyyy-MM-dd HH:mm:ss} | {reading.SensorName} | {reading.Value}°C | Valid: {reading.IsValid}";
                string logPath = "sensor_log.txt";

                File.AppendAllText(logPath, logEntry + Environment.NewLine);
                Console.WriteLine($"Logged: {logEntry}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logging error: {ex.Message}");
            }
        }

        //history readings data
        private List<Reading> _readingHistory = new List<Reading>();


        //save data to history and auto save every 10 readings
        public void StoreData(Reading reading)
        {
            _readingHistory.Add(reading);

            if (_readingHistory.Count % 10 == 0) //save every 10 readings
            {
                SaveHistoryToFile();
            }
        }

        //save history to file JSON
        private void SaveHistoryToFile()
        {
            try
            {
                string json = System.Text.Json.JsonSerializer.Serialize(_readingHistory, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText("sensor_history.json", json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"History save error: {ex.Message}");
            }
        }


        //take readings history
        public List<Reading> GetReadingHistory()
        {
            return _readingHistory;
        }

        //smoothing data with moving average (last 3 readings) 
        public double SmoothData()
        {
            if (_readingHistory.Count < 3)
                return _readingHistory.LastOrDefault()?.Value ?? 0;

            var recent = _readingHistory.TakeLast(3).Select(r => r.Value).ToList();
            
            return Math.Round(recent.Average(), 1);
        }

        //detect anomaly if the difference is > 2.0 from the average
        public bool DetectAnomaly(double currentTemperature)
        {
            if (_readingHistory.Count < 3)
                return false;

            var recent = _readingHistory.TakeLast(3).Select(r => r.Value).ToList();
            double average = recent.Average();

            return Math.Abs(currentTemperature - average) > 2.0;
        }

        //check threshold (more than MaxTemp + 1.0)
        public bool CheckThreshold(double temperature)
        {
            return temperature > MaxTemp + 1.0;
        }

        //fault injection for simulation error sensor
        private bool _faultInjected = false;
        private Random _random = new Random();

        public double InjectFault()
        {
            _faultInjected = true;

            //3 type fault: high temp, low temp, random spike
            int faultType = _random.Next(3);

            return faultType switch
            {
                0 => 35.0,  //high temperature fault (cooling failure)
                1 => 18.0,  //low temperature fault (sensor drift)
                2 => MinTemp + (_random.NextDouble() * (MaxTemp - MinTemp)), //random normal (hidden fault)
                _ => 0.0
            };
        }

        public void ResetFault()
        {
            _faultInjected = false;
        }

        public bool IsFaultInjected => _faultInjected;
    }
}
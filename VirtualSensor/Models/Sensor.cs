using System;
using System.IO;

namespace Sensors
{
    public class Sensor
    {
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



                //check whether the file exist
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

        //method for temperature data simulation
        public double SimulateData()
        {
            //generate a random temperature between MinTemp and MaxTemp
            double temperature = MinTemp + (random.NextDouble() * (MaxTemp - MinTemp));

            //add small noise (±0.3 degrees)
            double noise = (random.NextDouble() - 0.5) * 0.6;
            temperature += noise;

            return Math.Round(temperature, 1);
        }

        //method for data validation
        public bool ValidateData(double temperature)
        {
            //check whether the temperature is within the specified range
            if (temperature >= MinTemp && temperature <= MaxTemp)
                return true;
            else
                return false;
        }
    }
}
using Xunit;
using Sensors;

namespace Tests
{
    public class UnitTest1
    {
        [Fact]
        public void InitializeSensor_ValidConfig_ReturnsSensor()
        {
            var sensor = new Sensor("DC-Sensor-01", "Data Center A", 22.0, 24.0);

            Assert.NotNull(sensor);
            Assert.Equal("DC-Sensor-01", sensor.Name);
            Assert.Equal(22.0, sensor.MinTemp);
        }

        [Fact]
        public void SimulateData_ReturnsWithinRange()
        {
            //arrange
            var sensor = new Sensor("TestSensor", "TestLocation", 22.0, 24.0);

            //test:does the sensor generate a temperature between 21.5-24.5°C?
            double temperature = sensor.SimulateData();

            // Assert
            Assert.InRange(temperature, 21.5, 24.5); //allow some margin for noise
        }

        [Fact]
        public void ValidateData_ValidTemperature_ReturnsTrue()
        {
            // Arrange
            //test: Does the ValidateData() method return TRUE for a valid temperature?
            var sensor = new Sensor("TestSensor", "TestLocation", 22.0, 24.0);

            // Act & Assert
            Assert.True(sensor.ValidateData(23.0)); //normal tempt
            Assert.True(sensor.ValidateData(22.0)); //Lower limit
            Assert.True(sensor.ValidateData(24.0)); //Upper limit
        }

        [Fact]
        public void ValidateData_InvalidTemperature_ReturnsFalse()
        {
            // Arrange
            var sensor = new Sensor("TestSensor", "TestLocation", 22.0, 24.0);

            // Act & Assert
            Assert.False(sensor.ValidateData(21.9)); //below min
            Assert.False(sensor.ValidateData(24.1)); //above max
            Assert.False(sensor.ValidateData(30.0)); //way above
        }



        //test to make sure data that being simulated have variation
        [Fact]
        public void SimulateData_MultipleCalls_ReturnsDifferentValues()
        {
            //arrange
            var sensor = new Sensor("TestSensor", "TestLocation", 22.0, 24.0);
            var temperatures = new List<double>();

            //act
            for (int i = 0; i < 10; i++)
            {
                temperatures.Add(sensor.SimulateData());
            }

            //assert - should have some variation due to noise
            Assert.True(temperatures.Distinct().Count() > 1);
        }

        //test to verify file log created correctly
        [Fact]
        public void LogData_CreatesLogFileWithEntry()
        {
            // Clean up previous log file
            if (File.Exists("sensor_log.txt"))
                File.Delete("sensor_log.txt");

            //arrange
            var sensor = new Sensor("TestSensor", "TestLocation", 22.0, 24.0);
            var reading = new Reading
            {
                SensorName = "TestSensor",
                Value = 23.0,
                DateTime = new DateTime(2024, 1, 1, 10, 30, 0),
                IsValid = true
            };

            //act
            sensor.LogData(reading);

            //assert
            Assert.True(File.Exists("sensor_log.txt"));

            string logContent = File.ReadAllText("sensor_log.txt");
            // Use more flexible assertion
            Assert.Contains("2024-01-01 10:30:00", logContent);
            Assert.Contains("TestSensor", logContent);
            Assert.Contains("23", logContent); // Check for value without specific format
            Assert.Contains("Valid: True", logContent);
        }

        //test to make sure data reading save in history
        [Fact]
        public void StoreData_AddsReadingToHistory()
        {
            //arrange
            var sensor = new Sensor("TestSensor", "TestLocation", 22.0, 24.0);
            var reading = new Reading
            {
                SensorName = "TestSensor",
                Value = 23.5,
                DateTime = DateTime.Now,
                IsValid = true
            };

            //act
            sensor.StoreData(reading);
            var history = sensor.GetReadingHistory();

            //assert
            Assert.Single(history);
            Assert.Equal("TestSensor", history[0].SensorName);
            Assert.Equal(23.5, history[0].Value);
            Assert.True(history[0].IsValid);
        }


        //test for multiple reading in history
        [Fact]
        public void StoreData_MultipleReadings_MaintainsHistory()
        {
            //arrange
            var sensor = new Sensor("TestSensor", "TestLocation", 22.0, 24.0);

            //act
            for (int i = 0; i < 5; i++)
            {
                var reading = new Reading
                {
                    SensorName = "TestSensor",
                    Value = 23.0 + i,
                    DateTime = DateTime.Now.AddMinutes(i),
                    IsValid = true
                };
                sensor.StoreData(reading);
            }

            var history = sensor.GetReadingHistory();

            //assert
            Assert.Equal(5, history.Count);
            Assert.Equal(23.0, history[0].Value);
            Assert.Equal(27.0, history[4].Value);
        }

        //test for smoothing data(average)
        [Fact]
        public void SmoothData_WithHistory_ReturnsAverage()
        {
            var sensor = new Sensor("Test", "Loc", 22, 24);

            //add history
            sensor.StoreData(new Reading { Value = 23.0 });
            sensor.StoreData(new Reading { Value = 23.5 });
            sensor.StoreData(new Reading { Value = 24.0 });

            double smoothed = sensor.SmoothData();
            Assert.Equal(23.5, smoothed); // (23 + 23.5 + 24) / 3 = 23.5
        }


        //test to detect anomaly
        [Fact]
        public void DetectAnomaly_WithSpike_ReturnsTrue()
        {
            var sensor = new Sensor("Test", "Loc", 22, 24);

            //add normal readings
            sensor.StoreData(new Reading { Value = 23.0 });
            sensor.StoreData(new Reading { Value = 23.0 });
            sensor.StoreData(new Reading { Value = 23.0 });

            bool isAnomaly = sensor.DetectAnomaly(27.0);
            Assert.True(isAnomaly);
        }

        //test edge case for smoothing data without history
        [Fact]
        public void SmoothData_EmptyHistory_ReturnsZero()
        {
            var sensor = new Sensor("Test", "Loc", 22, 24);

            double smoothed = sensor.SmoothData();
            Assert.Equal(0, smoothed);
        }

        
        [Fact]
        public void DetectAnomaly_SingleReading_ReturnsFalse()
        {
            var sensor = new Sensor("Test", "Loc", 22, 24);
            sensor.StoreData(new Reading { Value = 23.0 });

            bool isAnomaly = sensor.DetectAnomaly(23.5);
            Assert.False(isAnomaly);
        }

        [Fact]
        public void ValidateData_EdgeCases_ReturnsCorrect()
        {
            var sensor = new Sensor("Test", "Loc", 22.0, 24.0);

            Assert.True(sensor.ValidateData(22.0));  //exact min
            Assert.True(sensor.ValidateData(24.0));  //exact max
            Assert.False(sensor.ValidateData(21.999)); //just below min
            Assert.False(sensor.ValidateData(24.001)); //just above max
        }

        //test for reset fault state
        [Fact]
        public void ResetFault_ClearsFaultState()
        {
            var sensor = new Sensor("Test", "Loc", 22, 24);

            sensor.InjectFault();
            Assert.True(sensor.IsFaultInjected);

            sensor.ResetFault();
            Assert.False(sensor.IsFaultInjected);
        }

        //test for autos ave after 10 readings
        [Fact]
        public void StoreData_AutoSavesAfter10Readings()
        {
            var sensor = new Sensor("Test", "Loc", 22, 24);

            //add 11 readings
            for (int i = 0; i < 11; i++)
            {
                sensor.StoreData(new Reading { Value = 23.0 + i });
            }

            //should have triggered auto-save
            Assert.True(File.Exists("sensor_history.json"));
        }

        [Fact]
        public void GetReadingHistory_ReturnsCopyOfHistory()
        {
            var sensor = new Sensor("Test", "Loc", 22, 24);
            sensor.StoreData(new Reading { Value = 23.0 });

            var history1 = sensor.GetReadingHistory();
            var history2 = sensor.GetReadingHistory();

            Assert.Equal(history1.Count, history2.Count);
        }

        //test for threshold checking
        [Fact]
        public void CheckThreshold_WithCustomThreshold_Works()
        {
            var sensor = new Sensor("Test", "Loc", 22, 24);

            //test default threshold (MaxTemp + 1.0 = 25.0)
            Assert.False(sensor.CheckThreshold(24.5));  // 24.5 < 25.0 → false
            Assert.False(sensor.CheckThreshold(25.0));  // 25.0 == 25.0 → false (strict >)
            Assert.True(sensor.CheckThreshold(25.1));   // 25.1 > 25.0 → true
        }

        //test fault injection basic  
        [Fact]
        public void InjectFault_ChangesFaultState()
        {
            var sensor = new Sensor("Test", "Loc", 22, 24);

            sensor.InjectFault();
            Assert.True(sensor.IsFaultInjected);
        }

        //test default constructor
        [Fact]
        public void DefaultConstructor_CreatesSensor()
        {
            var sensor = new Sensor();
            Assert.NotNull(sensor.GetReadingHistory());
        }

        //test threshold
        [Fact]
        public void CheckThreshold_WhenHighTemp_ReturnsTrue()
        {
            var sensor = new Sensor("Test", "Loc", 22, 24);
            bool alert = sensor.CheckThreshold(26.0);
            Assert.True(alert);
        }



    }
}
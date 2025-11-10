using Xunit;
using Sensors;

namespace Tests
{
    public class UnitTest1
    {
        [Fact]
        public void InitializeSensor_ValidConfig_ReturnsSensor()
        {
            // Skip file reading untuk sementara, test manual creation
            var sensor = new Sensor("DC-Sensor-01", "Data Center Room A", 22.0, 24.0);

            Assert.NotNull(sensor);
            Assert.Equal("DC-Sensor-01", sensor.Name);
            Assert.Equal("Data Center Room A", sensor.Location);
            Assert.Equal(22.0, sensor.MinTemp);
            Assert.Equal(24.0, sensor.MaxTemp);
        }

        [Fact]
        public void SimulateData_ReturnsWithinRange()
        {
            // Arrange
            var sensor = new Sensor("TestSensor", "TestLocation", 22.0, 24.0);

            // Test: Apakah sensor generate temperatur antara 21.5-24.5°C?
            // Act
            double temperature = sensor.SimulateData();

            // Assert
            Assert.InRange(temperature, 21.5, 24.5); // Allow some margin for noise
        }

        [Fact]
        public void ValidateData_ValidTemperature_ReturnsTrue()
        {
            // Arrange
            // Test: Apakah method ValidateData() mengembalikan TRUE untuk temperatur valid?
            var sensor = new Sensor("TestSensor", "TestLocation", 22.0, 24.0);

            // Act & Assert
            Assert.True(sensor.ValidateData(23.0)); //normal tempt
            Assert.True(sensor.ValidateData(22.0)); //Batas bawah
            Assert.True(sensor.ValidateData(24.0)); //Batas atas
        }

        [Fact]
        public void ValidateData_InvalidTemperature_ReturnsFalse()
        {
            // Arrange
            var sensor = new Sensor("TestSensor", "TestLocation", 22.0, 24.0);

            // Act & Assert
            Assert.False(sensor.ValidateData(21.9)); // Below min
            Assert.False(sensor.ValidateData(24.1)); // Above max
            Assert.False(sensor.ValidateData(30.0)); // Way above
        }

        [Fact]
        public void SimulateData_MultipleCalls_ReturnsDifferentValues()
        {
            // Arrange
            var sensor = new Sensor("TestSensor", "TestLocation", 22.0, 24.0);
            var temperatures = new List<double>();

            // Act
            for (int i = 0; i < 10; i++)
            {
                temperatures.Add(sensor.SimulateData());
            }

            // Assert - Should have some variation due to noise
            Assert.True(temperatures.Distinct().Count() > 1);
        }
    }
}
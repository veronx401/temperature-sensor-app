namespace Sensors
{
    //class to represent data sensor reading
    public class Reading
    {
        public int ReadingId { get; set; }
        public string SensorName { get; set; } = "";
        public double Value { get; set; } 
        public DateTime DateTime { get; set; } //timestamp
        public bool IsValid {  get; set; } //validation status
    }
}
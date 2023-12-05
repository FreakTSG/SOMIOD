namespace SOMIOD_IS.SqlHelpers
{
    public class Notification
    {
        private string EventType { get; set;}
        private string Data { get; set; }


        public Notification() { }
        public Notification(string eventType, string data)
        {
            this.EventType = eventType;
            this.Data = data;
        }
    }
}
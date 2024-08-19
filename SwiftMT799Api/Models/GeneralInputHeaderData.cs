namespace SwiftMT799Api.Models
{
    public class GeneralInputHeaderData
    {
        public int MessageId { get; set; }
        public string IOID { get; set; }
        public string SWIFTMessage { get; set; }
        public string Destination { get; set; }
        public string Priority { get; set; }
        public string Delivery { get; set; }
        public string Obsolescence { get; set; }
        public void AddData(int id, String data)
        {
            this.MessageId = id;
            this.IOID = data.Substring(0, 1);
            this.SWIFTMessage = data.Substring(1, 3);
            this.Destination = data.Substring(4, 12);
            if (data.Length > 16) this.Priority = data.Substring(16, 1);
            if (data.Length > 17) this.Delivery = data.Substring(17, 1);
            if (data.Length > 18) this.Obsolescence = data.Substring(18, 3);

        }
    }


}

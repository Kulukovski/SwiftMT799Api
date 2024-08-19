namespace SwiftMT799Api.Models
{
    public class GeneralHeaderData
    {
        public int MessageId { get; set; }
        public string App { get; set; }
        public string Service { get; set; }
        public string LTAddress { get; set; }
        public string SessionNumber { get; set; }
        public string Sequence { get; set; }
        /// <summary>
        /// takes as input the id key as well as the field1 value to parse it
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        public void AddData(int id, String data)
        {
            this.MessageId = id;
            this.App = data.Substring(0, 1);
            this.Service = data.Substring(1, 2);
            this.LTAddress = data.Substring(3, 12);
            this.SessionNumber = data.Substring(15, 4);
            this.Sequence = data.Substring(19, 6);

        }
    }
}

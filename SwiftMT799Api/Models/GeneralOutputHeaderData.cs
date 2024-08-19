namespace SwiftMT799Api.Models
{
    public class GeneralOutputHeaderData
    {
        public int MessageId { get; set; }
        public string IOID { get; set; }
        public string SWIFTMessage { get; set; }
        public string InputTime { get; set; }
        public string MIR { get; set; }
        public string OutputDate { get; set; }
        public string OutputTime { get; set; }
        public string Priority { get; set; }

        public void AddData(int id, String data)
        {
            this.MessageId = id;
            this.IOID = data.Substring(0, 1);
            this.SWIFTMessage = data.Substring(1, 3);
            this.InputTime = data.Substring(4, 4);
            this.MIR = data.Substring(8, 28);
            this.OutputDate = data.Substring(36, 6);
            this.OutputTime = data.Substring(42, 4);
            if (data.Length > 46) this.Priority = data.Substring(46, 1);

        }
    }
}

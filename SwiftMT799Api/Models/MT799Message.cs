namespace SwiftMT799Api.Models
{
    public class MT799Message
    {

        public int Id { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field4 { get; set; }
        public string Field5 { get; set; }
        public string Field20 { get; set; }
        public string Field21 { get; set; }
        public string Field79 { get; set; }
        public string FieldMAC { get; set; }
        public string FieldCHK { get; set; }

        private static readonly Dictionary<string, Action<MT799Message, string>> FieldMap = new Dictionary<string, Action<MT799Message, string>>
        {
            { "1", (msg, value) => msg.Field1 = value },
            { "2", (msg, value) => msg.Field2 = value },
            { "4", (msg, value) => msg.Field4 = value },
            { "5", (msg, value) => msg.Field5 = value },
            { "20", (msg, value) => msg.Field20 = value },
            { "21", (msg, value) => msg.Field21 = value },
            { "79", (msg, value) => msg.Field79 = value },
            { "MAC", (msg, value) => msg.FieldMAC = value },
            { "CHK", (msg, value) => msg.FieldCHK = value }
        };
        //this part of the model is necessary for inputting into the database when not all fields are present
        public void UpdateFields(Dictionary<string, string> fields)
        {

            foreach (var kvp in FieldMap)
            {
                if (fields.ContainsKey(kvp.Key))
                {
                    kvp.Value(this, fields[kvp.Key]);
                }
            }
        }
    }
}

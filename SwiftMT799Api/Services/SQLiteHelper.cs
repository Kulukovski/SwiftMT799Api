using System.Data.SQLite;
using System.Net;
using SwiftMT799Api.Models;

namespace SwiftMT799Api.Services 
{
    public class SQLiteHelper
    {
        private readonly string _connectionString;

        public SQLiteHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void InitializeDatabase()
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            //Creates the first table for most of the fields in the MT799 message
            string createTableMT799Messages = @"
            CREATE TABLE IF NOT EXISTS MT799Messages (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Field1 TEXT,
                Field2 TEXT,
                Field4 TEXT,
                Field5 TEXT,
                Field20 TEXT,
                Field21 TEXT,
                Field79 TEXT,
                FieldMAC TEXT,
                FieldCHK TEXT
            )";

            using (var command = new SQLiteCommand(createTableMT799Messages, connection))
            {
                command.ExecuteNonQuery();
            }

            //this is for subfields of the first block
            string createHeaderData = @"
            CREATE TABLE IF NOT EXISTS GeneralHeaderData (
                MessageId INTEGER,
                App TEXT,
                Service TEXT,
                LTAddress TEXT,
                SessionNumber TEXT,
                Sequence TEXT,
                FOREIGN KEY (MessageId) REFERENCES MT799Messages(Id) ON DELETE CASCADE
            )";

            using (var command = new SQLiteCommand(createHeaderData, connection))
            {
                command.ExecuteNonQuery();
            }
            // the last 2 are subfields for the 2 different types of block 2
            string createInputHeaderData = @"
            CREATE TABLE IF NOT EXISTS InputHeaderData (
                MessageId INTEGER,
                IOID TEXT,
                SWIFTMessage TEXT,
                Destination TEXT,
                Priority TEXT,
                Delivery TEXT,
                Obsolescence TEXT,
                FOREIGN KEY (MessageId) REFERENCES MT799Messages(Id) ON DELETE CASCADE
            )";

            using (var command = new SQLiteCommand(createInputHeaderData, connection))
            {
                command.ExecuteNonQuery();
            }

            string createOutputHeaderData = @"
            CREATE TABLE IF NOT EXISTS OutputHeaderData (
                MessageId INTEGER,
                IOID TEXT,
                SWIFTMessage TEXT,
                InputTime TEXT,
                MIR TEXT,
                OutputDate TEXT,
                OutputTime TEXT,
                Priority TEXT,
                FOREIGN KEY (MessageId) REFERENCES MT799Messages(Id) ON DELETE CASCADE
            )";

            using (var command = new SQLiteCommand(createOutputHeaderData, connection))
            {
                command.ExecuteNonQuery();
            }

        }
        public void InsertMessage(MT799Message message)
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            string insertQuery = @"
        INSERT INTO MT799Messages (Field1, Field2, Field4, Field5, Field20, Field21, Field79, FieldMAC, FieldCHK) 
        VALUES (@Field1, @Field2, @Field4, @Field5, @Field20, @Field21, @Field79, @FieldMAC, @FieldCHK)"
            ;
            //this inserts the values into the database 
            using var command = new SQLiteCommand(insertQuery, connection);
            command.Parameters.AddWithValue("@Field1", message.Field1);
            command.Parameters.AddWithValue("@Field2", message.Field2);
            command.Parameters.AddWithValue("@Field4", message.Field4);
            command.Parameters.AddWithValue("@Field5", message.Field5);
            command.Parameters.AddWithValue("@Field20", message.Field20);
            command.Parameters.AddWithValue("@Field21", message.Field21);
            command.Parameters.AddWithValue("@Field79", message.Field79);
            command.Parameters.AddWithValue("@FieldMAC", message.FieldMAC);
            command.Parameters.AddWithValue("@FieldCHK", message.FieldCHK);
            command.ExecuteNonQuery();

            long messageId;
            using (var lastIdCommand = new SQLiteCommand("SELECT last_insert_rowid()", connection))
            {
                messageId = (long)lastIdCommand.ExecuteScalar();
            }
            //the other three also are adding the subfields to their respective tables
            //but they create a new object to input the data
            //because some of the fields are not always present
            if (message.Field1 != null)
            {
                GeneralHeaderData data = new GeneralHeaderData();
                data.AddData((int)messageId, message.Field1);
 
                string insertGeneralHeaderData = @"
                    INSERT INTO GeneralHeaderData (MessageId, App, Service, LTAddress, SessionNumber, Sequence) 
                    VALUES (@MessageId, @App, @Service, @LTAddress, @SessionNumber, @Sequence);";

                using var generalCommand = new SQLiteCommand(insertGeneralHeaderData, connection);
                generalCommand.Parameters.AddWithValue("@MessageId", messageId);
                generalCommand.Parameters.AddWithValue("@App", data.App);
                generalCommand.Parameters.AddWithValue("@Service", data.Service);
                generalCommand.Parameters.AddWithValue("@LTAddress", data.LTAddress);
                generalCommand.Parameters.AddWithValue("@SessionNumber", data.SessionNumber);
                generalCommand.Parameters.AddWithValue("@Sequence", data.Sequence);
                generalCommand.ExecuteNonQuery();
            }
            if (message.Field2[0] == 'I' && message.Field2.Length >= 16)
            {
                GeneralInputHeaderData InputData = new GeneralInputHeaderData();
                InputData.AddData((int)messageId, message.Field2);

                string insertInputHeaderData = @"
                    INSERT INTO InputHeaderData (MessageId, IOID, SWIFTMessage, Destination, Priority, Delivery, Obsolescence) 
                    VALUES (@MessageId, @IOID, @SWIFTMessage, @Destination, @Priority, @Delivery, @Obsolescence);";

                using var inputCommand = new SQLiteCommand(insertInputHeaderData, connection);
                inputCommand.Parameters.AddWithValue("@MessageId", InputData.MessageId);
                inputCommand.Parameters.AddWithValue("@IOID", InputData.IOID);
                inputCommand.Parameters.AddWithValue("@SWIFTMessage", InputData.SWIFTMessage);
                inputCommand.Parameters.AddWithValue("@Destination", InputData.Destination);
                inputCommand.Parameters.AddWithValue("@Priority", InputData.Priority);
                inputCommand.Parameters.AddWithValue("@Delivery", InputData.Delivery);
                inputCommand.Parameters.AddWithValue("@Obsolescence", InputData.Obsolescence);
                inputCommand.ExecuteNonQuery();
            }


            if (message.Field2[0] == 'O' && message.Field2.Length >= 46)
            {
                GeneralOutputHeaderData OutputData = new GeneralOutputHeaderData();
                OutputData.AddData((int)messageId, message.Field2);
                string insertOutputHeaderData = @"
                    INSERT INTO OutputHeaderData (MessageId, IOID, SWIFTMessage, InputTime, MIR, OutputDate, OutputTime, Priority) 
                    VALUES (@MessageId, @IOID, @SWIFTMessage, @InputTime, @MIR, @OutputDate, @OutputTime, @Priority);";

                using var outputCommand = new SQLiteCommand(insertOutputHeaderData, connection);
                outputCommand.Parameters.AddWithValue("@MessageId", OutputData.MessageId);
                outputCommand.Parameters.AddWithValue("@IOID", OutputData.IOID);
                outputCommand.Parameters.AddWithValue("@SWIFTMessage", OutputData.SWIFTMessage);
                outputCommand.Parameters.AddWithValue("@InputTime", OutputData.InputTime);
                outputCommand.Parameters.AddWithValue("@MIR", OutputData.MIR);
                outputCommand.Parameters.AddWithValue("@OutputDate", OutputData.OutputDate);
                outputCommand.Parameters.AddWithValue("@OutputTime", OutputData.OutputTime);
                outputCommand.Parameters.AddWithValue("@Priority", OutputData.Priority);
                outputCommand.ExecuteNonQuery();
            }
        }
        
        //the next few functions simply print out the database
        //this would most likely not be part of the API if it was proper commercial use
        //but i added it for testing/convenience sake
        public IEnumerable<MT799Message> GetAllMessages()
        {
            var messages = new List<MT799Message>();

            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            string selectQuery = "SELECT * FROM MT799Messages";

            using var command = new SQLiteCommand(selectQuery, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var message = new MT799Message
                {
                    Id = reader.GetInt32(0),
                    Field1 = !reader.IsDBNull(1) ? reader.GetString(1) : null,
                    Field2 = !reader.IsDBNull(2) ? reader.GetString(2) : null,
                    Field4 = !reader.IsDBNull(3) ? reader.GetString(3) : null,
                    Field5 = !reader.IsDBNull(4) ? reader.GetString(4) : null,
                    Field20 = !reader.IsDBNull(5) ? reader.GetString(5) : null,
                    Field21 = !reader.IsDBNull(6) ? reader.GetString(6) : null,
                    Field79 = !reader.IsDBNull(7) ? reader.GetString(7) : null,
                    FieldMAC = !reader.IsDBNull(8) ? reader.GetString(8) : null,
                    FieldCHK = !reader.IsDBNull(9) ? reader.GetString(9) : null
                };

                messages.Add(message);
            }

            return messages;
        }
        public IEnumerable<GeneralHeaderData> GetAllGeneralHeaderData()
        {
            var headers = new List<GeneralHeaderData>();

            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            string selectQuery = "SELECT * FROM GeneralHeaderData";

            using var command = new SQLiteCommand(selectQuery, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var headerData = new GeneralHeaderData
                {
                    MessageId = reader.GetInt32(0),
                    App = !reader.IsDBNull(1) ? reader.GetString(1) : null,
                    Service = !reader.IsDBNull(2) ? reader.GetString(2) : null,
                    LTAddress = !reader.IsDBNull(3) ? reader.GetString(3) : null,
                    SessionNumber = !reader.IsDBNull(4) ? reader.GetString(4) : null,
                    Sequence = !reader.IsDBNull(5) ? reader.GetString(5) : null
                };

                headers.Add(headerData);
            }

            return headers;
        }
        public IEnumerable<GeneralInputHeaderData> GetAllInputHeaderData()
        {
            var headers = new List<GeneralInputHeaderData>();

            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            string selectQuery = "SELECT * FROM InputHeaderData";

            using var command = new SQLiteCommand(selectQuery, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var headerData = new GeneralInputHeaderData
                {
                    MessageId = reader.GetInt32(0),
                    IOID = !reader.IsDBNull(1) ? reader.GetString(1) : null,
                    SWIFTMessage = !reader.IsDBNull(2) ? reader.GetString(2) : null,
                    Destination = !reader.IsDBNull(3) ? reader.GetString(3) : null,
                    Priority = !reader.IsDBNull(4) ? reader.GetString(4) : null,
                    Delivery = !reader.IsDBNull(5) ? reader.GetString(5) : null,
                    Obsolescence = !reader.IsDBNull(6) ? reader.GetString(6) : null
                };

                headers.Add(headerData);
            }

            return headers;
        }
        public IEnumerable<GeneralOutputHeaderData> GetAllOutputHeaderData()
        {
            var headers = new List<GeneralOutputHeaderData>();

            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            string selectQuery = "SELECT * FROM OutputHeaderData";

            using var command = new SQLiteCommand(selectQuery, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var headerData = new GeneralOutputHeaderData
                {
                    MessageId = reader.GetInt32(0),
                    IOID = !reader.IsDBNull(1) ? reader.GetString(1) : null,
                    SWIFTMessage = !reader.IsDBNull(2) ? reader.GetString(2) : null,
                    InputTime = !reader.IsDBNull(3) ? reader.GetString(3) : null,
                    MIR = !reader.IsDBNull(4) ? reader.GetString(4) : null,
                    OutputDate = !reader.IsDBNull(5) ? reader.GetString(5) : null,
                    OutputTime = !reader.IsDBNull(6) ? reader.GetString(6) : null,
                    Priority = !reader.IsDBNull(7) ? reader.GetString(7) : null,
                };

                headers.Add(headerData);
            }

            return headers;
        }

        //there is also a clear table function incase I would like to use it at some point
        //or add it to the web API(again for testing)
        public void ClearTable()
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            string clearTableQuery = "DELETE FROM MT799Messages; DELETE FROM OutputHeaderData; DELETE FROM InputHeaderData; DELETE FROM GeneralHeaderData";

            using var command = new SQLiteCommand(clearTableQuery, connection);
            command.ExecuteNonQuery();


        }
    }
}

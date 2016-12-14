using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Diagnostics.Contracts;

using Sermo.Data.Contracts;

namespace Sermo.Data.AdoNet
{
    public class AdoNetMessageRepository : IMessageRepository
    {
        public AdoNetMessageRepository(IApplicationSettings applicationSettings, DbProviderFactory databaseFactory)
        {
            //Contract.Requires<ArgumentNullException>(applicationSettings != null);
            //Contract.Requires<ArgumentNullException>(databaseFactory != null);

            this.applicationSettings = applicationSettings;
            this.databaseFactory = databaseFactory; 
        }

        public void AddMessageToRoom(int roomID, string authorName, string text)
        {
            // skip database factory for now since it does not work yet
            using (var connection = new System.Data.SqlClient.SqlConnection())
            //using (var connection = databaseFactory.CreateConnection())
            {
                connection.ConnectionString = applicationSettings.GetValue("SermoConnectionString");
                connection.Open();

                using(var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "dbo.add_message_to_room";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transaction;
                    
                    var roomIdParameter = command.CreateParameter();
                    roomIdParameter.DbType = DbType.Int32;
                    roomIdParameter.ParameterName = "room_id";
                    roomIdParameter.Value = roomID;
                    command.Parameters.Add(roomIdParameter);

                    var authorNameParameter = command.CreateParameter();
                    authorNameParameter.DbType = DbType.String;
                    authorNameParameter.ParameterName = "author_name";
                    authorNameParameter.Value = authorName;
                    command.Parameters.Add(authorNameParameter);

                    var textParameter = command.CreateParameter();
                    textParameter.DbType = DbType.String;
                    textParameter.ParameterName = "text";
                    textParameter.Value = text;
                    command.Parameters.Add(textParameter);

                    command.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<MessageRecord> GetMessagesForRoomID(int roomID)
        {
            var roomMessages = new List<MessageRecord>();

            // skip database factory for now since it does not work yet
            using (var connection = new System.Data.SqlClient.SqlConnection())
            //using (var connection = databaseFactory.CreateConnection())
            {
                connection.ConnectionString = applicationSettings.GetValue("SermoConnectionString");
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "dbo.get_room_messages";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transaction;
                    //added parameter for roomID
                    var parameter = command.CreateParameter();
                    parameter.DbType = DbType.String;
                    parameter.ParameterName = "roomID";
                    parameter.Value = roomID;
                    command.Parameters.Add(parameter);

                    using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        int idIndex = reader.GetOrdinal("id");  //GetOrdinal should be called before loop to optimize performance
                        while (reader.Read())
                        {
                            var id = reader.GetInt64(idIndex);
                            var authorName = reader.GetString(reader.GetOrdinal("author_name"));
                            var text = reader.GetString(reader.GetOrdinal("text"));
                            roomMessages.Add(new MessageRecord(roomID, authorName, text));
                        }
                    }
                }
            }

            return roomMessages;
        }

        private readonly IApplicationSettings applicationSettings;
        private readonly DbProviderFactory databaseFactory;
    }
}

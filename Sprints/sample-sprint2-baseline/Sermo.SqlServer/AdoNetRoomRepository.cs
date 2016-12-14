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
    public class AdoNetRoomRepository : IRoomRepository
    {
        public AdoNetRoomRepository(IApplicationSettings applicationSettings, DbProviderFactory databaseFactory)
        {
            //Contract.Requires<ArgumentNullException>(applicationSettings != null);
            //Contract.Requires<ArgumentNullException>(databaseFactory != null);

            this.applicationSettings = applicationSettings;
            this.databaseFactory = databaseFactory; 
        }

        public void CreateRoom(long id, string name)
        {
            using(var connection = databaseFactory.CreateConnection())
            {
                connection.ConnectionString = applicationSettings.GetValue("SermoConnectionString");
                connection.Open();

                using(var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "dbo.create_room";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transaction;
                    var parameter = command.CreateParameter();
                    parameter.DbType = DbType.String;
                    parameter.ParameterName = "name";
                    parameter.Value = name;
                    command.Parameters.Add(parameter);
                    var parameter2 = command.CreateParameter();
                    parameter2.DbType = DbType.Int64;
                    parameter2.ParameterName = "id";
                    parameter2.Value = id;
                    command.Parameters.Add(parameter2);

                    command.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<RoomRecord> GetAllRooms()
        {
            var allRooms = new List<RoomRecord>();
            // skip database factory for now since it does not work yet
            using (var connection = new System.Data.SqlClient.SqlConnection())
            //using (var connection = databaseFactory.CreateConnection())
            {
                connection.ConnectionString = applicationSettings.GetValue("SermoConnectionString");
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "dbo.get_all_rooms";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = transaction;

                    using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetInt32(reader.GetOrdinal("id"));
                            var name = reader.GetString(reader.GetOrdinal("name"));
                            allRooms.Add(new RoomRecord(id, name));
                        }
                    }
                }
            }

            return allRooms;
        }

        private readonly IApplicationSettings applicationSettings;
        private readonly DbProviderFactory databaseFactory;
    }
}

using Npgsql;
namespace robot_controller_api.Persistence

{
    public class RobotCommandADO : IRobotCommandDataAccess
    {
        private const string CONNECTION_STRING = "Host=localhost;Username=postgres;Password=Geelong2004;Database=sit331";

        public List<RobotCommand> GetRobotCommands()
        {
            var robotCommands = new List<RobotCommand>();
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT * FROM robotcommand", conn);

            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                var id = dr.GetInt32(0);
                var name = dr.GetString(1);
                var description = dr.IsDBNull(2) ? null : dr.GetString(2);
                var isMoveCommand = dr.GetBoolean(3);
                var createdDate = dr.GetDateTime(4);
                var modifiedDate = dr.GetDateTime(5);

                var command = new RobotCommand(id, name, isMoveCommand, createdDate, modifiedDate, description);


                robotCommands.Add(command);
            }

            return robotCommands;
        }


        public void InsertRobotCommand(RobotCommand command)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();
            using var cmd = new NpgsqlCommand("INSERT INTO robotcommand (name, description, ismovecommand, createddate, modifieddate) " +
                "VALUES (@name, @description, @ismovecommand, @createddate, @modifieddate)", conn);

            cmd.Parameters.AddWithValue("name", command.Name);
            cmd.Parameters.AddWithValue("description", (object?)command.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("ismovecommand", command.IsMoveCommand);
            cmd.Parameters.AddWithValue("createddate", command.CreatedDate);
            cmd.Parameters.AddWithValue("modifieddate", command.ModifiedDate);

            cmd.ExecuteNonQuery();
        }

        public RobotCommand UpdateRobotCommand(int id, RobotCommand command)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();
            using var cmd = new NpgsqlCommand("UPDATE robotcommand SET name = @name, description = @description, ismovecommand = @ismovecommand, modifieddate = @modifieddate WHERE id = @id", conn);

            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("name", command.Name);
            cmd.Parameters.AddWithValue("description", (object?)command.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("ismovecommand", command.IsMoveCommand);
            cmd.Parameters.AddWithValue("modifieddate", DateTime.Now);

            cmd.ExecuteNonQuery();

            return new RobotCommand(id, command.Name, command.IsMoveCommand, command.CreatedDate, DateTime.Now, command.Description);
        }


        public void DeleteRobotCommand(int id)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();
            using var cmd = new NpgsqlCommand("DELETE FROM robotcommand WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);
            cmd.ExecuteNonQuery();
        }




    }
}

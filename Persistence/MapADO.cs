using Npgsql;
namespace MapController.Persistence
{
    public class MapADO : IMapCommandDataAccess
    {
        private const string CONNECTION_STRING = "Host=localhost;Username=postgres;Password=Geelong2004;Database=sit331";

        public List<Map> GetMaps()
        {
            var maps = new List<Map>();

            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            using var cmd = new NpgsqlCommand("SELECT * FROM map", conn);
            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                var id = dr.GetInt32(0);
                var name = dr.GetString(1);
                var rows = dr.GetInt32(2);
                var columns = dr.GetInt32(3);
                var description = dr.IsDBNull(4) ? null : dr.GetString(4);
                var createdDate = dr.GetDateTime(6);
                var modifiedDate = dr.GetDateTime(7);

                var map = new Map(id, columns, rows, name, createdDate, modifiedDate, description);
                maps.Add(map);
            }


            return maps;

        }

        public void InsertMaps(Map map)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            using var cmd = new NpgsqlCommand("INSERT INTO map (id, name, rows, columns, description, createddate, modifieddate) " +
                "VALUES (@id, @name, @rows, @columns, description,@createddate, @modifieddate);", conn);

            cmd.Parameters.AddWithValue("id", map.Id);
            cmd.Parameters.AddWithValue("name", map.Name);
            cmd.Parameters.AddWithValue("rows", map.Rows);
            cmd.Parameters.AddWithValue("columns", map.Columns);
            cmd.Parameters.AddWithValue("description", (object?)map.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("createddate", DateTime.Now);
            cmd.Parameters.AddWithValue("modifieddate", DateTime.Now);

            cmd.ExecuteNonQuery();
        }

        public void UpdateMap(int id, Map map)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            using var cmd = new NpgsqlCommand("UPDATE map SET name = @name, rows = @rows, columns = @columns, description = @description, modifieddate = @modifieddate WHERE id = @id;", conn);

            cmd.Parameters.AddWithValue("@name", map.Name);
            cmd.Parameters.AddWithValue("@rows", map.Rows);
            cmd.Parameters.AddWithValue("@columns", map.Columns);
            cmd.Parameters.AddWithValue("description", (object?)map.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@modifieddate", DateTime.Now);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
        }




        public void DeleteMap(int id)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();
            using var cmd = new NpgsqlCommand("DELETE FROM map WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);
            cmd.ExecuteNonQuery();
        }

    }
}

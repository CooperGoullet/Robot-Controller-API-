using MapController;
using MapController.Persistence;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FullImplementaionAPI.Persistence
{
    public class MapRepository : IRepository, IMapCommandDataAccess
    {
        private IRepository _repo => this;

        // Get all maps
        public List<Map> GetMaps()
        {
            var maps = _repo.ExecuteReader<Map>("SELECT * FROM public.map");
            return maps;
        }

        // Insert a new map
        public void InsertMaps(Map map)
        {
            var sqlParams = new NpgsqlParameter[]
            {
                new("name", map.Name),
                new("rows", map.Rows),
                new("columns", map.Columns),
                new("description", (object?)map.Description ?? DBNull.Value),
                new("createddate", DateTime.Now),
                new("modifieddate", DateTime.Now)
            };

            _repo.ExecuteReader<Map>(
                "INSERT INTO map (name, rows, columns, description, createddate, modifieddate) " +
                "VALUES (@name, @rows, @columns, @description, @createddate, @modifieddate) RETURNING *;",
                sqlParams
            );
        }

        // Update an existing map
        // Update an existing map (void return type as per the interface)
        public void UpdateMap(int id, Map map)
        {
            var sqlParams = new NpgsqlParameter[]
            {
                new("id", id),
                new("name", map.Name),
                new("rows", map.Rows),
                new("columns", map.Columns),
                new("description", (object?)map.Description ?? DBNull.Value),
                new("modifieddate", DateTime.Now)
            };

            _repo.ExecuteReader<Map>(
                "UPDATE map SET name=@name, rows=@rows, columns=@columns, description=@description, modifieddate=@modifieddate " +
                "WHERE id=@id;",
                sqlParams
            );
        }


        // Delete a map
        public void DeleteMap(int id)
        {
            var sqlParams = new NpgsqlParameter[]
            {
                new("id", id)
            };

            _repo.ExecuteReader<Map>(
                "DELETE FROM map WHERE id = @id;",
                sqlParams
            );
        }
    }
}

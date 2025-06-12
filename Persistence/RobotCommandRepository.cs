using robot_controller_api.Persistence;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using FullImplementaionAPI.Persistence;
using robot_controller_api;

public class RobotCommandRepository : IRepository, IRobotCommandDataAccess
{
    private IRepository _repo => this;

    public List<RobotCommand> GetRobotCommands()
    {
        var commands = _repo.ExecuteReader<RobotCommand>("SELECT * FROM public.robotcommand");
        return commands;
    }

    public RobotCommand UpdateRobotCommand(RobotCommand updatedCommand)
    {
        var sqlParams = new NpgsqlParameter[]{
            new("id", updatedCommand.Id),
            new("name", updatedCommand.Name),
            new("description", updatedCommand.Description ?? (object)DBNull.Value),
            new("ismovecommand", updatedCommand.IsMoveCommand)
        };
        var result = _repo.ExecuteReader<RobotCommand>(
            "UPDATE robotcommand SET name=@name, description=@description, ismovecommand=@ismovecommand, modifieddate=current_timestamp WHERE id=@id RETURNING *;",
            sqlParams)
            .Single();
        return result;
    }



    public void InsertRobotCommand(RobotCommand command)
    {
        var sqlParams = new NpgsqlParameter[]
        {
        new("name", command.Name),
        new("description", command.Description ?? (object)DBNull.Value),
        new("ismovecommand", command.IsMoveCommand),
        new("createddate", command.CreatedDate),
        new("modifieddate", command.ModifiedDate),
        };

        _repo.ExecuteReader<RobotCommand>(
            "INSERT INTO robotcommand (name, description, ismovecommand, createddate, modifieddate) " +
            "VALUES (@name, @description, @ismovecommand, @createddate, @modifieddate) RETURNING *;",
            sqlParams
        ); // You can call this to execute the query, but ignore the result.
    }


    public RobotCommand UpdateRobotCommand(int id, RobotCommand command)
    {
        var sqlParams = new NpgsqlParameter[]
        {
        new("id", id),
        new("name", command.Name),
        new("description", (object?)command.Description ?? DBNull.Value),
        new("ismovecommand", command.IsMoveCommand),
        new("modifieddate", command.ModifiedDate)
        };

        var result = _repo.ExecuteReader<RobotCommand>(
            "UPDATE robotcommand " +
            "SET name = @name, description = @description, " +
            "ismovecommand = @ismovecommand, modifieddate = @modifieddate " +
            "WHERE id = @id RETURNING *;",
            sqlParams
        ).Single();

        return result;
    }




    public void DeleteRobotCommand(int id)
    {
        var sqlParams = new NpgsqlParameter[]
        {
        new("id", id)
        };

        _repo.ExecuteReader<RobotCommand>(
            "DELETE FROM robotcommand WHERE id = @id;",
            sqlParams
        );
    }



    // Add any other methods needed (e.g., CreateRobotCommand, DeleteRobotCommand, etc.)
}


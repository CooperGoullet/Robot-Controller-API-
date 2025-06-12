using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;
using FullImplementaionAPI; // for UserModel
using FullImplementaionAPI.Persistence;

namespace FullImplementaionAPI.Persistence
{
    public class UserRepository : IRepository, IUserDataAccess
    {
        private IRepository _repo => this;

        public List<UserModel> GetUsers()
        {
            return _repo.ExecuteReader<UserModel>("SELECT * FROM public.user");
        }

        public UserModel GetUserById(int id)
        {
            var sqlParams = new NpgsqlParameter[]
            {
                new("id", id)
            };

            return _repo.ExecuteReader<UserModel>("SELECT * FROM public.user WHERE id = @id", sqlParams).FirstOrDefault();
        }

        public void InsertUser(UserModel user)
        {
            var sqlParams = new NpgsqlParameter[]
            {
                new("email", user.Email),
                new("firstname", user.FirstName),
                new("lastname", user.LastName),
                new("passwordhash", user.PasswordHash),
                new("description", (object?)user.Description ?? DBNull.Value),
                new("role", (object?)user.Role ?? DBNull.Value),
                new("createddate", DateTime.Now),
                new("modifieddate", DateTime.Now)
            };

            _repo.ExecuteReader<UserModel>(
                "INSERT INTO public.user (email, firstname, lastname, passwordhash, description, role, createddate, modifieddate) " +
                "VALUES (@email, @firstname, @lastname, @passwordhash, @description, @role, @createddate, @modifieddate) RETURNING *;",
                sqlParams
            );
        }

        public void UpdateUser(int id, UserModel user)
        {
            var sqlParams = new NpgsqlParameter[]
            {
                new("id", id),
                new("email", user.Email),
                new("firstname", user.FirstName),
                new("lastname", user.LastName),
                new("passwordhash", user.PasswordHash),
                new("description", (object?)user.Description ?? DBNull.Value),
                new("role", (object?)user.Role ?? DBNull.Value),
                new("modifieddate", DateTime.Now)
            };

            _repo.ExecuteReader<UserModel>(
                "UPDATE public.user SET email = @email, firstname = @firstname, lastname = @lastname, " +
                "passwordhash = @passwordhash, description = @description, role = @role, modifieddate = @modifieddate " +
                "WHERE id = @id;",
                sqlParams
            );
        }

        public void DeleteUser(int id)
        {
            var sqlParams = new NpgsqlParameter[]
            {
                new("id", id)
            };

            _repo.ExecuteReader<UserModel>(
                "DELETE FROM public.user WHERE id = @id;",
                sqlParams
            );
        }
    }
}

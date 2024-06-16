using POS.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace POS.Infrastructure.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly string _connectionString;
        private bool _disposed = false;

        public Repository(string connectionString) => _connectionString = connectionString;

        public async Task<IEnumerable<TEntity>> GetAllAsync(
            string? where,
            string? order)
        {
            var entities = new List<TEntity>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var tableName = typeof(TEntity).Name;
                using (var command = new SqlCommand($"{tableName}_GetAll", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Where", where);
                    command.Parameters.AddWithValue("@Order", order);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            entities.Add(MapToEntity(reader));
                        }
                    }
                }
            }
            return entities;
        }

        public async Task<object> GetAllPaginatedAsync(
            string? limit,
            string? offset,
            string? where,
            string? order)
        {
            var entities = new List<TEntity>();
            var pages = 0;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var tableName = typeof(TEntity).Name;
                using (var command = new SqlCommand($"{tableName}_GetPaginated", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Limit", limit);
                    command.Parameters.AddWithValue("@Offset", offset);
                    command.Parameters.AddWithValue("@Where", where);
                    command.Parameters.AddWithValue("@Order", order);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        do
                        {
                            while (await reader.ReadAsync())
                            {
                                if (reader.FieldCount == 1)
                                {
                                    pages = Convert.ToInt32(reader["TotalPages"]);
                                }
                                else
                                {
                                    if (reader.HasRows)
                                    {
                                        entities.Add(MapToEntity(reader));
                                    }
                                }
                            }
                        } while (reader.NextResult());
                    }
                }
            }

            var response = new
            {
                TotalPages = pages,
                Result = entities
            };

            return response;
        }

        public async Task<object> Create(TEntity entity)
        {
            int outputId;

            if (entity == null) throw new ArgumentNullException(nameof(entity));

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var tableName = typeof(TEntity).Name;
                using (var command = new SqlCommand($"{tableName}_Create", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    var outputParameter = AddParameters(command, entity);

                    command.ExecuteNonQuery();
                }
            }

            var response = new
            {
                Message = "Success",
                StatusCode = 200
            };

            return response;
        }

        public async Task Update(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var tableName = typeof(TEntity).Name;
                using (var command = new SqlCommand($"{tableName}_Update", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    AddParameters(command, entity);
                    command.ExecuteNonQuery();
                }
            }
        }

        public async Task Delete(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var tableName = typeof(TEntity).Name;
                using (var command = new SqlCommand($"{tableName}_Delete", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    AddParameter(command, "UpdatedBy", entity);
                    AddParameter(command, GetPrimaryKey(entity), entity);

                    command.ExecuteNonQuery();
                }
            }
        }

        public async Task SaveAsync()
        {
            // In ADO.NET, changes are immediately saved to the database, so this method can be left empty.
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                // Dispose of any resources if necessary
                _disposed = true;
            }
        }

        private TEntity MapToEntity(IDataRecord record)
        {
            var entity = Activator.CreateInstance<TEntity>();
            foreach (var property in typeof(TEntity).GetProperties())
            {
                if (!record.IsDBNull(record.GetOrdinal(property.Name)))
                {
                    property.SetValue(entity, record.GetValue(record.GetOrdinal(property.Name)));
                }
            }
            return entity;
        }

        private string GetColumns()
        {
            var properties = typeof(TEntity).GetProperties()
                                            .Where(p => p.Name != "Id")
                                            .Select(p => p.Name);
            return string.Join(", ", properties);
        }

        private string GetValues()
        {
            var properties = typeof(TEntity).GetProperties()
                                            .Where(p => p.Name != "Id")
                                            .Select(p => "@" + p.Name);
            return string.Join(", ", properties);
        }

        private string GetUpdateSetClause()
        {
            var properties = typeof(TEntity).GetProperties()
                                            .Where(p => p.Name != "Id")
                                            .Select(p => $"{p.Name} = @{p.Name}");
            return string.Join(", ", properties);
        }

        private SqlParameter AddParameters(SqlCommand command, TEntity entity)
        {
            var properties = typeof(TEntity).GetProperties();
            SqlParameter outputParameter = null;
            foreach (var property in properties)
            {
                var value = property.GetValue(entity) ?? DBNull.Value;
                var parameterName = "@" + property.Name;

                if (property.Name == "PS_DOC_LIN")
                {
                    command.Parameters.AddWithValue(parameterName, JsonConvert.SerializeObject(value));
                }
                else
                {
                    command.Parameters.AddWithValue(parameterName, value);
                }
            }

            return outputParameter;
        }

        private void AddParameter(SqlCommand command, string name, TEntity entity)
        {
            var property = typeof(TEntity).GetProperties()
                                             .FirstOrDefault(p => p.Name == name);
            if (property == null)
                throw new ArgumentException($"Entity does not have a property {name}");

            var value = property.GetValue(entity) ?? DBNull.Value;
            var parameterName = "@" + property.Name;

            command.Parameters.AddWithValue(parameterName, value);
        }

        private string GetPrimaryKey(TEntity entity)
        {
            var property = typeof(TEntity).GetProperties()
                                             .FirstOrDefault(p => p.GetCustomAttribute<KeyAttribute>() != null);
            if (property == null)
                throw new ArgumentException("Entity does not have a property with the [Key] attribute");

            return property.Name;
        }
    }
}

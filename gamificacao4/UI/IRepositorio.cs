using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using gamificacao4.DbContext;

namespace gamificacao4.UI;
public class Repositorio<T>
{
    private readonly string _connectionString;

    public Repositorio(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<T> ListarTodos()
    {
        List<T> lista = new List<T>();

        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            connection.Open();

            string tableName = typeof(T).Name;
            string query = $"SELECT * FROM {tableName}";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        T item = CriarInstancia(reader);
                        lista.Add(item);
                    }
                }
            }
        }

        return lista;
    }

    public void Inserir(T item)
    {
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            connection.Open();

            string tableName = typeof(T).Name;
            string columns = ObterColunas(tableName);
            string values = ObterValores(item);
            string query = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    public void Atualizar(T item, string idColumnName, long idValue)
    {
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            connection.Open();

            string tableName = typeof(T).Name;
            string setValues = ObterValoresAtualizacao(item);
            string query = $"UPDATE {tableName} SET {setValues} WHERE {idColumnName} = @idValue";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@idValue", idValue);
                command.ExecuteNonQuery();
            }
        }
    }

    public void Remover(string idColumnName, long idValue)
    {
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            connection.Open();

            string tableName = typeof(T).Name;
            string query = $"DELETE FROM {tableName} WHERE {idColumnName} = @idValue";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@idValue", idValue);
                command.ExecuteNonQuery();
            }
        }
    }

    private T CriarInstancia(MySqlDataReader reader)
    {
        T item = Activator.CreateInstance<T>();

        foreach (var property in typeof(T).GetProperties())
        {
            string columnName = property.Name;
            object value = reader[columnName];
            property.SetValue(item, value);
        }

        return item;
    }

    private string ObterColunas(string tableName)
    {
        string columns = "";

        foreach (var property in typeof(T).GetProperties())
        {
            string columnName = property.Name;
            columns += $"{columnName}, ";
        }

        columns = columns.TrimEnd(',', ' ');
        return columns;
    }

    private string ObterValores(T item)
    {
        string values = "";

        foreach (var property in typeof(T).GetProperties())
        {
            object value = property.GetValue(item);
            values += $"'{value}', ";
        }

        values = values.TrimEnd(',', ' ');
        return values;
    }

    private string ObterValoresAtualizacao(T item)
    {
        string setValues = "";

        foreach (var property in typeof(T).GetProperties())
        {
            string columnName = property.Name;
            object value = property.GetValue(item);
            setValues += $"{columnName} = '{value}', ";
        }

        setValues = setValues.TrimEnd(',', ' ');
        return setValues;
    }
}
// LazyDatabaseMySql.cs
//
// This file is integrated part of Lazy project
// Licensed under "Gnu General Public License Version 3"
//
// Created by Isaac Bezerra Saraiva
// Created on 2020, December 01

using System;
using System.Xml;
using System.Data;
using System.Linq;
using System.Collections.Generic;

using MySql.Data;
using MySql.Data.MySqlClient;

using Lazy;
using Lazy.Database;

namespace Lazy.Database.MySql
{
    public class LazyDatabaseMySql : LazyDatabase
    {
        #region Variables

        private MySqlCommand mySqlCommand;
        private MySqlConnection mySqlConnection;
        private MySqlDataAdapter mySqlDataAdapter;
        private MySqlTransaction mySqlTransaction;

        #endregion Variables

        #region Constructors

        public LazyDatabaseMySql()
        {
        }

        public LazyDatabaseMySql(String connectionString)
            : base(connectionString)
        {
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Open the connection with the database
        /// </summary>
        public override void OpenConnection()
        {
            #region Validations

            if (String.IsNullOrEmpty(this.ConnectionString) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionStringNullOrEmpty);

            if (this.mySqlConnection != null && this.mySqlConnection.State == ConnectionState.Open)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionAlreadyOpen);

            #endregion Validations

            if (this.mySqlConnection == null)
            {
                this.mySqlConnection = new MySqlConnection(this.ConnectionString);
                this.mySqlCommand = new MySqlCommand();
                this.mySqlCommand.Connection = this.mySqlConnection;
                this.mySqlDataAdapter = new MySqlDataAdapter();
            }

            if (this.mySqlConnection.State == ConnectionState.Closed)
                this.mySqlConnection.Open();
        }

        /// <summary>
        /// Close the connection with the database
        /// </summary>
        public override void CloseConnection()
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionAlreadyClosed);

            #endregion Validations

            this.mySqlTransaction = null;

            if (this.mySqlDataAdapter != null)
            {
                this.mySqlDataAdapter.Dispose();
                this.mySqlDataAdapter = null;
            }

            if (this.mySqlCommand != null)
            {
                this.mySqlCommand.Dispose();
                this.mySqlCommand = null;
            }

            if (this.mySqlConnection != null)
            {
                if (this.mySqlConnection.State == ConnectionState.Open)
                    this.mySqlConnection.Close();

                this.mySqlConnection.Dispose();
                this.mySqlConnection = null;
            }
        }

        /// <summary>
        /// Begin a new transaction
        /// </summary>
        public override void BeginTransaction()
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (this.mySqlTransaction != null)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionTransactionAlreadyStarted);

            #endregion Validations

            this.mySqlTransaction = this.mySqlConnection.BeginTransaction();
        }

        /// <summary>
        /// Commit current transaction
        /// </summary>
        public override void CommitTransaction()
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (this.mySqlTransaction == null)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionTransactionNotInitialized);

            #endregion Validations

            this.mySqlTransaction.Commit();
            this.mySqlTransaction = null;
        }

        /// <summary>
        /// Rollback current transaction
        /// </summary>
        public override void RollbackTransaction()
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (this.mySqlTransaction == null)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionTransactionNotInitialized);

            #endregion Validations

            this.mySqlTransaction.Rollback();
            this.mySqlTransaction = null;
        }

        /// <summary>
        /// Execute a sql sentence
        /// </summary>
        /// <param name="sql">The sql sentence to be executed</param>
        /// <param name="values">The sql sentence parameters values</param>
        /// <returns>The number of affected records</returns>
        public override Int32 QueryExecute(String sql, Object[] values)
        {
            return QueryExecute(sql, values, LazyDatabaseQuery.Parameter.Extract(sql));
        }

        /// <summary>
        /// Execute a sql sentence
        /// </summary>
        /// <param name="sql">The sql sentence to be executed</param>
        /// <param name="values">The sql sentence parameters values</param>
        /// <param name="parameters">The sql sentence parameters</param>
        /// <returns>The number of affected records</returns>
        public override Int32 QueryExecute(String sql, Object[] values, String[] parameters)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(sql) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionSqlNullOrEmpty);

            if ((values != null && values.Length >= 0) && parameters == null)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionValuesAndParametersNotMatch);

            if ((parameters != null && parameters.Length >= 0) && values == null)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionValuesAndParametersNotMatch);

            if (values != null && parameters != null && (values.Length != parameters.Length))
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionValuesAndParametersNotMatch);

            #endregion Validations

            this.mySqlCommand.Parameters.Clear();

            if (values != null)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    MySqlDbType dbType = (MySqlDbType)ConvertToDatabaseType(values[i].GetType());
                    MySqlParameter mySqlParameter = new MySqlParameter(parameters[i], dbType);
                    mySqlParameter.Value = values[i];

                    this.mySqlCommand.Parameters.Add(mySqlParameter);
                }
            }

            sql = LazyDatabaseQuery.Replace(sql, new String[] { ":" }, new String[] { "@" });

            this.mySqlCommand.CommandText = sql;
            this.mySqlCommand.CommandType = CommandType.Text;
            this.mySqlCommand.Transaction = this.mySqlTransaction;

            return this.mySqlCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Execute a sql sentence to retrieve a desired value
        /// </summary>
        /// <param name="sql">The sql sentence to be executed</param>
        /// <param name="values">The sql sentence parameters values</param>
        /// <returns>The desired value from the sql sentence</returns>
        public override Object QueryValue(String sql, Object[] values)
        {
            return QueryValue(sql, values, LazyDatabaseQuery.Parameter.Extract(sql));
        }

        /// <summary>
        /// Execute a sql sentence to retrieve a desired value
        /// </summary>
        /// <param name="sql">The sql sentence to be executed</param>
        /// <param name="values">The sql sentence parameters values</param>
        /// <param name="parameters">The sql sentence parameters</param>
        /// <returns>The desired value from the sql sentence</returns>
        public override Object QueryValue(String sql, Object[] values, String[] parameters)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(sql) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionSqlNullOrEmpty);

            if ((values != null && values.Length >= 0) && parameters == null)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionValuesAndParametersNotMatch);

            if ((parameters != null && parameters.Length >= 0) && values == null)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionValuesAndParametersNotMatch);

            if (values != null && parameters != null && (values.Length != parameters.Length))
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionValuesAndParametersNotMatch);

            #endregion Validations

            this.mySqlCommand.Parameters.Clear();

            if (values != null)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    MySqlDbType dbType = (MySqlDbType)ConvertToDatabaseType(values[i].GetType());
                    MySqlParameter mySqlParameter = new MySqlParameter(parameters[i], dbType);
                    mySqlParameter.Value = values[i];

                    this.mySqlCommand.Parameters.Add(mySqlParameter);
                }
            }

            sql = LazyDatabaseQuery.Replace(sql, new String[] { ":" }, new String[] { "@" });

            this.mySqlCommand.CommandText = sql;
            this.mySqlCommand.CommandType = CommandType.Text;
            this.mySqlCommand.Transaction = this.mySqlTransaction;

            DataTable dataTable = new DataTable("Table");
            this.mySqlDataAdapter.SelectCommand = this.mySqlCommand;
            this.mySqlDataAdapter.Fill(dataTable);

            if (dataTable.Rows.Count > 0)
                return dataTable.Rows[0][0];

            return null;
        }

        /// <summary>
        /// Execute a sql sentence to verify records existence
        /// </summary>
        /// <param name="sql">The sql sentence to be executed</param>
        /// <param name="values">The sql sentence parameters values</param>
        /// <returns>The records existence</returns>
        public override Boolean QueryFind(String sql, Object[] values)
        {
            return QueryFind(sql, values, LazyDatabaseQuery.Parameter.Extract(sql));
        }

        /// <summary>
        /// Execute a sql sentence to verify records existence
        /// </summary>
        /// <param name="sql">The sql sentence to be executed</param>
        /// <param name="values">The sql sentence parameters values</param>
        /// <param name="parameters">The sql sentence parameters</param>
        /// <returns>The records existence</returns>
        public override Boolean QueryFind(String sql, Object[] values, String[] parameters)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(sql) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionSqlNullOrEmpty);

            if ((values != null && values.Length >= 0) && parameters == null)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionValuesAndParametersNotMatch);

            if ((parameters != null && parameters.Length >= 0) && values == null)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionValuesAndParametersNotMatch);

            if (values != null && parameters != null && (values.Length != parameters.Length))
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionValuesAndParametersNotMatch);

            #endregion Validations

            this.mySqlCommand.Parameters.Clear();

            if (values != null)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    MySqlDbType dbType = (MySqlDbType)ConvertToDatabaseType(values[i].GetType());
                    MySqlParameter mySqlParameter = new MySqlParameter(parameters[i], dbType);
                    mySqlParameter.Value = values[i];

                    this.mySqlCommand.Parameters.Add(mySqlParameter);
                }
            }

            sql = LazyDatabaseQuery.Replace(sql, new String[] { ":" }, new String[] { "@" });

            this.mySqlCommand.CommandText = sql;
            this.mySqlCommand.CommandType = CommandType.Text;
            this.mySqlCommand.Transaction = this.mySqlTransaction;

            DataTable dataTable = new DataTable("Table");
            this.mySqlDataAdapter.SelectCommand = this.mySqlCommand;
            this.mySqlDataAdapter.Fill(dataTable);

            if (dataTable.Rows.Count > 0)
                return true;

            return false;
        }

        /// <summary>
        /// Execute a sql sentence to retrieve a single record
        /// </summary>
        /// <param name="sql">The sql sentence to be executed</param>
        /// <param name="tableName">The desired table name</param>
        /// <param name="values">The sql sentence parameters values</param>
        /// <returns>The record found</returns>
        public override DataRow QueryRecord(String sql, String tableName, Object[] values)
        {
            return QueryRecord(sql, tableName, values, LazyDatabaseQuery.Parameter.Extract(sql));
        }

        /// <summary>
        /// Execute a sql sentence to retrieve a single record
        /// </summary>
        /// <param name="sql">The sql sentence to be executed</param>
        /// <param name="tableName">The desired table name</param>
        /// <param name="values">The sql sentence parameters values</param>
        /// <param name="parameters">The sql sentence parameters</param>
        /// <returns>The record found</returns>
        public override DataRow QueryRecord(String sql, String tableName, Object[] values, String[] parameters)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(sql) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionSqlNullOrEmpty);

            if (String.IsNullOrEmpty(tableName) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionTableNameNullOrEmpty);

            if ((values != null && values.Length >= 0) && parameters == null)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionValuesAndParametersNotMatch);

            if ((parameters != null && parameters.Length >= 0) && values == null)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionValuesAndParametersNotMatch);

            if (values != null && parameters != null && (values.Length != parameters.Length))
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionValuesAndParametersNotMatch);

            #endregion Validations

            this.mySqlCommand.Parameters.Clear();

            if (values != null)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    MySqlDbType dbType = (MySqlDbType)ConvertToDatabaseType(values[i].GetType());
                    MySqlParameter mySqlParameter = new MySqlParameter(parameters[i], dbType);
                    mySqlParameter.Value = values[i];

                    this.mySqlCommand.Parameters.Add(mySqlParameter);
                }
            }

            sql = LazyDatabaseQuery.Replace(sql, new String[] { ":" }, new String[] { "@" });

            this.mySqlCommand.CommandText = sql;
            this.mySqlCommand.CommandType = CommandType.Text;
            this.mySqlCommand.Transaction = this.mySqlTransaction;

            DataTable dataTable = new DataTable(tableName);
            this.mySqlDataAdapter.SelectCommand = this.mySqlCommand;
            this.mySqlDataAdapter.Fill(dataTable);

            if (dataTable.Rows.Count > 0)
                return dataTable.Rows[0];

            return null;
        }

        /// <summary>
        /// Execute a sql sentence to retrieve many records
        /// </summary>
        /// <param name="sql">The sql sentence to be executed</param>
        /// <param name="tableName">The desired table name</param>
        /// <param name="values">The sql sentence parameters values</param>
        /// <returns>The generated table from the sql sentence</returns>
        public override DataTable QueryTable(String sql, String tableName, Object[] values)
        {
            return QueryTable(sql, tableName, values, LazyDatabaseQuery.Parameter.Extract(sql));
        }

        /// <summary>
        /// Execute a sql sentence to retrieve many records
        /// </summary>
        /// <param name="sql">The sql sentence to be executed</param>
        /// <param name="tableName">The desired table name</param>
        /// <param name="values">The sql sentence parameters values</param>
        /// <param name="parameters">The sql sentence parameters</param>
        /// <returns>The generated table from the sql sentence</returns>
        public override DataTable QueryTable(String sql, String tableName, Object[] values, String[] parameters)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(sql) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionSqlNullOrEmpty);

            if (String.IsNullOrEmpty(tableName) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionTableNameNullOrEmpty);

            if ((values != null && values.Length >= 0) && parameters == null)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionValuesAndParametersNotMatch);

            if ((parameters != null && parameters.Length >= 0) && values == null)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionValuesAndParametersNotMatch);

            if (values != null && parameters != null && (values.Length != parameters.Length))
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionValuesAndParametersNotMatch);

            #endregion Validations

            this.mySqlCommand.Parameters.Clear();

            if (values != null)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    MySqlDbType dbType = (MySqlDbType)ConvertToDatabaseType(values[i].GetType());
                    MySqlParameter mySqlParameter = new MySqlParameter(parameters[i], dbType);
                    mySqlParameter.Value = values[i];

                    this.mySqlCommand.Parameters.Add(mySqlParameter);
                }
            }

            sql = LazyDatabaseQuery.Replace(sql, new String[] { ":" }, new String[] { "@" });

            this.mySqlCommand.CommandText = sql;
            this.mySqlCommand.CommandType = CommandType.Text;
            this.mySqlCommand.Transaction = this.mySqlTransaction;

            DataTable dataTable = new DataTable(tableName);
            this.mySqlDataAdapter.SelectCommand = this.mySqlCommand;
            this.mySqlDataAdapter.Fill(dataTable);

            return dataTable;
        }

        /// <summary>
        /// Execute a sql stored procedure
        /// </summary>
        /// <param name="procedureName">The stored procedure name</param>
        /// <param name="tableName">The desired table name</param>
        /// <param name="values">The stored procedure parameters values</param>
        /// <param name="parameters">The stored procedure parameters</param>
        /// <returns>The generated table from the stored procedure</returns>
        public override DataTable QueryProcedure(String procedureName, String tableName, Object[] values, String[] parameters)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(procedureName) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionProcedureNameNullOrEmpty);

            if ((values != null && values.Length >= 0) && parameters == null)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionValuesAndParametersNotMatch);

            if ((parameters != null && parameters.Length >= 0) && values == null)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionValuesAndParametersNotMatch);

            if (values != null && parameters != null && (values.Length != parameters.Length))
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionValuesAndParametersNotMatch);

            #endregion Validations

            this.mySqlCommand.Parameters.Clear();

            if (values != null)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    MySqlDbType dbType = (MySqlDbType)ConvertToDatabaseType(values[i].GetType());
                    MySqlParameter mySqlParameter = new MySqlParameter(parameters[i], dbType);
                    mySqlParameter.Value = values[i];

                    this.mySqlCommand.Parameters.Add(mySqlParameter);
                }
            }

            this.mySqlCommand.CommandText = procedureName;
            this.mySqlCommand.CommandType = CommandType.StoredProcedure;
            this.mySqlCommand.Transaction = this.mySqlTransaction;

            DataTable dataTable = new DataTable(tableName);
            this.mySqlDataAdapter.SelectCommand = this.mySqlCommand;
            this.mySqlDataAdapter.Fill(dataTable);

            return dataTable;
        }

        /// <summary>
        /// Execute a sql select sentence
        /// </summary>
        /// <param name="tableName">The table name to select the record</param>
        /// <param name="dataRow">The datarow witch contains the key values</param>
        /// <returns>The datatable with selected records</returns>
        public override DataTable Select(String tableName, DataRow dataRow)
        {
            return Select(tableName, dataRow, DataRowState.Unchanged);
        }

        /// <summary>
        /// Execute a sql select sentence
        /// </summary>
        /// <param name="tableName">The table name to select the record</param>
        /// <param name="dataRow">The datarow witch contains the key values</param>
        /// <param name="dataRowState">The datarow state to be considered</param>
        /// <returns>The datatable with selected records</returns>
        public override DataTable Select(String tableName, DataRow dataRow, DataRowState dataRowState)
        {
            return Select(tableName, dataRow, dataRowState, new String[] { "*" });
        }

        /// <summary>
        /// Execute a sql select sentence
        /// </summary>
        /// <param name="tableName">The table name to select the record</param>
        /// <param name="dataRow">The datarow witch contains the key values</param>
        /// <param name="returnFields">The fields to be returned by the select sentence</param>
        /// <returns>The datatable with selected records</returns>
        public override DataTable Select(String tableName, DataRow dataRow, String[] returnFields)
        {
            return Select(tableName, dataRow, DataRowState.Unchanged, returnFields);
        }

        /// <summary>
        /// Execute a sql select sentence
        /// </summary>
        /// <param name="tableName">The table name to select the record</param>
        /// <param name="dataRow">The datarow witch contains the key values</param>
        /// <param name="dataRowState">The datarow state to be considered</param>
        /// <param name="returnFields">The fields to be returned by the select sentence</param>
        /// <returns>The datatable with selected records</returns>
        public override DataTable Select(String tableName, DataRow dataRow, DataRowState dataRowState, String[] returnFields)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(tableName) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionTableNameNullOrEmpty);

            if (dataRow == null || dataRow.Table.Columns.Count == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionDataRowNullOrColumnZeroLenght);

            if (dataRow.Table.PrimaryKey == null || dataRow.Table.PrimaryKey.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionDataTablePrimaryKeyNullOrZeroLenght);

            if (returnFields == null || returnFields.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionReturnFieldsNullOrZeroLenght);

            #endregion Validations

            String[] keyFields = null;
            Object[] keyValues = null;

            if (dataRowState.HasFlag(dataRow.RowState) == true)
            {
                Int32 columnIndex = 0;

                keyFields = new String[dataRow.Table.PrimaryKey.Length];
                keyValues = new Object[dataRow.Table.PrimaryKey.Length];
                foreach (DataColumn dataColumn in dataRow.Table.PrimaryKey)
                {
                    keyFields[columnIndex] = dataColumn.ColumnName;
                    keyValues[columnIndex] = (dataRow.RowState == DataRowState.Modified || dataRow.RowState == DataRowState.Deleted) ? dataRow[dataColumn, DataRowVersion.Original] : dataRow[dataColumn];
                    columnIndex++;
                }
            }

            return Select(tableName, keyFields, keyValues, returnFields);
        }

        /// <summary>
        /// Execute a sql select sentence
        /// </summary>
        /// <param name="tableName">The table name to select the record</param>
        /// <param name="keyFields">The table key fields</param>
        /// <param name="keyValues">The respective key fields values</param>
        /// <returns>The datatable with selected records</returns>
        public override DataTable Select(String tableName, String[] keyFields, Object[] keyValues)
        {
            return Select(tableName, keyFields, keyValues, new String[] { "*" });
        }

        /// <summary>
        /// Execute a sql select sentence
        /// </summary>
        /// <param name="tableName">The table name to select the record</param>
        /// <param name="keyFields">The table key fields</param>
        /// <param name="keyValues">The respective key fields values</param>
        /// <param name="returnFields">The fields to be returned by the select sentence</param>
        /// <returns>The datatable with selected records</returns>
        public override DataTable Select(String tableName, String[] keyFields, Object[] keyValues, String[] returnFields)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(tableName) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionTableNameNullOrEmpty);

            if (keyFields == null || keyFields.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyFieldsNullOrZeroLenght);

            if (keyValues == null || keyValues.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyValuesNullOrZeroLenght);

            if (keyFields.Length != keyValues.Length)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyFieldsAndKeyValuesNotMatch);

            if (returnFields == null || returnFields.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionReturnFieldsNullOrZeroLenght);

            #endregion Validations

            this.mySqlCommand.Parameters.Clear();

            String whereKeyFieldString = String.Empty;

            foreach (String keyField in keyFields)
                whereKeyFieldString += keyField + " = @" + keyField + " and ";
            whereKeyFieldString = whereKeyFieldString.Remove(whereKeyFieldString.Length - 5, 5); // Remove last " and "

            for (int i = 0; i < keyValues.Length; i++)
            {
                MySqlDbType dbType = (MySqlDbType)ConvertToDatabaseType(keyValues[i].GetType());
                MySqlParameter mySqlParameter = new MySqlParameter(keyFields[i], dbType);
                mySqlParameter.Value = keyValues[i];

                this.mySqlCommand.Parameters.Add(mySqlParameter);
            }

            String returnFieldString = String.Empty;

            foreach (String returnField in returnFields)
                returnFieldString += returnField + ",";
            returnFieldString = returnFieldString.Remove(returnFieldString.Length - 1, 1); // Remove last " , "

            String sql = "select " + returnFieldString + " from " + tableName + " where " + whereKeyFieldString;

            this.mySqlCommand.CommandText = sql;
            this.mySqlCommand.CommandType = CommandType.Text;
            this.mySqlCommand.Transaction = this.mySqlTransaction;

            DataTable dataTable = new DataTable(tableName);
            this.mySqlDataAdapter.SelectCommand = this.mySqlCommand;
            this.mySqlDataAdapter.Fill(dataTable);

            return dataTable;
        }

        /// <summary>
        /// Execute a sql select sentence
        /// </summary>
        /// <param name="tableName">The table name to select the record</param>
        /// <param name="dataTable">The datatable containg the records to be selected</param>
        /// <returns>The datatable with selected records</returns>
        public override DataTable SelectAll(String tableName, DataTable dataTable)
        {
            return SelectAll(tableName, dataTable, DataRowState.Unchanged);
        }

        /// <summary>
        /// Execute a sql select sentence
        /// </summary>
        /// <param name="tableName">The table name to select the record</param>
        /// <param name="dataTable">The datatable containg the records to be selected</param>
        /// <param name="dataRowState">The datarow state on datatable to be considered</param>
        /// <returns>The datatable with selected records</returns>
        public override DataTable SelectAll(String tableName, DataTable dataTable, DataRowState dataRowState)
        {
            return SelectAll(tableName, dataTable, dataRowState, new String[] { "*" });
        }

        /// <summary>
        /// Execute a sql select sentence
        /// </summary>
        /// <param name="tableName">The table name to select the record</param>
        /// <param name="dataTable">The datatable containg the records to be selected</param>
        /// <param name="dataRowState">The datarow state on datatable to be considered</param>
        /// <param name="returnFields">The fields to be returned by the select sentence</param>
        /// <returns>The datatable with selected records</returns>
        public override DataTable SelectAll(String tableName, DataTable dataTable, String[] returnFields)
        {
            return SelectAll(tableName, dataTable, DataRowState.Unchanged, returnFields);
        }

        /// <summary>
        /// Execute a sql select sentence
        /// </summary>
        /// <param name="tableName">The table name to select the record</param>
        /// <param name="dataTable">The datatable containg the records to be selected</param>
        /// <param name="dataRowState">The datarow state on datatable to be considered</param>
        /// <param name="returnFields">The fields to be returned by the select sentence</param>
        /// <returns>The datatable with selected records</returns>
        public override DataTable SelectAll(String tableName, DataTable dataTable, DataRowState dataRowState, String[] returnFields)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(tableName) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionTableNameNullOrEmpty);

            if (dataTable == null || dataTable.Rows.Count == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionDataTableNullOrRowsZeroLenght);

            if (dataTable.Columns.Count == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionDataTableRowsColumnsZeroLenght);

            if (dataTable.PrimaryKey == null || dataTable.PrimaryKey.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionDataTablePrimaryKeyNullOrZeroLenght);

            if (returnFields == null || returnFields.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionReturnFieldsNullOrZeroLenght);

            #endregion Validations

            Int32 columnIndex = 0;

            String[] keyFields = new String[dataTable.PrimaryKey.Length];
            foreach (DataColumn dataColumn in dataTable.PrimaryKey)
            {
                keyFields[columnIndex] = dataColumn.ColumnName;
                columnIndex++;
            }

            List<Object[]> keyValuesList = new List<Object[]>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                if (dataRowState.HasFlag(dataRow.RowState) == false)
                    continue;

                columnIndex = 0;
                keyValuesList.Add(new Object[dataTable.PrimaryKey.Length]);
                foreach (DataColumn dataColumn in dataTable.PrimaryKey)
                {
                    keyValuesList[keyValuesList.Count - 1][columnIndex] = (dataRow.RowState == DataRowState.Modified || dataRow.RowState == DataRowState.Deleted) ? dataRow[dataColumn, DataRowVersion.Original] : dataRow[dataColumn];
                    columnIndex++;
                }
            }

            return SelectAll(tableName, keyFields, keyValuesList, returnFields);
        }

        /// <summary>
        /// Execute a sql select sentence
        /// </summary>
        /// <param name="tableName">The table name to select the record</param>
        /// <param name="keyFields">The table key fields</param>
        /// <param name="keyValuesList">The list of respective key fields values</param>
        /// <returns>The datatable with selected records</returns>
        public override DataTable SelectAll(String tableName, String[] keyFields, List<Object[]> keyValuesList)
        {
            return SelectAll(tableName, keyFields, keyValuesList, new String[] { "*" });
        }

        /// <summary>
        /// Execute a sql select sentence
        /// </summary>
        /// <param name="tableName">The table name to select the record</param>
        /// <param name="keyFields">The table key fields</param>
        /// <param name="keyValuesList">The list of respective key fields values</param>
        /// <param name="returnFields">The fields to be returned by the select sentence</param>
        /// <returns>The datatable with selected records</returns>
        public override DataTable SelectAll(String tableName, String[] keyFields, List<Object[]> keyValuesList, String[] returnFields)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(tableName) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionTableNameNullOrEmpty);

            if (keyFields == null || keyFields.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyFieldsNullOrZeroLenght);

            if (keyValuesList == null || keyValuesList.Count == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyValuesListNullOrZeroLenght);

            if (returnFields == null || returnFields.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionReturnFieldsNullOrZeroLenght);

            #endregion Validations

            this.mySqlCommand.Parameters.Clear();

            String keyFieldString = String.Empty;
            String inKeyValueString = String.Empty;

            foreach (String keyField in keyFields)
                keyFieldString += keyField + ",";
            keyFieldString = keyFieldString.Remove(keyFieldString.Length - 1, 1); // Remove last ","

            for (int listIndex = 0; listIndex < keyValuesList.Count; listIndex++)
            {
                #region Validations

                if (keyFields.Length != keyValuesList[listIndex].Length)
                    throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyFieldsAndKeyValuesNotMatch);

                #endregion Validations

                // Generate in clausule
                inKeyValueString += "(";
                foreach (Object value in keyValuesList[listIndex])
                    inKeyValueString += ConvertToDatabaseStringFormat(value) + ",";
                inKeyValueString = inKeyValueString.Remove(inKeyValueString.Length - 1, 1); // Remove last ","
                inKeyValueString += "),";
            }
            inKeyValueString = inKeyValueString.Remove(inKeyValueString.Length - 1, 1); // Remove last "),"

            String returnFieldString = String.Empty;

            foreach (String returnField in returnFields)
                returnFieldString += returnField + ",";
            returnFieldString = returnFieldString.Remove(returnFieldString.Length - 1, 1); // Remove last " , "

            String sql = "select " + returnFieldString + " from " + tableName + " where (" + keyFieldString + ") in (" + inKeyValueString + ")";

            this.mySqlCommand.CommandText = sql;
            this.mySqlCommand.CommandType = CommandType.Text;
            this.mySqlCommand.Transaction = this.mySqlTransaction;

            DataTable dataTable = new DataTable(tableName);
            this.mySqlDataAdapter.SelectCommand = this.mySqlCommand;
            this.mySqlDataAdapter.Fill(dataTable);

            return dataTable;
        }

        /// <summary>
        /// Execute a sql insert sentence
        /// </summary>
        /// <param name="tableName">The table name to insert the record</param>
        /// <param name="dataRow">The datarow to be inserted</param>
        /// <returns>The number of affected records</returns>
        public override Int32 Insert(String tableName, DataRow dataRow)
        {
            return Insert(tableName, dataRow, DataRowState.Added);
        }

        /// <summary>
        /// Execute a sql insert sentence
        /// </summary>
        /// <param name="tableName">The table name to insert the record</param>
        /// <param name="dataRow">The datarow to be inserted</param>
        /// <param name="dataRowState">The datarow state to be considered</param>
        /// <returns>The number of affected records</returns>
        public override Int32 Insert(String tableName, DataRow dataRow, DataRowState dataRowState)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(tableName) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionTableNameNullOrEmpty);

            if (dataRow == null || dataRow.Table.Columns.Count == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionDataRowNullOrColumnZeroLenght);

            #endregion Validations

            String[] fields = null;
            Object[] values = null;

            if (dataRowState.HasFlag(dataRow.RowState) == true)
            {
                Int32 columnIndex = 0;

                fields = new String[dataRow.Table.Columns.Count];
                values = new Object[dataRow.Table.Columns.Count];
                foreach (DataColumn dataColumn in dataRow.Table.Columns)
                {
                    fields[columnIndex] = dataColumn.ColumnName;
                    values[columnIndex] = dataRow[dataColumn];
                    columnIndex++;
                }
            }

            return Insert(tableName, fields, values);
        }

        /// <summary>
        /// Execute a sql insert sentence
        /// </summary>
        /// <param name="tableName">The table name to insert the record</param>
        /// <param name="fields">The table fields to be included on the insert sentence</param>
        /// <param name="values">The respective fields values to be inserted on the table</param>
        /// <returns>The number of affected records</returns>
        public override Int32 Insert(String tableName, String[] fields, Object[] values)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(tableName) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionTableNameNullOrEmpty);

            if (fields == null || fields.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionFieldsNullOrZeroLenght);

            if (values == null || values.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionValuesNullOrZeroLenght);

            if (fields.Length != values.Length)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionFieldsAndValuesNotMatch);

            #endregion Validations

            this.mySqlCommand.Parameters.Clear();

            String fieldString = String.Empty;
            String parameterString = String.Empty;

            foreach (String field in fields)
                fieldString += "" + field + ",";
            fieldString = fieldString.Remove(fieldString.Length - 1, 1);

            for (int i = 0; i < values.Length; i++)
            {
                MySqlDbType dbType = (MySqlDbType)ConvertToDatabaseType(values[i].GetType());
                MySqlParameter mySqlParameter = new MySqlParameter(fields[i], dbType);
                mySqlParameter.Value = values[i];

                this.mySqlCommand.Parameters.Add(mySqlParameter);

                parameterString += "@" + fields[i] + ",";
            }
            parameterString = parameterString.Remove(parameterString.Length - 1, 1);

            String sql = "insert into " + tableName + " (" + fieldString + ") values (" + parameterString + ")";

            this.mySqlCommand.CommandText = sql;
            this.mySqlCommand.CommandType = CommandType.Text;
            this.mySqlCommand.Transaction = this.mySqlTransaction;

            return this.mySqlCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Execute a sql insert sentence for many records
        /// </summary>
        /// <param name="tableName">The table name to insert the records</param>
        /// <param name="dataTable">The datatable containg the records to be inserted</param>
        /// <returns>The number of affected records</returns>
        public override Int32 InsertAll(String tableName, DataTable dataTable)
        {
            return InsertAll(tableName, dataTable, DataRowState.Added);
        }

        /// <summary>
        /// Execute a sql insert sentence for many records
        /// </summary>
        /// <param name="tableName">The table name to insert the records</param>
        /// <param name="dataTable">The datatable containg the records to be inserted</param>
        /// <param name="dataRowState">The datarow state on datatable to be considered</param>
        /// <returns>The number of affected records</returns>
        public override Int32 InsertAll(String tableName, DataTable dataTable, DataRowState dataRowState)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(tableName) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionTableNameNullOrEmpty);

            if (dataTable == null || dataTable.Rows.Count == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionDataTableNullOrRowsZeroLenght);

            if (dataTable.Columns.Count == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionDataTableRowsColumnsZeroLenght);

            #endregion Validations

            Int32 columnIndex = 0;

            String[] fields = new String[dataTable.Columns.Count];
            foreach (DataColumn dataColumn in dataTable.Columns)
            {
                fields[columnIndex] = dataColumn.ColumnName;
                columnIndex++;
            }

            List<Object[]> valuesList = new List<Object[]>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                if (dataRowState.HasFlag(dataRow.RowState) == false)
                    continue;

                columnIndex = 0;
                valuesList.Add(new Object[dataTable.Columns.Count]);
                foreach (DataColumn dataColumn in dataTable.Columns)
                {
                    valuesList[valuesList.Count - 1][columnIndex] = dataRow[dataColumn];
                    columnIndex++;
                }
            }

            return InsertAll(tableName, fields, valuesList);
        }

        /// <summary>
        /// Execute a sql insert sentence for many records
        /// </summary>
        /// <param name="tableName">The table name to insert the records</param>
        /// <param name="fields">The table fields to be included on the insert sentence</param>
        /// <param name="valuesList">The list of respective fields values to be inserted on the table</param>
        /// <returns>The number of affected records</returns>
        public override Int32 InsertAll(String tableName, String[] fields, List<Object[]> valuesList)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(tableName) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionTableNameNullOrEmpty);

            if (fields == null || fields.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionFieldsNullOrZeroLenght);

            if (valuesList == null || valuesList.Count == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionValuesListNullOrZeroLenght);

            #endregion Validations

            this.mySqlCommand.Parameters.Clear();

            String fieldString = String.Empty;
            String valuesListString = String.Empty;

            foreach (String field in fields)
                fieldString += "" + field + ",";
            fieldString = fieldString.Remove(fieldString.Length - 1, 1);

            foreach (Object[] values in valuesList)
            {
                #region Validations

                if (fields.Length != values.Length)
                    throw new Exception(Properties.Resources.LazyDatabaseExceptionFieldsAndValuesNotMatch);

                #endregion Validations

                valuesListString += "(";
                for (int i = 0; i < values.Length; i++)
                    valuesListString += ConvertToDatabaseStringFormat(values[i]) + ",";
                valuesListString = valuesListString.Remove(valuesListString.Length - 1, 1);
                valuesListString += "),";
            }
            valuesListString = valuesListString.Remove(valuesListString.Length - 1, 1);

            String sql = "insert into " + tableName + " (" + fieldString + ") values " + valuesListString;

            this.mySqlCommand.CommandText = sql;
            this.mySqlCommand.CommandType = CommandType.Text;
            this.mySqlCommand.Transaction = this.mySqlTransaction;

            return this.mySqlCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Execute a sql update or insert sentence depending on record existence
        /// </summary>
        /// <param name="tableName">The table name to update or insert the record</param>
        /// <param name="dataRow">The datarow to be updated or inserted</param>
        /// <returns>The number of affected records</returns>
        public override Int32 Indate(String tableName, DataRow dataRow)
        {
            return Indate(tableName, dataRow, DataRowState.Added);
        }

        /// <summary>
        /// Execute a sql update or insert sentence depending on record existence
        /// </summary>
        /// <param name="tableName">The table name to update or insert the record</param>
        /// <param name="dataRow">The datarow to be updated or inserted</param>
        /// <param name="dataRowState">The datarow state to be considered</param>
        /// <returns>The number of affected records</returns>
        public override Int32 Indate(String tableName, DataRow dataRow, DataRowState dataRowState)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(tableName) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionTableNameNullOrEmpty);

            if (dataRow == null || dataRow.Table.Columns.Count == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionDataRowNullOrColumnZeroLenght);

            if (dataRow.Table.PrimaryKey == null || dataRow.Table.PrimaryKey.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionDataTablePrimaryKeyNullOrZeroLenght);

            #endregion Validations

            String[] nonKeyFields = null;
            Object[] nonKeyValues = null;
            String[] keyFields = null;
            Object[] keyValues = null;

            if (dataRowState.HasFlag(dataRow.RowState) == true)
            {
                Int32 columnIndex = 0;

                nonKeyFields = new String[dataRow.Table.Columns.Count - dataRow.Table.PrimaryKey.Length];
                nonKeyValues = new Object[dataRow.Table.Columns.Count - dataRow.Table.PrimaryKey.Length];
                foreach (DataColumn dataColumn in dataRow.Table.Columns)
                {
                    Int32 pkIndex = 0;
                    for (pkIndex = 0; pkIndex < dataRow.Table.PrimaryKey.Length; pkIndex++)
                    {
                        if (dataRow.Table.PrimaryKey[pkIndex].ColumnName == dataColumn.ColumnName)
                            break;
                    }

                    if (pkIndex == dataRow.Table.PrimaryKey.Length)
                    {
                        nonKeyFields[columnIndex] = dataColumn.ColumnName;
                        nonKeyValues[columnIndex] = dataRow[dataColumn];
                        columnIndex++;
                    }
                }

                columnIndex = 0;
                keyFields = new String[dataRow.Table.PrimaryKey.Length];
                keyValues = new Object[dataRow.Table.PrimaryKey.Length];
                foreach (DataColumn dataColumn in dataRow.Table.PrimaryKey)
                {
                    keyFields[columnIndex] = dataColumn.ColumnName;
                    keyValues[columnIndex] = dataRow.RowState == DataRowState.Modified ? dataRow[dataColumn, DataRowVersion.Original] : dataRow[dataColumn];
                    columnIndex++;
                }
            }

            return Indate(tableName, nonKeyFields, nonKeyValues, keyFields, keyValues);
        }

        /// <summary>
        /// Execute a sql update or insert sentence depending on record existence
        /// </summary>
        /// <param name="tableName">The table name to update or insert the record</param>
        /// <param name="nonKeyFields">The table non key fields to be included on the update or insert sentence</param>
        /// <param name="nonKeyValues">The respective non key fields values to be updated or inserted on the table</param>
        /// <param name="keyFields">The table key fields</param>
        /// <param name="keyValues">The respective key fields values</param>
        /// <returns>The number of affected records</returns>
        public override Int32 Indate(String tableName, String[] nonKeyFields, Object[] nonKeyValues, String[] keyFields, Object[] keyValues)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(tableName) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionTableNameNullOrEmpty);

            if (nonKeyFields == null || nonKeyFields.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionNonKeyFieldsNullOrZeroLenght);

            if (nonKeyValues == null || nonKeyValues.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionNonKeyValuesNullOrZeroLenght);

            if (nonKeyFields.Length != nonKeyValues.Length)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionNonKeyFieldsAndNonKeyValuesNotMatch);

            if (keyFields == null || keyFields.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyFieldsNullOrZeroLenght);

            if (keyValues == null || keyValues.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyValuesNullOrZeroLenght);

            if (keyFields.Length != keyValues.Length)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyFieldsAndKeyValuesNotMatch);

            #endregion Validations

            DataTable dataTable = Select(tableName, keyFields, keyValues, new String[] { "1" });

            if (dataTable.Rows.Count == 0)
            {
                String[] fields = new String[keyFields.Length + nonKeyFields.Length];
                keyFields.CopyTo(fields, 0);
                nonKeyFields.CopyTo(fields, keyFields.Length);

                String[] values = new String[keyValues.Length + nonKeyValues.Length];
                keyValues.CopyTo(values, 0);
                nonKeyValues.CopyTo(values, keyValues.Length);

                return Insert(tableName, fields, values);
            }
            else
            {
                return Update(tableName, nonKeyFields, nonKeyValues, keyFields, keyValues);
            }
        }

        /// <summary>
        /// Execute a sql update or insert sentence for many records depending on each record existence
        /// </summary>
        /// <param name="tableName">The table name to update or insert the records</param>
        /// <param name="dataTable">The datatable containg the records to be updated or inserted</param>
        /// <returns>The number of affected records</returns>
        public override Int32 IndateAll(String tableName, DataTable dataTable)
        {
            return IndateAll(tableName, dataTable, DataRowState.Added);
        }

        /// <summary>
        /// Execute a sql update or insert sentence for many records depending on each record existence
        /// </summary>
        /// <param name="tableName">The table name to update or insert the records</param>
        /// <param name="dataTable">The datatable containg the records to be updated or inserted</param>
        /// <param name="dataRowState">The datarow state on datatable to be considered</param>
        /// <returns>The number of affected records</returns>
        public override Int32 IndateAll(String tableName, DataTable dataTable, DataRowState dataRowState)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(tableName) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionTableNameNullOrEmpty);

            if (dataTable == null || dataTable.Rows.Count == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionDataTableNullOrRowsZeroLenght);

            if (dataTable.Columns.Count == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionDataTableRowsColumnsZeroLenght);

            if (dataTable.PrimaryKey == null || dataTable.PrimaryKey.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionDataTablePrimaryKeyNullOrZeroLenght);

            #endregion Validations

            Int32 columnIndex = 0;

            String[] nonKeyFields = new String[dataTable.Columns.Count - dataTable.PrimaryKey.Length];
            foreach (DataColumn dataColumn in dataTable.Columns)
            {
                Int32 pkIndex = 0;
                for (pkIndex = 0; pkIndex < dataTable.PrimaryKey.Length; pkIndex++)
                {
                    if (dataTable.PrimaryKey[pkIndex].ColumnName == dataColumn.ColumnName)
                        break;
                }

                if (pkIndex == dataTable.PrimaryKey.Length)
                {
                    nonKeyFields[columnIndex] = dataColumn.ColumnName;
                    columnIndex++;
                }
            }

            columnIndex = 0;
            String[] keyFields = new String[dataTable.PrimaryKey.Length];
            foreach (DataColumn dataColumn in dataTable.PrimaryKey)
            {
                keyFields[columnIndex] = dataColumn.ColumnName;
                columnIndex++;
            }

            List<Object[]> nonKeyValuesList = new List<Object[]>();
            List<Object[]> keyValuesList = new List<Object[]>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                if (dataRowState.HasFlag(dataRow.RowState) == false)
                    continue;

                columnIndex = 0;
                nonKeyValuesList.Add(new Object[dataTable.Columns.Count - dataTable.PrimaryKey.Length]);
                foreach (DataColumn dataColumn in dataTable.Columns)
                {
                    Int32 pkIndex = 0;
                    for (pkIndex = 0; pkIndex < dataTable.PrimaryKey.Length; pkIndex++)
                    {
                        if (dataTable.PrimaryKey[pkIndex].ColumnName == dataColumn.ColumnName)
                            break;
                    }

                    if (pkIndex == dataTable.PrimaryKey.Length)
                    {
                        nonKeyValuesList[nonKeyValuesList.Count - 1][columnIndex] = dataRow[dataColumn];
                        columnIndex++;
                    }
                }

                columnIndex = 0;
                keyValuesList.Add(new Object[dataTable.PrimaryKey.Length]);
                foreach (DataColumn dataColumn in dataTable.PrimaryKey)
                {
                    keyValuesList[keyValuesList.Count - 1][columnIndex] = dataRow.RowState == DataRowState.Modified ? dataRow[dataColumn, DataRowVersion.Original] : dataRow[dataColumn];
                    columnIndex++;
                }
            }

            return IndateAll(tableName, nonKeyFields, nonKeyValuesList, keyFields, keyValuesList);
        }

        /// <summary>
        /// Execute a sql update or insert sentence for many records depending on each record existence
        /// </summary>
        /// <param name="tableName">The table name to update or insert the records</param>
        /// <param name="nonKeyFields">The table non key fields to be included on the update or insert sentence</param>
        /// <param name="nonKeyValuesList">The list of respective non key fields values to be updated or inserted on the table</param>
        /// <param name="keyFields">The table key fields</param>
        /// <param name="keyValuesList">The list of respective key fields values</param>
        /// <returns>The number of affected records</returns>
        public override Int32 IndateAll(String tableName, String[] nonKeyFields, List<Object[]> nonKeyValuesList, String[] keyFields, List<Object[]> keyValuesList)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(tableName) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionTableNameNullOrEmpty);

            if (nonKeyFields == null || nonKeyFields.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionNonKeyFieldsNullOrZeroLenght);

            if (nonKeyValuesList == null || nonKeyValuesList.Count == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionNonKeyValuesListNullOrZeroLenght);

            if (keyFields == null || keyFields.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyFieldsNullOrZeroLenght);

            if (keyValuesList == null || keyValuesList.Count == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyValuesListNullOrZeroLenght);

            if (nonKeyValuesList.Count != keyValuesList.Count)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionNonKeyValuesListAndKeyValueListNotMatch);

            #endregion Validations

            String filterString = String.Empty;
            for (int i = 0; i < keyFields.Length; i++)
                filterString += keyFields[i] + " = '{" + i + "}' and ";
            filterString = filterString.Remove(filterString.Length - 5, 5); // Remove last " and "

            String[] fields = new String[keyFields.Length + nonKeyFields.Length];
            keyFields.CopyTo(fields, 0);
            nonKeyFields.CopyTo(fields, keyFields.Length);

            DataTable dataTable = SelectAll(tableName, keyFields, keyValuesList, fields);

            List<Object[]> valuesListInsert = new List<Object[]>();
            List<Object[]> valuesListUpdate = new List<Object[]>();
            List<Object[]> keyValuesListUpdate = new List<Object[]>();

            for (int listIndex = 0; listIndex < keyValuesList.Count; listIndex++)
            {
                String filter = String.Format(filterString, keyValuesList[listIndex]);
                DataRow[] dataRow = dataTable.Select(filter);

                if (dataRow.Length > 0)
                {
                    valuesListUpdate.Add(new Object[keyFields.Length + nonKeyFields.Length]);
                    keyValuesList[listIndex].CopyTo(valuesListUpdate[valuesListUpdate.Count - 1], 0);
                    nonKeyValuesList[listIndex].CopyTo(valuesListUpdate[valuesListUpdate.Count - 1], keyFields.Length);

                    keyValuesListUpdate.Add(keyValuesList[listIndex]);
                }
                else
                {
                    valuesListInsert.Add(new Object[keyFields.Length + nonKeyFields.Length]);
                    keyValuesList[listIndex].CopyTo(valuesListInsert[valuesListInsert.Count - 1], 0);
                    nonKeyValuesList[listIndex].CopyTo(valuesListInsert[valuesListInsert.Count - 1], keyFields.Length);
                }
            }

            Int32 rowsAffected = 0;

            if (valuesListInsert.Count > 0)
                rowsAffected += InsertAll(tableName, fields, valuesListInsert);

            if (valuesListUpdate.Count > 0)
                rowsAffected += UpdateAll(tableName, fields, valuesListUpdate, keyFields, keyValuesListUpdate);

            return rowsAffected;
        }

        /// <summary>
        /// Execute a sql update sentence
        /// </summary>
        /// <param name="tableName">The table name to update the record</param>
        /// <param name="dataRow">The datarow to be updated</param>
        /// <returns>The number of affected records</returns>
        public override Int32 Update(String tableName, DataRow dataRow)
        {
            return Update(tableName, dataRow, DataRowState.Modified);
        }

        /// <summary>
        /// Execute a sql update sentence
        /// </summary>
        /// <param name="tableName">The table name to update the record</param>
        /// <param name="dataRow">The datarow to be updated</param>
        /// <param name="dataRowState">The datarow state to be considered</param>
        /// <returns>The number of affected records</returns>
        public override Int32 Update(String tableName, DataRow dataRow, DataRowState dataRowState)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(tableName) == true)
                throw new Exception(Properties.Resources.LazyDatabaseExceptionTableNameNullOrEmpty);

            if (dataRow == null || dataRow.Table.Columns.Count == 0)
                throw new Exception(Properties.Resources.LazyDatabaseExceptionDataRowNullOrColumnZeroLenght);

            if (dataRow.Table.PrimaryKey == null || dataRow.Table.PrimaryKey.Length == 0)
                throw new Exception(Properties.Resources.LazyDatabaseExceptionDataTablePrimaryKeyNullOrZeroLenght);

            #endregion Validations

            String[] fields = null;
            Object[] values = null;
            String[] keyFields = null;
            Object[] keyValues = null;

            if (dataRowState.HasFlag(dataRow.RowState) == true)
            {
                Int32 columnIndex = 0;

                fields = new String[dataRow.Table.Columns.Count];
                values = new Object[dataRow.Table.Columns.Count];
                foreach (DataColumn dataColumn in dataRow.Table.Columns)
                {
                    fields[columnIndex] = dataColumn.ColumnName;
                    values[columnIndex] = dataRow[dataColumn];
                    columnIndex++;
                }

                columnIndex = 0;
                keyFields = new String[dataRow.Table.PrimaryKey.Length];
                keyValues = new Object[dataRow.Table.PrimaryKey.Length];
                foreach (DataColumn dataColumn in dataRow.Table.PrimaryKey)
                {
                    keyFields[columnIndex] = dataColumn.ColumnName;
                    keyValues[columnIndex] = (dataRow.RowState == DataRowState.Modified || dataRow.RowState == DataRowState.Deleted) ? dataRow[dataColumn, DataRowVersion.Original] : dataRow[dataColumn];
                    columnIndex++;
                }
            }

            return Update(tableName, fields, values, keyFields, keyValues);
        }

        /// <summary>
        /// Execute a sql update sentence
        /// </summary>
        /// <param name="tableName">The table name to update the record</param>
        /// <param name="fields">The table fields to be included on the update sentence</param>
        /// <param name="values">The respective fields values to be updated on the table</param>
        /// <param name="keyFields">The table key fields</param>
        /// <param name="keyValues">The respective key fields values</param>
        /// <returns>The number of affected records</returns>
        public override Int32 Update(String tableName, String[] fields, Object[] values, String[] keyFields, Object[] keyValues)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(tableName) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionTableNameNullOrEmpty);

            if (fields == null || fields.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionFieldsNullOrZeroLenght);

            if (values == null || values.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionValuesNullOrZeroLenght);

            if (fields.Length != values.Length)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionFieldsAndValuesNotMatch);

            if (keyFields == null || keyFields.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyFieldsNullOrZeroLenght);

            if (keyValues == null || keyValues.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyValuesNullOrZeroLenght);

            if (keyFields.Length != keyValues.Length)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyFieldsAndKeyValuesNotMatch);

            #endregion Validations

            this.mySqlCommand.Parameters.Clear();

            String fieldString = String.Empty;
            String keyFieldString = String.Empty;

            foreach (String field in fields)
                fieldString += field + " = @" + field + ", ";
            fieldString = fieldString.Remove(fieldString.Length - 2, 2);

            foreach (String keyField in keyFields)
                keyFieldString += keyField + " = @key" + keyField + " and ";
            keyFieldString = keyFieldString.Remove(keyFieldString.Length - 5, 5);

            for (int i = 0; i < values.Length; i++)
            {
                MySqlDbType dbType = (MySqlDbType)ConvertToDatabaseType(values[i].GetType());
                MySqlParameter mySqlParameter = new MySqlParameter(fields[i], dbType);
                mySqlParameter.Value = values[i];

                this.mySqlCommand.Parameters.Add(mySqlParameter);
            }

            for (int i = 0; i < keyValues.Length; i++)
            {
                MySqlDbType dbType = (MySqlDbType)ConvertToDatabaseType(keyValues[i].GetType());
                MySqlParameter mySqlParameter = new MySqlParameter("key" + keyFields[i], dbType);
                mySqlParameter.Value = keyValues[i];

                this.mySqlCommand.Parameters.Add(mySqlParameter);
            }

            String sql = "update " + tableName + " set " + fieldString + " where " + keyFieldString;

            this.mySqlCommand.CommandText = sql;
            this.mySqlCommand.CommandType = CommandType.Text;
            this.mySqlCommand.Transaction = this.mySqlTransaction;

            return this.mySqlCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Execute a sql update sentence for many records
        /// </summary>
        /// <param name="tableName">The table name to update the records</param>
        /// <param name="dataTable">The datatable containg the records to be updated</param>
        /// <returns>The number of affected records</returns>
        public override Int32 UpdateAll(String tableName, DataTable dataTable)
        {
            return UpdateAll(tableName, dataTable, DataRowState.Modified);
        }

        /// <summary>
        /// Execute a sql update sentence for many records
        /// </summary>
        /// <param name="tableName">The table name to update the records</param>
        /// <param name="dataTable">The datatable containg the records to be updated</param>
        /// <param name="dataRowState">The datarow state on datatable to be considered</param>
        /// <returns>The number of affected records</returns>
        public override Int32 UpdateAll(String tableName, DataTable dataTable, DataRowState dataRowState)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(tableName) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionTableNameNullOrEmpty);

            if (dataTable == null || dataTable.Rows.Count == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionDataTableNullOrRowsZeroLenght);

            if (dataTable.Columns.Count == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionDataTableRowsColumnsZeroLenght);

            if (dataTable.PrimaryKey == null || dataTable.PrimaryKey.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionDataTablePrimaryKeyNullOrZeroLenght);

            #endregion Validations

            Int32 columnIndex = 0;

            String[] fields = new String[dataTable.Columns.Count];
            foreach (DataColumn dataColumn in dataTable.Columns)
            {
                fields[columnIndex] = dataColumn.ColumnName;
                columnIndex++;
            }

            columnIndex = 0;
            String[] keyFields = new String[dataTable.PrimaryKey.Length];
            foreach (DataColumn dataColumn in dataTable.PrimaryKey)
            {
                keyFields[columnIndex] = dataColumn.ColumnName;
                columnIndex++;
            }

            List<Object[]> valuesList = new List<Object[]>();
            List<Object[]> keyValuesList = new List<Object[]>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                if (dataRowState.HasFlag(dataRow.RowState) == false)
                    continue;

                columnIndex = 0;
                valuesList.Add(new Object[dataTable.Columns.Count]);
                foreach (DataColumn dataColumn in dataTable.Columns)
                {
                    valuesList[valuesList.Count - 1][columnIndex] = dataRow[dataColumn];
                    columnIndex++;
                }

                columnIndex = 0;
                keyValuesList.Add(new Object[dataTable.PrimaryKey.Length]);
                foreach (DataColumn dataColumn in dataTable.PrimaryKey)
                {
                    keyValuesList[keyValuesList.Count - 1][columnIndex] = (dataRow.RowState == DataRowState.Modified || dataRow.RowState == DataRowState.Deleted) ? dataRow[dataColumn, DataRowVersion.Original] : dataRow[dataColumn];
                    columnIndex++;
                }
            }

            return UpdateAll(tableName, fields, valuesList, keyFields, keyValuesList);
        }

        /// <summary>
        /// Execute a sql update sentence for many records
        /// </summary>
        /// <param name="tableName">The table name to update the records</param>
        /// <param name="fields">The table fields to be included on the update sentence</param>
        /// <param name="valuesList">The list of respective fields values to be updated on the table</param>
        /// <param name="keyFields">The table key fields</param>
        /// <param name="keyValuesList">The list of respective key fields values</param>
        /// <returns>The number of affected records</returns>
        public override Int32 UpdateAll(String tableName, String[] fields, List<Object[]> valuesList, String[] keyFields, List<Object[]> keyValuesList)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(tableName) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionTableNameNullOrEmpty);

            if (fields == null || fields.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionFieldsNullOrZeroLenght);

            if (valuesList == null || valuesList.Count == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionValuesListNullOrZeroLenght);

            if (keyFields == null || keyFields.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyFieldsNullOrZeroLenght);

            if (keyValuesList == null || keyValuesList.Count == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyValuesListNullOrZeroLenght);

            if (valuesList.Count != keyValuesList.Count)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionValuesListAndKeyValueListNotMatch);

            #endregion Validations

            this.mySqlCommand.Parameters.Clear();

            String tempTableString = String.Empty;
            String innerJoinOnString = String.Empty;
            String setFieldString = String.Empty;
            String whereKeyFieldString = String.Empty;
            String inKeyValueString = String.Empty;

            // Generate temp table
            for (int listIndex = 0; listIndex < valuesList.Count; listIndex++)
            {
                #region Validations

                if (keyFields.Length != keyValuesList[listIndex].Length)
                    throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyFieldsAndKeyValuesNotMatch);

                if (fields.Length != valuesList[listIndex].Length)
                    throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionFieldsAndValuesNotMatch);

                #endregion Validations

                tempTableString += "select ";

                for (int columnIndex = 0; columnIndex < keyValuesList[listIndex].Length; columnIndex++)
                    tempTableString += ConvertToDatabaseStringFormat(keyValuesList[listIndex][columnIndex]) + " as " + ("key" + keyFields[columnIndex]) + ", ";

                for (int columnIndex = 0; columnIndex < valuesList[listIndex].Length; columnIndex++)
                    tempTableString += ConvertToDatabaseStringFormat(valuesList[listIndex][columnIndex]) + " as " + (fields[columnIndex]) + ", ";

                tempTableString = tempTableString.Remove(tempTableString.Length - 2, 2);
                tempTableString += " union all ";

                // Generate in clausule
                inKeyValueString += "(";
                foreach (Object value in keyValuesList[listIndex])
                    inKeyValueString += ConvertToDatabaseStringFormat(value) + ",";
                inKeyValueString = inKeyValueString.Remove(inKeyValueString.Length - 1, 1);
                inKeyValueString += "),";
            }
            tempTableString = tempTableString.Remove(tempTableString.Length - 11, 11); // Remove last " union all "
            inKeyValueString = inKeyValueString.Remove(inKeyValueString.Length - 1, 1); // Remove last ","

            // Generate on clausule
            foreach (String keyField in keyFields)
                innerJoinOnString += "DB." + keyField + " = TMP.key" + keyField + " and ";
            innerJoinOnString = innerJoinOnString.Remove(innerJoinOnString.Length - 5, 5); // Remove last " and "

            // Generate set fields
            foreach (String field in fields)
                setFieldString += "DB." + field + " = TMP." + field + ", ";
            setFieldString = setFieldString.Remove(setFieldString.Length - 2, 2); // Remove last ", "

            // Generate where key fields
            foreach (String keyField in keyFields)
                whereKeyFieldString += "DB." + keyField + ", ";
            whereKeyFieldString = whereKeyFieldString.Remove(whereKeyFieldString.Length - 2, 2); // Remove last ", "

            String sql = "update " + tableName + " AS DB inner join (" + tempTableString + ") AS TMP on " + innerJoinOnString + " set " + setFieldString + " where (" + whereKeyFieldString + ") in (" + inKeyValueString + ")";

            this.mySqlCommand.CommandText = sql;
            this.mySqlCommand.CommandType = CommandType.Text;
            this.mySqlCommand.Transaction = this.mySqlTransaction;

            return this.mySqlCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Execute a sql update or insert sentence depending on record existence
        /// </summary>
        /// <param name="tableName">The table name to update or insert the record</param>
        /// <param name="dataRow">The datarow to be updated or inserted</param>
        /// <returns>The number of affected records</returns>
        public override Int32 Upsert(String tableName, DataRow dataRow)
        {
            return Upsert(tableName, dataRow, DataRowState.Modified);
        }

        /// <summary>
        /// Execute a sql update or insert sentence depending on record existence
        /// </summary>
        /// <param name="tableName">The table name to update or insert the record</param>
        /// <param name="dataRow">The datarow to be updated or inserted</param>
        /// <param name="dataRowState">The datarow state to be considered</param>
        /// <returns>The number of affected records</returns>
        public override Int32 Upsert(String tableName, DataRow dataRow, DataRowState dataRowState)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(tableName) == true)
                throw new Exception(Properties.Resources.LazyDatabaseExceptionTableNameNullOrEmpty);

            if (dataRow == null || dataRow.Table.Columns.Count == 0)
                throw new Exception(Properties.Resources.LazyDatabaseExceptionDataRowNullOrColumnZeroLenght);

            if (dataRow.Table.PrimaryKey == null || dataRow.Table.PrimaryKey.Length == 0)
                throw new Exception(Properties.Resources.LazyDatabaseExceptionDataTablePrimaryKeyNullOrZeroLenght);

            #endregion Validations

            String[] fields = null;
            Object[] values = null;
            String[] keyFields = null;
            Object[] keyValues = null;

            if (dataRowState.HasFlag(dataRow.RowState) == true)
            {
                Int32 columnIndex = 0;

                fields = new String[dataRow.Table.Columns.Count];
                values = new Object[dataRow.Table.Columns.Count];
                foreach (DataColumn dataColumn in dataRow.Table.Columns)
                {
                    fields[columnIndex] = dataColumn.ColumnName;
                    values[columnIndex] = dataRow[dataColumn];
                    columnIndex++;
                }

                columnIndex = 0;
                keyFields = new String[dataRow.Table.PrimaryKey.Length];
                keyValues = new Object[dataRow.Table.PrimaryKey.Length];
                foreach (DataColumn dataColumn in dataRow.Table.PrimaryKey)
                {
                    keyFields[columnIndex] = dataColumn.ColumnName;
                    keyValues[columnIndex] = (dataRow.RowState == DataRowState.Modified || dataRow.RowState == DataRowState.Deleted) ? dataRow[dataColumn, DataRowVersion.Original] : dataRow[dataColumn];
                    columnIndex++;
                }
            }

            return Upsert(tableName, fields, values, keyFields, keyValues);
        }

        /// <summary>
        /// Execute a sql update or insert sentence depending on record existence
        /// </summary>
        /// <param name="tableName">The table name to update or insert the record</param>
        /// <param name="fields">The table fields to be included on the update or insert sentence</param>
        /// <param name="values">The respective fields values to be updated or inserted on the table</param>
        /// <param name="keyFields">The table key fields</param>
        /// <param name="keyValues">The respective key fields values</param>
        /// <returns>The number of affected records</returns>
        public override Int32 Upsert(String tableName, String[] fields, Object[] values, String[] keyFields, Object[] keyValues)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(tableName) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionTableNameNullOrEmpty);

            if (fields == null || fields.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionFieldsNullOrZeroLenght);

            if (values == null || values.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionValuesNullOrZeroLenght);

            if (fields.Length != values.Length)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionFieldsAndValuesNotMatch);

            if (keyFields == null || keyFields.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyFieldsNullOrZeroLenght);

            if (keyValues == null || keyValues.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyValuesNullOrZeroLenght);

            if (keyFields.Length != keyValues.Length)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyFieldsAndKeyValuesNotMatch);

            #endregion Validations

            DataTable dataTable = Select(tableName, keyFields, keyValues, new String[] { "1" });

            if (dataTable.Rows.Count == 1)
            {
                return Update(tableName, fields, values, keyFields, keyValues);
            }
            else
            {
                return Insert(tableName, fields, values);
            }
        }

        /// <summary>
        /// Execute a sql update or insert sentence for many records depending on each record existence
        /// </summary>
        /// <param name="tableName">The table name to update or insert the records</param>
        /// <param name="dataTable">The datatable containg the records to be updated or inserted</param>
        /// <returns>The number of affected records</returns>
        public override Int32 UpsertAll(String tableName, DataTable dataTable)
        {
            return UpsertAll(tableName, dataTable, DataRowState.Modified);
        }

        /// <summary>
        /// Execute a sql update or insert sentence for many records depending on each record existence
        /// </summary>
        /// <param name="tableName">The table name to update or insert the records</param>
        /// <param name="dataTable">The datatable containg the records to be updated or inserted</param>
        /// <param name="dataRowState">The datarow state on datatable to be considered</param>
        /// <returns>The number of affected records</returns>
        public override Int32 UpsertAll(String tableName, DataTable dataTable, DataRowState dataRowState)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(tableName) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionTableNameNullOrEmpty);

            if (dataTable == null || dataTable.Rows.Count == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionDataTableNullOrRowsZeroLenght);

            if (dataTable.Columns.Count == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionDataTableRowsColumnsZeroLenght);

            if (dataTable.PrimaryKey == null || dataTable.PrimaryKey.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionDataTablePrimaryKeyNullOrZeroLenght);

            #endregion Validations

            Int32 columnIndex = 0;

            String[] fields = new String[dataTable.Columns.Count];
            foreach (DataColumn dataColumn in dataTable.Columns)
            {
                fields[columnIndex] = dataColumn.ColumnName;
                columnIndex++;
            }

            columnIndex = 0;
            String[] keyFields = new String[dataTable.PrimaryKey.Length];
            foreach (DataColumn dataColumn in dataTable.PrimaryKey)
            {
                keyFields[columnIndex] = dataColumn.ColumnName;
                columnIndex++;
            }

            List<Object[]> valuesList = new List<Object[]>();
            List<Object[]> keyValuesList = new List<Object[]>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                if (dataRowState.HasFlag(dataRow.RowState) == false)
                    continue;

                columnIndex = 0;
                valuesList.Add(new Object[dataTable.Columns.Count]);
                foreach (DataColumn dataColumn in dataTable.Columns)
                {
                    valuesList[valuesList.Count - 1][columnIndex] = dataRow[dataColumn];
                    columnIndex++;
                }

                columnIndex = 0;
                keyValuesList.Add(new Object[dataTable.PrimaryKey.Length]);
                foreach (DataColumn dataColumn in dataTable.PrimaryKey)
                {
                    keyValuesList[keyValuesList.Count - 1][columnIndex] = (dataRow.RowState == DataRowState.Modified || dataRow.RowState == DataRowState.Deleted) ? dataRow[dataColumn, DataRowVersion.Original] : dataRow[dataColumn];
                    columnIndex++;
                }
            }

            return UpsertAll(tableName, fields, valuesList, keyFields, keyValuesList);
        }

        /// <summary>
        /// Execute a sql update or insert sentence for many records depending on each record existence
        /// </summary>
        /// <param name="tableName">The table name to update or insert the records</param>
        /// <param name="fields">The table fields to be included on the update or insert sentence</param>
        /// <param name="valuesList">The list of respective fields values to be updated or inserted on the table</param>
        /// <param name="keyFields">The table key fields</param>
        /// <param name="keyValuesList">The list of respective key fields values</param>
        /// <returns>The number of affected records</returns>
        public override Int32 UpsertAll(String tableName, String[] fields, List<Object[]> valuesList, String[] keyFields, List<Object[]> keyValuesList)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(tableName) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionTableNameNullOrEmpty);

            if (fields == null || fields.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionFieldsNullOrZeroLenght);

            if (valuesList == null || valuesList.Count == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionValuesListNullOrZeroLenght);

            if (keyFields == null || keyFields.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyFieldsNullOrZeroLenght);

            if (keyValuesList == null || keyValuesList.Count == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyValuesListNullOrZeroLenght);

            if (valuesList.Count != keyValuesList.Count)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionValuesListAndKeyValueListNotMatch);

            #endregion Validations
            
            String filterString = String.Empty;
            for (int i = 0; i < keyFields.Length; i++)
                filterString += keyFields[i] + " = '{" + i + "}' and ";
            filterString = filterString.Remove(filterString.Length - 5, 5); // Remove last " and "

            DataTable dataTable = SelectAll(tableName, keyFields, keyValuesList, fields);

            List<Object[]> valuesListInsert = new List<Object[]>();
            List<Object[]> valuesListUpdate = new List<Object[]>();
            List<Object[]> keyValuesListUpdate = new List<Object[]>();

            for (int listIndex = 0; listIndex < keyValuesList.Count; listIndex++)
            {
                String filter = String.Format(filterString, keyValuesList[listIndex]);
                DataRow[] dataRow = dataTable.Select(filter);

                if (dataRow.Length > 0)
                {
                    valuesListUpdate.Add(valuesList[listIndex]);
                    keyValuesListUpdate.Add(keyValuesList[listIndex]);
                }
                else
                {
                    valuesListInsert.Add(valuesList[listIndex]);
                }
            }

            Int32 rowsAffected = 0;

            if (valuesListUpdate.Count > 0)
                rowsAffected += UpdateAll(tableName, fields, valuesListUpdate, keyFields, keyValuesListUpdate);

            if (valuesListInsert.Count > 0)
                rowsAffected += InsertAll(tableName, fields, valuesListInsert);

            return rowsAffected;
        }

        /// <summary>
        /// Execute a sql delete sentence
        /// </summary>
        /// <param name="tableName">The table name to delete the record</param>
        /// <param name="dataRow">The datarow to be deleted</param>
        /// <returns>The number of affected records</returns>
        public override Int32 Delete(String tableName, DataRow dataRow)
        {
            return Delete(tableName, dataRow, DataRowState.Deleted);
        }

        /// <summary>
        /// Execute a sql delete sentence
        /// </summary>
        /// <param name="tableName">The table name to delete the record</param>
        /// <param name="dataRow">The datarow to be deleted</param>
        /// <param name="dataRowState">The datarow state to be considered</param>
        /// <returns>The number of affected records</returns>
        public override Int32 Delete(String tableName, DataRow dataRow, DataRowState dataRowState)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(tableName) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionTableNameNullOrEmpty);

            if (dataRow == null || dataRow.Table.Columns.Count == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionDataRowNullOrColumnZeroLenght);

            if (dataRow.Table.PrimaryKey == null || dataRow.Table.PrimaryKey.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionDataTablePrimaryKeyNullOrZeroLenght);

            #endregion Validations

            String[] keyFields = null;
            Object[] keyValues = null;

            if (dataRowState.HasFlag(dataRow.RowState) == true)
            {
                Int32 columnIndex = 0;

                keyFields = new String[dataRow.Table.PrimaryKey.Length];
                keyValues = new Object[dataRow.Table.PrimaryKey.Length];
                foreach (DataColumn dataColumn in dataRow.Table.PrimaryKey)
                {
                    keyFields[columnIndex] = dataColumn.ColumnName;
                    keyValues[columnIndex] = (dataRow.RowState == DataRowState.Modified || dataRow.RowState == DataRowState.Deleted) ? dataRow[dataColumn, DataRowVersion.Original] : dataRow[dataColumn];
                    columnIndex++;
                }
            }

            return Delete(tableName, keyFields, keyValues);
        }

        /// <summary>
        /// Execute a sql delete sentence
        /// </summary>
        /// <param name="tableName">The table name to delete the record</param>
        /// <param name="keyFields">The table key fields</param>
        /// <param name="keyValues">The respective key fields values</param>
        /// <returns>The number of affected records</returns>
        public override Int32 Delete(String tableName, String[] keyFields, Object[] keyValues)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(tableName) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionTableNameNullOrEmpty);

            if (keyFields == null || keyFields.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyFieldsNullOrZeroLenght);

            if (keyValues == null || keyValues.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyValuesNullOrZeroLenght);

            if (keyFields.Length != keyValues.Length)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyFieldsAndKeyValuesNotMatch);

            #endregion Validations

            this.mySqlCommand.Parameters.Clear();

            String whereKeyFieldString = String.Empty;

            foreach (String keyField in keyFields)
                whereKeyFieldString += keyField + " = @" + keyField + " and ";
            whereKeyFieldString = whereKeyFieldString.Remove(whereKeyFieldString.Length - 5, 5); // Remove last " and "

            for (int i = 0; i < keyValues.Length; i++)
            {
                MySqlDbType dbType = (MySqlDbType)ConvertToDatabaseType(keyValues[i].GetType());
                MySqlParameter mySqlParameter = new MySqlParameter(keyFields[i], dbType);
                mySqlParameter.Value = keyValues[i];

                this.mySqlCommand.Parameters.Add(mySqlParameter);
            }

            String sql = "delete from " + tableName + " where " + whereKeyFieldString;

            this.mySqlCommand.CommandText = sql;
            this.mySqlCommand.CommandType = CommandType.Text;
            this.mySqlCommand.Transaction = this.mySqlTransaction;

            return this.mySqlCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Execute a sql delete sentence for many records
        /// </summary>
        /// <param name="tableName">The table name to delete the records</param>
        /// <param name="dataTable">The datatable containg the records to be deleted</param>
        /// <returns>The number of affected records</returns>
        public override Int32 DeleteAll(String tableName, DataTable dataTable)
        {
            return DeleteAll(tableName, dataTable, DataRowState.Deleted);
        }

        /// <summary>
        /// Execute a sql delete sentence for many records
        /// </summary>
        /// <param name="tableName">The table name to delete the records</param>
        /// <param name="dataTable">The datatable containg the records to be deleted</param>
        /// <param name="dataRowState">The datarow state on datatable to be considered</param>
        /// <returns>The number of affected records</returns>
        public override Int32 DeleteAll(String tableName, DataTable dataTable, DataRowState dataRowState)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(tableName) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionTableNameNullOrEmpty);

            if (dataTable == null || dataTable.Rows.Count == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionDataTableNullOrRowsZeroLenght);

            if (dataTable.Columns.Count == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionDataTableRowsColumnsZeroLenght);

            if (dataTable.PrimaryKey == null || dataTable.PrimaryKey.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionDataTablePrimaryKeyNullOrZeroLenght);

            #endregion Validations

            Int32 columnIndex = 0;

            String[] keyFields = new String[dataTable.PrimaryKey.Length];
            foreach (DataColumn dataColumn in dataTable.PrimaryKey)
            {
                keyFields[columnIndex] = dataColumn.ColumnName;
                columnIndex++;
            }

            List<Object[]> keyValuesList = new List<Object[]>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                if (dataRowState.HasFlag(dataRow.RowState) == false)
                    continue;

                columnIndex = 0;
                keyValuesList.Add(new Object[dataTable.PrimaryKey.Length]);
                foreach (DataColumn dataColumn in dataTable.PrimaryKey)
                {
                    keyValuesList[keyValuesList.Count - 1][columnIndex] = (dataRow.RowState == DataRowState.Modified || dataRow.RowState == DataRowState.Deleted) ? dataRow[dataColumn, DataRowVersion.Original] : dataRow[dataColumn];
                    columnIndex++;
                }
            }

            return DeleteAll(tableName, keyFields, keyValuesList);
        }

        /// <summary>
        /// Execute a sql delete sentence for many records
        /// </summary>
        /// <param name="tableName">The table name to delete the records</param>
        /// <param name="keyFields">The table key fields</param>
        /// <param name="keyValuesList">The list of respective key fields values</param>
        /// <returns>The number of affected records</returns>
        public override Int32 DeleteAll(String tableName, String[] keyFields, List<Object[]> keyValuesList)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(tableName) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionTableNameNullOrEmpty);

            if (keyFields == null || keyFields.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyFieldsNullOrZeroLenght);

            if (keyValuesList == null || keyValuesList.Count == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyValuesListNullOrZeroLenght);

            #endregion Validations

            this.mySqlCommand.Parameters.Clear();

            String keyFieldString = String.Empty;
            String inKeyValueString = String.Empty;

            foreach (String keyField in keyFields)
                keyFieldString += keyField + ",";
            keyFieldString = keyFieldString.Remove(keyFieldString.Length - 1, 1); // Remove last ","

            for (int listIndex = 0; listIndex < keyValuesList.Count; listIndex++)
            {
                #region Validations

                if (keyFields.Length != keyValuesList[listIndex].Length)
                    throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyFieldsAndKeyValuesNotMatch);

                #endregion Validations

                // Generate in clausule
                inKeyValueString += "(";
                foreach (Object value in keyValuesList[listIndex])
                    inKeyValueString += ConvertToDatabaseStringFormat(value) + ",";
                inKeyValueString = inKeyValueString.Remove(inKeyValueString.Length - 1, 1); // Remove last ","
                inKeyValueString += "),";
            }
            inKeyValueString = inKeyValueString.Remove(inKeyValueString.Length - 1, 1); // Remove last "),"

            String sql = "delete from " + tableName + " where (" + keyFieldString + ") in (" + inKeyValueString + ")";

            this.mySqlCommand.CommandText = sql;
            this.mySqlCommand.CommandType = CommandType.Text;
            this.mySqlCommand.Transaction = this.mySqlTransaction;

            return this.mySqlCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Increment a table field value by one
        /// </summary>
        /// <param name="tableName">The increment table</param>
        /// <param name="keyFields">The increment table key fields</param>
        /// <param name="keyValues">The increment table key values</param>
        /// <param name="incrementField">The increment field</param>
        /// <returns>The incremented value</returns>
        public override Int32 Increment(String tableName, String[] keyFields, Object[] keyValues, String incrementField)
        {
            return IncrementRange(tableName, keyFields, keyValues, incrementField, 1)[0];
        }

        /// <summary>
        /// Increment a table field value by range
        /// </summary>
        /// <param name="tableName">The increment table</param>
        /// <param name="keyFields">The increment table key fields</param>
        /// <param name="keyValues">The increment table key values</param>
        /// <param name="incrementField">The increment field</param>
        /// <param name="range">The increment value</param>
        /// <returns>The incremented values</returns>
        public override Int32[] IncrementRange(String tableName, String[] keyFields, Object[] keyValues, String incrementField, Int32 range)
        {
            #region Validations

            if (this.mySqlConnection == null || this.mySqlConnection.State == ConnectionState.Closed)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionNotOpen);

            if (String.IsNullOrEmpty(tableName) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionTableNameNullOrEmpty);

            if (keyFields == null || keyFields.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyFieldsNullOrZeroLenght);

            if (keyValues == null || keyValues.Length == 0)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyValuesNullOrZeroLenght);

            if (keyFields.Length != keyValues.Length)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionKeyFieldsAndKeyValuesNotMatch);

            if (String.IsNullOrEmpty(incrementField) == true)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionIncrementFieldNullOrEmpty);

            if (range < 1)
                throw new Exception(Lazy.Database.Properties.Resources.LazyDatabaseExceptionIncrementRangeLowerThanOne);

            #endregion Validations

            String keyFieldsString = String.Empty;
            foreach (String keyField in keyFields)
                keyFieldsString += keyField + " = :" + keyField + " and ";
            keyFieldsString = keyFieldsString.Remove(keyFieldsString.Length - 5, 5);

            String sql = "select " + incrementField + " from " + tableName + " where " + keyFieldsString;
            Int32 lastIncrement = LazyConvert.ToInt32(QueryValue(sql, keyValues, keyFields), -1);

            String[] fields = null;
            Object[] values = null;

            if (lastIncrement == -1)
            {
                lastIncrement = 0;

                fields = new String[keyFields.Length + 1];
                keyFields.CopyTo(fields, 0);
                fields[fields.Length - 1] = incrementField;

                values = new Object[keyValues.Length + 1];
                keyValues.CopyTo(values, 0);
                values[values.Length - 1] = lastIncrement + range;

                Insert(tableName, fields, values);
            }
            else
            {
                fields = new String[] { incrementField };
                values = new Object[] { lastIncrement + range };

                Update(tableName, fields, values, keyFields, keyValues);
            }

            return Enumerable.Range(lastIncrement + 1, range).ToArray();
        }

        /// <summary>
        /// Convert a value to a database string format
        /// </summary>
        /// <param name="value">The value to be converted to the string format</param>
        /// <returns>The string format of the value</returns>
        protected override String ConvertToDatabaseStringFormat(Object value)
        {
            if (value.GetType() == typeof(Char)) return "'" + value.ToString() + "'";
            if (value.GetType() == typeof(Byte)) return value.ToString();
            if (value.GetType() == typeof(Int16)) return value.ToString();
            if (value.GetType() == typeof(Int32)) return value.ToString();
            if (value.GetType() == typeof(Int64)) return value.ToString();
            if (value.GetType() == typeof(UInt16)) return value.ToString();
            if (value.GetType() == typeof(UInt32)) return value.ToString();
            if (value.GetType() == typeof(UInt64)) return value.ToString();
            if (value.GetType() == typeof(String)) return "'" + value.ToString() + "'";
            if (value.GetType() == typeof(Boolean)) return "'" + value.ToString() + "'";
            if (value.GetType() == typeof(Byte[])) return "'" + value.ToString() + "'";
            if (value.GetType() == typeof(DateTime)) return "'" + String.Format("{0:yyyy/MM/dd HH:mm:ss}", value) + "'";
            if (value.GetType() == typeof(Decimal)) return "'" + value.ToString().Replace(',', '.') + "'";
            if (value.GetType() == typeof(float)) return "'" + value.ToString().Replace(',', '.') + "'";
            if (value.GetType() == typeof(double)) return "'" + value.ToString().Replace(',', '.') + "'";

            return "'" + value.ToString() + "'";
        }

        /// <summary>
        /// Convert a system type to a database type
        /// </summary>
        /// <param name="systemType">The system type to be converted</param>
        /// <returns>The database type</returns>
        protected override Int32 ConvertToDatabaseType(Type systemType)
        {
            if (systemType == typeof(Char)) return (Int32)MySqlDbType.VarChar;
            if (systemType == typeof(Byte)) return (Int32)MySqlDbType.Byte;
            if (systemType == typeof(Int16)) return (Int32)MySqlDbType.Int16;
            if (systemType == typeof(Int32)) return (Int32)MySqlDbType.Int32;
            if (systemType == typeof(Int64)) return (Int32)MySqlDbType.Int64;
            if (systemType == typeof(UInt16)) return (Int32)MySqlDbType.UInt16;
            if (systemType == typeof(UInt32)) return (Int32)MySqlDbType.UInt32;
            if (systemType == typeof(UInt64)) return (Int32)MySqlDbType.UInt64;
            if (systemType == typeof(String)) return (Int32)MySqlDbType.VarChar;
            if (systemType == typeof(Boolean)) return (Int32)MySqlDbType.Int16;
            if (systemType == typeof(Byte[])) return (Int32)MySqlDbType.VarBinary;
            if (systemType == typeof(DateTime)) return (Int32)MySqlDbType.DateTime;
            if (systemType == typeof(Decimal)) return (Int32)MySqlDbType.Decimal;
            if (systemType == typeof(float)) return (Int32)MySqlDbType.Float;
            if (systemType == typeof(double)) return (Int32)MySqlDbType.Double;

            return (Int32)MySqlDbType.VarChar;
        }

        #endregion Methods

        #region Properties

        public override Boolean InTransaction
        {
            get { return this.mySqlConnection != null && this.mySqlTransaction != null; }
        }

        #endregion Properties
    }
}

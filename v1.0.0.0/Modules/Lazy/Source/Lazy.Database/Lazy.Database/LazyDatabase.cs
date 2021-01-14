// LazyDatabase.cs
//
// This file is integrated part of Lazy project
// Licensed under "Gnu General Public License Version 3"
//
// Created by Isaac Bezerra Saraiva
// Created on 2020, November 06

using System;
using System.Xml;
using System.Data;
using System.Collections.Generic;

namespace Lazy.Database
{
    public abstract class LazyDatabase
    {
        #region Variables

        private String connectionString;

        #endregion Variables

        #region Constructors

        public LazyDatabase()
        {
        }

        public LazyDatabase(String connectionString)
        {
            this.connectionString = connectionString;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Open the connection with the database
        /// </summary>
        public abstract void OpenConnection();

        /// <summary>
        /// Close the connection with the database
        /// </summary>
        public abstract void CloseConnection();

        /// <summary>
        /// Begin a new transaction
        /// </summary>
        public abstract void BeginTransaction();

        /// <summary>
        /// Commit current transaction
        /// </summary>
        public abstract void CommitTransaction();

        /// <summary>
        /// Rollback current transaction
        /// </summary>
        public abstract void RollbackTransaction();

        /// <summary>
        /// Execute a sql sentence
        /// </summary>
        /// <param name="sql">The sql sentence to be executed</param>
        /// <param name="values">The sql sentence parameters values</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 QueryExecute(String sql, Object[] values);

        /// <summary>
        /// Execute a sql sentence
        /// </summary>
        /// <param name="sql">The sql sentence to be executed</param>
        /// <param name="values">The sql sentence parameters values</param>
        /// <param name="parameters">The sql sentence parameters</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 QueryExecute(String sql, Object[] values, String[] parameters);

        /// <summary>
        /// Execute a sql sentence to retrieve a desired value
        /// </summary>
        /// <param name="sql">The sql sentence to be executed</param>
        /// <param name="values">The sql sentence parameters values</param>
        /// <returns>The desired value from the sql sentence</returns>
        public abstract Object QueryValue(String sql, Object[] values);

        /// <summary>
        /// Execute a sql sentence to retrieve a desired value
        /// </summary>
        /// <param name="sql">The sql sentence to be executed</param>
        /// <param name="values">The sql sentence parameters values</param>
        /// <param name="parameters">The sql sentence parameters</param>
        /// <returns>The desired value from the sql sentence</returns>
        public abstract Object QueryValue(String sql, Object[] values, String[] parameters);

        /// <summary>
        /// Execute a sql sentence to verify records existence
        /// </summary>
        /// <param name="sql">The sql sentence to be executed</param>
        /// <param name="values">The sql sentence parameters values</param>
        /// <returns>The records existence</returns>
        public abstract Boolean QueryFind(String sql, Object[] values);

        /// <summary>
        /// Execute a sql sentence to verify records existence
        /// </summary>
        /// <param name="sql">The sql sentence to be executed</param>
        /// <param name="values">The sql sentence parameters values</param>
        /// <param name="parameters">The sql sentence parameters</param>
        /// <returns>The records existence</returns>
        public abstract Boolean QueryFind(String sql, Object[] values, String[] parameters);

        /// <summary>
        /// Execute a sql sentence to retrieve a single record
        /// </summary>
        /// <param name="sql">The sql sentence to be executed</param>
        /// <param name="tableName">The desired table name</param>
        /// <param name="values">The sql sentence parameters values</param>
        /// <returns>The record found</returns>
        public abstract DataRow QueryRecord(String sql, String tableName, Object[] values);

        /// <summary>
        /// Execute a sql sentence to retrieve a single record
        /// </summary>
        /// <param name="sql">The sql sentence to be executed</param>
        /// <param name="tableName">The desired table name</param>
        /// <param name="values">The sql sentence parameters values</param>
        /// <param name="parameters">The sql sentence parameters</param>
        /// <returns>The record found</returns>
        public abstract DataRow QueryRecord(String sql, String tableName, Object[] values, String[] parameters);

        /// <summary>
        /// Execute a sql sentence to retrieve many records
        /// </summary>
        /// <param name="sql">The sql sentence to be executed</param>
        /// <param name="tableName">The desired table name</param>
        /// <param name="values">The sql sentence parameters values</param>
        /// <returns>The generated table from the sql sentence</returns>
        public abstract DataTable QueryTable(String sql, String tableName, Object[] values);

        /// <summary>
        /// Execute a sql sentence to retrieve many records
        /// </summary>
        /// <param name="sql">The sql sentence to be executed</param>
        /// <param name="tableName">The desired table name</param>
        /// <param name="values">The sql sentence parameters values</param>
        /// <param name="parameters">The sql sentence parameters</param>
        /// <returns>The generated table from the sql sentence</returns>
        public abstract DataTable QueryTable(String sql, String tableName, Object[] values, String[] parameters);

        /// <summary>
        /// Execute a sql stored procedure
        /// </summary>
        /// <param name="procedureName">The stored procedure name</param>
        /// <param name="tableName">The desired table name</param>
        /// <param name="values">The stored procedure parameters values</param>
        /// <param name="parameters">The stored procedure parameters</param>
        /// <returns>The generated table from the stored procedure</returns>
        public abstract DataTable QueryProcedure(String procedureName, String tableName, Object[] values, String[] parameters);

        /// <summary>
        /// Execute a sql select sentence
        /// </summary>
        /// <param name="tableName">The table name to select the record</param>
        /// <param name="dataRow">The datarow witch contains the key values</param>
        /// <returns>The datatable with selected records</returns>
        public abstract DataTable Select(String tableName, DataRow dataRow);

        /// <summary>
        /// Execute a sql select sentence
        /// </summary>
        /// <param name="tableName">The table name to select the record</param>
        /// <param name="dataRow">The datarow witch contains the key values</param>
        /// <param name="dataRowState">The datarow state to be considered</param>
        /// <returns>The datatable with selected records</returns>
        public abstract DataTable Select(String tableName, DataRow dataRow, DataRowState dataRowState);

        /// <summary>
        /// Execute a sql select sentence
        /// </summary>
        /// <param name="tableName">The table name to select the record</param>
        /// <param name="dataRow">The datarow witch contains the key values</param>
        /// <param name="returnFields">The fields to be returned by the select sentence</param>
        /// <returns>The datatable with selected records</returns>
        public abstract DataTable Select(String tableName, DataRow dataRow, String[] returnFields);

        /// <summary>
        /// Execute a sql select sentence
        /// </summary>
        /// <param name="tableName">The table name to select the record</param>
        /// <param name="dataRow">The datarow witch contains the key values</param>
        /// <param name="dataRowState">The datarow state to be considered</param>
        /// <param name="returnFields">The fields to be returned by the select sentence</param>
        /// <returns>The datatable with selected records</returns>
        public abstract DataTable Select(String tableName, DataRow dataRow, DataRowState dataRowState, String[] returnFields);

        /// <summary>
        /// Execute a sql select sentence
        /// </summary>
        /// <param name="tableName">The table name to select the record</param>
        /// <param name="keyFields">The table key fields</param>
        /// <param name="keyValues">The respective key fields values</param>
        /// <returns>The datatable with selected records</returns>
        public abstract DataTable Select(String tableName, String[] keyFields, Object[] keyValues);

        /// <summary>
        /// Execute a sql select sentence
        /// </summary>
        /// <param name="tableName">The table name to select the record</param>
        /// <param name="keyFields">The table key fields</param>
        /// <param name="keyValues">The respective key fields values</param>
        /// <param name="returnFields">The fields to be returned by the select sentence</param>
        /// <returns>The datatable with selected records</returns>
        public abstract DataTable Select(String tableName, String[] keyFields, Object[] keyValues, String[] returnFields);

        /// <summary>
        /// Execute a sql select sentence
        /// </summary>
        /// <param name="tableName">The table name to select the record</param>
        /// <param name="dataTable">The datatable containg the records to be selected</param>
        /// <returns>The datatable with selected records</returns>
        public abstract DataTable SelectAll(String tableName, DataTable dataTable);

        /// <summary>
        /// Execute a sql select sentence
        /// </summary>
        /// <param name="tableName">The table name to select the record</param>
        /// <param name="dataTable">The datatable containg the records to be selected</param>
        /// <param name="dataRowState">The datarow state on datatable to be considered</param>
        /// <returns>The datatable with selected records</returns>
        public abstract DataTable SelectAll(String tableName, DataTable dataTable, DataRowState dataRowState);

        /// <summary>
        /// Execute a sql select sentence
        /// </summary>
        /// <param name="tableName">The table name to select the record</param>
        /// <param name="dataTable">The datatable containg the records to be selected</param>
        /// <param name="dataRowState">The datarow state on datatable to be considered</param>
        /// <param name="returnFields">The fields to be returned by the select sentence</param>
        /// <returns>The datatable with selected records</returns>
        public abstract DataTable SelectAll(String tableName, DataTable dataTable, String[] returnFields);

        /// <summary>
        /// Execute a sql select sentence
        /// </summary>
        /// <param name="tableName">The table name to select the record</param>
        /// <param name="dataTable">The datatable containg the records to be selected</param>
        /// <param name="dataRowState">The datarow state on datatable to be considered</param>
        /// <param name="returnFields">The fields to be returned by the select sentence</param>
        /// <returns>The datatable with selected records</returns>
        public abstract DataTable SelectAll(String tableName, DataTable dataTable, DataRowState dataRowState, String[] returnFields);

        /// <summary>
        /// Execute a sql select sentence
        /// </summary>
        /// <param name="tableName">The table name to select the record</param>
        /// <param name="keyFields">The table key fields</param>
        /// <param name="keyValuesList">The list of respective key fields values</param>
        /// <returns>The datatable with selected records</returns>
        public abstract DataTable SelectAll(String tableName, String[] keyFields, List<Object[]> keyValuesList);

        /// <summary>
        /// Execute a sql select sentence
        /// </summary>
        /// <param name="tableName">The table name to select the record</param>
        /// <param name="keyFields">The table key fields</param>
        /// <param name="keyValuesList">The list of respective key fields values</param>
        /// <param name="returnFields">The fields to be returned by the select sentence</param>
        /// <returns>The datatable with selected records</returns>
        public abstract DataTable SelectAll(String tableName, String[] keyFields, List<Object[]> keyValuesList, String[] returnFields);

        /// <summary>
        /// Execute a sql insert sentence
        /// </summary>
        /// <param name="tableName">The table name to insert the record</param>
        /// <param name="dataRow">The datarow to be inserted</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 Insert(String tableName, DataRow dataRow);

        /// <summary>
        /// Execute a sql insert sentence
        /// </summary>
        /// <param name="tableName">The table name to insert the record</param>
        /// <param name="dataRow">The datarow to be inserted</param>
        /// <param name="dataRowState">The datarow state to be considered</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 Insert(String tableName, DataRow dataRow, DataRowState dataRowState);

        /// <summary>
        /// Execute a sql insert sentence
        /// </summary>
        /// <param name="tableName">The table name to insert the record</param>
        /// <param name="fields">The table fields to be included on the insert sentence</param>
        /// <param name="values">The respective fields values to be inserted on the table</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 Insert(String tableName, String[] fields, Object[] values);

        /// <summary>
        /// Execute a sql insert sentence for many records
        /// </summary>
        /// <param name="tableName">The table name to insert the records</param>
        /// <param name="dataTable">The datatable containg the records to be inserted</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 InsertAll(String tableName, DataTable dataTable);

        /// <summary>
        /// Execute a sql insert sentence for many records
        /// </summary>
        /// <param name="tableName">The table name to insert the records</param>
        /// <param name="dataTable">The datatable containg the records to be inserted</param>
        /// <param name="dataRowState">The datarow state on datatable to be considered</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 InsertAll(String tableName, DataTable dataTable, DataRowState dataRowState);

        /// <summary>
        /// Execute a sql insert sentence for many records
        /// </summary>
        /// <param name="tableName">The table name to insert the records</param>
        /// <param name="fields">The table fields to be included on the insert sentence</param>
        /// <param name="valuesList">The list of respective fields values to be inserted on the table</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 InsertAll(String tableName, String[] fields, List<Object[]> valuesList);

        /// <summary>
        /// Execute a sql update or insert sentence depending on record existence
        /// </summary>
        /// <param name="tableName">The table name to update or insert the record</param>
        /// <param name="dataRow">The datarow to be updated or inserted</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 Indate(String tableName, DataRow dataRow);

        /// <summary>
        /// Execute a sql update or insert sentence depending on record existence
        /// </summary>
        /// <param name="tableName">The table name to update or insert the record</param>
        /// <param name="dataRow">The datarow to be updated or inserted</param>
        /// <param name="dataRowState">The datarow state to be considered</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 Indate(String tableName, DataRow dataRow, DataRowState dataRowState);

        /// <summary>
        /// Execute a sql update or insert sentence depending on record existence
        /// </summary>
        /// <param name="tableName">The table name to update or insert the record</param>
        /// <param name="nonKeyFields">The table non key fields to be included on the update or insert sentence</param>
        /// <param name="nonKeyValues">The respective non key fields values to be updated or inserted on the table</param>
        /// <param name="keyFields">The table key fields</param>
        /// <param name="keyValues">The respective key fields values</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 Indate(String tableName, String[] nonKeyFields, Object[] nonKeyValues, String[] keyFields, Object[] keyValues);

        /// <summary>
        /// Execute a sql update or insert sentence for many records depending on each record existence
        /// </summary>
        /// <param name="tableName">The table name to update or insert the records</param>
        /// <param name="dataTable">The datatable containg the records to be updated or inserted</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 IndateAll(String tableName, DataTable dataTable);

        /// <summary>
        /// Execute a sql update or insert sentence for many records depending on each record existence
        /// </summary>
        /// <param name="tableName">The table name to update or insert the records</param>
        /// <param name="dataTable">The datatable containg the records to be updated or inserted</param>
        /// <param name="dataRowState">The datarow state on datatable to be considered</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 IndateAll(String tableName, DataTable dataTable, DataRowState dataRowState);

        /// <summary>
        /// Execute a sql update or insert sentence for many records depending on each record existence
        /// </summary>
        /// <param name="tableName">The table name to update or insert the records</param>
        /// <param name="nonKeyFields">The table non key fields to be included on the update or insert sentence</param>
        /// <param name="nonKeyValuesList">The list of respective non key fields values to be updated or inserted on the table</param>
        /// <param name="keyFields">The table key fields</param>
        /// <param name="keyValuesList">The list of respective key fields values</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 IndateAll(String tableName, String[] nonKeyFields, List<Object[]> nonKeyValuesList, String[] keyFields, List<Object[]> keyValuesList);

        /// <summary>
        /// Execute a sql update sentence
        /// </summary>
        /// <param name="tableName">The table name to update the record</param>
        /// <param name="dataRow">The datarow to be updated</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 Update(String tableName, DataRow dataRow);

        /// <summary>
        /// Execute a sql update sentence
        /// </summary>
        /// <param name="tableName">The table name to update the record</param>
        /// <param name="dataRow">The datarow to be updated</param>
        /// <param name="dataRowState">The datarow state to be considered</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 Update(String tableName, DataRow dataRow, DataRowState dataRowState);

        /// <summary>
        /// Execute a sql update sentence
        /// </summary>
        /// <param name="tableName">The table name to update the record</param>
        /// <param name="fields">The table fields to be included on the update sentence</param>
        /// <param name="values">The respective fields values to be updated on the table</param>
        /// <param name="keyFields">The table key fields</param>
        /// <param name="keyValues">The respective key fields values</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 Update(String tableName, String[] fields, Object[] values, String[] keyFields, Object[] keyValues);

        /// <summary>
        /// Execute a sql update sentence for many records
        /// </summary>
        /// <param name="tableName">The table name to update the records</param>
        /// <param name="dataTable">The datatable containg the records to be updated</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 UpdateAll(String tableName, DataTable dataTable);

        /// <summary>
        /// Execute a sql update sentence for many records
        /// </summary>
        /// <param name="tableName">The table name to update the records</param>
        /// <param name="dataTable">The datatable containg the records to be updated</param>
        /// <param name="dataRowState">The datarow state on datatable to be considered</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 UpdateAll(String tableName, DataTable dataTable, DataRowState dataRowState);

        /// <summary>
        /// Execute a sql update sentence for many records
        /// </summary>
        /// <param name="tableName">The table name to update the records</param>
        /// <param name="fields">The table fields to be included on the update sentence</param>
        /// <param name="valuesList">The list of respective fields values to be updated on the table</param>
        /// <param name="keyFields">The table key fields</param>
        /// <param name="keyValuesList">The list of respective key fields values</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 UpdateAll(String tableName, String[] fields, List<Object[]> valuesList, String[] keyFields, List<Object[]> keyValuesList);

        /// <summary>
        /// Execute a sql update or insert sentence depending on record existence
        /// </summary>
        /// <param name="tableName">The table name to update or insert the record</param>
        /// <param name="dataRow">The datarow to be updated or inserted</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 Upsert(String tableName, DataRow dataRow);

        /// <summary>
        /// Execute a sql update or insert sentence depending on record existence
        /// </summary>
        /// <param name="tableName">The table name to update or insert the record</param>
        /// <param name="dataRow">The datarow to be updated or inserted</param>
        /// <param name="dataRowState">The datarow state to be considered</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 Upsert(String tableName, DataRow dataRow, DataRowState dataRowState);

        /// <summary>
        /// Execute a sql update or insert sentence depending on record existence
        /// </summary>
        /// <param name="tableName">The table name to update or insert the record</param>
        /// <param name="fields">The table fields to be included on the update or insert sentence</param>
        /// <param name="values">The respective fields values to be updated or inserted on the table</param>
        /// <param name="keyFields">The table key fields</param>
        /// <param name="keyValues">The respective key fields values</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 Upsert(String tableName, String[] fields, Object[] values, String[] keyFields, Object[] keyValues);

        /// <summary>
        /// Execute a sql update or insert sentence for many records depending on each record existence
        /// </summary>
        /// <param name="tableName">The table name to update or insert the records</param>
        /// <param name="dataTable">The datatable containg the records to be updated or inserted</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 UpsertAll(String tableName, DataTable dataTable);

        /// <summary>
        /// Execute a sql update or insert sentence for many records depending on each record existence
        /// </summary>
        /// <param name="tableName">The table name to update or insert the records</param>
        /// <param name="dataTable">The datatable containg the records to be updated or inserted</param>
        /// <param name="dataRowState">The datarow state on datatable to be considered</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 UpsertAll(String tableName, DataTable dataTable, DataRowState dataRowState);

        /// <summary>
        /// Execute a sql update or insert sentence for many records depending on each record existence
        /// </summary>
        /// <param name="tableName">The table name to update or insert the records</param>
        /// <param name="fields">The table fields to be included on the update or insert sentence</param>
        /// <param name="valuesList">The list of respective fields values to be updated or inserted on the table</param>
        /// <param name="keyFields">The table key fields</param>
        /// <param name="keyValuesList">The list of respective key fields values</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 UpsertAll(String tableName, String[] fields, List<Object[]> valuesList, String[] keyFields, List<Object[]> keyValuesList);

        /// <summary>
        /// Execute a sql delete sentence
        /// </summary>
        /// <param name="tableName">The table name to delete the record</param>
        /// <param name="dataRow">The datarow to be deleted</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 Delete(String tableName, DataRow dataRow);

        /// <summary>
        /// Execute a sql delete sentence
        /// </summary>
        /// <param name="tableName">The table name to delete the record</param>
        /// <param name="dataRow">The datarow to be deleted</param>
        /// <param name="dataRowState">The datarow state to be considered</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 Delete(String tableName, DataRow dataRow, DataRowState dataRowState);

        /// <summary>
        /// Execute a sql delete sentence
        /// </summary>
        /// <param name="tableName">The table name to delete the record</param>
        /// <param name="keyFields">The table key fields</param>
        /// <param name="keyValues">The respective key fields values</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 Delete(String tableName, String[] keyFields, Object[] keyValues);

        /// <summary>
        /// Execute a sql delete sentence for many records
        /// </summary>
        /// <param name="tableName">The table name to delete the records</param>
        /// <param name="dataTable">The datatable containg the records to be deleted</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 DeleteAll(String tableName, DataTable dataTable);

        /// <summary>
        /// Execute a sql delete sentence for many records
        /// </summary>
        /// <param name="tableName">The table name to delete the records</param>
        /// <param name="dataTable">The datatable containg the records to be deleted</param>
        /// <param name="dataRowState">The datarow state on datatable to be considered</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 DeleteAll(String tableName, DataTable dataTable, DataRowState dataRowState);

        /// <summary>
        /// Execute a sql delete sentence for many records
        /// </summary>
        /// <param name="tableName">The table name to delete the records</param>
        /// <param name="keyFields">The table key fields</param>
        /// <param name="keyValuesList">The list of respective key fields values</param>
        /// <returns>The number of affected records</returns>
        public abstract Int32 DeleteAll(String tableName, String[] keyFields, List<Object[]> keyValuesList);

        /// <summary>
        /// Increment a table field value by one
        /// </summary>
        /// <param name="tableName">The increment table</param>
        /// <param name="keyFields">The increment table key fields</param>
        /// <param name="keyValues">The increment table key values</param>
        /// <param name="incrementField">The increment field</param>
        /// <returns>The incremented value</returns>
        public abstract Int32 Increment(String tableName, String[] keyFields, Object[] keyValues, String incrementField);

        /// <summary>
        /// Increment a table field value by range
        /// </summary>
        /// <param name="tableName">The increment table</param>
        /// <param name="keyFields">The increment table key fields</param>
        /// <param name="keyValues">The increment table key values</param>
        /// <param name="incrementField">The increment field</param>
        /// <param name="range">The increment value</param>
        /// <returns>The incremented values</returns>
        public abstract Int32[] IncrementRange(String tableName, String[] keyFields, Object[] keyValues, String incrementField, Int32 range);

        /// <summary>
        /// Convert a value to a database string format
        /// </summary>
        /// <param name="value">The value to be converted to the string format</param>
        /// <returns>The string format of the value</returns>
        protected abstract String ConvertToDatabaseStringFormat(Object value);

        /// <summary>
        /// Convert a system type to a database type
        /// </summary>
        /// <param name="systemType">The system type to be converted</param>
        /// <returns>The database type</returns>
        protected abstract Int32 ConvertToDatabaseType(Type systemType);

        #endregion Methods

        #region Properties

        public String ConnectionString
        {
            get { return this.connectionString; }
            set { this.connectionString = value; }
        }

        public abstract Boolean InTransaction
        {
            get;
        }

        #endregion Properties
    }
}
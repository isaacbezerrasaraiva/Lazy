// LazyJsonConverterDataTable.cs
//
// This file is integrated part of Lazy project
// Licensed under "Gnu General Public License Version 3"
//
// Created by Isaac Bezerra Saraiva
// Created on 2021, January 01

using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Lazy.Json
{
    public class LazyJsonConverterDataTable : JsonConverter
    {
        #region Consts

        private const string ROW_STATE = "RowState";
        private const string ROW_STATE_ADDED = "Added";
        private const string ROW_STATE_MODIFIED = "Modified";
        private const string ROW_STATE_UNCHANGED = "Unchanged";
        private const string ROW_STATE_DELETED = "Deleted";
        private const string ORIGINAL_KEY = "OriginalKey";

        #endregion Consts

        #region Variables
        #endregion Variables

        #region Constructors

        public LazyJsonConverterDataTable()
        {
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Determines if this instance can convert the target DataSet
        /// </summary>
        /// <param name="valueType">Type of the DataSet object</param>
        /// <returns>The conversion capabilities</returns>
        public override Boolean CanConvert(Type valueType)
        {
            return typeof(DataTable).IsAssignableFrom(valueType);
        }

#nullable enable annotations

        /// <summary>
        /// Write json from object
        /// </summary>
        /// <param name="writer">The json writer</param>
        /// <param name="value">The object to parse to json</param>
        /// <param name="serializer">The json serializer</param>
        public override void WriteJson(JsonWriter writer, Object? value, JsonSerializer serializer)
        {
            #region Write null

            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            #endregion Write null

            DataTable dataTable = (DataTable)value;
            DefaultContractResolver? defaultContractResolver = serializer.ContractResolver as DefaultContractResolver;

            writer.WriteStartArray();

            foreach (DataRow dataRow in dataTable.Rows)
            {
                #region Write RowState

                writer.WriteStartObject();
                writer.WritePropertyName(ROW_STATE);

                switch (dataRow.RowState)
                {
                    case DataRowState.Added: writer.WriteValue(ROW_STATE_ADDED); break;
                    case DataRowState.Modified: writer.WriteValue(ROW_STATE_MODIFIED); break;
                    case DataRowState.Unchanged: writer.WriteValue(ROW_STATE_UNCHANGED); break;
                    case DataRowState.Deleted: writer.WriteValue(ROW_STATE_DELETED); break;
                }

                writer.WriteEndObject();

                #endregion Write RowState

                #region Write OriginalKey

                if (dataRow.RowState == DataRowState.Modified)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName(ORIGINAL_KEY);
                    writer.WriteStartObject();

                    foreach (DataColumn dataColumn in dataRow.Table.PrimaryKey)
                    {
                        writer.WritePropertyName(dataColumn.ColumnName);
                        writer.WriteValue(dataRow[dataColumn.ColumnName, DataRowVersion.Original]);
                    }

                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }

                #endregion Write OriginalKey

                #region Write columns

                writer.WriteStartObject();

                foreach (DataColumn column in dataRow.Table.Columns)
                {
                    Object columnValue = dataRow[column];

                    if (serializer.NullValueHandling == NullValueHandling.Ignore && (columnValue == null || columnValue == DBNull.Value))
                        continue;

                    writer.WritePropertyName((defaultContractResolver != null) ? defaultContractResolver.GetResolvedPropertyName(column.ColumnName) : column.ColumnName);
                    serializer.Serialize(writer, columnValue);
                }

                writer.WriteEndObject();

                #endregion Write columns
            }

            writer.WriteEndArray();
        }

        /// <summary>
        /// Read json to object
        /// </summary>
        /// <param name="reader">The json reader</param>
        /// <param name="objectType">The target object type</param>
        /// <param name="existingValue">The existing value of the target object</param>
        /// <param name="serializer">The json serializer</param>
        /// <returns>The object parsed from json</returns>
        public override Object? ReadJson(JsonReader reader, Type objectType, Object? existingValue, JsonSerializer serializer)
        {
            #region Read null

            if (reader.TokenType == JsonToken.Null)
                return null;

            #endregion Read null

            if (!(existingValue is DataTable dataTable))
                dataTable = (DataTable)Activator.CreateInstance(objectType);

            #region Read table name

            if (reader.TokenType == JsonToken.PropertyName)
            {
                dataTable.TableName = (String)reader.Value!;

                #region Read empty table

                reader.Read();

                if (reader.TokenType == JsonToken.Null)
                    return dataTable;

                #endregion Read empty table
            }

            #endregion Read table name

            #region Validate token

            if (reader.TokenType != JsonToken.StartArray)
                throw new JsonSerializationException(String.Format(Lazy.Json.Properties.Resources.LazyJsonTokenUnexpected, reader.TokenType));

            #endregion Validate token

            #region Read rows

            reader.Read();

            while (reader.TokenType != JsonToken.EndArray)
            {
                ReadRows(reader, dataTable, serializer);
                reader.Read();
            }

            #endregion Read rows

            return dataTable;
        }

        /// <summary>
        /// Read json to object
        /// </summary>
        /// <param name="reader">The json reader</param>
        /// <param name="dataTable">The datatable row destination</param>
        /// <param name="serializer">The json serializer</param>
        private static void ReadRows(JsonReader reader, DataTable dataTable, JsonSerializer serializer)
        {
            DataRow dataRow = dataTable.NewRow();
            reader.Read();

            #region Read RowState

            String rowState = null;

            if (reader.TokenType == JsonToken.PropertyName)
            {
                String propertyValue = ((String)reader.Value!).ToUpper();
                
                #region Validate RowState property missing
                
                if (propertyValue != ROW_STATE.ToUpper())
                    throw new JsonSerializationException(Lazy.Json.Properties.Resources.LazyJsonPropertyMissingRowState);

                #endregion Validate RowState property missing

                reader.Read(); // StartObject
                rowState = ((String)reader.Value!).ToUpper();
                reader.Read(); // EndObject
                reader.Read(); // StartObject
                reader.Read(); // PropertyName
            }

            #endregion Read RowState

            #region Read OriginalKey

            Dictionary<String, Object> originalKeyList = new Dictionary<String, Object>();

            if (rowState == ROW_STATE_MODIFIED.ToUpper())
            {
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    String propertyValue = ((String)reader.Value!).ToUpper();

                    #region Validate OriginalKey property missing

                    if (propertyValue != ORIGINAL_KEY.ToUpper())
                        throw new JsonSerializationException(Lazy.Json.Properties.Resources.LazyJsonPropertyMissingOriginalKey);

                    #endregion Validate OriginalKey property missing

                    reader.Read(); // StartObject
                    reader.Read(); // PropertyName

                    while (reader.TokenType != JsonToken.EndObject)
                    {
                        String originalKeyName = (String)reader.Value!;

                        reader.Read(); // PropertyValue
                        Object originalKeyValue = reader.Value;

                        originalKeyList.Add(originalKeyName, originalKeyValue);

                        reader.Read(); // PropertyName or EndObject if done
                    }

                    reader.Read(); // EndObject
                    reader.Read(); // StartObject
                    reader.Read(); // PropertyName
                }
            }

            #endregion Read OriginalKey

            #region Read row

            while (reader.TokenType == JsonToken.PropertyName)
            {
                String columnName = (String)reader.Value!;

                reader.Read();

                DataColumn dataColumn = dataTable.Columns[columnName];

                if (dataColumn == null)
                {
                    Type columnType = GetColumnDataType(reader);
                    dataColumn = new DataColumn(columnName, columnType);
                    dataTable.Columns.Add(dataColumn);
                }

                if (dataColumn.DataType == typeof(DataTable))
                {
                    if (reader.TokenType == JsonToken.StartArray)
                        reader.Read();

                    DataTable nestedDataTable = new DataTable();

                    while (reader.TokenType != JsonToken.EndArray)
                    {
                        ReadRows(reader, nestedDataTable, serializer);
                        reader.Read();
                    }

                    dataRow[columnName] = nestedDataTable;
                }
                else if (dataColumn.DataType.IsArray && dataColumn.DataType != typeof(Byte[]))
                {
                    if (reader.TokenType == JsonToken.StartArray)
                        reader.Read();

                    List<Object?> objectList = new List<Object?>();

                    while (reader.TokenType != JsonToken.EndArray)
                    {
                        objectList.Add(reader.Value);
                        reader.Read();
                    }

                    Array destinationArray = Array.CreateInstance(dataColumn.DataType.GetElementType(), objectList.Count);
                    ((IList)objectList).CopyTo(destinationArray, 0);

                    dataRow[columnName] = destinationArray;
                }
                else
                {
                    Object columnValue = (reader.Value != null) ? serializer.Deserialize(reader, dataColumn.DataType) ?? DBNull.Value : DBNull.Value;
                    dataRow[columnName] = columnValue;
                }

                reader.Read();
            }

            dataRow.EndEdit();
            dataTable.Rows.Add(dataRow);

            #endregion Read row

            #region Apply RowState

            dataRow.AcceptChanges();

            if (rowState == ROW_STATE_ADDED.ToUpper())
            {
                dataRow.SetAdded();
            }
            else if (rowState == ROW_STATE_MODIFIED.ToUpper())
            {
                #region Apply OriginalKey

                Dictionary<String, Object> currentKeyList = new Dictionary<String, Object>();

                foreach (KeyValuePair<String, Object> originalKey in originalKeyList)
                {
                    currentKeyList.Add(originalKey.Key, dataRow[originalKey.Key]);
                    dataRow[originalKey.Key] = originalKey.Value;
                }

                dataRow.AcceptChanges();

                foreach (KeyValuePair<String, Object> currentKey in currentKeyList)
                    dataRow[currentKey.Key] = currentKey.Value;

                #endregion Apply OriginalKey

                if (dataRow.RowState == DataRowState.Unchanged)
                    dataRow.SetModified();
            }
            else if (rowState == ROW_STATE_DELETED.ToUpper())
            {
                dataRow.Delete();
            }

            #endregion Apply RowState
        }

#nullable disable annotations

        /// <summary>
        /// Get column type
        /// </summary>
        /// <param name="reader">The json reader</param>
        /// <returns>The column type</returns>
        private static Type GetColumnDataType(JsonReader reader)
        {
            JsonToken tokenType = reader.TokenType;

            switch (tokenType)
            {
                case JsonToken.Integer:
                case JsonToken.Boolean:
                case JsonToken.Float:
                case JsonToken.String:
                case JsonToken.Date:
                case JsonToken.Bytes:
                    return reader.ValueType!;

                case JsonToken.Null:
                case JsonToken.Undefined:
                case JsonToken.EndArray:
                    return typeof(String);

                case JsonToken.StartArray:
                    reader.Read();
                    if (reader.TokenType == JsonToken.StartObject)
                        return typeof(DataTable);
                    Type arrayType = GetColumnDataType(reader);
                    return arrayType.MakeArrayType();

                default:
                    throw new JsonSerializationException(String.Format(Lazy.Json.Properties.Resources.LazyJsonTokenUnexpected, tokenType));
            }
        }

        #endregion Methods

        #region Properties
        #endregion Properties
    }
}
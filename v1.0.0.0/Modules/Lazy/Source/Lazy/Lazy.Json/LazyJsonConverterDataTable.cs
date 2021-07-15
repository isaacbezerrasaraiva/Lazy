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
        private const string ROW_PROPS = "Props";
        private const string ROW_DATA = "Data";
        private const string ROW_DATA_ORIGINAL = "Original";
        private const string ROW_DATA_CURRENT = "Current";

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
                writer.WriteStartObject();

                #region Write Props

                writer.WritePropertyName(ROW_PROPS);
                writer.WriteStartObject();

                #region Write RowState

                writer.WritePropertyName(ROW_STATE);

                switch (dataRow.RowState)
                {
                    case DataRowState.Added: writer.WriteValue(ROW_STATE_ADDED); break;
                    case DataRowState.Modified: writer.WriteValue(ROW_STATE_MODIFIED); break;
                    case DataRowState.Unchanged: writer.WriteValue(ROW_STATE_UNCHANGED); break;
                    case DataRowState.Deleted: writer.WriteValue(ROW_STATE_DELETED); break;
                }

                #endregion Write RowState

                writer.WriteEndObject();

                #endregion Write Props

                #region Write Data

                writer.WritePropertyName(ROW_DATA);
                writer.WriteStartObject();

                #region Write Data Original

                writer.WritePropertyName(ROW_DATA_ORIGINAL);
                writer.WriteStartObject();

                if (dataRow.RowState == DataRowState.Modified)
                {
                    foreach (DataColumn dataColumn in dataRow.Table.PrimaryKey)
                    {
                        writer.WritePropertyName(dataColumn.ColumnName);
                        writer.WriteValue(dataRow[dataColumn.ColumnName, DataRowVersion.Original]);
                    }
                }

                writer.WriteEndObject();

                #endregion Write Data Original

                #region Write Data Current

                writer.WritePropertyName(ROW_DATA_CURRENT);
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

                #endregion Write Data Current

                writer.WriteEndObject();

                #endregion Write Data

                writer.WriteEndObject();
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
            if (reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType != JsonToken.PropertyName)
                throw new JsonSerializationException(String.Format(Lazy.Json.Properties.Resources.LazyJsonTokenUnexpected, reader.TokenType));

            if (!(existingValue is DataTable dataTable))
                dataTable = (DataTable)Activator.CreateInstance(objectType);
            dataTable.TableName = (String)reader.Value!;

            reader.Read(); // StartArray
            if (reader.TokenType == JsonToken.Null)
                return dataTable;

            // Validate table start array
            if (reader.TokenType != JsonToken.StartArray)
                throw new JsonSerializationException(String.Format(Lazy.Json.Properties.Resources.LazyJsonTokenUnexpected, reader.TokenType));

            reader.Read(); // StartObject or EndArray
            if (reader.TokenType == JsonToken.EndArray)
                return dataTable;

            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonSerializationException(String.Format(Lazy.Json.Properties.Resources.LazyJsonTokenUnexpected, reader.TokenType));

            while (reader.TokenType != JsonToken.EndArray)
                ReadRows(reader, dataTable, serializer);

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
            String propertyName = null;
            Object propertyValue = null;

            String rowState = null;
            Dictionary<String, Object> originalData = null;
            Dictionary<String, Object> currentData = null;

            reader.Read(); // PropertyName or EndObject
            while (reader.TokenType != JsonToken.EndObject)
            {
                if (reader.TokenType != JsonToken.PropertyName)
                    throw new JsonSerializationException(String.Format(Lazy.Json.Properties.Resources.LazyJsonTokenUnexpected, reader.TokenType));

                propertyName = ((String)reader.Value!).ToUpper();

                if (propertyName == ROW_PROPS.ToUpper())
                {
                    reader.Read(); // StartObject
                    if (reader.TokenType != JsonToken.StartObject)
                        throw new JsonSerializationException(String.Format(Lazy.Json.Properties.Resources.LazyJsonTokenUnexpected, reader.TokenType));

                    reader.Read(); // PropertyName or EndObject
                    while (reader.TokenType != JsonToken.EndObject)
                    {
                        if (reader.TokenType != JsonToken.PropertyName)
                            throw new JsonSerializationException(String.Format(Lazy.Json.Properties.Resources.LazyJsonTokenUnexpected, reader.TokenType));

                        propertyName = ((String)reader.Value!).ToUpper();

                        if (propertyName == ROW_STATE.ToUpper())
                        {
                            #region Read row state

                            reader.Read(); // PropertyValue
                            if (reader.TokenType == JsonToken.Null)
                                throw new JsonSerializationException(String.Format(Lazy.Json.Properties.Resources.LazyJsonTokenUnexpected, reader.TokenType));

                            rowState = ((String)reader.Value!).ToUpper();

                            #endregion Read row state
                        }

                        reader.Read(); // PropertyName or EndObject
                    }
                }
                else if (propertyName == ROW_DATA.ToUpper())
                {
                    reader.Read(); // StartObject
                    if (reader.TokenType != JsonToken.StartObject)
                        throw new JsonSerializationException(String.Format(Lazy.Json.Properties.Resources.LazyJsonTokenUnexpected, reader.TokenType));

                    reader.Read(); // PropertyName or EndObject
                    while (reader.TokenType != JsonToken.EndObject)
                    {
                        if (reader.TokenType != JsonToken.PropertyName)
                            throw new JsonSerializationException(String.Format(Lazy.Json.Properties.Resources.LazyJsonTokenUnexpected, reader.TokenType));

                        propertyName = ((String)reader.Value!).ToUpper();

                        if (propertyName == ROW_DATA_ORIGINAL.ToUpper())
                        {
                            #region Read original data

                            originalData = new Dictionary<String, Object>();

                            reader.Read(); // StartObject
                            if (reader.TokenType != JsonToken.StartObject)
                                throw new JsonSerializationException(String.Format(Lazy.Json.Properties.Resources.LazyJsonTokenUnexpected, reader.TokenType));

                            reader.Read(); // PropertyName or EndObject
                            while (reader.TokenType != JsonToken.EndObject)
                            {
                                if (reader.TokenType != JsonToken.PropertyName)
                                    throw new JsonSerializationException(String.Format(Lazy.Json.Properties.Resources.LazyJsonTokenUnexpected, reader.TokenType));

                                propertyName = (String)reader.Value!;
                                reader.Read(); // PropertyValue
                                propertyValue = ReadPropertyValue(reader);
                                reader.Read(); // PropertyName or EndObject

                                originalData.Add(propertyName, propertyValue);
                            }

                            #endregion Read original data
                        }
                        else if (propertyName == ROW_DATA_CURRENT.ToUpper())
                        {
                            #region Read current data

                            currentData = new Dictionary<String, Object>();

                            reader.Read(); // StartObject
                            if (reader.TokenType != JsonToken.StartObject)
                                throw new JsonSerializationException(String.Format(Lazy.Json.Properties.Resources.LazyJsonTokenUnexpected, reader.TokenType));

                            reader.Read(); // PropertyName or EndObject
                            while (reader.TokenType != JsonToken.EndObject)
                            {
                                if (reader.TokenType != JsonToken.PropertyName)
                                    throw new JsonSerializationException(String.Format(Lazy.Json.Properties.Resources.LazyJsonTokenUnexpected, reader.TokenType));

                                propertyName = (String)reader.Value!;
                                reader.Read(); // PropertyValue
                                propertyValue = ReadPropertyValue(reader);
                                reader.Read(); // PropertyName or EndObject

                                currentData.Add(propertyName, propertyValue);
                            }

                            #endregion Read current data
                        }
                        else
                        {
                            throw new JsonSerializationException(String.Format(Lazy.Json.Properties.Resources.LazyJsonTokenExpected, ROW_DATA_ORIGINAL + " or " + ROW_DATA_CURRENT));
                        }

                        reader.Read(); // PropertyName or EndObject
                    }
                }
                else
                {
                    throw new JsonSerializationException(String.Format(Lazy.Json.Properties.Resources.LazyJsonTokenExpected, ROW_PROPS + " or " + ROW_DATA));
                }

                reader.Read(); // PropertyName or EndObject
            }

            reader.Read(); // StartObject or EndArray

            if (rowState != ROW_STATE_ADDED.ToUpper() && rowState != ROW_STATE_MODIFIED.ToUpper() && rowState != ROW_STATE_DELETED.ToUpper() && rowState != ROW_STATE_UNCHANGED.ToUpper())
                throw new JsonSerializationException(Lazy.Json.Properties.Resources.LazyJsonPropertyMissingRowState);

            DataRow dataRow = dataTable.NewRow();
            dataTable.Rows.Add(dataRow);
            dataRow.AcceptChanges();

            if (rowState == ROW_STATE_MODIFIED.ToUpper())
            {
                if (originalData == null)
                    throw new JsonSerializationException(Lazy.Json.Properties.Resources.LazyJsonPropertyMissingOriginalData);

                foreach (KeyValuePair<String, Object> item in originalData)
                {
                    if (dataTable.Columns.Contains(item.Key) == false)
                        dataTable.Columns.Add(new DataColumn(item.Key, item.Value.GetType()));

                    dataRow[item.Key] = item.Value;
                }

                dataRow.AcceptChanges();
            }

            foreach (KeyValuePair<String, Object> item in currentData)
            {
                if (dataTable.Columns.Contains(item.Key) == false)
                    dataTable.Columns.Add(new DataColumn(item.Key, item.Value.GetType()));

                dataRow[item.Key] = item.Value;
            }

            if (rowState == ROW_STATE_ADDED.ToUpper())
            {
                dataRow.AcceptChanges();
                dataRow.SetAdded();
            }
            else if (rowState == ROW_STATE_DELETED.ToUpper())
            {
                dataRow.AcceptChanges();
                dataRow.Delete();
            }
            else if (rowState == ROW_STATE_UNCHANGED.ToUpper())
            {
                dataRow.AcceptChanges();
            }
        }

#nullable disable annotations

        /// <summary>
        /// Read property value
        /// </summary>
        /// <param name="reader">The json reader</param>
        /// <returns>The property value</returns>
        private static Object ReadPropertyValue(JsonReader reader)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Integer: return (Int64)reader.Value;
                case JsonToken.Boolean: return (Boolean)reader.Value;
                case JsonToken.Float: return (Double)reader.Value;
                case JsonToken.String: return (String)reader.Value;
                case JsonToken.Date: return (DateTime)reader.Value;
                case JsonToken.Bytes: return (Byte[])reader.Value;
                default: return DBNull.Value;
            }
        }

        #endregion Methods

        #region Properties
        #endregion Properties
    }
}
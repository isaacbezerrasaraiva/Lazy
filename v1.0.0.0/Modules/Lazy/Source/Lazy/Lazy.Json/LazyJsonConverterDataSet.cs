// LazyJsonConverterDataSet.cs
//
// This file is integrated part of Lazy project
// Licensed under "Gnu General Public License Version 3"
//
// Created by Isaac Bezerra Saraiva
// Created on 2021, January 01

using System;
using System.Data;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Lazy.Json
{
    public class LazyJsonConverterDataSet : JsonConverter
    {
        #region Variables
        #endregion Variables

        #region Constructors

        public LazyJsonConverterDataSet()
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
            return typeof(DataSet).IsAssignableFrom(valueType);
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

            DataSet dataSet = (DataSet)value;
            DefaultContractResolver? defaultContractResolver = serializer.ContractResolver as DefaultContractResolver;
            LazyJsonConverterDataTable jsonConverterDataTable = new LazyJsonConverterDataTable();

            writer.WriteStartObject();

            foreach (DataTable table in dataSet.Tables)
            {
                writer.WritePropertyName((defaultContractResolver != null) ? defaultContractResolver.GetResolvedPropertyName(table.TableName) : table.TableName);
                jsonConverterDataTable.WriteJson(writer, table, serializer);
            }

            writer.WriteEndObject();
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

            // Allow handle typed DataSets
            DataSet dataSet = (DataSet)Activator.CreateInstance(objectType);

            LazyJsonConverterDataTable jsonConverterDataTable = new LazyJsonConverterDataTable();

            reader.Read();

            while (reader.TokenType == JsonToken.PropertyName)
            {
                DataTable dataTable = dataSet.Tables[(String)reader.Value!];
                Boolean exists = (dataTable != null);

                // Must parse even if DataTable already exists on DataSet to advance the reader of json
                dataTable = (DataTable)jsonConverterDataTable.ReadJson(reader, typeof(DataTable), dataTable, serializer)!;

                if (exists == false)
                    dataSet.Tables.Add(dataTable);

                reader.Read();
            }

            return dataSet;
        }

#nullable disable annotations

        #endregion Methods

        #region Properties
        #endregion Properties
    }
}

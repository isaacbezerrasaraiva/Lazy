// LazyDatabaseQuery.cs
//
// This file is integrated part of Lazy project
// Licensed under "Gnu General Public License Version 3"
//
// Created by Isaac Bezerra Saraiva
// Created on 2020, December 01

using System;
using System.Xml;
using System.Data;
using System.Text;
using System.Collections.Generic;

namespace Lazy.Database
{
    public class LazyDatabaseQuery
    {
        #region Variables
        #endregion Variables

        #region Constructors

        public LazyDatabaseQuery()
        {
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Replace the old string value to the new string value on a sql sentence
        /// </summary>
        /// <param name="sql">The sql sentence to be replaced</param>
        /// <param name="oldValue">The old string value to be replaced</param>
        /// <param name="newValue">The new string value to replace</param>
        /// <param name="skipQuotes">Skip values on quotes (') or double quotes (")</param>
        /// <returns>The sql sentence with the replaced value</returns>
        public static String Replace(String sql, String oldValue, String newValue, Boolean skipQuotes = true)
        {
            #region Validations

            if (String.IsNullOrEmpty(sql) == true)
                return sql;

            if (String.IsNullOrEmpty(oldValue) == true)
                return sql;

            if (newValue == null)
                newValue = String.Empty;

            #endregion Validations

            StringBuilder sqlBuilder = new StringBuilder();

            for (int sqlIndex = 0; sqlIndex < sql.Length; sqlIndex++)
            {
                if (skipQuotes == true)
                {
                    if (sql[sqlIndex] == '\'')
                    {
                        sqlIndex++;
                        while (sqlIndex < sql.Length && sql[sqlIndex] != '\'')
                            sqlIndex++;
                        continue;
                    }

                    if (sql[sqlIndex] == '\"')
                    {
                        sqlIndex++;
                        while (sqlIndex < sql.Length && sql[sqlIndex] != '\"')
                            sqlIndex++;
                        continue;
                    }
                }

                if (oldValue.Length <= (sql.Length - sqlIndex))
                {
                    if (sql.Substring(sqlIndex, oldValue.Length) == oldValue)
                    {
                        sqlBuilder.Append(sql.Substring(0, sqlIndex));
                        sqlBuilder.Append(newValue);

                        sql = sql.Substring(sqlIndex + oldValue.Length, sql.Length - (sqlIndex + oldValue.Length));

                        sqlIndex = 0;
                    }
                }
            }

            sqlBuilder.Append(sql);
            sql = sqlBuilder.ToString();
            sqlBuilder = null;

            return sql;
        }

        /// <summary>
        /// Replace the old string values to the new string values on a sql sentence
        /// </summary>
        /// <param name="sql">The sql sentence to be replaced</param>
        /// <param name="oldValues">The old string values to be replaced</param>
        /// <param name="newValues">The new string values to replace</param>
        /// <param name="skipQuotes">Skip values on quotes (') or double quotes (")</param>
        /// <returns>The sql sentence with the replaced values</returns>
        public static String Replace(String sql, String[] oldValues, String[] newValues, Boolean skipQuotes = true)
        {
            #region Validations

            if (String.IsNullOrEmpty(sql) == true)
                return sql;

            if (oldValues == null || oldValues.Length == 0)
                return sql;

            if (newValues == null)
                newValues = new String[] { };

            #endregion Validations

            StringBuilder sqlBuilder = new StringBuilder();

            for (int sqlIndex = 0; sqlIndex < sql.Length; sqlIndex++)
            {
                if (skipQuotes == true)
                {
                    if (sql[sqlIndex] == '\'')
                    {
                        sqlIndex++;
                        while (sqlIndex < sql.Length && sql[sqlIndex] != '\'')
                            sqlIndex++;
                        continue;
                    }

                    if (sql[sqlIndex] == '\"')
                    {
                        sqlIndex++;
                        while (sqlIndex < sql.Length && sql[sqlIndex] != '\"')
                            sqlIndex++;
                        continue;
                    }
                }

                for (int valueIndex = 0; valueIndex < oldValues.Length; valueIndex++)
                {
                    if (oldValues[valueIndex].Length <= (sql.Length - sqlIndex))
                    {
                        if (sql.Substring(sqlIndex, oldValues[valueIndex].Length) == oldValues[valueIndex])
                        {
                            sqlBuilder.Append(sql.Substring(0, sqlIndex));
                            sqlBuilder.Append(valueIndex < newValues.Length ? newValues[valueIndex] : String.Empty);

                            sql = sql.Substring(sqlIndex + oldValues[valueIndex].Length, sql.Length - (sqlIndex + oldValues[valueIndex].Length));

                            sqlIndex = 0;

                            break;
                        }
                    }
                }
            }

            sqlBuilder.Append(sql);
            sql = sqlBuilder.ToString();
            sqlBuilder = null;

            return sql;
        }

        #endregion Methods

        #region Properties
        #endregion Properties

        #region InternalClass

        public static class Parameter
        {
            #region Variables
            #endregion Variables

            #region Methods

            /// <summary>
            /// Extract parameters from a sql sentence
            /// </summary>
            /// <param name="sql">The sql sentence to extract the parameters</param>
            /// <param name="pChar">The sql sentence parameter character identification</param>
            /// <returns>The extracted parameters</returns>
            public static String[] Extract(String sql, Char pChar = ':')
            {
                #region Validations

                if (String.IsNullOrEmpty(sql) == true)
                    return null;

                #endregion Validations

                List<String> parameterList = new List<String>();

                for (int sqlIndex = 0; sqlIndex < sql.Length; sqlIndex++)
                {
                    if (sql[sqlIndex] == '\'')
                    {
                        sqlIndex++;
                        while (sqlIndex < sql.Length && sql[sqlIndex] != '\'')
                            sqlIndex++;
                        continue;
                    }

                    if (sql[sqlIndex] == '\"')
                    {
                        sqlIndex++;
                        while (sqlIndex < sql.Length && sql[sqlIndex] != '\"')
                            sqlIndex++;
                        continue;
                    }

                    if (sql[sqlIndex] == pChar)
                    {
                        sqlIndex++;
                        if (sqlIndex < sql.Length && Char.IsLetterOrDigit(sql[sqlIndex]) == true)
                        {
                            sqlIndex++;
                            int parameterIndex = sqlIndex;
                            for (parameterIndex = sqlIndex; parameterIndex < sql.Length; parameterIndex++)
                            {
                                if (Char.IsLetterOrDigit(sql[parameterIndex]) == true)
                                    continue;
                                else
                                    break;
                            }

                            String parameter = sql.Substring(sqlIndex - 1, parameterIndex - (sqlIndex - 1));
                            if (parameterList.Contains(parameter) == false)
                                parameterList.Add(parameter);

                            sqlIndex = (parameterIndex - 1);
                        }
                    }
                }

                if (parameterList.Count == 0)
                    return null;

                return parameterList.ToArray();
            }

            #endregion Methods

            #region Properties
            #endregion Properties
        }

        #endregion InternalClass
    }
}
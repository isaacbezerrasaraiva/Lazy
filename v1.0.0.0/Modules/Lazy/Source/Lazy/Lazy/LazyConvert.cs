// LazyConvert.cs
//
// This file is integrated part of Lazy project
// Licensed under "Gnu General Public License Version 3"
//
// Created by Isaac Bezerra Saraiva
// Created on 2020, December 10

using System;
using System.Xml;
using System.Data;

namespace Lazy
{
    public static class LazyConvert
    {
        /// <summary>
        /// Try convert an Object value to Int16 value
        /// </summary>
        /// <param name="value">The Object value to convert</param>
        /// <returns>The converted Int16 value</returns>
        public static Int16 ToInt16(Object value)
        {
            return Convert.ToInt16(value);
        }

        /// <summary>
        /// Try convert an Object value to Int16 value
        /// </summary>
        /// <param name="value">The Object value to convert</param>
        /// <param name="failValue">The Int16 value to return if fail to convert</param>
        /// <param name="nullAsZero">Indicate if null value will be considered as zero</param>
        /// <returns>The converted Int16 value or the fail Int16 value</returns>
        public static Int16 ToInt16(Object value, Int16 failValue, Boolean nullAsZero = false)
        {
            if (value == null && nullAsZero == false)
                return failValue;

            try { return Convert.ToInt16(value); }
            catch { return failValue; }
        }

        /// <summary>
        /// Try convert an Object value to Int32 value
        /// </summary>
        /// <param name="value">The Object value to convert</param>
        /// <returns>The converted Int32 value</returns>
        public static Int32 ToInt32(Object value)
        {
            return Convert.ToInt32(value);
        }

        /// <summary>
        /// Try convert an Object value to Int32 value
        /// </summary>
        /// <param name="value">The Object value to convert</param>
        /// <param name="failValue">The Int32 value to return if fail to convert</param>
        /// <param name="nullAsZero">Indicate if null value will be considered as zero</param>
        /// <returns>The converted Int32 value or the fail Int32 value</returns>
        public static Int32 ToInt32(Object value, Int32 failValue, Boolean nullAsZero = false)
        {
            if (value == null && nullAsZero == false)
                return failValue;

            try { return Convert.ToInt32(value); }
            catch { return failValue; }
        }

        /// <summary>
        /// Try convert an Object value to String value
        /// </summary>
        /// <param name="value">The Object value to convert</param>
        /// <returns>The converted String value</returns>
        public static String ToString(Object value)
        {
            return Convert.ToString(value);
        }

        /// <summary>
        /// Try convert an Object value to String value
        /// </summary>
        /// <param name="value">The Object value to convert</param>
        /// <param name="failValue">The String value to return if fail to convert</param>
        /// <returns>The converted String value or the fail String value</returns>
        public static String ToString(Object value, String failValue)
        {
            try { return Convert.ToString(value); }
            catch { return failValue; }
        }
    }
}
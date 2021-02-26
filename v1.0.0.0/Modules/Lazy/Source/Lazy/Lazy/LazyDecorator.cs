// LazyDecorator.cs
//
// This file is integrated part of Lazy project
// Licensed under "Gnu General Public License Version 3"
//
// Created by Isaac Bezerra Saraiva
// Created on 2021, January 09

using System;
using System.Xml;
using System.Data;
using System.Reflection;

namespace Lazy
{
    public class LazyDecorator : Attribute
    {
        #region Variables
        #endregion Variables

        #region Constructors

        public LazyDecorator(String code, String name)
        {
            this.Code = code;
            this.Name = name;
        }

        public LazyDecorator(String code, String name, Object data)
        {
            this.Code = code;
            this.Name = name;
            this.Data = data;
        }

        #endregion Constructors

        #region Methods

        public static LazyDecorator GetCustomAttributeFromClass(Type type, Int32 index = 0)
        {
            Object[] attributeArray = type.GetCustomAttributes(typeof(LazyDecorator), false);

            if (attributeArray.Length > index)
                return (LazyDecorator)attributeArray[index];

            return null;
        }

        public static LazyDecorator GetCustomAttributeFromClassField(Type type, String field, Int32 index = 0)
        {
            FieldInfo fieldInfo = type.GetField(field, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            if (fieldInfo != null)
            {
                Object[] attributeArray = fieldInfo.GetCustomAttributes(typeof(LazyDecorator), false);

                if (attributeArray.Length > index)
                    return (LazyDecorator)attributeArray[index];
            }

            return null;
        }

        public static LazyDecorator GetCustomAttributeFromEnum(Type type, Int32 index = 0)
        {
            return GetCustomAttributeFromClass(type, index);
        }

        public static LazyDecorator GetCustomAttributeFromEnumValue(Object value, Int32 index = 0)
        {
            return GetCustomAttributeFromClassField(value.GetType(), Enum.GetName(value.GetType(), value), index);
        }

        #endregion Methods

        #region Properties

        public String Code { get; }

        public String Name { get; }

        public Object Data { get; }

        #endregion Properties
    }
}
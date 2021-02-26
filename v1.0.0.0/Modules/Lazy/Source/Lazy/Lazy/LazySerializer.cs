// LazySerializer.cs
//
// This file is integrated part of Lazy project
// Licensed under "Gnu General Public License Version 3"
//
// Created by Isaac Bezerra Saraiva
// Created on 2021, February 22

using System;
using System.IO;
using System.Xml;
using System.Data;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace Lazy
{
    public static class LazySerializer
    {
        public static class Binary
        {
            #region Variables
            #endregion Variables

            #region Methods

            public static String Serialize(Object source)
            {
                if (source == null)
                    throw new Exception(Lazy.Properties.Resources.LazyExceptionSourceObjectNull);

                String destine = null;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(memoryStream, source);
                    binaryFormatter = null;

                    destine = Convert.ToBase64String(memoryStream.ToArray());
                }

                return destine;
            }

            public static Object Deserialize(String source)
            {
                if (String.IsNullOrEmpty(source) == true)
                    throw new Exception(Lazy.Properties.Resources.LazyExceptionSourceStringNull);

                Object destine = null;
                using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(source)))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    destine = binaryFormatter.Deserialize(memoryStream);
                    binaryFormatter = null;
                }

                return destine;
            }

            public static Object Clone(Object source)
            {
                if (source == null)
                    throw new Exception(Lazy.Properties.Resources.LazyExceptionSourceObjectNull);

                Object destine = null;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(memoryStream, source);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    destine = binaryFormatter.Deserialize(memoryStream);
                    binaryFormatter = null;
                }
                
                return destine;
            }

            #endregion Methods

            #region Properties
            #endregion Properties
        }
    }
}
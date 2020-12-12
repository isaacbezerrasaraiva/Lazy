﻿// LazyActivator.cs
//
// This file is integrated part of Ark project
// Licensed under "Gnu General Public License Version 3"
//
// Created by Isaac Bezerra Saraiva
// Created on 2020, November 19

using System;
using System.Xml;
using System.Data;
using System.Reflection;

namespace Lazy
{
    public static class LazyActivator
    {
        public static class Local
        {
            /// <summary>
            /// Create an object instance of the specified class located on specified assembly
            /// </summary>
            /// <param name="assemblyPath">The assembly path witch contains the desired class</param>
            /// <param name="classFullName">The class name with its namespace</param>
            /// <returns>The object instance</returns>
            public static Object CreateInstance(String assemblyPath, String classFullName)
            {
                return Activator.CreateInstance(Assembly.LoadFrom(assemblyPath).GetType(classFullName));
            }

            /// <summary>
            /// Create an object instance of the specified class located on specified assembly
            /// </summary>
            /// <param name="assemblyPath">The assembly path witch contains the desired class</param>
            /// <param name="classFullName">The class name with its namespace</param>
            /// <param name="parameters">The parameters of desired object constructor</param>
            /// <returns>The object instance</returns>
            public static Object CreateInstance(String assemblyPath, String classFullName, Object[] parameters)
            {
                return Activator.CreateInstance(Assembly.LoadFrom(assemblyPath).GetType(classFullName), parameters);
            }
        }
    }
}
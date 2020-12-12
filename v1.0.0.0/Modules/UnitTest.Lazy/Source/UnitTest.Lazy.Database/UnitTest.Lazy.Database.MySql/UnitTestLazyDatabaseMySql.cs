// UnitTestLazyDatabaseMySql.cs
//
// This file is integrated part of Lazy project
// Licensed under "Gnu General Public License Version 3"
//
// Created by Isaac Bezerra Saraiva
// Created on 2020, December 01

using System;
using System.Xml;
using System.Data;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Lazy;
using Lazy.Database;
using Lazy.Database.MySql;

namespace UnitTest.Lazy.Database.MySql
{
    [TestClass]
    public class UnitTestLazyDatabaseMySql
    {
        private const string CONNECTION_STRING = "Server=localhost;Database=UnitTestLazy;Uid=unittestlazy;Pwd=unittestlazy;";

        [TestMethod]
        public void TestOpenConnection()
        {
            // Arrange
            LazyDatabaseMySql databaseMySql = new LazyDatabaseMySql(CONNECTION_STRING);

            // Act
            databaseMySql.OpenConnection();

            // Assert
            try { databaseMySql.OpenConnection(); Assert.Fail(); }
            catch (Exception exp) { Assert.AreEqual(exp.Message, global::Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionAlreadyOpen); }

            // Finalize
            try { databaseMySql.CloseConnection(); }
            catch { /* Just grant that the connection was closed */}
        }

        [TestMethod]
        public void TestCloseConnection()
        {
            // Arrange
            LazyDatabaseMySql databaseMySql = new LazyDatabaseMySql(CONNECTION_STRING);

            // Act
            databaseMySql.OpenConnection();
            databaseMySql.CloseConnection();

            // Assert
            try { databaseMySql.CloseConnection(); }
            catch (Exception exp) { Assert.AreEqual(exp.Message, global::Lazy.Database.Properties.Resources.LazyDatabaseExceptionConnectionAlreadyClosed); }
        }

        [TestMethod]
        public void TestBeginTransaction()
        {
            // Arrange
            LazyDatabaseMySql databaseMySql = new LazyDatabaseMySql(CONNECTION_STRING);

            // Act
            databaseMySql.OpenConnection();
            databaseMySql.BeginTransaction();

            // Assert
            Assert.AreEqual(databaseMySql.InTransaction, true);

            // Finalize
            try { databaseMySql.CloseConnection(); }
            catch { /* Just grant that the connection was closed */}
        }

        [TestMethod]
        public void TestCommitTransaction()
        {
            // Arrange
            LazyDatabaseMySql databaseMySql = new LazyDatabaseMySql(CONNECTION_STRING);

            // Act
            databaseMySql.OpenConnection();
            databaseMySql.BeginTransaction();

            String sql = "select max(IdTest) from TestCommitTransaction";
            Int32 IdTest = (Int32)databaseMySql.QueryValue(sql, null) + 1;

            sql = "insert into TestCommitTransaction (IdTest, SomeTestData) values (:IdTest, :SomeTestData)";
            databaseMySql.QueryExecute(sql, new Object[] { IdTest, "SomeTestData" + IdTest });

            databaseMySql.CommitTransaction();

            sql = "select 1 from TestCommitTransaction where IdTest = :IdTest";
            Boolean recordCommited = databaseMySql.QueryFind(sql, new Object[] { IdTest });

            // Assert
            Assert.AreEqual(recordCommited, true);

            // Finalize
            try { databaseMySql.CloseConnection(); }
            catch { /* Just grant that the connection was closed */}
        }

        [TestMethod]
        public void TestRollbackTransaction()
        {
            // Arrange
            LazyDatabaseMySql databaseMySql = new LazyDatabaseMySql(CONNECTION_STRING);

            // Act
            databaseMySql.OpenConnection();
            databaseMySql.BeginTransaction();

            String sql = "select max(IdTest) from TestRollbackTransaction";
            Int32 IdTest = (Int32)databaseMySql.QueryValue(sql, null) + 1;

            sql = "insert into TestRollbackTransaction (IdTest, SomeTestData) values (:IdTest, :SomeTestData)";
            databaseMySql.QueryExecute(sql, new Object[] { IdTest, "SomeTestData" + IdTest });

            databaseMySql.RollbackTransaction();

            sql = "select 1 from TestRollbackTransaction where IdTest = :IdTest";
            Boolean recordCommited = databaseMySql.QueryFind(sql, new Object[] { IdTest });

            // Assert
            Assert.AreEqual(recordCommited, false);

            // Finalize
            try { databaseMySql.CloseConnection(); }
            catch { /* Just grant that the connection was closed */}
        }

        [TestMethod]
        public void TestQueryExecute()
        {
            // Arrange
            LazyDatabaseMySql databaseMySql = new LazyDatabaseMySql(CONNECTION_STRING);
            databaseMySql.OpenConnection();

            // Act
            String sql = "insert into TestQueryExecute (IdTest, SomeTestData) values (:IdTest, :SomeTestData)";
            databaseMySql.QueryExecute(sql, new Object[] { 4, "SomeTestDataWithId4" });
            databaseMySql.QueryExecute(sql, new Object[] { 5, "SomeTestDataWithId5" });
            databaseMySql.QueryExecute(sql, new Object[] { 6, "SomeTestDataWithId6" });

            sql = "delete from TestQueryExecute where IdTest = :IdTest";
            databaseMySql.QueryExecute(sql, new Object[] { 5 });

            sql = "select 1 from TestQueryExecute where IdTest = :IdTest";
            Boolean recordExists4 = databaseMySql.QueryFind(sql, new Object[] { 4 });
            Boolean recordExists5 = databaseMySql.QueryFind(sql, new Object[] { 5 });
            Boolean recordExists6 = databaseMySql.QueryFind(sql, new Object[] { 6 });

            // Clear data to avoid primary key errors on nexts executions
            sql = "delete from TestQueryExecute where IdTest = :IdTest";
            databaseMySql.QueryExecute(sql, new Object[] { 4 });
            databaseMySql.QueryExecute(sql, new Object[] { 6 });

            // Assert
            Assert.AreEqual(recordExists4, true);
            Assert.AreEqual(recordExists5, false);
            Assert.AreEqual(recordExists6, true);

            // Finalize
            try { databaseMySql.CloseConnection(); }
            catch { /* Just grant that the connection was closed */}
        }

        [TestMethod]
        public void TestQueryValue()
        {
            // Arrange
            LazyDatabaseMySql databaseMySql = new LazyDatabaseMySql(CONNECTION_STRING);
            databaseMySql.OpenConnection();

            // Act
            String sql = "select IdTest from TestQueryValue where SomeTestData = :SomeTestData";
            Int32 IdTest = (Int32)databaseMySql.QueryValue(sql, new Object[] { "SomeTestDataWithId2" });

            sql = "select SomeTestData from TestQueryValue where IdTest = :IdTest";
            String SomeTestData = (String)databaseMySql.QueryValue(sql, new Object[] { 3 });

            // Assert
            Assert.AreEqual(IdTest, 2);
            Assert.AreEqual(SomeTestData, "SomeTestDataWithId3");

            // Finalize
            try { databaseMySql.CloseConnection(); }
            catch { /* Just grant that the connection was closed */}
        }

        [TestMethod]
        public void TestQueryFind()
        {
            // Arrange
            LazyDatabaseMySql databaseMySql = new LazyDatabaseMySql(CONNECTION_STRING);
            databaseMySql.OpenConnection();

            // Act
            String sql = "select 1 from TestQueryFind where IdTest = :IdTest";
            Boolean recordExists1 = databaseMySql.QueryFind(sql, new Object[] { 1 });

            sql = "select 1 from TestQueryFind where SomeTestData = :SomeTestData";
            Boolean recordExists2 = databaseMySql.QueryFind(sql, new Object[] { "SomeTestDataWithId2" });

            sql = "select 1 from TestQueryFind where IdTest in (1,2,3)";
            Boolean recordExistsAll = databaseMySql.QueryFind(sql, null);

            sql = "select 1 from TestQueryFind where IdTest = :IdTest";
            Boolean recordExists4 = databaseMySql.QueryFind(sql, new Object[] { 4 });

            sql = "select 1 from TestQueryFind where SomeTestData = :SomeTestData";
            Boolean recordExists5 = databaseMySql.QueryFind(sql, new Object[] { "SomeTestDataWithId5" });

            // Assert
            Assert.AreEqual(recordExists1, true);
            Assert.AreEqual(recordExists2, true);
            Assert.AreEqual(recordExistsAll, true);
            Assert.AreEqual(recordExists4, false);
            Assert.AreEqual(recordExists5, false);

            // Finalize
            try { databaseMySql.CloseConnection(); }
            catch { /* Just grant that the connection was closed */}
        }

        [TestMethod]
        public void TestQueryRecord()
        {
            // Arrange

            // Act

            // Assert
        }

        [TestMethod]
        public void TestQueryTable()
        {
            // Arrange

            // Act

            // Assert
        }

        [TestMethod]
        public void TestQueryProcedure()
        {
            // Arrange
            LazyDatabaseMySql databaseMySql = new LazyDatabaseMySql(CONNECTION_STRING);
            databaseMySql.OpenConnection();

            // Act
            Object[] values = new Object[] { "0" };
            String[] parameters = new String[] { "inIdTest" };
            DataTable dataTableProcedure = databaseMySql.QueryProcedure("SpTestQueryProcedure", "ProcedureResult", values, parameters);

            Boolean recordExists1 = dataTableProcedure.Select("IdTest = 1").Length > 0;
            Boolean recordExists2 = dataTableProcedure.Select("IdTest = 2").Length > 0;
            Boolean recordExists3 = dataTableProcedure.Select("IdTest = 3").Length > 0;

            // Assert
            Assert.AreEqual(recordExists1, true);
            Assert.AreEqual(recordExists2, true);
            Assert.AreEqual(recordExists3, true);
        }

        [TestMethod]
        public void TestInsert()
        {
            // Arrange

            // Act

            // Assert
        }

        [TestMethod]
        public void TestInsertAll()
        {
            // Arrange

            // Act

            // Assert
        }

        [TestMethod]
        public void TestUpdate()
        {
            // Arrange

            // Act

            // Assert
        }

        [TestMethod]
        public void TestUpdateAll()
        {
            // Arrange

            // Act

            // Assert
        }

        [TestMethod]
        public void TestDelete()
        {
            // Arrange

            // Act

            // Assert
        }

        [TestMethod]
        public void TestDeleteAll()
        {
            // Arrange

            // Act

            // Assert
        }

        [TestMethod]
        public void TestUpsert()
        {
            // Arrange

            // Act

            // Assert
        }

        [TestMethod]
        public void TestUpsertAll()
        {
            // Arrange

            // Act

            // Assert
        }
    }
}
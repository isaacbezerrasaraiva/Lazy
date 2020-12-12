drop user if exists 'unittestlazy'@'localhost';
drop database if exists UnitTestLazy;
create database UnitTestLazy;
create user 'unittestlazy'@'localhost' identified by 'unittestlazy';
grant all privileges on UnitTestLazy.* to 'unittestlazy'@'localhost';
flush privileges;
use UnitTestLazy;



-- Create test tables ------------------------------------------------------------------------------------------------------
-- -------------------------------------------------------------------------------------------------------------------------

create table TestCommitTransaction
(
	IdTest integer,
    SomeTestData varchar(32),
    constraint Pk_TestCommitTransaction primary key (IdTest)
);

create table TestCommitTransactionNested
(
	IdTest integer,
    SomeTestData varchar(32),
    constraint Pk_TestCommitTransactionNested primary key (IdTest)
);

create table TestRollbackTransaction
(
	IdTest integer,
    SomeTestData varchar(32),
    constraint Pk_TestRollbackTransaction primary key (IdTest)
);

create table TestRollbackTransactionNested
(
	IdTest integer,
    SomeTestData varchar(32),
    constraint Pk_TestRollbackTransactionNested primary key (IdTest)
);

create table TestQueryExecute
(
	IdTest integer,
    SomeTestData varchar(32),
    constraint Pk_TestQueryExecute primary key (IdTest)
);

create table TestQueryValue
(
	IdTest integer,
    SomeTestData varchar(32),
    constraint Pk_TestQueryValue primary key (IdTest)
);

create table TestQueryFind
(
	IdTest integer,
    SomeTestData varchar(32),
    constraint Pk_TestQueryFind primary key (IdTest)
);

create table TestQueryRecord
(
	IdTest integer,
    SomeTestData varchar(32),
    constraint Pk_TestQueryRecord primary key (IdTest)
);

create table TestQueryTable
(
	IdTest integer,
    SomeTestData varchar(32),
    constraint Pk_TestQueryTable primary key (IdTest)
);

create table TestQueryProcedure
(
	IdTest integer,
    SomeTestData varchar(32),
    constraint Pk_TestQueryProcedure primary key (IdTest)
);

create table TestInsert
(
	IdTest integer,
    SomeTestData varchar(32),
    constraint Pk_TestInsert primary key (IdTest)
);

create table TestInsertAll
(
	IdTest integer,
    SomeTestData varchar(32),
    constraint Pk_TestInsertAll primary key (IdTest)
);

create table TestUpdate
(
	IdTest integer,
    SomeTestData varchar(32),
    constraint Pk_TestUpdate primary key (IdTest)
);

create table TestUpdateAll
(
	IdTest integer,
    SomeTestData varchar(32),
    constraint Pk_TestUpdateAll primary key (IdTest)
);

create table TestDelete
(
	IdTest integer,
    SomeTestData varchar(32),
    constraint Pk_TestDelete primary key (IdTest)
);

create table TestDeleteAll
(
	IdTest integer,
    SomeTestData varchar(32),
    constraint Pk_TestDeleteAll primary key (IdTest)
);

create table TestUpsert
(
	IdTest integer,
    SomeTestData varchar(32),
    constraint Pk_TestUpsert primary key (IdTest)
);

create table TestUpsertAll
(
	IdTest integer,
    SomeTestData varchar(32),
    constraint Pk_TestUpsertAll primary key (IdTest)
);



-- Initialize test data ----------------------------------------------------------------------------------------------------
-- -------------------------------------------------------------------------------------------------------------------------

insert into TestCommitTransaction (IdTest, SomeTestData) values (1, 'SomeTestDataWithId1');
insert into TestCommitTransaction (IdTest, SomeTestData) values (2, 'SomeTestDataWithId2');
insert into TestCommitTransaction (IdTest, SomeTestData) values (3, 'SomeTestDataWithId3');

insert into TestCommitTransactionNested (IdTest, SomeTestData) values (1, 'SomeTestDataWithId1');
insert into TestCommitTransactionNested (IdTest, SomeTestData) values (2, 'SomeTestDataWithId2');
insert into TestCommitTransactionNested (IdTest, SomeTestData) values (3, 'SomeTestDataWithId3');

insert into TestRollbackTransaction (IdTest, SomeTestData) values (1, 'SomeTestDataWithId1');
insert into TestRollbackTransaction (IdTest, SomeTestData) values (2, 'SomeTestDataWithId2');
insert into TestRollbackTransaction (IdTest, SomeTestData) values (3, 'SomeTestDataWithId3');

insert into TestRollbackTransactionNested (IdTest, SomeTestData) values (1, 'SomeTestDataWithId1');
insert into TestRollbackTransactionNested (IdTest, SomeTestData) values (2, 'SomeTestDataWithId2');
insert into TestRollbackTransactionNested (IdTest, SomeTestData) values (3, 'SomeTestDataWithId3');

insert into TestQueryExecute (IdTest, SomeTestData) values (1, 'SomeTestDataWithId1');
insert into TestQueryExecute (IdTest, SomeTestData) values (2, 'SomeTestDataWithId2');
insert into TestQueryExecute (IdTest, SomeTestData) values (3, 'SomeTestDataWithId3');

insert into TestQueryValue (IdTest, SomeTestData) values (1, 'SomeTestDataWithId1');
insert into TestQueryValue (IdTest, SomeTestData) values (2, 'SomeTestDataWithId2');
insert into TestQueryValue (IdTest, SomeTestData) values (3, 'SomeTestDataWithId3');

insert into TestQueryFind (IdTest, SomeTestData) values (1, 'SomeTestDataWithId1');
insert into TestQueryFind (IdTest, SomeTestData) values (2, 'SomeTestDataWithId2');
insert into TestQueryFind (IdTest, SomeTestData) values (3, 'SomeTestDataWithId3');

insert into TestQueryRecord (IdTest, SomeTestData) values (1, 'SomeTestDataWithId1');
insert into TestQueryRecord (IdTest, SomeTestData) values (2, 'SomeTestDataWithId2');
insert into TestQueryRecord (IdTest, SomeTestData) values (3, 'SomeTestDataWithId3');

insert into TestQueryTable (IdTest, SomeTestData) values (1, 'SomeTestDataWithId1');
insert into TestQueryTable (IdTest, SomeTestData) values (2, 'SomeTestDataWithId2');
insert into TestQueryTable (IdTest, SomeTestData) values (3, 'SomeTestDataWithId3');

insert into TestQueryProcedure (IdTest, SomeTestData) values (1, 'SomeTestDataWithId1');
insert into TestQueryProcedure (IdTest, SomeTestData) values (2, 'SomeTestDataWithId2');
insert into TestQueryProcedure (IdTest, SomeTestData) values (3, 'SomeTestDataWithId3');

insert into TestInsert (IdTest, SomeTestData) values (1, 'SomeTestDataWithId1');
insert into TestInsert (IdTest, SomeTestData) values (2, 'SomeTestDataWithId2');
insert into TestInsert (IdTest, SomeTestData) values (3, 'SomeTestDataWithId3');

insert into TestInsertAll (IdTest, SomeTestData) values (1, 'SomeTestDataWithId1');
insert into TestInsertAll (IdTest, SomeTestData) values (2, 'SomeTestDataWithId2');
insert into TestInsertAll (IdTest, SomeTestData) values (3, 'SomeTestDataWithId3');

insert into TestUpdate (IdTest, SomeTestData) values (1, 'SomeTestDataWithId1');
insert into TestUpdate (IdTest, SomeTestData) values (2, 'SomeTestDataWithId2');
insert into TestUpdate (IdTest, SomeTestData) values (3, 'SomeTestDataWithId3');

insert into TestUpdateAll (IdTest, SomeTestData) values (1, 'SomeTestDataWithId1');
insert into TestUpdateAll (IdTest, SomeTestData) values (2, 'SomeTestDataWithId2');
insert into TestUpdateAll (IdTest, SomeTestData) values (3, 'SomeTestDataWithId3');

insert into TestDelete (IdTest, SomeTestData) values (1, 'SomeTestDataWithId1');
insert into TestDelete (IdTest, SomeTestData) values (2, 'SomeTestDataWithId2');
insert into TestDelete (IdTest, SomeTestData) values (3, 'SomeTestDataWithId3');

insert into TestDeleteAll (IdTest, SomeTestData) values (1, 'SomeTestDataWithId1');
insert into TestDeleteAll (IdTest, SomeTestData) values (2, 'SomeTestDataWithId2');
insert into TestDeleteAll (IdTest, SomeTestData) values (3, 'SomeTestDataWithId3');

insert into TestUpsert (IdTest, SomeTestData) values (1, 'SomeTestDataWithId1');
insert into TestUpsert (IdTest, SomeTestData) values (2, 'SomeTestDataWithId2');
insert into TestUpsert (IdTest, SomeTestData) values (3, 'SomeTestDataWithId3');

insert into TestUpsertAll (IdTest, SomeTestData) values (1, 'SomeTestDataWithId1');
insert into TestUpsertAll (IdTest, SomeTestData) values (2, 'SomeTestDataWithId2');
insert into TestUpsertAll (IdTest, SomeTestData) values (3, 'SomeTestDataWithId3');

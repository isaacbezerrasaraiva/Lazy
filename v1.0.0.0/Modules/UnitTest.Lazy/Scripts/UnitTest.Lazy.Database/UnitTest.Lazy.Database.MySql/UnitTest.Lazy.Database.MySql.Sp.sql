use UnitTestLazy;

delimiter $$

drop procedure if exists SpTestQueryProcedure $$
create procedure SpTestQueryProcedure
(in inIdTest integer)
begin

	select IdTest, SomeTestData from TestQueryProcedure;
    
end $$



delimiter ;
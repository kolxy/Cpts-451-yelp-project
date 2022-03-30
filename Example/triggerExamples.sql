---

TRIGGER EXAMPLES

--trigger example 1 
--whenever a new employee is inserted, if his/her department 
--is not defined in the system, add it to the department table.

CREATE OR REPLACE FUNCTION defineDept() RETURNS trigger AS '
BEGIN 
   INSERT INTO Dept
   (SELECT New.dno, NULL as dname, NULL as mgr
    WHERE (NOT EXISTS ( SELECT *
                        FROM Dept
            	          WHERE Dept.dno = NEW.dno)) );
   RETURN NEW;
END
' LANGUAGE plpgsql; 

CREATE TRIGGER newDept
BEFORE INSERT ON Emp1
FOR EACH ROW
WHEN (NEW.dno IS NOT NULL)
EXECUTE PROCEDURE defineDept();


--test trigger 1
INSERT INTO Emp1 VALUES ('411-11-1111','Paul',1212,40000)
SELECT * FROM Dept ORDER BY dno;
--clean test
DELETE FROM Emp1 WHERE ssn = '411-11-1111'
DELETE FROM dept where dno = 1212
DROP TRIGGER newDept on emp1;

-- other suplimentary queries
CREATE TABLE Emp1(
	ssn CHAR(11),
	ename CHAR(30),
	dno INTEGER REFERENCES Dept(dno),
	sal FLOAT,
    PRIMARY KEY(ssn,dno) );

INSERT INTO Emp1 
(  SELECT * 
   FROM Emp  ); 

DROP TABLE Emp1;

----------------------------------------------------------
---------------------------------------------------------- 

--trigger example 2
--"No update can reduce employees’ salaries."
CREATE OR REPLACE FUNCTION resetSalary() RETURNS trigger AS '
BEGIN 
   UPDATE  Emp1
   SET  sal= OLD.sal
   WHERE  ssn = NEW.ssn;
   RETURN NEW;
END
' LANGUAGE plpgsql;  
    
CREATE TRIGGER NoLowerSalary
AFTER UPDATE OF sal ON Emp1
FOR EACH ROW
WHEN (OLD.sal > NEW.sal)
EXECUTE PROCEDURE resetSalary();
 
--test trigger 2
UPDATE Emp1 
SET sal = 1000
WHERE dno=111;

SELECT * FROM Emp1 ORDER BY dno;
-- clean
DROP TRIGGER NoLowerSalary on Emp1;

----------------------------------------------------------
----------------------------------------------------------

--trigger example 3
--Maintain the number of employees in the department table up-to-date

-- Assume Dept has a numEmp attribute which is initialized to 0. 
ALTER TABLE Dept 
ADD COLUMN numEmp INTEGER DEFAULT 0;

CREATE OR REPLACE FUNCTION UpdateEmpCount() RETURNS trigger AS '
BEGIN 
   UPDATE  Dept
   SET  numEmp= numEmp+1
   WHERE Dept.dno=NEW.dno;
   RETURN NEW;
END
' LANGUAGE plpgsql;  
    
CREATE TRIGGER employeeCount
AFTER INSERT ON Emp1
FOR EACH ROW
WHEN (NEW.dno IS NOT NULL)
EXECUTE PROCEDURE UpdateEmpCount();
 
--test trigger 2
INSERT INTO Emp1 VALUES ('611-11-1111','Patt',333,50000)
SELECT * FROM Dept ORDER BY dno;
--clean
DROP TRIGGER UpdateEmpCount on Emp1;

----------------------------------------------------------
----------------------------------------------------------

--trigger example 3 (version2)
--Maintain the number of employees in the department table up-to-date

CREATE OR REPLACE FUNCTION UpdateEmpCount() RETURNS trigger AS '
BEGIN 
   UPDATE  Dept
   SET  numEmp= (SELECT COUNT(ssn) FROM Emp1 WHERE Dept.dno=Emp1.dno);
   RETURN NULL;
END
' LANGUAGE plpgsql;  
    
CREATE TRIGGER employeeCount
AFTER INSERT ON Emp1
FOR EACH STATEMENT
EXECUTE PROCEDURE UpdateEmpCount();

-- test trigger 3
-- setup
ALTER TABLE Dept 
ADD COLUMN numEmp INTEGER;
SELECT * FROM Dept;
SELECT * FROM Dept ORDER BY dno;

UPDATE Dept as D
SET numEmp = (SELECT COUNT(ssn)
                FROM Emp1 as E
                WHERE E.dno = D.dno)
-- execute
INSERT INTO Emp1 VALUES ('811-11-1111','Pat1',111,50000), 
                          ('812-11-1111','Pat2',111,50000),
                          ('813-11-1111','Pat3',111,50000),
                          ('814-11-1111','Pat4',111,50000)

--clean
DROP TRIGGER employeeCount on Emp1;               
ALTER TABLE Dept
DROP COLUMN numEmp;

					  
----------------------------------------------------------
----------------------------------------------------------

--trigger example 4
--all employees should work on some project (total participation)
--
SELECT * FROM ProjectEmp order by ssn;
SELECT * FROM Emp1
WHERE ssn NOT IN(
           	SELECT ssn FROM ProjectEmp)

CREATE OR REPLACE FUNCTION assign2Proj() RETURNS trigger AS '
DECLARE 
  x int;
BEGIN 
   x = (SELECT COUNT(*) FROM ProjectEmp WHERE ssn = OLD.ssn);
   IF (x<=1) THEN 
   	 INSERT INTO ProjectEmp VALUES (OLD.proj_id,OLD.ssn, OLD.begindate);
   END IF;
   RETURN NEW;
END
' LANGUAGE plpgsql; 

CREATE TRIGGER totalConstraint
AFTER DELETE ON ProjectEmp
FOR EACH ROW
EXECUTE PROCEDURE assign2Proj();

 
--test trigger 1
DELETE FROM ProjectEmp WHERE ssn='128-00-1111';
INSERT INTO  ProjectEmp VALUES(3,'128-00-1111','1/1/2017');
--clean
DROP TRIGGER totalConstraint on ProjectEmp;

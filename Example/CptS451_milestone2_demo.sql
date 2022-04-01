
--GLOSSARY
--table names
businesstable
usertable
tipstable
friendstable
checkin
businesscategory
businessattribute
businesshours

--some attribute names
zipcode
business_id
city  (business city)
name   (business name)
user_id
friend_id
numtips
numCheckins

user_id
tipcount  (user)
totallikes (user)

tipdate
tiptext
likes  (tip)

checkindate


--1.
SELECT COUNT(*) 
FROM  businesstable;
SELECT COUNT(*) 
FROM  usertable;
SELECT COUNT(*) 
FROM  tipstable;
SELECT COUNT(*) 
FROM  friendstable;
SELECT COUNT(*) 
FROM  checkin;
SELECT COUNT(*) 
FROM  businesscategory;
SELECT COUNT(*) 
FROM  businessattribute;
SELECT COUNT(*) 
FROM  businesshours;



--2. Run the following queries on your business table, checkin table and review table. Make sure to change the attribute names based on your schema. 

SELECT zipcode, COUNT(distinct C.category)
FROM businesstable as B, businesscategory as C
WHERE B.business_id = C.business_id
GROUP BY zipcode
HAVING count(distinct C.category)>300
ORDER BY zipcode;

SELECT zipcode, COUNT(distinct A.attribute)
FROM businesstable as B, businessattribute as A
WHERE B.business_id = A.business_id
GROUP BY zipcode
HAVING count(distinct A.attribute) = 30;


SELECT usertable.user_id, count(friend_id)
FROM usertable, friendstable
WHERE usertable.user_id = friendstable.user_id AND 
      usertable.user_id = 'NxtYkOpXHSy7LWRKJf3z0w'
GROUP BY usertable.user_id;


--3. Run the following queries on your business table, checkin table and tips table. Make sure to change the attribute names based on your schema. 


SELECT business_id, name, city, numtips, numCheckins
FROM businesstable 
WHERE business_id ='K8M3OeFCcAnxuxtTc0BQrQ';

SELECT user_id, name, tipcount, totallikes
FROM usertable
WHERE user_id = 'NxtYkOpXHSy7LWRKJf3z0w';

-----------

SELECT COUNT(*) 
FROM checkin
WHERE business_id ='K8M3OeFCcAnxuxtTc0BQrQ';

SELECT count(*)
FROM tipstable
WHERE  business_id = 'K8M3OeFCcAnxuxtTc0BQrQ';


--4. 
--Type the following statements. Make sure to change the attribute names based on your schema. 

SELECT business_id,name, city, numCheckins, numtips
FROM businesstable 
WHERE business_id ='hDD6-yk1yuuRIvfdtHsISg';

INSERT INTO checkin (business_id, checkindate)
VALUES ('hDD6-yk1yuuRIvfdtHsISg','2022-03-31 10:45:07');


--5.
--Type the following statements. Make sure to change the attribute names based on your schema.  

-- same as the above query in part4
SELECT business_id,name, city, numCheckins, numtips
FROM businesstable 
WHERE business_id ='hDD6-yk1yuuRIvfdtHsISg';

SELECT user_id, name, tipcount, totallikes
FROM usertable
WHERE user_id = '3z1EttCePzDn9OZbudD5VA';


INSERT INTO tipstable (user_id, business_id, tipdate, tiptext,likes)  
VALUES ('3z1EttCePzDn9OZbudD5VA','hDD6-yk1yuuRIvfdtHsISg', '2022-03-31 13:00','EVERYTHING IS AWESOME',0);

UPDATE tipstable 
SET likes = likes+1
WHERE user_id = '3z1EttCePzDn9OZbudD5VA' AND 
      business_id = 'hDD6-yk1yuuRIvfdtHsISg' AND 
      tipdate ='2022-03-31 13:00';

      
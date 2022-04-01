
--GLOSSARY
--table names
business
the_user
tips
user_follow
checkin
business_category
business_attribute
business_hours

--some attribute names
zipcode
business_id
city  (business city)
name   (business name)
user_id
friend_id
num_tips
num_checkings

user_id
tip_count  (user)
total_likes (user)

timestamp
text
likes  (tip)

timestamp


--1.
SELECT COUNT(*) 
FROM  business;
SELECT COUNT(*) 
FROM  the_user;
SELECT COUNT(*) 
FROM  tips;
SELECT COUNT(*) 
FROM  user_follow;
SELECT COUNT(*) 
FROM  checkin;
SELECT COUNT(*) 
FROM  business_category;
SELECT COUNT(*) 
FROM  business_attribute;
SELECT COUNT(*) 
FROM  business_hours;



--2. Run the following queries on your business table, checkin table and review table. Make sure to change the attribute names based on your schema. 

SELECT zipcode, COUNT(distinct C.name)
FROM business as B, business_category as C
WHERE B.business_id = C.business_id
GROUP BY zipcode
HAVING count(distinct C.name)>300
ORDER BY zipcode;

SELECT zipcode, COUNT(distinct A.name)
FROM business as B, business_attribute as A
WHERE B.business_id = A.business_id
GROUP BY zipcode
HAVING count(distinct A.name) = 30;


SELECT the_user.user_id, count(friend_id)
FROM the_user, user_follow
WHERE the_user.user_id = user_follow.user_id AND 
      the_user.user_id = 'NxtYkOpXHSy7LWRKJf3z0w'
GROUP BY the_user.user_id;


--3. Run the following queries on your business table, checkin table and tips table. Make sure to change the attribute names based on your schema. 


SELECT business_id, name, city, num_tips, num_checkings
FROM business 
WHERE business_id ='K8M3OeFCcAnxuxtTc0BQrQ';

SELECT user_id, name, tip_count, likes
FROM the_user
WHERE user_id = 'NxtYkOpXHSy7LWRKJf3z0w';

-----------

SELECT COUNT(*) 
FROM checkin
WHERE business_id ='K8M3OeFCcAnxuxtTc0BQrQ';

SELECT count(*)
FROM tips
WHERE  business_id = 'K8M3OeFCcAnxuxtTc0BQrQ';


--4. 
--Type the following statements. Make sure to change the attribute names based on your schema. 

SELECT business_id,name, city, num_checkings, num_tips
FROM business 
WHERE business_id ='hDD6-yk1yuuRIvfdtHsISg';

-- HAVE PROBLEMS
INSERT INTO checkin (business_id, checkindate)
VALUES ('hDD6-yk1yuuRIvfdtHsISg','2022-03-31 10:45:07');


--5.
--Type the following statements. Make sure to change the attribute names based on your schema.  

-- same as the above query in part4
SELECT business_id,name, city, num_checkings, num_tips
FROM business 
WHERE business_id ='hDD6-yk1yuuRIvfdtHsISg';

SELECT user_id, name, tip_count, total_likes
FROM the_user
WHERE user_id = '3z1EttCePzDn9OZbudD5VA';


INSERT INTO tips (user_id, business_id, timestamp, tiptext,likes)  
VALUES ('3z1EttCePzDn9OZbudD5VA','hDD6-yk1yuuRIvfdtHsISg', '2022-03-31 13:00','EVERYTHING IS AWESOME',0);

UPDATE tips 
SET likes = likes+1
WHERE user_id = '3z1EttCePzDn9OZbudD5VA' AND 
      business_id = 'hDD6-yk1yuuRIvfdtHsISg' AND 
      timestamp ='2022-03-31 13:00';

      
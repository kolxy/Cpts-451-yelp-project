-- update business checking cnt
update business
set num_checkings = cnt.check_cnt
from (
	select business_id, count(business_id) as check_cnt
	from business
	natural join checkin
	group by business_id ) as cnt
where business.business_id = cnt.business_id;

-- update business tip cnt
update business
set num_tips = cnt.tipcnt
from ( 
	select business_id, count(business_id) as tipcnt
	from business
	natural join tips
	group by business.business_id) as cnt
where business.business_id = cnt.business_id;


-- update the_user totallikes cnt
update the_user
set total_likes = cnt.likes_cnt
from (
	select user_id, sum(likes) as likes_cnt
	from the_user
	natural join tips
	group by user_id) as cnt
where the_user.user_id = cnt.user_id;

-- update the_user tip_count
update the_user
set tip_count = cnt.tipcnt
from (select user_id, count(user_id) as tipcnt
	  from the_user
	  natural join tips
	  group by user_id) as cnt
where the_user.user_id = cnt.user_id;





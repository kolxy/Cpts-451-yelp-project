-- update business checking cnt
update business
set num_checkings = cnt.check_cnt
from (
	select business_id, count(business_id) as check_cnt
	from business
	natural join checkin
	group by business_id ) as cnt
where business.business_id = cnt.business_id

-- update business tip cnt
update business
set num_tips = cnt.tipcnt
from ( 
	select business_id, count(business_id) as tipcnt
	from business
	natural join tips
	group by business.business_id) as cnt
where business.business_id = cnt.business_id




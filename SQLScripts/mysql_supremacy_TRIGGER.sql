DROP TRIGGER IF EXISTS newtiplike on tips;
DROP FUNCTION IF EXISTS incre_tiplikes;

DROP FUNCTION IF EXISTS incre_checkin;
DROP TRIGGER IF EXISTS newcheckin on checkin;

DROP TRIGGER IF EXISTS newtip on tips;
DROP FUNCTION IF EXISTS incre_tipcnt;

 


-- a) Whenever a user provides a tip for a business, the “numTips” value for that business and the 
--    “tipCount” value for the user should be updated.  

create or replace function incre_tipcnt() returns trigger as '
begin
    update business
    set num_tips = num_tips + 1
    where business.business_id = new.business_id;
    update the_user
    set tip_count = tip_count + 1
    where the_user.user_id = new.user_id;
    return new;
end;
'LANGUAGE plpgsql; 

create trigger newtip
after insert on tips
for each row 
execute function incre_tipcnt(); 

-- b) when a customer checks-in a business, the “numCheckins” attribute value for that 
--    business should be updated. 

create or replace function incre_checkin() returns trigger as '
begin
    update business
    set num_checkings = num_checkings + 1
    where business.business_id = new.business_id;
    return new;
end;
'LANGUAGE plpgsql; 

create trigger newcheckin
after insert on checkin
for each row
execute function incre_checkin();


-- c) When a user likes a tip, the “totalLikes” attribute value for the user who wrote that tip should be 
-- updated.  
create or replace function incre_tiplikes() returns trigger as '
begin
    update the_user
    set total_likes = total_likes + 1
    where the_user.user_id = new.user_id;
    return new;
end;
'LANGUAGE plpgsql; 

create trigger newtiplike
after update on tips
for each row
execute function incre_tiplikes();



CREATE OR REPLACE FUNCTION "public"."getdistance"("lat1" float8, "long1" float8, "lat2" float8, "long2" float8)
  RETURNS "pg_catalog"."float8" AS $BODY$
begin
    RETURN round(2 * 3963.1676 * asin(sqrt(pow(sin((radians(lat2) - radians(lat1)) / 2.0), 2) + cos(radians(lat1)) * cos(radians(lat2)) * pow(sin((radians(long2) - radians(long1)) / 2.0), 2)))::Numeric, 2);
end;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
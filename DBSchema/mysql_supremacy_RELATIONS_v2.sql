DROP TABLE IF EXISTS "business_attribute";
DROP TABLE IF EXISTS "business_category";
DROP TABLE IF EXISTS "business_hour";
DROP TABLE IF EXISTS "tips";
DROP TABLE IF EXISTS "checkin";
DROP TABLE IF EXISTS "user_follow";
DROP TABLE IF EXISTS "business";
DROP TABLE IF EXISTS "the_user";
 

CREATE TABLE "business" (
  "business_id" varchar PRIMARY KEY,
  "name" varchar,
  "city" varchar,
  "state" char(2),
  "zipcode" varchar,
  "address" varchar,
  "stars" float4,
  "latitude" float8,
  "longitude" float8,
  "num_checkings" int8,
  "num_tips" int8,
  "is_open" BOOLEAN,
  check ("num_tips" >= 0.0 and "stars">= 0.0)
);

CREATE TABLE "the_user" (
  "user_id" varchar PRIMARY KEY,
  "name" varchar,
  "yelp_since" timestamp(6),
  "latitude" float8,
  "longtiude" float8,
  "average_stars" float8,
  "tip_count" int8,
  "total_likes" int8,
  "useful" int8,
  "funny" int8,
  "cool" int8,
  "fans" int8,
  check ("average_stars" >= 0.0 and "tip_count" >= 0 and "fans" >= 0)
);

CREATE TABLE "business_attribute" (
  "business_id" varchar,
  "name" varchar,
  "value" varchar,
  PRIMARY KEY ("business_id", "name"),
  FOREIGN KEY ("business_id") REFERENCES "business"("business_id")
  ON DELETE CASCADE
  ON UPDATE CASCADE
);

CREATE TABLE "business_category" (
  "business_id" varchar,
  "name" varchar,
  PRIMARY KEY ("business_id", "name"),
  FOREIGN KEY ("business_id") REFERENCES "business"("business_id")
  ON DELETE CASCADE
  ON UPDATE CASCADE
);

CREATE TABLE "business_hour" (
  "business_id" varchar,
  "day" varchar,
  "open" time(6),
  "close" time(6),
  PRIMARY KEY ("business_id", "day"),
  FOREIGN KEY ("business_id") REFERENCES "business"("business_id")
  ON DELETE CASCADE
  ON UPDATE CASCADE
);

CREATE TABLE "tips" (
  "business_id" varchar,
  "user_id" varchar,
  "timestamp" timestamp(6),
  "likes" int8,
  "text" text,
  PRIMARY KEY ("business_id", "user_id", "timestamp"),
  FOREIGN KEY ("business_id") REFERENCES "business"("business_id")
  ON DELETE CASCADE
  ON UPDATE CASCADE,
  FOREIGN KEY ("user_id") REFERENCES "the_user"("user_id")
  ON DELETE CASCADE
  ON UPDATE CASCADE
);

CREATE TABLE  "checkin" (
  "business_id" varchar,
  "year" int8,
  "month" int8,
  "day" int8,
  "clock_time" time,
  PRIMARY KEY ("business_id", "year", "month", "day", "clock_time"),
  FOREIGN KEY ("business_id") REFERENCES "business"("business_id")
  ON DELETE CASCADE
  ON UPDATE CASCADE
);

CREATE TABLE "user_follow" (
  "user_id" varchar,
  "friend_id" varchar,
  PRIMARY KEY ("user_id", "friend_id"),
  FOREIGN KEY ("user_id") REFERENCES "the_user"("user_id")
  ON DELETE CASCADE
  ON UPDATE CASCADE,
  FOREIGN KEY("friend_id") REFERENCES "the_user"("user_id")
  ON DELETE CASCADE
  ON UPDATE CASCADE
);

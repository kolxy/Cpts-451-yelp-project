CREATE TABLE "business" (
  "business_id" varchar(22) PRIMARY KEY,
  "name" varchar(255),
  "city" varchar(255),
  "state" char(2),
  "zipcode" varchar(255),
  "address" varchar(255),
  "rating" float4,
  "latitude" float8,
  "longitude " float8
);

CREATE TABLE "user" (
  "user_id" varchar(22) PRIMARY KEY,
  "name" varchar(255),
  "yelp_since" timestamp(6),
  "latitude" float8,
  "longtiude" float8,
  "average_stars" float8,
  "vote_count" int8
);

CREATE TABLE "business_attribute" (
  "business_id" varchar(22),
  "name" varchar(255),
  "value" varchar(255),
  PRIMARY KEY ("business_id", "name"),
  FOREIGN KEY ("business_id") REFERENCES "business"("business_id")
  ON DELETE CASCADE
  ON UPDATE CASCADE
);

CREATE TABLE "business_category" (
  "business_id" varchar(22),
  "name" varchar(22),
  PRIMARY KEY ("business_id", "name"),
  FOREIGN KEY ("business_id") REFERENCES "business"("business_id")
  ON DELETE CASCADE
  ON UPDATE CASCADE
);

CREATE TABLE "business_hour" (
  "business_id" varchar(22),
  "day" varchar(22),
  "open" time(6),
  "close" time(6),
  PRIMARY KEY ("business_id", "day"),
  FOREIGN KEY ("business_id") REFERENCES "business"("business_id")
  ON DELETE CASCADE
  ON UPDATE CASCADE
);

CREATE TABLE "tips" (
  "business_id" varchar(22),
  "user_id" varchar(22),
  "timestamp" timestamp(6),
  "text" text,
  "likes" int8,
  PRIMARY KEY ("business_id", "user_id", "timestamp"),
  FOREIGN KEY ("business_id") REFERENCES "business"("business_id")
  ON DELETE CASCADE
  ON UPDATE CASCADE,
  FOREIGN KEY ("user_id") REFERENCES "user"("user_id")
  ON DELETE CASCADE
  ON UPDATE CASCADE
);

CREATE TABLE  "checkin" (
  "business_id" varchar(22),
  "timestamp" timestamp(6),
  PRIMARY KEY ("business_id", "timestamp"),
  FOREIGN KEY ("business_id") REFERENCES "business"("business_id")
  ON DELETE CASCADE
  ON UPDATE CASCADE
);

CREATE TABLE "user_follow" (
  "from" varchar(22),
  "to" varchar(22),
  PRIMARY KEY ("from", "to"),
  FOREIGN KEY ("from") REFERENCES "user"("user_id")
  ON DELETE CASCADE
  ON UPDATE CASCADE,
  FOREIGN KEY("to") REFERENCES "user"("user_id")
  ON DELETE CASCADE
  ON UPDATE CASCADE
);

from itertools import count
import json
from tabnanny import check
import psycopg2
import collections
import os
from dotenv import load_dotenv
import time

load_dotenv()
dbname = os.getenv("DBNAME")
user = os.getenv("USER")
host = os.getenv("HOST")
password = os.getenv("PASSWORD")
port = os.getenv("PORT")
data_directory = os.path.dirname(__file__) + "/../data/"

def cleanStr4SQL(s):
    return s.replace("\n"," ")

def insert_business_data(cur, conn): 
    data_business = []
    data_category = []
    data_attribute = []
    data_hour = []
    with open(data_directory + 'yelp_business.JSON','r') as f:
        line = f.readline()
        count_line = 0
        while line:
            data = json.loads(line)
            data_business.append((data['business_id'],data["name"], data["address"], data["state"], data["city"], data["postal_code"], data["latitude"], data["longitude"], data["stars"], 0 , 0 , [False,True][data["is_open"]] ))
            
            categories = data["categories"].split(', ')
            for category in categories:
                data_category.append((data['business_id'], category))
            
            flatAttributes = parseAttributes(data["attributes"], [])
            for att, val in flatAttributes:
                data_attribute.append((data['business_id'], att, val))

            formatedHours = parseHours(data["hours"])
            for day, range in formatedHours:
                start, end = range
                data_hour.append((data['business_id'], day, start, end))
                  
            line = f.readline()
            count_line += 1
            if count_line % 1000 == 0:
                print("Reading Business", count_line)
    print("Inserting...")
    query_business = "INSERT INTO business (business_id, name, address, state, city, zipcode, latitude, longitude, stars, num_checkings, num_tips, is_open) VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)"
    query_category = "INSERT INTO business_category (business_id, name) values (%s, %s)"
    query_attribute = "INSERT INTO business_attribute (business_id, name, value) VALUES (%s, %s, %s)"
    query_hour = "INSERT INTO business_hour (business_id, day, open, close) VALUES (%s, %s, %s, %s)"
    cur.executemany(query_business, data_business)
    conn.commit()
    cur.executemany(query_category, data_category)
    conn.commit()
    cur.executemany(query_attribute, data_attribute)
    conn.commit()
    cur.executemany(query_hour, data_hour)
    conn.commit()
    print("business inserted: ", len(data_business))
    print("business_category inserted: ", len(data_category))
    print("business_attribute inserted: ", len(data_attribute))
    print("business_hour inserted: ", len(data_hour))
    f.close()


"""[summary: recursively flatten the nested dictionary using tail recursion]
   [Return: list]
"""
def parseAttributes(dic, acc):
    for k,v in dic.items():
        if isinstance(v, dict):
            parseAttributes(v, acc)
        else:
            tup = (k, v)
            acc.append(tup)
    return acc

"""[summary: Reformat the hour dict to list in format: [('Monday', ['17:30', '21:30']), ('Wednesday', ['17:30', '21:30']),...]
   [Return: list]
"""
def parseHours(dic):
    res = []
    for k, v in dic.items():
        timelst = v.split('-')
        tup = (k, timelst)
        res.append(tup)
    return res

def insert_user_data(cur, conn):
    data_user = []
    with open(data_directory + 'yelp_user.JSON','r') as f:  
        line = f.readline()
        count_line = 0
        while line:
            data = json.loads(line)
            data_user.append((data['user_id'], data["name"], data["yelping_since"], 0, 0, data['average_stars'], data['tipcount'], 0, data['useful'], data['funny'], data['cool'], data['fans']))
                         
            line = f.readline()
            count_line += 1
            if count_line % 10000 == 0:
                print("Reading user", count_line)
    print("Inserting...")
    query_user = "INSERT INTO the_user (user_id, name, yelp_since, latitude, longtiude, average_stars, tip_count, total_likes, useful, funny, cool, fans)  VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)"
    cur.executemany(query_user, data_user)
    conn.commit()
    print("users inserted: ", len(data_user))
    f.close()


def insert_user_follow(cur, conn):
    data_follow = []
    with open(data_directory + 'yelp_user.JSON','r') as f:  
        line = f.readline()
        count_line = 0
        visited = set()
        while line:
            data = json.loads(line)
            # insert user_follow table info    
            for friend_id in data["friends"]:
                # cur_userid = data['user_id']
                # if (cur_userid, friend_id) in visited: continue
                # visited.add((cur_userid, friend_id))
                data_follow.append((data['user_id'], friend_id))
                               
            line = f.readline()
            count_line +=1
            if count_line % 10000 == 0:
                print("Reading user_follow:", count_line)
    print("Inserting...")
    query_follow = "INSERT INTO user_follow (user_id, friend_id) VALUES (%s, %s)"
    cur.executemany(query_follow, data_follow)
    conn.commit()
    print("follows inserted: ", len(data_follow))
    f.close()
    

def insert_checkin_data(cur, conn):
    data_checkin = []
    with open(data_directory + 'yelp_checkin.JSON','r') as f:  
        line = f.readline()
        count_line = 0
        
        while line:
            data = json.loads(line)
            checking_dates = parseDates(data['date'])
            # insert checkin table info
            for date in checking_dates:
                data_checkin.append((data['business_id'], date[0], date[1], date[2], date[3]))

            line = f.readline()
            count_line +=1
            if count_line % 10000 == 0:
                print("Reading checkin:", count_line)
    print("Inserting...")
    query_checkin = "INSERT INTO checkin (business_id, year, month, day, clock_time)  VALUES (%s, %s, %s, %s, %s)"
    cur.executemany(query_checkin, data_checkin)
    conn.commit()
    print("checkin inserted: ", len(data_checkin))
    f.close()
     
    

def parseDates( dateStr):
    dates = dateStr.split(",")
    res = []
    for date in dates:
        dayTime = date.split(" ")
        day = dayTime[0].split("-")
        day.append(dayTime[1])
        res.append(day)
    return res

def insert_tip_data(cur, conn):
    data_tip = []
    with open(data_directory + 'yelp_tip.JSON','r') as f:  
        line = f.readline()
        count_line = 0
        while line:
            data = json.loads(line)
            data_tip.append((data['business_id'], data['user_id'], data['date'], data['text'], data['likes']))
            
            line = f.readline()
            count_line +=1
            if count_line % 10000 == 0:
                print("Reading tip:", count_line)
    print("Inserting...")
    query_tip = "INSERT INTO tips (business_id, user_id, timestamp, text, likes) VALUES (%s, %s, %s, %s, %s)"
    cur.executemany(query_tip, data_tip)
    conn.commit()
    print("tips inserted: ", len(data_tip))
    f.close()
    
def db_connect():
    conn = None
    try:
        conn = psycopg2.connect(dbname = dbname, user = user, host = host, password = password, port = port)
    except:
        print('Unable to connect to the database!')
    cur = conn.cursor()
    return cur, conn

def db_close(cur, conn):
    cur.close()
    conn.close()

if __name__ == "__main__":
    start = time.time()
    cur, conn = db_connect()
    insert_business_data(cur, conn)
    insert_user_data(cur, conn)
    insert_user_follow(cur,conn)
    insert_checkin_data(cur, conn)
    insert_tip_data(cur, conn)
    db_close(cur, conn)
    end = time.time()
    diff = int(end - start)
    print("Time taken: " + str(diff) + "s")
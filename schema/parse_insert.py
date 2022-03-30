import json
from tabnanny import check
import psycopg2
import collections
import os
from dotenv import load_dotenv

load_dotenv()
dbname = os.getenv("DBNAME")
user = os.getenv("USER")
host = os.getenv("HOST")
password = os.getenv("PASSWORD")
port = os.getenv("PORT")
data_directory = os.path.dirname(__file__) + "/../data/"

def cleanStr4SQL(s):
    return s.replace("'","`").replace("\n"," ")

def insert_business_data(cur, conn): 
    with open(data_directory + 'yelp_business.JSON','r') as f:
        line = f.readline()
        count_line = 0
        while line:
            data = json.loads(line)
        
            # insert business table info
            try:
                cur.execute("INSERT INTO business (business_id, name, address, state, city, zipcode, latitude, longitude, stars, num_checkings, num_tips, is_open)"
                       + " VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)", 
                         (data['business_id'],cleanStr4SQL(data["name"]), cleanStr4SQL(data["address"]), data["state"], data["city"], data["postal_code"], data["latitude"], data["longitude"], data["stars"], 0 , 0 , [False,True][data["is_open"]] ) )              
            except Exception as e:
                print("Insert to business table failed!",e)
            conn.commit()
            
            # insert business_category table info
            categories = data["categories"].split(', ')
            for category in categories:
                try:
                    cur.execute("INSERT INTO business_category (business_id, name)" 
                                + "VALUES (%s, %s)", (data['business_id'], category))
                except Exception as e:
                    print("Insert to business_category table failed",e)
            conn.commit()
                    
            # insert business_attribute info
            flatAttributes = parseAttributes(data["attributes"], [])
            for att, val in flatAttributes:
                try:
                    cur.execute("INSERT INTO business_attribute (business_id, name, value)" 
                                + "VALUES (%s, %s, %s)", (data['business_id'], att, val))
                except Exception as e:
                    print("Insert to business_attribute table failed",e)
            conn.commit()

            # insert business_hour info
            formatedHours = parseHours(data["hours"])
            for day, range in formatedHours:
                start, end = range
                try:
                    cur.execute("INSERT INTO business_hour (business_id, day, open, close)" 
                                + "VALUES (%s, %s, %s, %s)", (data['business_id'], day, start, end))
                except Exception as e:
                    print("Insert to business_attribute table failed",e)
            conn.commit()
            line = f.readline()
            count_line +=1       
    print("business inserted: ", count_line)
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
    with open(data_directory + 'yelp_user.JSON','r') as f:  
        line = f.readline()
        count_line = 0
        while line:
            data = json.loads(line)
            # insert the_user table info
            try:
                cur.execute("INSERT INTO the_user (user_id, name, yelp_since, latitude, longtiude, average_stars, tip_count, total_likes, useful, funny, cool, fans)" 
                            + " VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)",
                            (data['user_id'], data["name"], data["yelping_since"], 0, 0, data['average_stars'], data['tipcount'], 0, data['useful'], data['funny'], data['cool'], data['fans']))
            except Exception as e:
                print("Insert to user table failed!",e)
                return 
            conn.commit()
             
            line = f.readline()
            count_line +=1
            
    print("users added: ", count_line)
    f.close()


def insert_user_follow(cur, conn):
    # user_follow table info insert
    with open(data_directory + 'yelp_user.JSON','r') as f:  
        line = f.readline()
        count_line = 0
        visited = set()
        while line:
            data = json.loads(line)
            
            # insert user_follow table info    
            for friend_id in data["friends"]:
                cur_userid = data['user_id']
                if (cur_userid, friend_id) in visited: continue
                visited.add((cur_userid, friend_id))
                try:
                    cur.execute("INSERT INTO user_follow (user_id, friend_id)"
                                + " VALUES (%s, %s)",
                                (data['user_id'], friend_id))
                except Exception as e:
                    print("Insert to user_follow table failed!",e)
                    return                 
            conn.commit()
            line = f.readline()
            count_line +=1
    print("number of user friend attribute done: : ", count_line)
    f.close()
    

def insert_checkin_data(cur, conn):
    with open(data_directory + 'yelp_checkin.JSON','r') as f:  
        line = f.readline()
        count_line = 0
        
        while line:
            data = json.loads(line)
            checking_dates = parseDates(data['date'])
            # insert checkin table info
            for date in checking_dates:
                try:
                    cur.execute("INSERT INTO checkin (business_id, year, month, day, clock_time)"
                                    +" VALUES (%s, %s, %s, %s, %s)",
                                    (data['business_id'], date[0], date[1], date[2], date[3]))
                except Exception as e:
                    print("Insert to user_follow table failed!",e)
                    return   
            conn.commit()
            line = f.readline()
            count_line +=1
             
    print("business checkin added: ", count_line)
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
    with open(data_directory + 'yelp_tip.JSON','r') as f:  
        line = f.readline()
        count_line = 0
        while line:
            data = json.loads(line)
            # insert tips info
            try:
                cur.execute("INSERT INTO tips (business_id, user_id, timestamp, text, likes)"
                            +" VALUES (%s, %s, %s, %s, %s)",
                            (data['business_id'], data['user_id'], data['date'], data['text'], data['likes']))
            except Exception as e:
                print("Insert to tips table failed!",e)
                return 
            conn.commit()
            line = f.readline()
            count_line +=1
    print("tip added: ", count_line)
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
    cur, conn = db_connect()
    insert_business_data(cur, conn)
    insert_user_data(cur, conn)
    insert_user_follow(cur,conn)
    insert_checkin_data(cur, conn)
    insert_tip_data(cur, conn)
    db_close(cur, conn)
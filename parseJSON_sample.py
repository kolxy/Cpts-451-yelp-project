import json

def cleanStr4SQL(s):
    return s.replace("'","`").replace("\n"," ")

def parseBusinessData():
    #read the JSON file
    # We assume that the Yelp data files are available in the current directory. If not, you should specify the path when you "open" the function. 
    with open('.//yelp_business.JSON','r') as f:  
        outfile =  open('.//business.txt', 'w')
        line = f.readline()
        count_line = 0
        #read each JSON abject and extract data
        while line:
            data = json.loads(line)
            outfile.write("{} - business info : '{}' ; '{}' ; '{}' ; '{}' ; '{}' ; '{}' ; {} ; {} ; {} ; {}\n".format(
                              str(count_line), # the line count
                              cleanStr4SQL(data['business_id']),
                              cleanStr4SQL(data["name"]),
                              cleanStr4SQL(data["address"]),
                              cleanStr4SQL(data["state"]),
                              cleanStr4SQL(data["city"]),
                              cleanStr4SQL(data["postal_code"]),
                              str(data["latitude"]),
                              str(data["longitude"]),
                              str(data["stars"]),
                              str(data["is_open"])) )

            #process business categories
            categories = data["categories"].split(', ')
            outfile.write("      categories: {}\n".format(str(categories)))

            
            # TO-DO : write your own code to process attributes
            # make sure to **recursively** parse all attributes at all nesting levels. You should not assume a particular nesting level. 
            outfile.write("") 

            # TO-DO : write your own code to process hours data
            outfile.write("") 

            outfile.write('\n')

            line = f.readline()
            count_line +=1
    print(count_line)
    outfile.close()
    f.close()

def parseUserData():
    # TO-DO : write code to parse yelp_user.JSON
    pass

def parseCheckinData():
    # TO-DO : write code to parse yelp_checkin.JSON
    pass


def parseTipData():
    # TO-DO : write code to parse yelp_tip.JSON
    pass

if __name__ == "__main__":
    parseBusinessData()
    parseUserData()
    parseCheckinData()
    parseTipData()

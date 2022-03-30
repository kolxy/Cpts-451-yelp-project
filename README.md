# Yelp Dataset for CptS 451 Spring 2022 Term Project

## Import data
### Step 1
Install all of python modules required for parsing and inserting
```bash
pip install -r requirements.txt
```
or for Mac users
```bash
pip3 install -r requirements.txt
```
### Step 2
Modify `.env` file in root directory depending on your database setup.

### Step 3
Import the `./DBSchema/mysql_supremacy_RELATIONS_v2.sql` file into your database to create tables.

### Step 4
To load data into DB, first make sure data files are saved in `./data/`, then running the following commands.
```bash
cd DBPopulate
python mysql_supremacy_parseAndInsert.py
```
or for Mac users
```bash
cd DBPopulate
python3 mysql_supremacy_parseAndInsert.py
```
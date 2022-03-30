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
To import data into DB, first make sure data files are saved in `$PATH/Milestone2/data/`, then running the following commands.
```bash
cd schema
python mysql_supremacy_parseandinsert.py
```
or for Mac users
```bash
cd schema
python3 mysql_supremacy_parseandinsert.py
```
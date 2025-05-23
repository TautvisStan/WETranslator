#dataset: https://opus.nlpl.eu/QED/en&lt/v2.0a/QED
from sqlalchemy import create_engine, Column, Integer, String
from sqlalchemy.orm import DeclarativeBase, sessionmaker
import pandas as pd
import os
from config import db_string

engine = create_engine(db_string)
class Base(DeclarativeBase):
    pass 

class Dataset_QED_Entry(Base):
    __tablename__ = 'dataset_qed_v2.0a' 
    id = Column(Integer, primary_key=True, autoincrement=True)
    en = Column(String(255))
    lt = Column(String(255))

def FillDatabaseTable(session_maker, file_en : str, file_lt : str):
    with session_maker() as session: 
        f_en = open(file_en, encoding="utf-8")
        f_lt = open(file_lt, encoding="utf-8")
        l_en = f_en.readline().strip()
        l_lt = f_lt.readline().strip()
        # print(l_en)
        # print(l_lt)
        while (l_en != "" or l_lt != ""):
            a = Dataset_QED_Entry(en = l_en, lt= l_lt)
            session.add(a)
            session.commit()
            l_en = f_en.readline().strip()
            l_lt = f_lt.readline().strip()
            
        f_en.close()
        f_lt.close()


Base.metadata.create_all(engine)
 
session_maker = sessionmaker(engine)
if __name__ == "__main__":
    FillDatabaseTable(session_maker, "./DB_Dataset/datasets/QED.en-lt.en", "./DB_Dataset/datasets/QED.en-lt.lt")

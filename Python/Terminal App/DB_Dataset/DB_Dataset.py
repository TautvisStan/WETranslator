#dataset: https://opus.nlpl.eu/QED/en&lt/v2.0a/QED
from sqlalchemy import create_engine, Column, Integer, String
from sqlalchemy.orm import DeclarativeBase, sessionmaker
import pandas as pd
import os
import config

engine = create_engine(config.db_string)
class Base(DeclarativeBase):
    pass 

class Dataset_QED_Entry(Base):
    __tablename__ = 'dataset_qed_v2.0a' 
    id = Column(Integer, primary_key=True, autoincrement=True)
    en = Column(String(255))
    lt = Column(String(255))

def FillDatabaseTable(session_maker, table : str, file_en : str, file_lt : str):
    with session_maker() as session: 
        f_en = open(file_en, encoding="utf-8")
        f_lt = open(file_lt, encoding="utf-8")
        l_en = f_en.readline().strip()
        l_lt = f_lt.readline().strip()
        print(l_en)
        print(l_lt)
        while (l_en != "" or l_lt != ""):
            a = Dataset_QED_Entry(en = l_en, lt= l_lt)
            session.add(a)
            session.commit()
            l_en = f_en.readline().strip()
            l_lt = f_lt.readline().strip()
            
        # darbuotojas = Darbuotojas(vardas=vardas, pavarde=pavarde, gimimo_data=gimimo_data, pareigos=pareigos, atlyginimas=atlyginimas)
        # session.add(darbuotojas)
        # session.commit()
        f_en.close()
        f_lt.close()


Base.metadata.create_all(engine)
 
session_maker = sessionmaker(engine)
print(os.getcwd())
FillDatabaseTable(session_maker, "asd", "./DB_Dataset/datasets/QED.en-lt.en", "./DB_Dataset/datasets/QED.en-lt.lt")

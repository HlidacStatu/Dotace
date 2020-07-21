import os
import pandas as pd
import json
import numpy as np
import re
import logging

dotace = pd.read_excel("d:\\Downloads\\agrofert.xlsx")

def json2dict(value):
    value = value.replace(": True", ": true").replace(": False", ": false").replace(": None", ": null").replace("'", '"')
    return json.loads(value)

def sumcerpani(x, year):
    if x == None:
        return 0
    suma = 0
    for roz in x:
        if "cerpani" in roz and roz["cerpani"] != None:
            for cer in roz["cerpani"]:
                if ("rok" in cer and cer["rok"] == year) or ("rok" not in cer and "rok" in roz and roz["rok"] == str(year)):
                    if "castkaSpotrebovana" in cer and cer["castkaSpotrebovana"] != None:
                        suma += float(cer["castkaSpotrebovana"])
    return suma

dotace["rozhodnuti"] = dotace["rozhodnuti"].apply(json2dict)

#dotace["rozhodnuti"] = dotace["rozhodnuti"].map(dict2json)

for year in range(2010, 2021):
    dotace["cerpano"+str(year)] = dotace["rozhodnuti"].apply(lambda x: sumcerpani(x,year))


dotace.groupby("zdroj").sum().drop("ico", axis=1)
(dotace.groupby("zdroj").sum().drop("ico", axis=1)/1000000).to_excel("d:\\Downloads\\summary.xlsx")



#pokusy
dotace["cerpano2017"] = dotace["rozhodnuti"].apply(lambda x: sumcerpani(x,2017))

dotace[dotace["zdroj"]== "szif 2017"].iloc[0]["rozhodnuti"]

True => true
False => false
z = '[{"castkaPozadovana": 13160.0, "castkaRozhodnuta": 13160.0, "rok": 2003, "jePujcka": false, "cerpani": [{"castkaSpotrebovana": 13160.0, "rok": 2003}], "zdrojFinanci": "Ministerstvo zemědělství", "poskytovatel": "Ministerstvo zemědělství", "icoPoskytovatele": "00020478"}]'
[{"castkaPozadovana": null, "castkaRozhodnuta": 15667340.0, "rok": 2017, "jePujcka": false, "cerpani": [{"castkaSpotrebovana": 4692852.0, "rok": 2018}, {"castkaSpotrebovana": 5157070.0, "rok": 2017}, {"castkaSpotrebovana": 5287510.0, "rok": 2019}], "zdrojFinanci": "Technologická agentura České republiky", "poskytovatel": "Technologická agentura České republiky", "icoPoskytovatele": None}]
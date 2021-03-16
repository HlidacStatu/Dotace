#!/bin/sh

cd "$(dirname $0)"

cd cedr
python3 LoadCedr.py
cd ..

cd czechinvest
python3 LoadCzinvest.py
cd ..

cd dotinfo
python3 LoadDotinfo.py
cd ..

cd eufondy
python3 LoadEufondy.py
cd ..

cd szif
python3 LoadSzif.py
cd ..

cd DeMinimis

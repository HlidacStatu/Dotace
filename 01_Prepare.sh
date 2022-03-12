#!/bin/sh

cd "$(dirname $0)"

cd Czechinvest
python3 LoadCzinvest.py
cd ..

cd Dotinfo
python3 LoadDotinfo.py
cd ..

cd Eufondy
python3 LoadEufondy.py
cd ..

cd Szif
python3 LoadSzif.py
cd ..

cd DeMinimis
./run.sh
cd ..
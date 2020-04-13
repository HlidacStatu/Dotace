#!/bin/sh

cd "$(dirname $0)"

./01_Prepare.sh
python3 02_Clean.py
python3 03_Final.py

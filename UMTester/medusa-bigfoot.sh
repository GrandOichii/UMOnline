#!/bin/sh
dotnet run -- \
    --first='../.generated/loadouts/Medusa/Medusa.json'\
    --second='../.generated/loadouts/Bigfoot/Bigfoot.json'\
    --times=1\
    --log\
    --seed=1
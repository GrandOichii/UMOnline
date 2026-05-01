#!/bin/sh
set -e

cd UMModel.Scripts

dotnet run -lp Migrate
dotnet run -lp ImportLoadouts
dotnet run -lp UpdateCoreScript
dotnet run SetPublicLoadouts\
    Medusa\
    "King Arthur"\
    Sinbad\
    Alice\
    Robin Hood\
    Bigfoot
dotnet run CreateContentUpdate
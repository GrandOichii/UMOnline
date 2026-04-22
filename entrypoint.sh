#!/bin/bash
set -e

ConnectionStrings__UMContext="Server=db;Database=um-db;Username=user;Password=password" ./update-server-db.sh

exec dotnet UMServer.dll
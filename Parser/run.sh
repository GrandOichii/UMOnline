mkdir ../card-scripts
rm ../card-scripts/*
mkdir ../reports
rm ../reports/*
dotnet run \
    ../cards.json\
    ../reports\
    ../card-scripts 
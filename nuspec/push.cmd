@echo off
copy *.nupkg C:\users\daewon.kim\dropbox\nuget\ /y
nuget push *.nupkg -ApiKey oy2lmdhkfy4i2ptcweeadyca44pnetpebnre5auezghb54 -Source https://www.nuget.org/api/v2/package
pause
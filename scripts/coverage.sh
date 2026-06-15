#!/bin/bash

dotnet test \
/p:CollectCoverage=true \
/p:CoverletOutputFormat=cobertura \
/p:CoverletOutput=./TestResults/coverage.cobertura.xml

rm -rf CoverageReport

reportgenerator \
-reports:tests/Api.Tests/TestResults/coverage.cobertura.xml \
-targetdir:CoverageReport \
-reporttypes:Html

xdg-open CoverageReport/index.html
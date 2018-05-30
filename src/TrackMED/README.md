# TrackMEDXLS - Create TrackMED Collections Using .NET Framework 4.7

Employs .NET Framework rather than .NET Core because the latter rejects Microsoft.Office.Interop.Excel Nuget package

## Overview

Using Excel Interop, this project creates all collections required by the TrackMED system in the TrackMEDV2 database either of two places depending 
on the MongoSettings/Tmongoconnection setting in appsettings.json of the API server, TrackMEDApi_NetCoreM_470 :

- Mongolab (MLab) server, change mongoconnection to "mongodb://trackmedv2user:matthew24v14@ds044229.mlab.com:44229/trackmedv2"
- c:/data/db, change mongoconnection to "mongodb://localhost:27017/trackmedv2" and start 'mongod' in a command line

Raw data is read from `TrackMEDDataAug2016.xlsx` in System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase) which
translates currently to `C:\CIT\VS\Workspaces\RESMED\TrackMEDXLS\src\TrackMED\bin\net47\`
file:\C:\CIT\VS\Workspaces\RESMED\TrackMEDXLS\src\TrackMED\bin\Debug\net47\TrackMEDDataAug2016.xlsx
----------------------------------------------------------------------------------------------------------
## appsettings.json of TrackMEDApi_NetCoreM_470

    "MongoSettings": {
        "TrackMEDApi": "http://localhost:5000/",
        "Initialize": true,
        "MergeOnly": false,
        "mongoconnection": "mongodb://trackmedv2user:matthew24v14@ds044229.mlab.com:44229/trackmedv2"
    }
}
// "TrackMEDApi": "https://trackmedapi.azurewebsites.net/",
// "TrackMEDApi": "http://localhost:5000/",

// "mongoconnection": "mongodb://trackmedv2user:matthew24v14@ds044229.mlab.com:44229/trackmedv2"
// "mongoconnection": "mongodb://localhost:27017/trackmedv2",
---------------------------------------------------------------------------------------------------------
# Ed-Fi-X Student Engagement Tracking and Reports

Remote Learning Student Engagement Tracking and Reports. These reports cover some basic measures to track student engagement during COVID-19 and Remote Learning.

## Overview

This distribution contains a set of tools that enable the tracking of Student's Learning events. Once we have learning events in a data store we create reports to ensure that student engagement is active and consistent.

These scripts are provided as-is, but the Michael and Susan Dell Foundation and the Ed-Fi Alliance welcomes feedback on additions or changes that would make these resources more user-friendly. Feedback is best shared by raising a ticket on the Ed-Fi Tracker [Exchange Contributions Project](https://tracker.ed-fi.org/projects/EXC).

## Limitations

This repository includes a Google Chrome plugin that captures student navigation. The known limitation is that the students will have to use Google Chrome browser.

## Prerequisites

1. MySQL Server 5.6 or higher, a user with enough priviledges to create schema and the connection string to be able to connect to the database.
2. (Optional) v3.x Ed-Fi ODS database, a user with read rights and a connection string to be able to access the database. This is a recomended option so that we can populate  denormalized student information data up to the reporting database.
3. An internet connection to download and install the binaries.

## Setup Instructions


1. Download the source code in zip format 
<br/><img src="img/download.png" width="300" >
2. Unzip it to a known location like C:\temp\edfi\StudentEngegement
<br/><img src="img/explorer1.png" width="400" >

3. Edit <b>config.json</b> file located in C:\temp\edfi\StudentEngegement\Ed-Fi-X-StudentEngagement-master\Installer and set the following properties: (Look at config.sample.json for an example of how to fill it in.)
   * _StudentLearningEventsConnectionString_ and,
   * _EdFiODSConnectionString_

<img src="img/configjson.png" width="600" >
4. Open PowerShell as an "Administrator"
<br/><img src="img/powershell1.png" width="400" >
5. Navigate to the path where you unzipped the code and into the ~\Installer folder.
<br/><img src="img/powershell2.png" width="500" >
6. Execute the following command:

```PowerShell
C:\temp\edfi\StudentEngegement\Ed-Fi-X-StudentEngagement-master\Installer> .\Install.ps1
```
<br/><img src="img/powershell3.png" width="500" >
<br/>*Note: If you skipped or did not configure the <b>config.json</b> you will see some warnings and it will not let you proceed.
Continue by choosing the type of installation you wish to use. For the sake of these instructions we will use "Install everything"

```PowerShell
C:\temp\edfi\StudentEngegement\Ed-Fi-X-StudentEngagement-master\Installer> ./Install-StudentEngagementTracker
```
Once the install succeeds it will open a chrome browser with the URL of the API and the plugin installed on the top right corner.<br/><img src="img/chrome1.png" width="600" >
<br/> At the end a Chrome window will be open to the webapi URL. Chrome will install the plugin from the web store.

## Configure Google Data Studio Report
We have provided a sample Google Data Studio report template. To be able to update the report we need you to first create a data source and then make a copy of the report.

### Create Data Source

1. On the Data Studio page click on the _Create_ button
<br/><img src="img/CreateDataSource1.png" width="200" >
2. Click on _Data source_
<br/><img src="img/CreateDataSource2.png" width="200" >
3. Input _MySQL_ in the search box
<br/><img src="img/CreateDataSource3.png" width="400" >
4. Configure the Connector
   1. Input the server, database name and credentials and click on the _AUTHENTICATE_ button. This will load the tables on the right.
   2. On the TABLES column, select the _studentengagementreport_ view and 
   3. Finally click on the _Connect_ button on the top right of the screen
<br/><img src="img/CreateDataSource4.png" width="700" >


### Copy report

1. Copy and paste the report link on the web browser (https://datastudio.google.com/open/1A5gklBFxf_1ukqJiI3_kSYLdNY01MypX?usp=sharing)
2. When the report loads, click on the copy icon located on the top right of the screen.
<br/><img src="img/CopySharedReport1.png" width="500" >
3. When prompted, select the new data source you created previously and click on the _Copy Report_ button
<br/><img src="img/CopySharedReport2.png" width="500" >
4. Enjoy


## Production Release Notes (Coming soon).

## Contributing

Looking for an easy way to get started? Search for tickets with label
"up-for-grabs" in Tracker **[Link that text to a pre-existing query for the
project]**; these are nice-to-have but low priority tickets that should not
require in-depth knowledge of the code base and architecture.

## Legal Information

Copyright (c) 2020 Ed-Fi Alliance, LLC and contributors.

Licensed under the [Apache License, Version 2.0](LICENSE) (the "License").

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

See [NOTICES](NOTICES.md) for additional copyright and license notifications.
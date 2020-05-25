# Ed-Fi-X Student Engagement Tracking and Reports

Remote Learning Student Engagement Tracking and Reports. These reports cover some of the basic measures to track student engagement during COVID19  and Remote Learning.

## Overview

This distribution contains a set of tools that enable the tracking of Student's Learning events. Once we have learning events in a data store we create reports to ensure that student engagement is active and consistent.

These scripts are provided as-is, but the Michael and Susan Dell Foundation and the Ed-Fi Alliance welcomes feedback on additions or changes that would make these resources more user friendly. Feedback is best shared by raising a ticket on the Ed-Fi Tracker [Exchange Contributions Project](https://tracker.ed-fi.org/projects/EXC).

## Limitations

This repository includes a google chrome plugin that captures student navigation. The known limitation is that the students will have to use Google Chrome browser.

## Prerequisites

1. MySQL Server 5.6 or higher 
2. MsSQL Server 2017 or higher with edfi 3.4 database ETL student information data
3. Internet connection to download and install missing dependencies

## Setup Instructions


1. Edit [config.json](./config.json) file to configure properties _StudentLearningEventsConnectionString_ and _EdFiODSConnectionString_ in _BinaryMetadata.ApiBinaries.ConnectionString_
<br/><img src="img/configjson.png" width="300" >
1. Download the scripts in zip format 
<br/><img src="img/download.png" width="300" >
2. Unzip them to a known location like C:\temp\edfi\StudentEngegement
<br/><img src="img/explorer1.png"  width="400" >
3. Open PowerShell as an "Administrator"
<br/><img src="img/powershell1.png" width="400" >
4. Navigate to the path where you unziped the scripts
<br/><img src="img/Powershell2.png" width="400" >
5. Execute the following command:

```PowerShell
C:\temp\edfi\StudentEngegement\> .\Install.ps1
```
<br/><img src="img/Powershell3.png" width="400" >
<br/>There will be some wearnings if you didn't configure the connection strings in [config.json](./config.json). Then choose the type of installation you wish to use. For the sake of this tutorial we will do "Install everything"

```PowerShell
C:\temp\edfi\StudentEngegement\> ./Install-StudentEngagementTracker
```

<br/><img src="img/chrome1.png" width="400" >
<br/> At the end a chrome window will be open to the webapi url. Chrome will install the plugin from the web store.


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
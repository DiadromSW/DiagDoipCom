[![Qodana](https://github.com/DiadromSW/DiagDoipCom/actions/workflows/code_qodana.yml/badge.svg?branch=develop)](https://github.com/DiadromSW/DiagDoipCom/actions/workflows/code_qodana.yml) 

# Introduction 
DiagCom represents a vehicle communication REST service based on DoIP(IS013400)/UDS(ISO14229). It identifies on the local network vehicles that have Doip support and gives the possibility to run diagnostic services on detected vehicles. Identification is done by reading UDP Vehicle Identification Messages, that provide information about vehicle IP and VIN. 
Besides vehicle detection and running Diagnostic services, the service has few predefined vehicle operations, such as Clear and Read DTC and Read Battery Voltage. As well as a few logg-related endpoints as start/stop logging and extract logging for a specific vehicle. 
  
The DiagCom service can be installed via Wix installer and run as windows service on http://localhost:5001 with local system account. It can be also used as a library by importing DiagCom nuget(https://www.nuget.org/packages/DiagCom.DoipCommunication/) or by using directly from the provided source code.

# Get started
The DiagCom installer can be found attached as a [release](https://github.com/DiadromSW/DiagDoipCom/releases/tag/Diadrom). It is a self-contained package that will install DiagCom and start it as a local service on port 5001. Service Api can be explored and tested with swagger http://localhost:5001/index.html. 

DiagCom is built to communicate with all the vehicle that has DoIP support. The vehicle needs to be connected to the same network as DiagCom through an Ethernet to ODB cable. Ignition ON is required as well to identify and create DoIP connection to the vehicle.

# DiagCom Api
DiagCom uses Swagger to document and visualise endpoints. It can be accessed via (https://localhost:5001/index.html). There are a Vehicle Comminication API description on the [Wiki](https://github.com/DiadromSW/DiagDoipCom/wiki/DiagCom) .

The following diagnostic services endpoints are exposed by DiagCom:

- GetConnectedVehicles - Returns a list of connected vehicles VINs. Vehicles are identified by catching UDP messages brodcasted by Doip Ecu. No TCP connection to the vehicle is established at this operation. 

- RunDiagnosticSequence - Creates from payload a sequence of diagnostic services and runs them synchronously on a specified vehicle. This request requires a JSON diagnostic sequence object as input. The JSON structure are document in Swagger. It contains a list of services that need to be run on a vehicle and information about parameters or routines that need to be parsed by DiagCom. The response can be the raw data or parsed values in case the parsed object are specified.

- ReadClearDTCs - This request runs a functional Read DTCs and functional Clear DTCs operation if flag Erase is true before reading. If any ECU does not respond a physical request is sent. The result is merged with payload input ECUs and not responding ECUs are detected.

- ReadBatteryVoltage -  Request the battery voltage of a vehicle with a specified VIN. Did 0xDD02  read to return the value in millivolts.

- RawDiagnosticSequenceAsync - provides the same functionality as DiagnosticService without Parsning and without DiagnosticSequense as input. Check Swagger for more details.

# DiagCom Solution
The provided solution has 8 main projects:

- DiagCom.RestApi - represents the Rest APi wrapper of DiagCom. It is responsible for starting the service on a specified host and resolving project dependencies.
- DiagCom.Commands - can be imported and used without RestApi. It contains predefined vehicle commands, connections and running context. 
- DiagCom.Uds - Contains logic related to diagnostic services, and retries on the UDP level.
- DiagCom.Doip - Handles Doip Request/Response  
- DiagCom.LocalCommunication - Contains connection logic and processing of Diagnostic messages received from vehicle
- DiagComSetup - The project is to build the DiagCom installer. Wixtoolset extension needs to be available in VS. This can be found in the Visual Studio tab Extension -> Manage Extensions
- Logging -  has two methods to log both done via Nlog (https://nlog-project.org/). The first is logging into the log file "SystemLog.log". This logging begins when the DiagCom service starts. SystemLog logs the general flow for DiagCom. The configuration of the log rules is static in the "nlog.config" file in the RestApi project. The second logging method is for "Vin Logs" These logs ISO14229. The configuration is dynamic at LogHandler.StartLogging().  
- CustomActions - There is a Cors certificate required to run DiagCom as Service. It is created as a custom action during DiagCom installation. 

# References and Licenses

| Reference                                       | Version | License Type    | License                                                               |
|-------------------------------------------------|---------|-----------------|-----------------------------------------------------------------------|
| coverlet.collector                              | 3.1.2   | MIT             | https://licenses.nuget.org/MIT                                        |
| FluentValidation.AspNetCore                     | 11.2.2  | Apache-2.0      | https://licenses.nuget.org/Apache-2.0                                 |
| Microsoft.AspNetCore.Mvc.Core                   | 2.2.5   |                 | https://raw.githubusercontent.com/aspnet/AspNetCore/2.0.0/LICENSE.txt |
| Microsoft.Extensions.FileProviders.Abstractions | 6.0.0   | MIT             | https://licenses.nuget.org/MIT                                        |
| Microsoft.Extensions.FileProviders.Physical     | 6.0.0   | MIT             | https://licenses.nuget.org/MIT                                        |
| Microsoft.Extensions.Hosting                    | 7.0.0   | MIT             | https://licenses.nuget.org/MIT                                        |
| Microsoft.Extensions.Hosting.WindowsServices    | 7.0.0   | MIT             | https://licenses.nuget.org/MIT                                        |
| Microsoft.Extensions.Logging                    | 7.0.0   | MIT             | https://licenses.nuget.org/MIT                                        |
| Microsoft.Extensions.Logging                    | 6.0.0   | MIT             | https://licenses.nuget.org/MIT                                        |
| Microsoft.NET.Test.Sdk                          | 17.3.2  | LICENSE_NET.txt | https://www.nuget.org/packages/Microsoft.NET.Test.Sdk/17.3.2/License  |
| Microsoft.VisualStudio.Threading                | 17.5.22 | MIT             | https://licenses.nuget.org/MIT                                        |
| Moq                                             | 4.18.4  |                 | https://raw.githubusercontent.com/moq/moq4/main/License.txt           |
| Newtonsoft.Json                                 | 13.0.3  | MIT             | https://licenses.nuget.org/MIT                                        |
| NLog.Web.AspNetCore                             | 5.2.1   | BSD-3-Clause    | https://licenses.nuget.org/BSD-3-Clause                               |
| NUnit                                           | 3.13.3  | LICENSE.txt     | https://www.nuget.org/packages/NUnit/3.13.3/License                   |
| NUnit.Analyzers                                 | 3.3.0   | license.txt     | https://www.nuget.org/packages/NUnit.Analyzers/3.3.0/License          |
| NUnit3TestAdapter                               | 4.2.1   | MIT             | https://licenses.nuget.org/MIT                                        |
| Swashbuckle.AspNetCore.SwaggerGen               | 6.4.0   | MIT             | https://licenses.nuget.org/MIT                                        |
| Swashbuckle.AspNetCore.SwaggerUI                | 6.4.0   | MIT             | https://licenses.nuget.org/MIT                                        |
| System.IO.FileSystem.AccessControl              | 5.0.0   | MIT             | https://licenses.nuget.org/MIT                                        |
| System.Management                               | 6.0.0   | MIT             | https://licenses.nuget.org/MIT                                        |



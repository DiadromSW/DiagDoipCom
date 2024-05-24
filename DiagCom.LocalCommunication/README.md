# Introduction 
DiagCom represents a package for vehicle communication based on Doip(IS013400)/UDS(ISO14229). It identifies on the local network vehicles that have Doip support and gives the possibility to run diagnostic services on detected vehicles. Identification is done by reading UDP Vehicle Identification Messages, that provide information about vehicle IP and VIN. 
Besides vehicle detection and running Diagnostic services, the package has few predefined vehicle commands, such as Clear and Read DTC and Read Battery Voltage. As well as a few logg-related endpoints as start/stop logging and extract logging for a specific vehicle. 
  

# Diagnostic Communication Api

In order to run diagnostic commands a vehicle monitor process and a running context need to be created. It can be done by using dependency injection or by an instance of identification monitor and context api(DiagComApi). Vehicle identification monitor(DoipEntityMonitor) is an async method that can be run in a separate process.  
```cs
var loggerFactory = new LoggerFactory();
var entityLockupLogger = loggerFactory. CreateLogger<EthernetDoipEntityLookup>();

var doipEntityLookup = new EthernetDoipEntityLookup(entityLockupLogger);
var monitor = new DoipEntityMonitor(_doipEntityLookup);
var cancelationToken = new CancellationTokenSource();

await monitor.StartMonitoringAsync(cancelationToken.Token);
```

 DiagComApi needs DoipEntityMonitor, logs and local parse to be instantiated. It contains ExecuteAsync method that runs predefined commands on the specified vehicle.

**Get Connected Vehicles**

This command returns VINs received and extracted from brodcasted as UDP Vehicle Identification Messages. The result is a list of strings.

```cs
var command = new GetVinsCommand();
var result = await diagComApi.ExecuteAsync(command);
```


**Read and Clear DTCs**


Read vehicle's DTCs and runs functional before reading clear operation if erase flag is true.
If any ECU does not respond a physical request read dtc is sent. Result is merged with input ECUs and not responding ECUs are detected.


```cs
var readDtcsCommand = new ReadDtcCommand(ecus:new ushort[3] { 312, 232, 323 }, erase:true);
var dtcsResult = await diagComApi.ExecuteAsync(vin, readDtcsCommand);
```


**Get Dtc Status**

Retrieve DTC extended data associated with defined DTC. Extended data consist of status and indicator. 

```cs
var getDtcsStatus = new GetDtcStatusCommand(ecuAddress:"1001", dtcId:"32F321");
var dtcsStatusResult = await diagComApi.ExecuteAsync(vin, getDtcsStatus);

```


**RawDiagnosticSequence**

Executes a sequence of diagnostic services.

```cs
var rawDiagSequence = new SingleDiagnosticServiceCommand(targetAddress:321, request: new byte[] { 0x19, 0x02});
var rawDiagSequenceResult = await diagComApi.ExecuteAsync(vin, rawDiagSequence);

```


**Run Diagnostic Sequence**

Creates a sequence of diagnostic services and runs them synchronous on specified vehicle.

```cs
var diagSequence = new DiagnosticSequence()
{
    Identifier = "id",
    Sequence = new List<DiagnosticService>()
    {
        new DiagnosticService
        {
            EcuAddress = 321,
            Description = "Service to run description",
            Service = "22",
            Payload = "DD01"
        }
    }
};

var diagSequenceCommand = new DiagnosticSequenceCommand(sequence: diagSequence);
var diagSequenceResult = await diagComApi.ExecuteAsync(vin, diagSequenceCommand);
```

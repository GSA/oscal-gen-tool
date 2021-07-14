
# OSCAL-Generation Tool
This application captures the data required for generating an OSCAL XML System Security Plan (SSP), Security Assessment Plan (SAP) and Security Assessment Report (SAR) file in XML based on the NIST OSCAL Milestone 3 schema utilizing a Microsoft ASPX.NET web application.  
# Why the project is useful
NIST is developing the Open Security Controls Assessment Language (OSCAL), a set of hierarchical, formatted, XML- and JSON-based formats that provide a standardized representation for different categories of information pertaining to the publication, implementation, and assessment of security controls. OSCAL is being developed through a collaborative approach with the public. The OSCAL website (https://csrc.nist.gov/Projects/Open-Security-Controls-Assessment-Language) provides an overview of the OSCAL project, including an XML and JSON schema reference and examples
The application provides a way to capture the required data and output an OSCAL compliant XML file for the Milestone 3 release of the SSP, SAP and SAR.
# Project Requirements
The application is coded in C# as an ASP.NET web application Visual Studio project and is meant to run in standalone mode ONLY.   This project utilizes the OpenXML (DocumentFormat.OpenXml) namespace to perform XML parsing and perform document rendering.
# System Software Requirements
Windows 10
Visual Studio 2019 Community Edition
SQL Server Express 2019
SQL Server Management Studio (SMSS) (latest version)

# Getting started with this project
1. Install Visual Studio 2019 Community Edition
2. Clone and checkout the Project("https://github.com/GSA/oscal-generator”)
3. Clean and Build the Project
4. Download and install SQL Server Express 2019 (https://www.microsoft.com/en-us/sql-server/sql-server-downloads).   
•	Configure security for both SQL Server and Windows Authentication Mode.
•	Install as default or named instance to your computer.
5. Download and install SMSS (https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15).
6. Locate the /DB folder in the downloaded project, unzip the compressed archive OWT.zip, and restore the OWT.BAK (SQL backup) with SMSS.
7. Modify the web.config file in the project and enter your SQL server instance name on the DBCONN string and appropriate credentials.   The default web.config will have (local) in the server name from the project download.  It will also have a default user id of OWT and password “p@ssw0rd” and will be configured for SQL Server authentication.   Feel free to change this to Windows Integrated Authentication if so desired.
8.  Run the project in Visual Studio.


# Known Issues
You may have the following error when running the code for the first time
"Could not find a part of the path '\OSCAL-Conversion\OSCAL Generator\bin\roslyn\csc.exe'".
This issue can be resolved by installing the Nuget Package:  Microsoft.CodeDom.Providers.DotNetComplierPlatform.NoInstall
# License Information
This project is being released under the GNU General Public License (GNU GPL) 3.0 model. Under that model there is no warranty on the open source code.   Software under the GPL may be run for all purposes, including commercial purposes and even as a tool for creating proprietary software.
# Disclaimer
This project will ONLY work to produce the XML for the Milestone release 3 of the OSCAL schema from the NIST website (https://csrc.nist.gov/Projects/Open-Security-Controls-Assessment-Language).
# Getting help with this project
Contact the GSA FedRAMP Project Management Office for more information or support.
# Originator of Code
VITG, INC.  http://www.volpegroup.com


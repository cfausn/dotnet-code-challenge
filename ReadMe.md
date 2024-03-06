# Mindex Coding Challenge
## What's Provided
A simple [.Net 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) web application has been created and bootstrapped 
with data. The application contains information about all employees at a company. On application start-up, an in-memory 
database is bootstrapped with a serialized snapshot of the database. While the application runs, the data may be
accessed and mutated in the database without impacting the snapshot.

### How to Run
You can run this by executing `dotnet run` on the command line or in [Visual Studio Community Edition](https://www.visualstudio.com/downloads/).


### How to Use
The following endpoints are available to use:
```
* CREATE
    * HTTP Method: POST 
    * URL: localhost:8080/api/employee
    * PAYLOAD: Employee
    * RESPONSE: Employee
* READ
    * HTTP Method: GET 
    * URL: localhost:8080/api/employee/{id}
    * RESPONSE: Employee
* UPDATE
    * HTTP Method: PUT 
    * URL: localhost:8080/api/employee/{id}
    * PAYLOAD: Employee
    * RESPONSE: Employee
* READ
    * HTTP Method: GET
    * URL: localhost:8080/api/employee/reportingStructure/{id}
    * RESPONSE: ReportingStructure JSON object
* CREATE
    * HTTP Method: POST
    * URL: localhost:8080/api/compensation/{employeeId}
    * PAYLOAD: Compensation JSON object excluding Employee object
    * RESPONSE: Compensation JSON object including Employee object

* READ
    * HTTP Method: GET
    * URL: localhost:8080/api/compensation/{employeeId}
    * RESPONSE: Compensation JSON object
```
The Employee has a JSON schema of:
```json
{
  "type":"Employee",
  "properties": {
    "employeeId": {
      "type": "string"
    },
    "firstName": {
      "type": "string"
    },
    "lastName": {
          "type": "string"
    },
    "position": {
          "type": "string"
    },
    "department": {
          "type": "string"
    },
    "directReports": {
      "type": "array",
      "items" : "string"
    }
  }
}
```
Compensation has a JSON schema of:
```json
{
  "type": "Compensation",
  "properties": {
    "employee": { "$ref": "#/definitions/Employee" },
    "salary": { "type": "number" },
    "effectiveDate": { "type": "string", "format": "date-time" }
  }
}
```

For all endpoints that require an "id" in the URL, this is the "employeeId" field.

## What to Implement
Clone or download the repository, do not fork it.

### Task 1
Create a new type, ReportingStructure, that has two properties: employee and numberOfReports.

For the field "numberOfReports", this should equal the total number of reports under a given employee. The number of 
reports is determined to be the number of directReports for an employee and all of their direct reports. For example, 
given the following employee structure:
```
                    John Lennon
                /               \
         Paul McCartney         Ringo Starr
                               /        \
                          Pete Best     George Harrison
```
The numberOfReports for employee John Lennon (employeeId: 16a596ae-edd3-4847-99fe-c4518e82c86f) would be equal to 4. 

This new type should have a new REST endpoint created for it. This new endpoint should accept an employeeId and return 
the fully filled out ReportingStructure for the specified employeeId. The values should be computed on the fly and will 
not be persisted.

### Task 2
Create a new type, Compensation. A Compensation has the following fields: employee, salary, and effectiveDate. Create 
two new Compensation REST endpoints. One to create and one to read by employeeId. These should persist and query the 
Compensation from the persistence layer.

#### Concurrency Handling

The application has been enhanced to handle concurrent updates to Compensation records. When multiple updates for the same employee are attempted concurrently, the application ensures data consistency and applies the last update.

## Using Docker
This application is Docker-compatible, allowing for easy setup and deployment. The provided `Dockerfile` and `docker-compose.yml` facilitate building and running both the application and its integration tests in isolated containers.

### Running the Application with Docker
1. Navigate to the project root directory in your terminal.
2. Build and run the application using Docker Compose:
    ```bash
    docker-compose up --build
    ```
    This command builds the application and test images and runs containers based on those images. The `--build` flag ensures that Docker re-builds the images if the source code has changed since the last build.
3. The application is now running and accessible at `http://localhost:8080`.

### Running Tests with Docker
Integration tests run automatically as part of the Docker Compose setup. The test container waits for the application container to become fully operational before executing the tests to ensure all endpoints are available.

Results of the tests are displayed in the console output. To re-run the tests, simply re-execute the `docker-compose up --build` command, which also re-builds and runs the application.

### Stopping the Application
To stop the application and test containers, use the following command in your terminal:
```bash
docker-compose down
```

## Delivery

This updated README provides a clear and concise guide for users to understand the functionalities offered by your application, including how to interact with new endpoints related to `ReportingStructure` and `Compensation`. It also introduces the notion of concurrency handling, informing users about the application's behavior in concurrent update scenarios.

# Background-Tasks-with-hosted-services
Queued background tasks .net core 2.1




In ASP.NET Core, background tasks can be implemented as hosted services. A hosted service is a class with background task logic that implements the IHostedService interface.



This project is divided into three main sections of development, and each of them is targeting
different level of skills, experience and coding patterns.





Dynamic Link Library (DLL) 

In the first part, we are going to create a library project. In this library, we will consume a
third party (external) free API, which provides information about IP addresses. This library
will expose a couple of public classes, through which the rest of the code will
communicate with the library and therefore with the API.



The Net library that will be created, will encapsulate all the necessary logic and code for the
communication with IPStack . 

IPStack is the external service we are going to consume.  https://ipstack.com

The library will handle any possible exceptions that may be thrown during the request to the
api, and if any, it must throw a custom exception named “IPServiceNotAvailableException” to the
caller.





 WebApi 

In the second part, we are going to create a WebApi project. This project will use the
library we previously created, and expose some functionality to web. For example, it may
serve a request for details for a specific IP, use the library to get the necessary
information, and send them back to client.



The most important Task

We will implement a background service to handle the long running tasks.


Batch Request Job 
In the last part, we are going to add some extra functionality to WebApi. It needs to
support a batch operation for updating IP details. First, it must expose a method where a
post request can be made providing an array of items of IP details that should be
updated. The caller will get a GUID as response, with which they can call a second
method of the WebApi later to get information about the progress of the job. Posted
items, will be put into a buffer, where they will be processed in batches of 10 items at a
time.

# Testing
Below are the remaining notes and recommendations out of taking and working through several test, mock, and reporting
frameworks to match the expected scope and complexity of the services projects/

## Recommendation
This section covers the recommended selections for repository structure, testing frameowrk and mock creation that have
been used in the accompanying unit tests.

### Testing Framework: xUnit
In general the provided frameworks have much of a sameness, likely due to all containing first party support in the .NET
frameowrk due to all being part of the .NET foundation and so the choice of xUnit comes down to a few small points in
its favour.

Simplicity: xUnit cuts out some of the attribute verbiage used in the other testing frameowrks, which have been seen as
a common point of confusion and code duplication, having two points of setup (constructor and specific attribute marked
method) could create some hard to track down side effects for differing test setups and so its better to avoid this
possibility outright.

Parallelism: While other frameworks do not have anything particular against test parallelism, they do not have it
specifically supported, addressed and documented in the way that the xUnit framework tries to. As the complexity of
tests grow and in order to cleanly design around unexpected results from run order this speaks in favour of providing
future options to the services projects.

### Mocking Library: Moq
One of the long running and most stable libraries, it has a simple method for creating mock classes within managed code
and has no special requirements. It is recommended for any actions that intend to be mocked (such as downstream request
calls) that the whole handling of the request logic be mocked to simplify testing and setup

A note for HTTP requests if wishing to go the route of url interception it is recommended to create an `HttpClient`
instance with the constructor allowing for provisioning of a `HttpMessageHandler` that can intercept request handling
and return

### Repository Structure
Several structural options without a solutions file were tried in order to work out the simplest project structure to
include tests but avoid shipping test related code or data with a final build, in the end the solutions file was the
right way to go so this project structure was modified to place tests in their own separate project that made reference
to the source services project.

Note this should have no effect on the build (`dotnet build)`, `dotnet publish`) processes for the most part.
Documentation will be added to the main level readme about building the services only in a generic form.

### Optional: Code Coverage
Will not make this a hard recommendation with the general difficulties of integrating it into pipelines, however
coverage reports are useful tools for identifying what areas may require more testing scrutiny.

[ReportGenerator](https://github.com/danielpalme/ReportGenerator) Is one such tool for producing code coverage reports,
allowing for a breakdown by file/class as well as line coverage indicator and count marks.


## Notes and references

### Testing Frameworks
In general the testing framework options are much of a sameness, they all experience first party support within the
.NET Frameowrk ecosystem among the dotnet toolchain and major IDEs and as such any could suffice fo the project.

#### NUnit
One of the longest running testing frameworks, originally ported from JUnit, making use of the common verbiage for
annotations such as Setup, TearDown, Test and assert statements.

In its favour is the fact that it has been an incredibly long running project, especially because of its initial JUnit
roots. There is a more verbose setup in tests here, owing to its JUnit roots and the languages lack of attribute
equivalent long ago which many comment on it leading to confusion or duplication of setup, this is probably the main
thing against its pick as the main option.


#### xUnit
Created by the original NUnit developers it is more of a fresh start designed to more match the .NET based languages
than the Java originating NUnit and to integrate with multiple sets of development tools. Notably it has a focus of
test parallelism  and matching test structures to language capabilities delegating things like setup and tear down to
standard class constructor and dispose semantics.


#### MSTest
The original .NET testing framework which for the longest time was only available in the Visual Studio IDE but has since
become more usable through the general dotnet framework and tooling. Sematic's are similar to that of NUnit with the only
feature it contains beyond its set as assembly level setup and tear down attributers.

While this may be the original and main testing framework for .NET development, it appears to get no more attention than
its competitors especially with them being included for stewardship by the .NET foundation. Not long ago the tie in of
setup of Microsoft fakes and Visual studio with MSTest may have been a point in its favour if that was the preferred
mocking option however the lack (until recently) of .NET Core support for that feature shows a general lag of support.



### Mocking Libraries

#### Moq
A fairly traditional and simple to use mocking library where the user is able to initialise mocks of a given class type
and define the expected response values for given methods and inputs. In its favour is both the ease of creating new
mock objects and the long life of the project.

#### Telerik JustMock Lite
Relatively straightforward mocking system with the only real main comment to put behind it is its maintained by Telerik
and as such this may be a consideration if deciding to go all in on their other products. Note that some of the
functionality is locked behind a paid version, notably features around entity framework and LINQ result mocking.

#### Microsoft Fakes
This seems to be a Microsoft method for creating stubs and shims but unlike other options relies on some library
reference level configuration as opposed to the traditional create fake object and add properties of more traditional
mocking libraries.

There has been a considerable lag in getting this up to support with the .NET core framework and shows a general lag in
development, while there may be some usefulness in the project reference level setup approach the expected complexity
of individual service projects means its unlikely to be of any use.

#### Pose
Pose seems to have been a project to emulate Microsoft Fakes but from a purely managed code perspective, its only worth
mentioning that it exists as a potential alternative to a Microsoft Fakes style of mocking as the project appears to
have been abandoned and has seen no significant update in years.

## Reference notes
* https://learn.microsoft.com/en-us/dotnet/core/testing/
* https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-dotnet-test
* https://xunit.net/
* dotnet new supports all of xUnit, NUnit and mstest for project template generation
  * need to work out what the ideal is here as they are all similar-but-different in how they define things
* look into CICD integration on each in azure pipelines
  * https://learn.microsoft.com/en-us/azure/devops/pipelines/ecosystems/dotnet-core?view=azure-devops&tabs=dotnetfive
  * seems to rely on the `dotnet test` command execution so should make it easy to integrate all examples with
  * work out in-memory vs temp file for db data testing and how that works with pipelines
  * will probably be a recommendation for in-memory to avoid complications
  * code coverage integrations, do we add this as a requirement? good to have but does it enforce laziness

### mocking
* current library options:
  * moq: seems to be the most referenced one
    * LINQ style call support makes for simpler reading of setup logic
    * https://github.com/moq/moq4
  * JustMock Lite: free version of Telerik mocking suite, may be useful due to Telerik tie in
    * strongly typed mocks to reduce setup errors (need to see if moq lets this typing slide at compile)
    * https://github.com/telerik/JustMockLite
  * microsoft fakes
    * provides a stubs and shims set of functionality
    * seems to require specific visual studio integration (or possibly msbuild/mstest for running as opposed to dotnet)
    * requires microsoft visual studio enterprise
    * https://learn.microsoft.com/en-us/visualstudio/test/isolating-code-under-test-with-microsoft-fakes?view=vs-2022
  * Pose
    * open source managed code equivalent of microsoft fakes
    * does not appear to have been updated in years
    * https://github.com/tonerdo/pose

### Test Coverage
* https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-code-coverage?tabs=linux
* https://github.com/danielpalme/ReportGenerator
* not a direct microsoft tool but one references in their learn articles
* no other direct recommendation from the testing documentation sections
* creating reports seems less programmatic with each run of the underlying coverage output generating a unique folder for results each time, as well as generation being a 2 part process.
* install as a global tool `dotnet tool install -g dotnet-reportgenerator-globaltool`

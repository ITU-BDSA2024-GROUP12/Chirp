---
title: _Chirp!_ Project Report
subtitle: ITU BDSA 2024 Group 12
author:
- "Johannes johje@itu.dk"
- "Christian cmol@itu.dk"
- "Mathias bamj@itu.dk"
- "Rasmus rsni@itu.dk"
- "Peter pblo@itu.dk"
numbersections: true
---

# Design and Architecture of _Chirp!_

## Domain model

Here comes a description of our domain model.

![Illustration of the _Chirp!_ data model as UML class diagram.](./Images/DomainModel.png)

## Architecture — In the small

![Onion Diagram](./Images/Onion.drawio.svg)

## Architecture of deployed application

![Deployed Application](./Images/DeployedApplication.png)
![CSharp Middleware](./Images/cSharp_middleware_pipeline.svg)

## User activities
![User activities](./Images/ActivityDiagram.png)


## Sequence of functionality/calls trough _Chirp!_
![Sequence Diagram Authorized](./Images/sequenceDiagramAuthorized.png)
![Sequence Diagram NonAuthorized](./Images/sequenceDiagramNonAuthorized.png)
![Sequence Diagram Private Timeline](./Images/sequenceDiagramPrivateTimeline.png)

# Process

## Build, test, release, and deployment
![Build, test, release, and deployment](./Images/BDT.jpg)

## Team work

## How to make _Chirp!_ work locally
To make _Chirp!_ work locally, you have to first have to install prerequisites

- dotnet 7 sdk
- git
- pwsh


 clone the repository

```bash
git clone https://github.com/ITU-BDSA2024-GROUP12/Chirp.git
```

Change directory to `Chirp.Web`
```bash
cd Chirp/src/Chirp.Web
```

Add user secrets by typing:
```bash
dotnet user-secrets init
dotnet user-secrets set "authentication_github_clientId" "Ov23liDE0T7SBaQRUByB"
dotnet user-secrets set "authentication_github_clientSecret" "0c6877a3701918d0def7b409dac6efd53b5b15f3"
```
(These secrets have been generated specifically for use in the exam)

GitHub OAuth has been configured to the Chirp! application running on <https://localhost:7102/>

Start the application with `dotnet run` 
```bash
dotnet run
```

Your application should start listening on two ports, you should open the `https` one. which, if not changed directly should be localhost port 7102 `https://localhost:7102`

## How to run test suite locally

To test the application locally, first change director back to root
```bash
cd ../../
```
You should now be in `C:/YOURPC/SOMEWHERE/Chirp`

if playwright is not installed, you should install it with
```bash
pwsh test/Chirp.UI.Tests/bin/Debug/net7.0/playwright.ps1 install --with-deps
```

(OBS. since the application uses dotnet 7, newer versions of powershell might be incompatible)

You can now test the whole project with `dotnet test`
```bash
dotnet test
```


# Ethics

## License
The MIT License

## LLMs, ChatGPT, CoPilot, and others
During the development of _Chirp!_, we used LLMs for debugging code, warnings and making functional regex's.  
Some of the reponses were quite helpful, and worked. While others did not work continued to not work, for example, if a promt was in a wrong direction, correctness wise, the LLMs would not catch that, and try to solve an if not already unsolvable, then just a wrong answer to a problem.


Even though some of the promts did not yield the correct response/result, some of the reasonings the LLMs gave, would make the development better in the lager stages.

Other LLMs like build in intellisense, with codecompletions has also been used, to speed up typing. This did also come with downsides, such as it opscuring the intended completion with other, and sometimes wrong code.

The LLMs summed up, sped up our work, by a small factor.
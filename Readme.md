# GreenDot Durable Demo


This is the Azure Durable Function test project (https://github.com/dchen1-greendotcorp/FunctionDurableAppTest).

## First http trigger with data
trigger url [http://localhost:7225/api/CreateAccount]
trigger data: 
{
  "userName": "davidchen",
  "accountId":"davidchen-create-request"
}

##First trigger running result
1. two activities (SaveAccountActivity, ArchiveAccount) ok;
2. third one (NotifyAccountActivity) throw exception Orchestration. 
result:
{
	"finishOrchestrationSuccess": false,
	"runningStatus":""
}

## Second http trigger with data
trigger url [http://localhost:7225/api/CreateAccount]
{
  "userName": "davidchen",
  "accountId":"davidchen-create-request"
}

##Second trigger running result
1. two activities (SaveAccountActivity, ArchiveAccount) business did not run since first round running good;
2. third one (NotifyAccountActivity) running good this time. 
result:
{
	"finishOrchestrationSuccess": good,
	"runningStatus":""
}

## Code practice

### 1. Read Orchestrator function code constraints [https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-code-constraints?tabs=csharp]

### 2. Add Microsoft.Azure.WebJobs.Extensions.DurableTask.Analyzers package in the project. When you build the project, it may give some warnings of contrains, thought you shoud not fully depend on it.

### 3. Make all trigger functions return 200, with "finishOrchestrationSuccess", add additional data in the return if needed. Callers can check finishOrchestrationSuccess to decide retry call trigger functions. 



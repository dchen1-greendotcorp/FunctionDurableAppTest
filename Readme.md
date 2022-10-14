# GreenDot Durable Demo


This is the Azure Durable Function test project (https://github.com/dchen1-greendotcorp/FunctionDurableAppTest).

## First http trigger with data
trigger url [http://localhost:7225/api/CreateAccountRequest]
trigger data: {
  "userName": "davidchen"
}

##First trigger running result
1. two activities (SaveAccount, ArchiveAccount) ok;
2. third one (NotifyAccount) throw exception first time and be caught by Orchestration. Orchestration is still in running status.

## Second http trigger with data
trigger url [http://localhost:7225/api/CreateAccountRequest]
{
  "userName": "davidchen",
  "processInstanceId":"98ea0614785c42a2ac5a757aef7d8966",
  "accountId":"9da16122-041a-4c50-b47c-e9f6da5626a0"
}

##Second trigger running result
1. two activities (SaveAccount, ArchiveAccount) did not run since first round running good;
2. third one (NotifyAccount) running good this time. 

## Code practice

### 1. Read Orchestrator function code constraints [https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-code-constraints?tabs=csharp]

### 2. Add Microsoft.Azure.WebJobs.Extensions.DurableTask.Analyzers package in the project. When you build the project, it may give some warnings of contrains, thought you shoud not fully depend on it.

### 3. Orchestrator can try catch exceptions.

### 4. Use context.WaitForExternalEvent to register events in Orchestrator

### 5. Http trigger Send events with client.RaiseEventAsync 

### 6. Using infinity while loop and cancellation token including timer to controll the Orchestrator running status

### 7. Using client.GetStatusAsync(instanceId, true, true) to check Orchestrator status.



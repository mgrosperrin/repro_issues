var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.AspireNoDebug_ApiService>("apiservice");

builder.AddProject<Projects.AspireNoDebug_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();

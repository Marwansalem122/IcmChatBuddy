var builder = DistributedApplication.CreateBuilder(args);

var ollama=builder.AddOllama("ollama")
    .WithDataVolume();
//var chatModel=ollama.AddModel("chat", "llama3.2");
var chatModel=ollama.AddModel("chat", "llama3.2:1b");

//builder.AddContainer("open-webui","ghcr.io/open-webui/open-webui","main")
//     .WithHttpEndpoint(port: 3000, targetPort: 8080, name: "http")
//    .WithEnvironment("OLLAMA_BASE_URL", ollama.GetEndpoint("http"))
//    .WithLifetime(ContainerLifetime.Persistent)
//    .WaitFor(ollama);
builder.AddProject<Projects.IcmChat>("icmchat")
    .WithReference(chatModel)
    .WaitFor(chatModel);
   

builder.Build().Run();

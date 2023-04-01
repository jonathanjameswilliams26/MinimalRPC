global using OneOf;
global using FluentValidation;
global using MinimalRPC.Endpoints.Common;
using MinimalRPC;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureServices();
var app = builder.Build();
app.Configure();
app.Run();
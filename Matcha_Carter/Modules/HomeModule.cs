using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace CarterAndMVC;
    public class HomeModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app) => app.MapGet("/", () => "Hello from Carter!");
    }


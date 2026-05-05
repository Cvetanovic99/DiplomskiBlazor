using Diplomski.RatingHub.Application;
using Diplomski.RatingHub.Infrastructure;
using Diplomski.RatingHub.Infrastructure.Auth.Models;
using Diplomski.RatingHub.Infrastructure.Persistence.Contexts;
using Diplomski.RatingHub.Web;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Diplomski.RatingHub.Web.Components;
using Diplomski.RatingHub.Web.Components.Account;
using Diplomski.RatingHub.Web.Endpoints;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRadzenComponents();


builder.Services.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddWeb(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();
app.MapFileUploadEndpoints();

app.Run();
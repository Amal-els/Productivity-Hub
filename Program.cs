using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TeamProject.Data;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using TeamProject.Models;
using TeamProject.Services;
using System.Text.Json.Serialization;
using TeamProject.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using TeamProject.Services.Implementations;
using TeamProject.Services.Interfaces;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IEmailNotifService, EmailNotifService>();

builder.Services.AddScoped<IReminderService, ReminderService>();

builder.Services.AddHostedService<ReminderBackgroundService>();
builder.Services
    .AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        );
    });
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<UserEmailServiceFactory>();
builder.Services.AddTransient<IEmailSender, EmailSender>();

// Register your services with DI
builder.Services.AddScoped<ItoDoListService, toDoListService>();
builder.Services.AddScoped<ItoDoTaskService, toDoTaskService>();
builder.Services.AddScoped<INoteService, NoteService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

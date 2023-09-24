using Azure.Storage.Blobs;
using FFMpegCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TimelineSITE.Areas.Albums.Services;
using TimelineSITE.Data;
using TimelineSITE.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var sqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<UsersDbContext>(options =>
    options.UseSqlServer(sqlConnectionString));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(sqlConnectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<UsersDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Manage", policy => policy.RequireRole("Administrator"));
    options.AddPolicy("Users", policy => policy.RequireRole("Administrator", "User"));
});

var azureConnectionString = builder.Configuration.GetConnectionString("AzureBlobStorage");
builder.Services.AddSingleton(provider =>
{
    return new BlobServiceClient(azureConnectionString);
});

builder.Services.AddScoped<AlbumPermissions>();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = long.MaxValue;
});

GlobalFFOptions.Configure(new FFOptions { BinaryFolder = "./ffmpeg", TemporaryFilesFolder = "/tmp" });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

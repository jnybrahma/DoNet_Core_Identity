using Microsoft.AspNetCore.Authorization;
using WebApp_IdentityExample.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth",options =>
{
    options.Cookie.Name = "MyCookieAuth";
    //options.LoginPath = "/Account/Login";
    //options.AccessDeniedPath ="/";
    options.ExpireTimeSpan = TimeSpan.FromSeconds(15);

});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin"));
    options.AddPolicy("MustBelongToHRDepartment", policy => policy.RequireClaim("Department", "HR"));
    options.AddPolicy("HRManagerOnly", policy => policy
            .RequireClaim("Department", "HR")
            .RequireClaim("Manager")
            .Requirements.Add(new HRManagerProbationRequirement(3)));
    ;
});

builder.Services.AddSingleton<IAuthorizationHandler, HRManagerProbingRequirementHandler>();

builder.Services.AddHttpClient("OurWebAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7130/");
});

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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

app.UseSession();

app.MapRazorPages();

app.Run();

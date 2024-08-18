using currentworkingsassyplanner.Data;
using currentworkingsassyplanner.Email;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IEmailSender, EmailSender>();


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<SPContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("SassyPlannerContext") ?? throw new 
InvalidOperationException("Connection string 'SPContext' not found")));

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<SPContext>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(
options =>
{
    options.Stores.MaxLengthForKeys = 128;
}
)

.AddEntityFrameworkStores<SPContext>()
.AddRoles<IdentityRole>()
.AddDefaultUI()
.AddDefaultTokenProviders();
/*builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<SPContext>();*/

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmins", policy => policy.RequireRole("Admin"));
});

//builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
//{
    //options.Conventions.AuthorizeFolder("/Admin", "RequireAdmins");
//});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<SPContext>();
    context.Database.EnsureCreated();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<SPContext>();
    context.Database.Migrate();
    var userMgr = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleMgr = services.GetRequiredService<RoleManager<IdentityRole>>();
    IdentitySeedClass.Initialize(context, userMgr, roleMgr).Wait();
}

app.Run();
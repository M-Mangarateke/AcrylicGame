using AcrylicGame.Data;
using AcrylicGame.Models;
using AcrylicGame.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Password policy
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    options.User.RequireUniqueEmail = true;
});

// Add Identity with Roles
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Dummy email sender
builder.Services.AddTransient<IEmailSender, DummyEmailSender>();

// MVC and Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Seed roles and users
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var db = services.GetRequiredService<ApplicationDbContext>();

    // Seed Branches with real data
    var branches = new List<Branch>
{
    new Branch { Name = "Durban", Address = "84 Innes Road, Windermere", Phone = "0734428860" },
    new Branch { Name = "Rivonia", Address = "8 Rivonia Road, Edenburg", Phone = "0832354258" },
    new Branch { Name = "Sunninghill", Address = "5 Humber Street Sunninghill", Phone = "0835014723" },
    new Branch { Name = "Parkhurst", Address = "38 6th Street, Randburg", Phone = "0681962045" }
};

    foreach (var branch in branches)
    {
        if (!await db.Branches.AnyAsync(b => b.Name == branch.Name))
        {
            db.Branches.Add(branch);
            Console.WriteLine($"Branch seeded: {branch.Name}");
        }
    }
    await db.SaveChangesAsync();


    // Seed Roles
    string[] roles = { "Admin", "Staff", "Client" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
            Console.WriteLine($"Role created: {role}");
        }
    }

    // Seed Admin
    async Task AssignExclusiveRole(string email, string password, string role)
    {
        var admin = await userManager.FindByEmailAsync(email);
        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = email,
                Email = email
            };
            await userManager.CreateAsync(admin, password);
        }

        var currentRoles = await userManager.GetRolesAsync(admin);
        await userManager.RemoveFromRolesAsync(admin, currentRoles);
        await userManager.AddToRoleAsync(admin, role);

        Console.WriteLine($"Admin '{email}' assigned to role '{role}'");
    }

    await AssignExclusiveRole("acrylicadmin@gmail.com", "Admin123!", "Admin");

    // Seed Branch-specific staff with FullName
    var staffAccounts = new List<(string Email, string Password, string BranchName, string FullName)>
{
    ("KimLove.durban@acrylic.com", "Durban123", "Durban", "Kim Love"),
    ("CassieKage.rivonia@acrylic.com", "Rivonia123", "Rivonia", "Cassie Kage"),
    ("MphoSun.sunninghill@acrylic.com", "Sunninghill123", "Sunninghill", "Mpho Sun"),
    ("BongiZuma.parkhurst@acrylic.com", "Parkhurst123", "Parkhurst", "Bongi Zuma")
};

    foreach (var (email, password, branchName, fullName) in staffAccounts)
    {
        var branch = await db.Branches.FirstOrDefaultAsync(b => b.Name == branchName);
        if (branch == null)
        {
            Console.WriteLine($"Skipped seeding for {email}, branch '{branchName}' not found.");
            continue;
        }

        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                BranchId = branch.Id,
                FullName = fullName
            };
            await userManager.CreateAsync(user, password);
        }

        var currentRoles = await userManager.GetRolesAsync(user);
        await userManager.RemoveFromRolesAsync(user, currentRoles);
        await userManager.AddToRoleAsync(user, "Staff");

        Console.WriteLine($"Staff '{email}' ({fullName}) linked to branch '{branchName}'");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

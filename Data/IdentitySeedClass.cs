using Microsoft.AspNetCore.Identity;

namespace currentworkingsassyplanner.Data
{
    public class IdentitySeedClass
    {
        public static async Task Initialize(SPContext context,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();
            string adminRole = "Admin";
            string memberRole = "Menmber";
            string password4all = "P@55word";

            if(await roleManager.FindByNameAsync(adminRole)==null)
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }
            if(await roleManager.FindByNameAsync(memberRole)==null)
            {
                await roleManager.CreateAsync(new IdentityRole(memberRole));
            }
            if(await userManager.FindByNameAsync("admin@sassyplanners.com")==null)
            {
                var user = new IdentityUser
                {
                    UserName = "admin@sassyplanners.com",
                    Email = "admin@sassyplanners.com",
                    PhoneNumber = "01234 56789"
                };
                var result = await userManager.CreateAsync(user);
                if(result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password4all);
                    await userManager.AddToRoleAsync(user, adminRole);
                }
            }
            if (await userManager.FindByNameAsync("member@ucm.ac.uk") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "member@ucm.ac.uk",
                    Email = "member@ucm.ac.uk",
                    PhoneNumber = "01234 56789"
                };
                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password4all);
                    await userManager.AddToRoleAsync(user, memberRole);
                }
            }
        }
    }
}

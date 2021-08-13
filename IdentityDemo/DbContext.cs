using System;
using IdentityDemo.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityDemo
{
   public class DbContext:IdentityDbContext<IdentityUser, IdentityRole, string>
  //  public class DbContext:IdentityDbContext<IdentityUser>
    {
        public DbContext(DbContextOptions<DbContext> options)
    : base(options)
        {
        }
        public DbSet<ApplicationRegisteration> RegisteredApps { get; set; }
    }
}

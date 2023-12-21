using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using RobertTest.Models;
using System;

namespace RobertTest.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
           
        }
        public DbSet<Customer> customers  { get; set; }

}
}

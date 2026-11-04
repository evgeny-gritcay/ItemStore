using ItemStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ItemStore.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Item> Items => Set<Item>();
}

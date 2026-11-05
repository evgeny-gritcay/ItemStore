using ItemStore.Api.Data;
using ItemStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ItemStore.Tests;

public class ItemsRepositoryTests
{
    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Create_And_Get_Item_Works()
    {
        var db = CreateDb();
        var item = new Item { Name = "Клавиатура", Price = 3500m, Quantity = 5 };

        db.Items.Add(item);
        await db.SaveChangesAsync();

        var saved = await db.Items.FindAsync(item.Id);
        Assert.NotNull(saved);
        Assert.Equal("Клавиатура", saved.Name);
    }

    [Fact]
    public async Task Delete_Removes_Item()
    {
        var db = CreateDb();
        var item = new Item { Name = "Мышь", Price = 1500m, Quantity = 2 };
        db.Items.Add(item);
        await db.SaveChangesAsync();

        db.Items.Remove(item);
        await db.SaveChangesAsync();

        Assert.Empty(await db.Items.ToListAsync());
    }

    [Fact]
    public async Task Update_Changes_Price()
    {
        await using var db = CreateDb();
        var item = new Item { Name = "Наушники", Price = 5000m, Quantity = 3 };
        db.Items.Add(item);
        await db.SaveChangesAsync();

        // Act: меняем цену у отслеживаемой сущности
        item.Price = 6500m;
        await db.SaveChangesAsync();

        // Assert: в базе лежит уже новая цена
        var updated = await db.Items.FindAsync(item.Id);
        Assert.NotNull(updated);
        Assert.Equal(6500m, updated.Price);
    }

    [Fact]
    public async Task Find_NonExisting_Item_Returns_Null()
    {
        await using var db = CreateDb();

        var found = await db.Items.FindAsync(999);

        Assert.Null(found);
    }

    [Fact]
    public async Task GetItems_Returns_All_Added()
    {
        await using var db = CreateDb();
        db.Items.AddRange(
            new Item { Name = "Клавиатура", Price = 3500m, Quantity = 5 },
            new Item { Name = "Мышь", Price = 1500m, Quantity = 2 },
            new Item { Name = "Монитор", Price = 18000m, Quantity = 1 });
        await db.SaveChangesAsync();

        var items = await db.Items.ToListAsync();

        Assert.Equal(3, items.Count);
    }
}
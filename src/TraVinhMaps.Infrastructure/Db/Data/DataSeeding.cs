// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using TraVinhMaps.Application.Common.Extensions;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Infrastructure.Db.Data;

public static class DataSeeding
{

    public static async Task SeedData(IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<IHost>>();
        var mongoDbSettings = serviceProvider.GetRequiredService<IOptions<MongoDbSetting>>().Value;

        try
        {
            var client = new MongoClient(mongoDbSettings.ConnectionString);
            var database = client.GetDatabase(mongoDbSettings.DatabaseName);

            await SeedOcopProducts(database, logger);
            await SeedTagSample(database, logger);
            var markers = await seedMarker(database, logger);
            if (markers.Any())
            {
                var markerId = markers.First().Id; // lấy Id đầu tiên làm ví dụ
                await seedTypeDestination(database, logger, markerId); // truyền Id vào seedTypeDestination
            }
            await SeedRole(database, logger);
            await SeedUserAccount(database, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }

    private static async Task SeedOcopProducts(IMongoDatabase database, ILogger<IHost> logger)
    {
        var collection = database.GetCollection<OcopProduct>("OcopProduct");
        var count = await collection.CountDocumentsAsync(FilterDefinition<OcopProduct>.Empty);
        if (count > 0)
        {
            logger.LogInformation("OcopProducts collection already contains data. Skipping seeding.");
            return;
        }

        logger.LogInformation("Seeding OcopProducts collection...");

        // Create some sample OcopTypes to reference
        var ocopTypeId = await SeedOcopType(database);
        var companyId = await SeedCompany(database);
        var tagId = await SeedTag(database);
        var sellingLinkId = await SeedSellingLink(database);

        var product = new List<OcopProduct>
        {
          new OcopProduct
{
    Id = ObjectId.GenerateNewId().ToString(),
    ProductName = "Tra Vinh Coconut Candy",
    ProductDescription = "Traditional coconut candy with unique Tra Vinh flavor.",
    ProductImage = new List<string>
    {
          "https://example.com/coconut-candy.jpg",
                "https://example.com/candy-pack.jpg"
    },
    ProductPrice = "35000",
    OcopTypeId = ocopTypeId,
    CompanyId = companyId,
    TagId = tagId,
    Status = true,
    CreatedAt = DateTime.UtcNow,
    Sellocations = new List<SellLocation>
    {
        new SellLocation
        {
            LocationName = "Cua hang dac san so 3",
            LocationAddress = "45 Le Loi, Phuong 1, TP Tra Vinh",
            Location = new Location
            {
                Type = "point",
                Coordinates = new List<double> { 106.350, 9.950 }
            }
        },
        new SellLocation
        {
            LocationName = "Cho Tra Vinh",
            LocationAddress = "78 Nguyen Thi Minh Khai, Phuong 2",
            Location = new Location
            {
                Type = "point",
                Coordinates = new List<double> { 106.342, 9.948 }
            }
        },
    },
    OcopPoint = 5,
    OcopYearRelease = 2022,
    SellingLinkId = sellingLinkId,
},

new OcopProduct
{
    Id = ObjectId.GenerateNewId().ToString(),
    ProductName = "Tra Vinh Palm Sugar",
    ProductDescription = "Organic palm sugar sourced from Tra Vinh's palm trees. Known for its rich caramel flavor and natural sweetness.",
     ProductImage = new List<string> {
                "https://example.com/coconut-candy.jpg",
                "https://example.com/candy-pack.jpg"
            },
    ProductPrice = "55000",
    OcopTypeId = ocopTypeId,
    CompanyId = companyId,
    TagId = tagId,
    Status = true,
    CreatedAt = DateTime.UtcNow,
    Sellocations = new List<SellLocation>
    {
        new SellLocation
        {
            LocationName = "Cua hang duong thot not",
            LocationAddress = "123 Tran Phu, Phuong 3, TP Tra Vinh",
            Location = new Location
            {
                Type = "point",
                Coordinates = new List<double> { 106.355, 9.952 }
            }
        },
        new SellLocation
        {
            LocationName = "Sieu thi Co.op Mart Tra Vinh",
            LocationAddress = "56 Nguyen Dang, TP Tra Vinh",
            Location = new Location
            {
                Type = "point",
                Coordinates = new List<double> { 106.349, 9.947 }
            }
        },
        new SellLocation
        {
            LocationName = "Cua hang dac san Tra Vinh",
            LocationAddress = "22 Pham Thai Buong, Phuong 4",
            Location = new Location
            {
                Type = "point",
                Coordinates = new List<double> { 106.340, 9.955 }
            }
        },
    },
    OcopPoint = 3,
    OcopYearRelease = 2023,
    SellingLinkId = sellingLinkId,
},

new OcopProduct
{
    Id = ObjectId.GenerateNewId().ToString(),
    ProductName = "Tra Vinh Dried Fish",
    ProductDescription = "Premium quality dried fish produced with traditional methods from Tra Vinh's coastal communities.",
 ProductImage = new List<string> {
                "https://example.com/coconut-candy.jpg",
                "https://example.com/candy-pack.jpg"
            },
    ProductPrice = "85000",
    OcopTypeId = ocopTypeId,
    CompanyId = companyId,
    TagId = tagId,
    Status = true,
    CreatedAt = DateTime.UtcNow,
    Sellocations = new List<SellLocation>
    {
        new SellLocation
        {
            LocationName = "Cua hang hai san Tra Vinh",
            LocationAddress = "67 Le Duan, TP Tra Vinh",
            Location = new Location
            {
                Type = "point",
                Coordinates = new List<double> { 106.347, 9.949 }
            }
        },
        new SellLocation
        {
            LocationName = "Cho hai san Tra Vinh",
            LocationAddress = "34 Dinh Tien Hoang, Phuong 2",
            Location = new Location
            {
                Type = "point",
                Coordinates = new List<double> { 106.338, 9.944 }
            }
        },
    },
    OcopPoint = 4,
    OcopYearRelease = 2021,
    SellingLinkId = sellingLinkId,
}

            };

        await collection.InsertManyAsync(product);
    }

    private static async Task SeedTagSample(IMongoDatabase database, ILogger<IHost> logger)
    {
        var collection = database.GetCollection<Tags>("Tags");
        var count = await collection.CountDocumentsAsync(FilterDefinition<Tags>.Empty);
        if (count > 0)
        {
            logger.LogInformation("Tags collection already contains data. Skipping seeding.");
            return;
        }

        logger.LogInformation("Seeding Tag collection...");

        var Tags = new List<Tags>()
        {
            new Tags
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = "Introduce",
                image = "https://res.cloudinary.com/ddaj2hsk5/image/upload/v1747206098/coconut-tree_htkwqe.png",
                CreatedAt = DateTime.UtcNow,
            },
            new Tags
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = "Ocop",
                image = "https://res.cloudinary.com/ddaj2hsk5/image/upload/v1747206105/plantingtree_oyhi7m.png",
                CreatedAt = DateTime.UtcNow,
            },
            new Tags
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = "Tip Travel",
                image = "https://res.cloudinary.com/ddaj2hsk5/image/upload/v1747206098/lightbulb_mdvz4n.png",
                CreatedAt = DateTime.UtcNow,
            },
            new Tags
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = "Destination",
                image = "https://res.cloudinary.com/ddaj2hsk5/image/upload/v1747206098/destination_n8kp0f.png",
                CreatedAt = DateTime.UtcNow,
            },
            new Tags
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = "Local specialty",
                image = "https://res.cloudinary.com/ddaj2hsk5/image/upload/v1747206098/food_v6izlc.png",
                CreatedAt = DateTime.UtcNow,
            },
            new Tags
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = "Stay",
                image = "https://res.cloudinary.com/ddaj2hsk5/image/upload/v1747206098/hotel_rzhyqr.png",
                CreatedAt = DateTime.UtcNow,
            },
             new Tags
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = "Festivals",
                image = "https://res.cloudinary.com/ddaj2hsk5/image/upload/v1747206098/dragon-boat_vwsqix.png",
                CreatedAt = DateTime.UtcNow,
            },
              new Tags
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = "Utilities",
                image = "https://res.cloudinary.com/ddaj2hsk5/image/upload/v1747206099/resource-allocation_gpmfgq.png",
                CreatedAt = DateTime.UtcNow,
            },
        };
        await collection.InsertManyAsync(Tags);
    }

    private static async Task<List<Marker>> seedMarker(IMongoDatabase database, ILogger<IHost> logger)
    {
        var collection = database.GetCollection<Marker>("Marker");
        var count = await collection.CountDocumentsAsync(FilterDefinition<Marker>.Empty);
        if (count > 0)
        {
            logger.LogInformation("Tags collection already contains data. Skipping seeding.");
            return await collection.Find(FilterDefinition<Marker>.Empty).ToListAsync();
        }
        logger.LogInformation("Seeding Tag collection...");

        var markers = new List<Marker>()
        {
            new Marker
            {
                Id= ObjectId.GenerateNewId().ToString(),
                Name = "Buiding Destination",
                Image = "https://res.cloudinary.com/ddaj2hsk5/image/upload/v1747207304/building_aczwaz.png",
                Status = true,
                CreatedAt = DateTime.UtcNow,
            },
            new Marker
            {
                Id= ObjectId.GenerateNewId().ToString(),
                Name = "Natural Destination",
                Image = "https://res.cloudinary.com/ddaj2hsk5/image/upload/v1747207304/park_fa2ttf.png",
                Status = true,
                CreatedAt = DateTime.UtcNow,
            }
        };
        await collection.InsertManyAsync(markers);
        return markers;
    }

    private static async Task seedTypeDestination(IMongoDatabase database, ILogger<IHost> logger, string markerId)
    {
        var collection = database.GetCollection<DestinationType>("DestinationType");
        var count = await collection.CountDocumentsAsync(FilterDefinition<DestinationType>.Empty);
        if (count > 0)
        {
            logger.LogInformation("Tags collection already contains data. Skipping seeding.");
            return;
        }
        logger.LogInformation("Seeding Tag collection...");
        var destinationTypeList = new List<DestinationType>()
        {
            new DestinationType
            {
                Id = ObjectId.GenerateNewId().ToString(),
                CreatedAt= DateTime.UtcNow,
                MarkerId= markerId,
                Name= "Religious Buildings",
                Status= true,
                UpdateAt = DateTime.UtcNow,
            },
        };
        await collection.InsertManyAsync(destinationTypeList);
    }
    // Role seeding method
    private static async Task SeedRole(IMongoDatabase database, ILogger<IHost> logger)
    {
        var collection = database.GetCollection<Role>("Role");
        var count = await collection.CountDocumentsAsync(FilterDefinition<Role>.Empty);
        if (count > 0)
        {
            logger.LogInformation("Role collection already contains data. Skipping seeding.");
            return;
        }

        logger.LogInformation("Seeding Role collection...");

        var role = new List<Role>()
        {
            new Role
            {
                Id = ObjectId.GenerateNewId().ToString(),
                RoleName = "super-admin",
                RoleStatus = true,
                CreatedAt = DateTime.Now
            },
            new Role
            {
                Id = ObjectId.GenerateNewId().ToString(),
                RoleName = "admin",
                RoleStatus = true,
                CreatedAt = DateTime.Now
            },
            new Role
            {
                Id = ObjectId.GenerateNewId().ToString(),
                RoleName = "user",
                RoleStatus = true,
                CreatedAt = DateTime.Now
            }
        };

        await collection.InsertManyAsync(role);
    }

    // seeding method account user
    private static async Task SeedUserAccount(IMongoDatabase database, ILogger<IHost> logger)
    {
        var collection = database.GetCollection<User>("User");
        var count = await collection.CountDocumentsAsync(FilterDefinition<User>.Empty);
        if (count > 0)
        {
            logger.LogInformation("Role collection already contains data. Skipping seeding.");
            return;
        }

        logger.LogInformation("Seeding Role collection...");

        var role = await database.GetCollection<Role>("Role")
            .Find(r => r.RoleName == "admin")
            .FirstOrDefaultAsync();

        if (role == null)
        {
            logger.LogError("Role 'admin' not found. Cannot seed user accounts.");
            return;
        }
        var userList = new List<User>()
        {
            new User
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Email = "joonnguyen2712@gmail.com",
                Password = HashingExtension.HashWithSHA256("27122003"),
                CreatedAt = DateTime.UtcNow,
                RoleId = role.Id, // Replace with actual role ID
                Status = true,
                IsForbidden = false,
            },
            new User
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Email = "haoncce171957@fpt.edu.vn",
                Password = HashingExtension.HashWithSHA256("27122003"),
                CreatedAt = DateTime.UtcNow,
                RoleId = role.Id, // Replace with actual role ID
                Status = true,
                IsForbidden = false,
            },
             new User
             {
                 Id = ObjectId.GenerateNewId().ToString(),
                 PhoneNumber = "0869251053",
                 Password = HashingExtension.HashWithSHA256("27122003"),
                 RoleId    = role.Id, // Replace with actual role ID
                 CreatedAt = DateTime.UtcNow,
                 Status = true,
                 IsForbidden = false,
             }
        };

        await collection.InsertManyAsync(userList);
    }

    private static async Task<string> SeedSellingLink(IMongoDatabase database)
    {
        return ObjectId.GenerateNewId().ToString();
    }

    private static async Task<string> SeedTag(IMongoDatabase database)
    {
        return ObjectId.GenerateNewId().ToString();
    }

    private static async Task<string> SeedCompany(IMongoDatabase database)
    {
        return ObjectId.GenerateNewId().ToString();
    }

    private static async Task<string> SeedOcopType(IMongoDatabase database)
    {
        return ObjectId.GenerateNewId().ToString();
    }


}


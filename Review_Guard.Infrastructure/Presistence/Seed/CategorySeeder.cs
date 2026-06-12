#region User Seeder

using Review_Guard.Domain.Enums;
using Review_Guard.Domain.ValueObject;

public static class UserSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("UserSeeder");

        try
        {
            if (await context.Users.AnyAsync())
            {
                logger.LogInformation("Users already seeded");
                return;
            }

            var passwordHasher = new PasswordHasher();

            var usersData = new[]
            {
                ("Ahmed Hassan", "ahmed.hassan@gmail.com"),
                ("Mohamed Adel", "m.adel@gmail.com"),
                ("Omar Khaled", "omar.k@gmail.com"),
                ("Mahmoud Tarek", "mahmoud.tarek@gmail.com"),
                ("Mostafa Wael", "mostafa.wael@gmail.com"),
                ("Yasmine Ali", "yasmine.ali@gmail.com"),
                ("Mariam Nabil", "mariam.nabil@gmail.com"),
                ("Salma Hossam", "salma.hossam@gmail.com"),
                ("Nada Ashraf", "nada.ashraf@gmail.com"),
                ("Sara Mohamed", "sara.m@gmail.com"),
                ("Karim Samy", "karim.samy@gmail.com"),
                ("Ali Reda", "ali.reda@gmail.com"),
                ("Hana Emad", "hana.emad@gmail.com"),
                ("Nour Essam", "nour.essam@gmail.com"),
                ("Ibrahim Yasser", "ibrahim.yasser@gmail.com"),
                ("Mina Nader", "mina.nader@gmail.com"),
                ("Fatma Samir", "fatma.samir@gmail.com"),
                ("Amr Hesham", "amr.hesham@gmail.com"),
                ("Rana Tarek", "rana.tarek@gmail.com"),
                ("Youssef Emad", "youssef.emad@gmail.com"),

                ("Khaled Samir", "khaled.samir@gmail.com"),
                ("Eslam Wael", "eslam.wael@gmail.com"),
                ("Tamer Ashraf", "tamer.ashraf@gmail.com"),
                ("Shahd Mohamed", "shahd.m@gmail.com"),
                ("Malak Emad", "malak.emad@gmail.com"),
                ("Menna Allah", "menna.allah@gmail.com"),
                ("Ayman Nabil", "ayman.nabil@gmail.com"),
                ("Sherif Adel", "sherif.adel@gmail.com"),
                ("Hossam Mostafa", "hossam.m@gmail.com"),
                ("Reem Khaled", "reem.khaled@gmail.com"),
                ("Farah Ali", "farah.ali@gmail.com"),
                ("Habiba Yasser", "habiba.y@gmail.com"),
                ("Ziad Samy", "ziad.samy@gmail.com"),
                ("Bassant Tarek", "bassant.t@gmail.com"),
                ("Marwan Adel", "marwan.adel@gmail.com"),
                ("Doaa Emad", "doaa.emad@gmail.com"),
                ("Nadine Ashraf", "nadine.ashraf@gmail.com"),
                ("Islam Mohamed", "islam.m@gmail.com"),
                ("Alaa Nader", "alaa.nader@gmail.com"),
                ("Hager Samir", "hager.samir@gmail.com")
            };

            var users = new List<User>();

            foreach (var item in usersData)
            {
                var user = User.Create(
                    item.Item1,
                    item.Item2,
                    passwordHasher.HashPassword("UserError@123#"),
                    Roles.User);

                user.VerifyEmail("seed-admin");

                users.Add(user);
            }

            users[0].IncreaseTrustScore(15);
            users[1].IncreaseTrustScore(10);
            users[2].IncreaseTrustScore(5);

            users[3].DecreaseTrustScore(20);
            users[4].DecreaseTrustScore(35);

            users[5].IncreaseTrustScore(8);
            users[6].IncreaseTrustScore(12);

            await context.Users.AddRangeAsync(users);

            await context.SaveChangesAsync();

            logger.LogInformation("Users seeded successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while seeding users");
            throw;
        }
    }
}

#endregion

#region Business Category Seeder

public static class BusinessCategorySeeder
{
    /// <summary>
    /// Seeds all business categories with real-world Egyptian market coverage.
    /// Idempotent — skips if any categories already exist.
    /// </summary>
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("BusinessCategorySeeder");

        try
        {
            if (await context.BusinessCategories.AnyAsync())
            {
                logger.LogInformation("Business categories already seeded — skipping.");
                return;
            }

            // ── Category definitions ─────────────────────────────────────────
            // Each tuple: (Name)
            // All categories are auto-approved as part of initial seed data.
            var categoryNames = new[]
            {
                // ── Food & Beverage ──────────────────────────────────────────
                "Restaurant",
                "Cafe & Coffee Shop",
                "Bakery & Pastry",
                "Fast Food",
                "Juice Bar & Smoothies",
                "Ice Cream & Desserts",
                "Food Truck",

                // ── Hospitality & Travel ─────────────────────────────────────
                "Hotel",
                "Resort & Spa",
                "Hostel & Budget Stay",
                "Chalet & Villa Rental",
                "Cruise & Nile Boat",

                // ── Health & Wellness ────────────────────────────────────────
                "Hospital & Clinic",
                "Pharmacy",
                "Dental Clinic",
                "Optical Center",
                "Gym & Fitness Center",
                "Spa & Beauty Center",
                "Physiotherapy Center",

                // ── Beauty & Personal Care ────────────────────────────────────
                "Hair Salon",
                "Barbershop",
                "Nail Salon",
                "Tattoo & Piercing Studio",

                // ── Retail & Shopping ─────────────────────────────────────────
                "Clothing & Fashion",
                "Electronics & Gadgets",
                "Supermarket & Grocery",
                "Bookstore",
                "Sports & Outdoor Gear",
                "Furniture & Home Decor",
                "Jewellery & Accessories",
                "Toys & Games",
                "Auto Parts & Accessories",

                // ── Education & Training ──────────────────────────────────────
                "School & Institute",
                "Language Center",
                "Tutoring & Private Lessons",
                "Driving School",
                "Online Learning Center",

                // ── Professional Services ─────────────────────────────────────
                "Law Firm & Legal Services",
                "Accounting & Tax Services",
                "Real Estate Agency",
                "Insurance Company",
                "Translation Services",
                "Printing & Advertising Agency",

                // ── Technology & Digital ──────────────────────────────────────
                "IT & Software Company",
                "Mobile Repair Shop",
                "Computer & Laptop Repair",
                "Photography Studio",
                "Gaming Center",

                // ── Automotive ────────────────────────────────────────────────
                "Car Showroom",
                "Car Wash & Detailing",
                "Auto Repair & Maintenance",
                "Tyre Shop",

                // ── Entertainment & Leisure ───────────────────────────────────
                "Cinema & Theater",
                "Amusement Park",
                "Sports Club",
                "Bowling & Entertainment",
                "Billiards & Social Club",
                "Escape Room",

                // ── Kids & Family ─────────────────────────────────────────────
                "Nursery & Kindergarten",
                "Kids Play Area",
                "Pediatric Clinic",

                // ── Home & Maintenance ────────────────────────────────────────
                "Cleaning & Housekeeping",
                "Plumbing & Electrical",
                "Interior Design",
                "Moving & Relocation",

                // ── Pets ──────────────────────────────────────────────────────
                "Veterinary Clinic",
                "Pet Shop & Grooming",

                // ── Religious & Community ─────────────────────────────────────
                "Mosque",
                "Church",

                // ── Other ─────────────────────────────────────────────────────
                "Government & Public Service",
                "Charity & NGO",
                "Other"
            };

            var categories = categoryNames
                .Select(name =>
                {
                    var cat = BusinessCategory.Create(name);
                    cat.Approve();
                    return cat;
                })
                .ToList();

            await context.BusinessCategories.AddRangeAsync(categories);
            await context.SaveChangesAsync();

            logger.LogInformation(
                "Business categories seeded successfully — {Count} categories added.",
                categories.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while seeding business categories.");
            throw;
        }
    }
}

#endregion

#region Business Seeder

public static class BusinessSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("BusinessSeeder");

        try
        {
            if (await context.Businesses.AnyAsync())
                return;

            var admins = await context.Admins.ToListAsync();
            var users = await context.Users.ToListAsync();
            var categories = await context.BusinessCategories.ToListAsync();

            var restaurantCategory = categories.First(x => x.Name == "Restaurant");
            var hotelCategory = categories.First(x => x.Name == "Hotel");

            for (int i = 0; i < 18; i++)
                users[i].ChangeRole(Roles.BusinessOwner);

            // ── Business definitions (index, Name, Description, Category, images[]) ──
            // Each business has 3 unique Unsplash images (primary + 2 gallery)
            var definitions = new[]
            {
                // 0 ─ Felfela Restaurant
                (users[0],  "Felfela Restaurant",    "Traditional Egyptian food in the heart of Cairo since 1959",      restaurantCategory,
                 new[] { "https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=800",  // restaurant interior warm
                         "https://images.unsplash.com/photo-1565299507177-b0ac66763828?w=800",  // Egyptian food plate
                         "https://images.unsplash.com/photo-1466978913421-dad2ebd01d17?w=800" }), // table setting

                // 1 ─ Abou El Sid
                (users[1],  "Abou El Sid",           "Authentic Egyptian cuisine with vintage Cairene atmosphere",       restaurantCategory,
                 new[] { "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=800",  // fine dining room
                         "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=800",  // colorful food
                         "https://images.unsplash.com/photo-1547592180-85f173990554?w=800" }), // oriental mezze

                // 2 ─ Four Seasons Cairo
                (users[2],  "Four Seasons Cairo",    "Luxury 5-star hotel overlooking the Nile",                        hotelCategory,
                 new[] { "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800",  // luxury hotel pool
                         "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?w=800",  // hotel room elegant
                         "https://images.unsplash.com/photo-1445019980597-93fa8acb246c?w=800" }), // hotel lobby grand

                // 3 ─ Steigenberger Hotel
                (users[3],  "Steigenberger Hotel",   "5-star landmark hotel in Downtown Cairo",                         hotelCategory,
                 new[] { "https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=800",  // hotel exterior night
                         "https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=800",  // hotel room luxury
                         "https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=800" }), // hotel pool blue

                // 4 ─ Sequoia
                (users[4],  "Sequoia",               "Fine dining on the Nile with international flavours",             restaurantCategory,
                 new[] { "https://images.unsplash.com/photo-1537047902294-62a40c20a6ae?w=800",  // upscale outdoor restaurant
                         "https://images.unsplash.com/photo-1476224203421-9ac39bcb3b21?w=800",  // gourmet dish
                         "https://images.unsplash.com/photo-1600891964092-4316c288032e?w=800" }), // sunset dining

                // 5 ─ Marriott Cairo
                (users[5],  "Marriott Cairo",        "Luxury hotel stay in a restored 19th-century palace",             hotelCategory,
                 new[] { "https://images.unsplash.com/photo-1529290130-4ca3753253ae?w=800",  // palace hotel exterior
                         "https://images.unsplash.com/photo-1610641818989-c2051b5e2cfd?w=800",  // hotel suite
                         "https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?w=800" }), // hotel swimming pool

                // 6 ─ Kazouza
                (users[6],  "Kazouza",               "Fresh seafood and Nile fish in a relaxed setting",                restaurantCategory,
                 new[] { "https://images.unsplash.com/photo-1504754524776-8f4f37790ca0?w=800",  // seafood platter
                         "https://images.unsplash.com/photo-1559847844-5315695dadae?w=800",  // fish grill
                         "https://images.unsplash.com/photo-1519708227418-c8fd9a32b7a2?w=800" }), // casual seafood restaurant

                // 7 ─ Hilton Alexandria
                (users[7],  "Hilton Alexandria",     "Beachfront luxury hotel on the Mediterranean coast",              hotelCategory,
                 new[] { "https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800",  // beachfront hotel
                         "https://images.unsplash.com/photo-1584132967334-10e028bd69f7?w=800",  // hotel sea view room
                         "https://images.unsplash.com/photo-1461175827210-5ceac3e39dd2?w=800" }), // hotel beach pool

                // 8 ─ Andrea Mariouteya
                (users[8],  "Andrea Mariouteya",     "Iconic open-air grill restaurant on the Mariouteya canal",        restaurantCategory,
                 new[] { "https://images.unsplash.com/photo-1544025162-d76694265947?w=800",  // open BBQ grill
                         "https://images.unsplash.com/photo-1529193591184-b1d58069ecdd?w=800",  // outdoor dining garden
                         "https://images.unsplash.com/photo-1555939594-58d7cb561ad1?w=800" }), // grilled chicken

                // 9 ─ Zooba
                (users[9],  "Zooba",                 "Modern Egyptian street food elevated to gourmet level",           restaurantCategory,
                 new[] { "https://images.unsplash.com/photo-1561758033-d89a9ad46330?w=800",  // street food colourful
                         "https://images.unsplash.com/photo-1512058564366-18510be2db19?w=800",  // falafel wraps
                         "https://images.unsplash.com/photo-1548943487-a2e4e43b4853?w=800" }), // modern casual dining

                // 10 ─ Crave Restaurant
                (users[10], "Crave Restaurant",      "International cuisine, gourmet burgers and creative cocktails",   restaurantCategory,
                 new[] { "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=800",  // gourmet burger
                         "https://images.unsplash.com/photo-1482049016688-2d3e1b311543?w=800",  // food spread table
                         "https://images.unsplash.com/photo-1550547660-d9450f859349?w=800" }), // modern restaurant bar

                // 11 ─ Mori Sushi
                (users[11], "Mori Sushi",            "Premium Japanese sushi and omakase dining",                       restaurantCategory,
                 new[] { "https://images.unsplash.com/photo-1553621042-f6e147245754?w=800",  // sushi platter
                         "https://images.unsplash.com/photo-1617196034183-421b4040ed20?w=800",  // sushi chef
                         "https://images.unsplash.com/photo-1562802378-063ec186a863?w=800" }), // Japanese restaurant interior

                // 12 ─ Cilantro Cafe
                (users[12], "Cilantro Cafe",         "Egypt's favourite specialty coffee chain since 2003",             restaurantCategory,
                 new[] { "https://images.unsplash.com/photo-1445116572660-236099ec97a0?w=800",  // coffee shop cosy
                         "https://images.unsplash.com/photo-1509042239860-f550ce710b93?w=800",  // latte art
                         "https://images.unsplash.com/photo-1525610553991-2bede1a236e2?w=800" }), // cafe pastry display

                // 13 ─ Kempinski Nile Hotel
                (users[13], "Kempinski Nile Hotel",  "Iconic luxury hotel with panoramic Nile views in Garden City",    hotelCategory,
                 new[] { "https://images.unsplash.com/photo-1498503182468-3b51cbb6cb24?w=800",  // luxury hotel nile view
                         "https://images.unsplash.com/photo-1564501049412-61c2a3083791?w=800",  // executive room
                         "https://images.unsplash.com/photo-1506059612708-99d6c258160e?w=800" }), // hotel rooftop pool

                // 14 ─ Tolip Hotel
                (users[14], "Tolip Hotel",           "Premium business hotel in the heart of Cairo's business district",hotelCategory,
                 new[] { "https://images.unsplash.com/photo-1600011689032-8b628b8a8747?w=800",  // modern hotel atrium
                         "https://images.unsplash.com/photo-1578683010236-d716f9a3f461?w=800",  // contemporary suite
                         "https://images.unsplash.com/photo-1590490360182-c33d57733427?w=800" }), // hotel bar lounge

                // 15 ─ Jaz Sharm Dreams
                (users[15], "Jaz Sharm Dreams",      "All-inclusive beachfront resort and spa in Sharm El Sheikh",      hotelCategory,
                 new[] { "https://images.unsplash.com/photo-1571003123894-1f0594d2b5d9?w=800",  // resort infinity pool
                         "https://images.unsplash.com/photo-1540541338287-41700207dee6?w=800",  // beach resort day
                         "https://images.unsplash.com/photo-1531088009183-5ff5b7c95f91?w=800" }), // spa treatment

                // 16 ─ Sheraton Cairo
                (users[16], "Sheraton Cairo",        "Classic 5-star business hotel with iconic pyramid views",         hotelCategory,
                 new[] { "https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=800",  // hotel outdoor pool pyramids feel
                         "https://images.unsplash.com/photo-1618773928121-c32242e63f39?w=800",  // classic hotel room
                         "https://images.unsplash.com/photo-1559508551-44bff1de756b?w=800" }), // hotel conference

                // 17 ─ Sunrise Aqua Joy
                (users[17], "Sunrise Aqua Joy",      "Award-winning beachfront resort in Hurghada on the Red Sea",      hotelCategory,
                 new[] { "https://images.unsplash.com/photo-1573843981267-be1999ff37cd?w=800",  // red sea resort
                         "https://images.unsplash.com/photo-1562778612-e1e0cda9915c?w=800",  // beach bungalow
                         "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?w=800" }), // resort pool aerial
            };

            var businesses = new List<Business>();

            foreach (var (user, name, description, category, images) in definitions)
            {
                var business = Business.Create(user.Id, name, description, category.Id);

                business.Approve(admins[0].Id);

                // First image = primary cover, rest = gallery
                business.AddImage(images[0], 1, isPrimary: true);
                business.AddImage(images[1], 2);
                business.AddImage(images[2], 3);

                businesses.Add(business);
            }

            await context.Businesses.AddRangeAsync(businesses);
            await context.SaveChangesAsync();

            logger.LogInformation("Businesses seeded successfully — {Count} businesses added.", businesses.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while seeding businesses.");
            throw;
        }
    }
}

#endregion

#region Branch Seeder

public static class BranchSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("BranchSeeder");

        try
        {
            if (await context.Branches.AnyAsync())
                return;

            var businesses = await context.Businesses.ToListAsync();
            var users = await context.Users.ToListAsync();

            for (int i = 18; i < 35; i++)
                users[i].ChangeRole(Roles.BranchManager);

            // ── Branch definitions ────────────────────────────────────────────────
            // (BusinessIndex, Address, City, Country, Phone, ManagerIndex, images[])
            var definitions = new[]
            {
                // businesses[0] Felfela Restaurant
                (0,  "12 Talaat Harb St",         "Cairo",          "Egypt", "01011111111", 18,
                 new[] { "https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=800",
                         "https://images.unsplash.com/photo-1466978913421-dad2ebd01d17?w=800",
                         "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=800" }),

                (0,  "5 El Ahram St",             "Cairo",          "Egypt", "01011111112", 19,
                 new[] { "https://images.unsplash.com/photo-1565299507177-b0ac66763828?w=800",
                         "https://images.unsplash.com/photo-1547592180-85f173990554?w=800",
                         "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=800" }),

                // businesses[1] Abou El Sid
                (1,  "El Corniche, Maadi",         "Cairo",          "Egypt", "01011111113", 20,
                 new[] { "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=800",
                         "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=800",
                         "https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=800" }),

                (1,  "Syria St, Heliopolis",       "Cairo",          "Egypt", "01011111114", 21,
                 new[] { "https://images.unsplash.com/photo-1547592180-85f173990554?w=800",
                         "https://images.unsplash.com/photo-1466978913421-dad2ebd01d17?w=800",
                         "https://images.unsplash.com/photo-1504754524776-8f4f37790ca0?w=800" }),

                // businesses[2] Four Seasons Cairo
                (2,  "1 Corniche El Nil, Maadi",   "Cairo",          "Egypt", "01011111115", 22,
                 new[] { "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800",
                         "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?w=800",
                         "https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=800" }),

                (2,  "35 Garden City, Garden City","Cairo",          "Egypt", "01011111116", 23,
                 new[] { "https://images.unsplash.com/photo-1445019980597-93fa8acb246c?w=800",
                         "https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=800",
                         "https://images.unsplash.com/photo-1531088009183-5ff5b7c95f91?w=800" }),

                // businesses[3] Steigenberger
                (3,  "11 Ahmed Ragheb St, New Cairo","Cairo",        "Egypt", "01011111117", 24,
                 new[] { "https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=800",
                         "https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=800",
                         "https://images.unsplash.com/photo-1590490360182-c33d57733427?w=800" }),

                // businesses[4] Sequoia
                (4,  "Agouza, Abu El Feda St",    "Alexandria",     "Egypt", "01011111118", 25,
                 new[] { "https://images.unsplash.com/photo-1537047902294-62a40c20a6ae?w=800",
                         "https://images.unsplash.com/photo-1476224203421-9ac39bcb3b21?w=800",
                         "https://images.unsplash.com/photo-1600891964092-4316c288032e?w=800" }),

                (4,  "Smouha District",            "Alexandria",     "Egypt", "01011111119", 26,
                 new[] { "https://images.unsplash.com/photo-1476224203421-9ac39bcb3b21?w=800",
                         "https://images.unsplash.com/photo-1600891964092-4316c288032e?w=800",
                         "https://images.unsplash.com/photo-1537047902294-62a40c20a6ae?w=800" }),

                // businesses[5] Marriott Cairo
                (5,  "Hurghada Marina",            "Red Sea",        "Egypt", "01011111120", 27,
                 new[] { "https://images.unsplash.com/photo-1529290130-4ca3753253ae?w=800",
                         "https://images.unsplash.com/photo-1610641818989-c2051b5e2cfd?w=800",
                         "https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?w=800" }),

                // businesses[6] Kazouza
                (6,  "14 Mecca St, Mansoura",      "Dakahlia",       "Egypt", "01011111121", 28,
                 new[] { "https://images.unsplash.com/photo-1504754524776-8f4f37790ca0?w=800",
                         "https://images.unsplash.com/photo-1559847844-5315695dadae?w=800",
                         "https://images.unsplash.com/photo-1519708227418-c8fd9a32b7a2?w=800" }),

                // businesses[7] Hilton Alexandria
                (7,  "North Coast Km 128",         "Matrouh",        "Egypt", "01011111122", 29,
                 new[] { "https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800",
                         "https://images.unsplash.com/photo-1584132967334-10e028bd69f7?w=800",
                         "https://images.unsplash.com/photo-1461175827210-5ceac3e39dd2?w=800" }),

                // businesses[8] Andrea Mariouteya
                (8,  "Mariouteya Canal, Sheikh Zayed","Giza",         "Egypt", "01011111123", 30,
                 new[] { "https://images.unsplash.com/photo-1544025162-d76694265947?w=800",
                         "https://images.unsplash.com/photo-1529193591184-b1d58069ecdd?w=800",
                         "https://images.unsplash.com/photo-1555939594-58d7cb561ad1?w=800" }),

                (8,  "6th October City Branch",    "Giza",           "Egypt", "01011111124", 31,
                 new[] { "https://images.unsplash.com/photo-1529193591184-b1d58069ecdd?w=800",
                         "https://images.unsplash.com/photo-1555939594-58d7cb561ad1?w=800",
                         "https://images.unsplash.com/photo-1544025162-d76694265947?w=800" }),

                // businesses[9] Zooba
                (9,  "The Cairo Festival City Mall","Cairo",          "Egypt", "01011111125", 32,
                 new[] { "https://images.unsplash.com/photo-1561758033-d89a9ad46330?w=800",
                         "https://images.unsplash.com/photo-1512058564366-18510be2db19?w=800",
                         "https://images.unsplash.com/photo-1548943487-a2e4e43b4853?w=800" }),

                (9,  "Madinaty Open Air Mall",      "Cairo",          "Egypt", "01011111126", 33,
                 new[] { "https://images.unsplash.com/photo-1512058564366-18510be2db19?w=800",
                         "https://images.unsplash.com/photo-1548943487-a2e4e43b4853?w=800",
                         "https://images.unsplash.com/photo-1561758033-d89a9ad46330?w=800" }),

                // businesses[10] Crave Restaurant
                (10, "San Stefano Grand Plaza",     "Alexandria",     "Egypt", "01011111127", 34,
                 new[] { "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=800",
                         "https://images.unsplash.com/photo-1482049016688-2d3e1b311543?w=800",
                         "https://images.unsplash.com/photo-1550547660-d9450f859349?w=800" }),

                // businesses[11] Mori Sushi
                (11, "City Stars Mall, Heliopolis",  "Cairo",         "Egypt", "01011111128", 18,
                 new[] { "https://images.unsplash.com/photo-1553621042-f6e147245754?w=800",
                         "https://images.unsplash.com/photo-1617196034183-421b4040ed20?w=800",
                         "https://images.unsplash.com/photo-1562802378-063ec186a863?w=800" }),

                // businesses[12] Cilantro Cafe
                (12, "Point 90 Mall, Madinaty",     "Cairo",          "Egypt", "01011111129", 19,
                 new[] { "https://images.unsplash.com/photo-1445116572660-236099ec97a0?w=800",
                         "https://images.unsplash.com/photo-1509042239860-f550ce710b93?w=800",
                         "https://images.unsplash.com/photo-1525610553991-2bede1a236e2?w=800" }),

                (12, "26 July St, Zamalek",          "Cairo",         "Egypt", "01011111130", 20,
                 new[] { "https://images.unsplash.com/photo-1509042239860-f550ce710b93?w=800",
                         "https://images.unsplash.com/photo-1525610553991-2bede1a236e2?w=800",
                         "https://images.unsplash.com/photo-1445116572660-236099ec97a0?w=800" }),

                // businesses[13] Kempinski
                (13, "12 Ahmed Ragheb St, Garden City","Cairo",       "Egypt", "01011111131", 21,
                 new[] { "https://images.unsplash.com/photo-1498503182468-3b51cbb6cb24?w=800",
                         "https://images.unsplash.com/photo-1564501049412-61c2a3083791?w=800",
                         "https://images.unsplash.com/photo-1506059612708-99d6c258160e?w=800" }),

                // businesses[14] Tolip Hotel
                (14, "New Alamein Towers",           "Matrouh",       "Egypt", "01011111132", 22,
                 new[] { "https://images.unsplash.com/photo-1600011689032-8b628b8a8747?w=800",
                         "https://images.unsplash.com/photo-1578683010236-d716f9a3f461?w=800",
                         "https://images.unsplash.com/photo-1590490360182-c33d57733427?w=800" }),

                // businesses[15] Jaz Sharm Dreams
                (15, "Naama Bay, South Sinai",       "Sharm El Sheikh","Egypt","01011111133", 23,
                 new[] { "https://images.unsplash.com/photo-1571003123894-1f0594d2b5d9?w=800",
                         "https://images.unsplash.com/photo-1540541338287-41700207dee6?w=800",
                         "https://images.unsplash.com/photo-1531088009183-5ff5b7c95f91?w=800" }),

                (15, "Hadaba Heights, Sharm",        "Sharm El Sheikh","Egypt","01011111134", 24,
                 new[] { "https://images.unsplash.com/photo-1540541338287-41700207dee6?w=800",
                         "https://images.unsplash.com/photo-1531088009183-5ff5b7c95f91?w=800",
                         "https://images.unsplash.com/photo-1571003123894-1f0594d2b5d9?w=800" }),

                // businesses[16] Sheraton Cairo
                (16, "Galaa Square, Dokki",           "Giza",          "Egypt", "01011111135", 25,
                 new[] { "https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=800",
                         "https://images.unsplash.com/photo-1618773928121-c32242e63f39?w=800",
                         "https://images.unsplash.com/photo-1559508551-44bff1de756b?w=800" }),

                // businesses[17] Sunrise Aqua Joy
                (17, "Sahl Hasheesh Bay",             "Hurghada",      "Egypt", "01011111136", 26,
                 new[] { "https://images.unsplash.com/photo-1573843981267-be1999ff37cd?w=800",
                         "https://images.unsplash.com/photo-1562778612-e1e0cda9915c?w=800",
                         "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?w=800" }),

                (17, "Makadi Bay Resort",             "Hurghada",      "Egypt", "01011111137", 27,
                 new[] { "https://images.unsplash.com/photo-1562778612-e1e0cda9915c?w=800",
                         "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?w=800",
                         "https://images.unsplash.com/photo-1573843981267-be1999ff37cd?w=800" }),
            };

            var branches = new List<Branch>();

            foreach (var (bi, address, city, country, phone, mi, images) in definitions)
            {
                var branch = Branch.Create(
                    businesses[bi].Id, address, city, country, phone, users[mi].Id);

                branch.AddImage(images[0], 1, isPrimary: true);
                branch.AddImage(images[1], 2);
                branch.AddImage(images[2], 3);

                branches.Add(branch);
            }

            await context.Branches.AddRangeAsync(branches);
            await context.SaveChangesAsync();

            logger.LogInformation("Branches seeded successfully — {Count} branches added.", branches.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while seeding branches.");
            throw;
        }
    }
}

#endregion

#region Proof Seeder

public static class ProofSeeder
{
    // Realistic receipt/invoice image URLs — each proof gets a unique MediaAsset
    private static readonly string[] ProofImageUrls =
    {
        "https://images.unsplash.com/photo-1554224155-6726b3ff858f?w=800",  // receipt paper
        "https://images.unsplash.com/photo-1586281380349-632531db7ed4?w=800",  // invoice document
        "https://images.unsplash.com/photo-1607863680198-23d4b2565df0?w=800",  // mobile payment receipt
        "https://images.unsplash.com/photo-1601597111158-2fceff292cdc?w=800",  // credit card slip
        "https://images.unsplash.com/photo-1563013544-824ae1b704d3?w=800",  // booking confirmation
        "https://images.unsplash.com/photo-1585974738771-84483dd9f89f?w=800",  // food order receipt
    };

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("ProofSeeder");

        try
        {
            if (await context.Proofs.AnyAsync())
                return;

            var users = await context.Users
                .Where(x => x.Role == Roles.User)
                .ToListAsync();

            var branches = await context.Branches
                .Include(x => x.Business)
                .ToListAsync();

            var admin = await context.Admins.FirstAsync();

            var proofs = new List<Proof>();
            var mediaAssets = new List<MediaAsset>();
            var used = new HashSet<string>();

            while (proofs.Count < 120)
            {
                var user = users[Random.Shared.Next(users.Count)];
                var branch = branches[Random.Shared.Next(branches.Count)];
                var key = $"{user.Id}-{branch.Id}";

                if (used.Contains(key))
                    continue;

                used.Add(key);

                var proof = Proof.CreateFromOrder(
                    user.Id,
                    branch.Id,
                    $"ORD-{10000 + proofs.Count}-{DateTime.UtcNow.Ticks}");

                proof.Verify(admin.Id);

                // Attach a receipt/invoice MediaAsset to this proof
                var imageUrl = ProofImageUrls[proofs.Count % ProofImageUrls.Length];
                var media = MediaAsset.CreateForProof(proof.Id, imageUrl);
                mediaAssets.Add(media);

                proofs.Add(proof);
            }

            await context.Proofs.AddRangeAsync(proofs);
            await context.SaveChangesAsync();

            // Save proof MediaAssets after proofs are persisted (so FK is valid)
            await context.MediaAssets.AddRangeAsync(mediaAssets);
            await context.SaveChangesAsync();

            logger.LogInformation(
                "Proofs seeded successfully — {Count} proofs + {MediaCount} receipt images added.",
                proofs.Count, mediaAssets.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while seeding proofs.");
            throw;
        }
    }
}

#endregion

#region Review Seeder

public static class ReviewSeeder
{
    private static string EnsureValidContent(string content)
    {
        if (content.Length >= 20)
            return content;

        return $"{content} Excellent experience overall.";
    }
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("ReviewSeeder");

        try
        {
            if (await context.Reviews.AnyAsync())
            {
                logger.LogInformation("Reviews already seeded");
                return;
            }

            // IMPORTANT
            // ONLY NORMAL USERS
            var users = await context.Users
                .Where(x => x.Role == Roles.User)
                .ToListAsync();

            var branches = await context.Branches.ToListAsync();

            var proofs = await context.Proofs.ToListAsync();

            var businesses = await context.Businesses
                .Include(x => x.BusinessCategory)
                .ToListAsync();

            var admin = await context.Admins.FirstAsync();

            var reviews = new List<Review>();

            string[] restaurantComments =
            {
                "Excellent food and very fast service.",
                "Amazing grilled meals and fresh ingredients.",
                "Fantastic seafood dishes.",
                "Very delicious oriental food.",
                "The desserts were amazing.",
                "Highly recommended restaurant.",
                "Portions were large and tasty.",
                "The atmosphere was cozy and relaxing.",
                "Fresh ingredients and amazing flavors.",
                "One of the best restaurants in Cairo.",
                "مكان ممتاز والخدمة سريعة.",
                "الطعام كان رائع جدًا.",
                "الخدمة ممتازة والطعام طازج.",
                "المكان هادئ ومناسب للعائلات.",
                "Best sushi restaurant I've visited."
            };

            string[] hotelComments =
            {
                "Hotel rooms were very clean.",
                "Luxury experience and professional staff.",
                "Very comfortable hotel stay.",
                "Breakfast quality was excellent.",
                "The hotel view was stunning.",
                "الفندق نظيف جدًا والخدمة ممتازة.",
                "إقامة مريحة جدًا.",
                "الغرف واسعة ونظيفة.",
                "تجربة فندقية ممتازة.",
                "Very luxurious and peaceful stay."
            };

            foreach (var proof in proofs)
            {
                // EXTRA SAFETY
                var user = users.FirstOrDefault(x => x.Id == proof.UserId);

                // Skip owners/managers if any old proof exists
                if (user is null)
                    continue;

                var branch = branches.First(x => x.Id == proof.BranchId);

                var business = businesses
                    .First(x => x.Id == branch.BusinessId);

                var comments = business.BusinessCategory.Name == "Hotel"
                    ? hotelComments
                    : restaurantComments;

                decimal foodRating = Math.Round(
                    (decimal)(Random.Shared.NextDouble() * 2 + 3),
                    1);

                decimal serviceRating = Math.Round(
                    (decimal)(Random.Shared.NextDouble() * 2 + 3),
                    1);

                decimal cleanlinessRating = Math.Round(
                    (decimal)(Random.Shared.NextDouble() * 2 + 3),
                    1);

                decimal ambienceRating = Math.Round(
                    (decimal)(Random.Shared.NextDouble() * 2 + 3),
                    1);

                decimal valueRating = Math.Round(
                    (decimal)(Random.Shared.NextDouble() * 2 + 3),
                    1);

                foodRating = Math.Clamp(foodRating, 1, 5);
                serviceRating = Math.Clamp(serviceRating, 1, 5);
                cleanlinessRating = Math.Clamp(cleanlinessRating, 1, 5);
                ambienceRating = Math.Clamp(ambienceRating, 1, 5);
                valueRating = Math.Clamp(valueRating, 1, 5);

                var review = Review.Create(
                    user.Id,
                    branch.Id,
                    foodRating,
                    serviceRating,
                    cleanlinessRating,
                    ambienceRating,
                    valueRating,
                    $"Review for {business.Name} Branch",
                    EnsureValidContent(
                          comments[Random.Shared.Next(comments.Length)]),
                    proof.Id);

                review.Approve(admin.Id);

                var averageRating =
                    (foodRating +
                     serviceRating +
                     cleanlinessRating +
                     ambienceRating +
                     valueRating) / 5m;

                if (averageRating >= 4.5m)
                {
                    user.IncreaseTrustScore(TrustScore.ApprovalBonus);
                }
                else if (averageRating >= 4m)
                {
                    user.IncreaseTrustScore(3);
                }
                else
                {
                    user.IncreaseTrustScore(1);
                }

                user.RecordReviewSubmission();

                reviews.Add(review);
            }

            await context.Reviews.AddRangeAsync(reviews);

            await context.SaveChangesAsync();

            var approvedReviews = await context.Reviews
                .Include(r => r.User)
                .Where(r => r.Status == ReviewStatus.Approved)
                .ToListAsync();

            foreach (var branch in branches)
            {
                var branchReviews = approvedReviews
                    .Where(r => r.BranchId == branch.Id)
                    .ToList();

                branch.RecalculateRatings(
                    branchReviews.Select(r =>
                        (
                            r.OverallRating,
                            r.User.TrustScoreValue / 100m
                        )),
                    pendingCount: 0
                );
            }

            await context.SaveChangesAsync();

            logger.LogInformation("Reviews seeded successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while seeding reviews");
            throw;
        }
    }

}

#endregion
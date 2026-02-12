using System.Security.AccessControl;
using Diplomski.RatingHub.Domain.Constants;
using Diplomski.RatingHub.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Diplomski.RatingHub.Infrastructure.Persistence.Extensions;

public static class ModelBuilderExtension
{
    public static void SeedCities(this ModelBuilder modelBuilder)
    {
        List<City> cities = new List<City>();
        
        cities.Add(new City{ Id = 1, Name = "Beograd", Longitude = 20.46, Latitude = 44.82 });
        cities.Add(new City{ Id = 2, Name = "Novi Sad", Longitude = 19.8425, Latitude = 45.2542 });
        cities.Add(new City{ Id = 3, Name = "Nis", Longitude = 21.8958, Latitude = 43.3208 });
        cities.Add(new City{ Id = 4, Name = "Zemun", Longitude = 20.4, Latitude = 44.85 });
        cities.Add(new City{ Id = 5, Name = "Kragujevac", Longitude = 20.9172, Latitude = 44.0101 });
        cities.Add(new City{ Id = 6, Name = "Sabac", Longitude = 19.7, Latitude = 44.75 });
        cities.Add(new City{ Id = 7, Name = "Smederevo", Longitude = 20.9333, Latitude = 44.6667 });
        cities.Add(new City{ Id = 8, Name = "Valjevo", Longitude = 19.8833, Latitude = 44.2667 });
        cities.Add(new City{ Id = 9, Name = "Loznica", Longitude = 19.2258, Latitude = 44.5333 });
        cities.Add(new City{ Id = 10, Name = "Pancevo", Longitude = 20.6403, Latitude = 44.8706 });
        cities.Add(new City{ Id = 11, Name = "Sremska Mitrovica", Longitude = 19.6125, Latitude = 44.97 });
        cities.Add(new City{ Id = 12, Name = "Stara Pazova", Longitude = 20.1667, Latitude = 44.9833 });
        cities.Add(new City{ Id = 13, Name = "Cacak", Longitude = 20.35, Latitude = 43.8833 });
        cities.Add(new City{ Id = 14, Name = "Pozarevac", Longitude = 21.1833, Latitude = 44.6167 });
        cities.Add(new City{ Id = 15, Name = "Zrenjanin", Longitude = 20.3894, Latitude = 45.3833 });
        cities.Add(new City{ Id = 16, Name = "Kraljevo", Longitude = 20.6875, Latitude = 43.7236 });
        cities.Add(new City{ Id = 17, Name = "Novi Pazar", Longitude = 20.5161, Latitude = 43.1378 });
        cities.Add(new City{ Id = 18, Name = "Uzice", Longitude = 19.85, Latitude = 43.85 });
        cities.Add(new City{ Id = 19, Name = "Krusevac", Longitude = 21.3306, Latitude = 43.5806 });
        cities.Add(new City{ Id = 20, Name = "Lazarevac", Longitude = 20.25, Latitude = 44.3667 });
        cities.Add(new City{ Id = 21, Name = "Vranje", Longitude = 21.9, Latitude = 42.55 });
        cities.Add(new City{ Id = 22, Name = "Ruma", Longitude = 19.8333, Latitude = 45 });
        cities.Add(new City{ Id = 23, Name = "Paracin", Longitude = 21.4167, Latitude = 43.8667 });
        cities.Add(new City{ Id = 24, Name = "Sombor", Longitude = 19.1167, Latitude = 45.7833 });
        cities.Add(new City{ Id = 25, Name = "Pirot", Longitude = 22.6, Latitude = 43.1667 });
        cities.Add(new City{ Id = 26, Name = "Mladenovac", Longitude = 20.7, Latitude = 44.4333 });
        cities.Add(new City{ Id = 27, Name = "Zajecar", Longitude = 22.3, Latitude = 43.9167 });
        cities.Add(new City{ Id = 28, Name = "Batajnica", Longitude = 20.2833, Latitude = 44.9 });
        cities.Add(new City{ Id = 29, Name = "Indija", Longitude = 20.0833, Latitude = 45.05 });
        cities.Add(new City{ Id = 30, Name = "Arandelovac", Longitude = 20.5667, Latitude = 44.3 });
        cities.Add(new City{ Id = 31, Name = "Borca", Longitude = 20.45, Latitude = 44.87 });
        cities.Add(new City{ Id = 32, Name = "Surcin", Longitude = 20.2723, Latitude = 44.7919 });
        cities.Add(new City{ Id = 33, Name = "Aleksinac", Longitude = 21.7, Latitude = 43.55 });
        cities.Add(new City{ Id = 34, Name = "Smederevska Palanka", Longitude = 20.9565, Latitude = 44.3663 });
        cities.Add(new City{ Id = 35, Name = "Bujanovac", Longitude = 21.7667, Latitude = 42.4667 });
        cities.Add(new City{ Id = 36, Name = "Velika Plana", Longitude = 21.0768, Latitude = 44.334 });
        cities.Add(new City{ Id = 37, Name = "Gornji Milanovac", Longitude = 20.46, Latitude = 44.0242 });
        cities.Add(new City{ Id = 38, Name = "Prokuplje", Longitude = 21.5903, Latitude = 43.2361 });
        cities.Add(new City{ Id = 39, Name = "Becej", Longitude = 20.0333, Latitude = 45.6167 });
        cities.Add(new City{ Id = 40, Name = "Prijepolje", Longitude = 19.6333, Latitude = 43.3833 });
        cities.Add(new City{ Id = 41, Name = "Vrsac", Longitude = 21.3033, Latitude = 45.1167 });
        cities.Add(new City{ Id = 42, Name = "Kula", Longitude = 19.5333, Latitude = 45.6 });
        cities.Add(new City{ Id = 43, Name = "Negotin", Longitude = 22.5306, Latitude = 44.2292 });
        cities.Add(new City{ Id = 44, Name = "Jagodina", Longitude = 21.25, Latitude = 43.9667 });
        cities.Add(new City{ Id = 45, Name = "Sid", Longitude = 19.2333, Latitude = 45.1167 });
        cities.Add(new City{ Id = 46, Name = "Bor", Longitude = 22.1, Latitude = 44.0833 });
        cities.Add(new City{ Id = 47, Name = "Kovin", Longitude = 20.9667, Latitude = 44.75 });
        cities.Add(new City{ Id = 48, Name = "Tutin", Longitude = 20.3364, Latitude = 42.9897 });
        cities.Add(new City{ Id = 49, Name = "Knjazevac", Longitude = 22.2575, Latitude = 43.5675 });
        cities.Add(new City{ Id = 50, Name = "Petrovac na Mlavi", Longitude = 21.4194, Latitude = 44.3783 });
        cities.Add(new City{ Id = 51, Name = "Subotica", Longitude = 19.6656, Latitude = 46.1003 });
        cities.Add(new City{ Id = 52, Name = "Vlasotince", Longitude = 22.1333, Latitude = 42.9667 });
        cities.Add(new City{ Id = 53, Name = "Kikinda", Longitude = 20.45, Latitude = 45.8333 });
        cities.Add(new City{ Id = 54, Name = "Temerin", Longitude = 19.8833, Latitude = 45.4167 });
        cities.Add(new City{ Id = 55, Name = "Ivanjica", Longitude = 20.2297, Latitude = 43.5811 });
        cities.Add(new City{ Id = 56, Name = "Kaluderica", Longitude = 20.55, Latitude = 44.75 });
        cities.Add(new City{ Id = 57, Name = "Aleksandrovac", Longitude = 21.0464, Latitude = 43.4589 });
        cities.Add(new City{ Id = 58, Name = "Bajina Basta", Longitude = 19.55, Latitude = 43.95 });
        cities.Add(new City{ Id = 59, Name = "Backa Palanka", Longitude = 19.4, Latitude = 45.25 });
        cities.Add(new City{ Id = 60, Name = "Raska", Longitude = 20.6156, Latitude = 43.2919 });
        cities.Add(new City{ Id = 61, Name = "Vrbas", Longitude = 19.65, Latitude = 45.5667 });
        cities.Add(new City{ Id = 62, Name = "Svilajnac", Longitude = 21.1964, Latitude = 44.2356 });
        cities.Add(new City{ Id = 63, Name = "Apatin", Longitude = 18.9833, Latitude = 45.6667 });
        cities.Add(new City{ Id = 64, Name = "Despotovac", Longitude = 21.4333, Latitude = 44.0833 });
        cities.Add(new City{ Id = 65, Name = "Topola", Longitude = 20.7, Latitude = 44.25 });
        cities.Add(new City{ Id = 66, Name = "Surdulica", Longitude = 22.1667, Latitude = 42.6833 });
        cities.Add(new City{ Id = 67, Name = "Lebane", Longitude = 21.7333, Latitude = 42.9167 });
        cities.Add(new City{ Id = 68, Name = "Vladicin Han", Longitude = 22.0667, Latitude = 42.7 });
        cities.Add(new City{ Id = 69, Name = "Leskovac", Longitude = 21.95, Latitude = 43 });
        cities.Add(new City{ Id = 70, Name = "Lucani", Longitude = 20.1333, Latitude = 43.8667 });
        cities.Add(new City{ Id = 71, Name = "Kladovo", Longitude = 22.6131, Latitude = 44.6067 });
        cities.Add(new City{ Id = 72, Name = "Senta", Longitude = 20.09, Latitude = 45.9314 });
        cities.Add(new City{ Id = 73, Name = "Pecinci", Longitude = 19.9667, Latitude = 44.9 });
        cities.Add(new City{ Id = 74, Name = "Arilje", Longitude = 20.0986, Latitude = 43.7514 });
        cities.Add(new City{ Id = 75, Name = "Majdanpek", Longitude = 21.9333, Latitude = 44.4167 });
        cities.Add(new City{ Id = 76, Name = "Varvarin", Longitude = 21.3667, Latitude = 43.7167 });
        cities.Add(new City{ Id = 77, Name = "Veliko Gradiste", Longitude = 21.5167, Latitude = 44.75 });
        cities.Add(new City{ Id = 78, Name = "Vladimirci", Longitude = 19.7833, Latitude = 44.6167 });
        cities.Add(new City{ Id = 79, Name = "Krupanj", Longitude = 19.3667, Latitude = 44.3667 });
        cities.Add(new City{ Id = 80, Name = "Trstenik", Longitude = 20.9833, Latitude = 43.6167 });
        cities.Add(new City{ Id = 81, Name = "Zitorada", Longitude = 21.7167, Latitude = 43.1833 });
        cities.Add(new City{ Id = 82, Name = "Srbobran", Longitude = 19.7833, Latitude = 45.5333 });
        cities.Add(new City{ Id = 83, Name = "Brus", Longitude = 21.0378, Latitude = 43.3814 });
        cities.Add(new City{ Id = 84, Name = "Kursumlija", Longitude = 21.2667, Latitude = 43.15 });
        cities.Add(new City{ Id = 85, Name = "Titel", Longitude = 20.3, Latitude = 45.2047 });
        cities.Add(new City{ Id = 86, Name = "Kucevo", Longitude = 21.6667, Latitude = 44.4833 });
        cities.Add(new City{ Id = 87, Name = "Lajkovac", Longitude = 20.1667, Latitude = 44.3667 });
        cities.Add(new City{ Id = 88, Name = "Petrovaradin", Longitude = 19.8667, Latitude = 45.25 });
        cities.Add(new City{ Id = 89, Name = "Cajetina", Longitude = 19.7167, Latitude = 43.75 });
        cities.Add(new City{ Id = 90, Name = "Mionica", Longitude = 20.0833, Latitude = 44.25 });
        cities.Add(new City{ Id = 91, Name = "Svrljig", Longitude = 22.1167, Latitude = 43.4167 });
        cities.Add(new City{ Id = 92, Name = "Knic", Longitude = 20.7194, Latitude = 43.9264 });
        cities.Add(new City{ Id = 93, Name = "Niska Banja", Longitude = 22.0061, Latitude = 43.2933 });
        cities.Add(new City{ Id = 94, Name = "Sjenica", Longitude = 20, Latitude = 43.2667 });
        cities.Add(new City{ Id = 95, Name = "Merosina", Longitude = 21.7167, Latitude = 43.2833 });
        cities.Add(new City{ Id = 96, Name = "Kostolac", Longitude = 21.17, Latitude = 44.7147 });
        cities.Add(new City{ Id = 97, Name = "Nova Varos", Longitude = 19.8154, Latitude = 43.4604 });
        cities.Add(new City{ Id = 98, Name = "Secanj", Longitude = 20.7725, Latitude = 45.3667 });
        cities.Add(new City{ Id = 99, Name = "Novi Becej", Longitude = 20.1167, Latitude = 45.6 });
        cities.Add(new City{ Id = 100, Name = "Osecina", Longitude = 19.6, Latitude = 44.3667 });
        cities.Add(new City{ Id = 101, Name = "Bela Palanka", Longitude = 22.3167, Latitude = 43.2167 });
        cities.Add(new City{ Id = 102, Name = "Kosjeric", Longitude = 19.9167, Latitude = 44 });
        cities.Add(new City{ Id = 103, Name = "Backa Topola", Longitude = 19.6333, Latitude = 45.8167 });
        cities.Add(new City{ Id = 104, Name = "Batocina", Longitude = 21.0833, Latitude = 44.15 });
        cities.Add(new City{ Id = 105, Name = "Blace", Longitude = 21.2859, Latitude = 43.2958 });
        cities.Add(new City{ Id = 106, Name = "Raca", Longitude = 20.9833, Latitude = 44.2333 });
        cities.Add(new City{ Id = 107, Name = "Malo Crnice", Longitude = 21.2833, Latitude = 44.5667 });
        cities.Add(new City{ Id = 108, Name = "Rekovac", Longitude = 21.1333, Latitude = 43.8667 });
        cities.Add(new City{ Id = 109, Name = "Bela Crkva", Longitude = 21.4169, Latitude = 44.8975 });
        cities.Add(new City{ Id = 110, Name = "Ada", Longitude = 20.1333, Latitude = 45.8 });
        cities.Add(new City{ Id = 111, Name = "Vrnjacka Banja", Longitude = 20.8936, Latitude = 43.6236 });
        cities.Add(new City{ Id = 112, Name = "Kanjiza", Longitude = 20.05, Latitude = 46.0667 });
        cities.Add(new City{ Id = 113, Name = "Cuprija", Longitude = 21.3667, Latitude = 43.9333 });
        cities.Add(new City{ Id = 114, Name = "Cicevac", Longitude = 21.45, Latitude = 43.7167 });
        cities.Add(new City{ Id = 115, Name = "Zabalj", Longitude = 20.0667, Latitude = 45.3667 });
        cities.Add(new City{ Id = 116, Name = "Razanj", Longitude = 21.55, Latitude = 43.6667 });
        cities.Add(new City{ Id = 117, Name = "Odzaci", Longitude = 19.2667, Latitude = 45.5167 });
        cities.Add(new City{ Id = 118, Name = "Sremski Karlovci", Longitude = 19.9333, Latitude = 45.2 });
        cities.Add(new City{ Id = 119, Name = "Gadzin Han", Longitude = 22.0322, Latitude = 43.2228 });
        cities.Add(new City{ Id = 120, Name = "Golubac", Longitude = 21.6333, Latitude = 44.65 });
        cities.Add(new City{ Id = 121, Name = "Beocin", Longitude = 19.7333, Latitude = 45.2 });
        cities.Add(new City{ Id = 122, Name = "Dimitrovgrad", Longitude = 22.7833, Latitude = 43.0167 });
        cities.Add(new City{ Id = 123, Name = "Sokobanja", Longitude = 21.8667, Latitude = 43.65 });
        cities.Add(new City{ Id = 124, Name = "Backi Petrovac", Longitude = 19.5917, Latitude = 45.3606 });
        cities.Add(new City{ Id = 125, Name = "Lapovo", Longitude = 21.1, Latitude = 44.1833 });
        cities.Add(new City{ Id = 126, Name = "Novi Knezevac", Longitude = 20.1, Latitude = 46.05 });
        cities.Add(new City{ Id = 127, Name = "Bogatic", Longitude = 19.4833, Latitude = 44.8333 });
        cities.Add(new City{ Id = 128, Name = "Medveda", Longitude = 21.5847, Latitude = 42.8431 });
        cities.Add(new City{ Id = 129, Name = "Ub", Longitude = 20.0739, Latitude = 44.4561 });
        cities.Add(new City{ Id = 130, Name = "Bac", Longitude = 19.2333, Latitude = 45.3833 });
        cities.Add(new City{ Id = 131, Name = "Mali Idos", Longitude = 19.6644, Latitude = 45.7069 });
        cities.Add(new City{ Id = 132, Name = "Kovacica", Longitude = 20.6214, Latitude = 45.1117 });
        cities.Add(new City{ Id = 133, Name = "Irig", Longitude = 19.85, Latitude = 45.1 });
        cities.Add(new City{ Id = 134, Name = "Coka", Longitude = 20.15, Latitude = 45.9333 });
        cities.Add(new City{ Id = 135, Name = "Opovo", Longitude = 20.4303, Latitude = 45.0519 });
        cities.Add(new City{ Id = 136, Name = "Plandiste", Longitude = 21.1217, Latitude = 45.2269 });
        cities.Add(new City{ Id = 137, Name = "Alibunar", Longitude = 20.9656, Latitude = 45.0806 });
        cities.Add(new City{ Id = 138, Name = "Zitiste", Longitude = 20.55, Latitude = 45.4833 });
        cities.Add(new City{ Id = 139, Name = "Nova Crnja", Longitude = 20.6, Latitude = 45.6667 });
        cities.Add(new City{ Id = 140, Name = "Crna Trava", Longitude = 22.299, Latitude = 42.8101 });
        cities.Add(new City{ Id = 141, Name = "Obrenovac", Longitude = 20.2, Latitude = 44.6539 });
        cities.Add(new City{ Id = 142, Name = "Priboj", Longitude = 19.5258, Latitude = 43.5836 });
        cities.Add(new City{ Id = 143, Name = "Pozega", Longitude = 20.0368, Latitude = 43.8459 });
        cities.Add(new City{ Id = 144, Name = "Babusnica", Longitude = 22.4115, Latitude = 43.068 });
        cities.Add(new City{ Id = 145, Name = "Ljubovija", Longitude = 19.3728, Latitude = 44.1869 });
        cities.Add(new City{ Id = 146, Name = "Ljig", Longitude = 20.2375, Latitude = 44.2213 });
        cities.Add(new City{ Id = 147, Name = "Mali Zvornik", Longitude = 19.1214, Latitude = 44.3992 });
        cities.Add(new City{ Id = 148, Name = "Doljevac", Longitude = 21.8334, Latitude = 43.1968 });
        cities.Add(new City{ Id = 149, Name = "Boljevac", Longitude = 21.9519, Latitude = 43.8247 });
        cities.Add(new City{ Id = 150, Name = "Bojnik", Longitude = 21.718, Latitude = 43.0142 });
        cities.Add(new City{ Id = 151, Name = "Koceljeva", Longitude = 19.807, Latitude = 44.4708 });
        cities.Add(new City{ Id = 152, Name = "Bosilegrad", Longitude = 22.4728, Latitude = 42.5005 });
        cities.Add(new City{ Id = 153, Name = "Zabari", Longitude = 21.2143, Latitude = 44.3562 });
        cities.Add(new City{ Id = 154, Name = "Trgoviste", Longitude = 22.0921, Latitude = 42.3514 });
        cities.Add(new City{ Id = 155, Name = "Zagubica", Longitude = 21.7902, Latitude = 44.1979 });
        cities.Add(new City{ Id = 156, Name = "Presevo", Longitude = 21.65, Latitude = 42.3067 });
        cities.Add(new City{ Id = 157, Name = "Sopot", Longitude = 20.5758, Latitude = 44.5193 });
        cities.Add(new City{ Id = 158, Name = "Grocka", Longitude = 20.7153, Latitude = 44.672 });
        cities.Add(new City{ Id = 159, Name = "Barajevo", Longitude = 20.4179, Latitude = 44.579 });

        modelBuilder.Entity<City>(e =>
        {
            e.HasData(cities);
        });
    }

    public static void SeedIdentityRoles(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityRole>().HasData(
        [
            new IdentityRole { Name = Roles.Administrator, NormalizedName = Roles.Administrator.ToUpper() },
            new IdentityRole { Name = Roles.RegularUser, NormalizedName = Roles.RegularUser.ToUpper() }
        ]);
    }
}
using Microsoft.EntityFrameworkCore;
using TalepOnaySistemiAPI.Models;

namespace TalepOnaySistemiAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Rol> Roller { get; set; }

        public DbSet<Musteri> Musteriler { get; set; }

        public DbSet<Kullanici> Kullanicilar { get; set; }

        public DbSet<Talep> Talepler { get; set; }

        public DbSet<TalepDurumu> TalepDurumlari { get; set; }

        public DbSet<TalepTipi> TalepTipleri { get; set; }

        public DbSet<TalepNotu> TalepNotlari { get; set; }

        public DbSet<TalepGecmisi> TalepGecmisi { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Talepler tablosunda SQL trigger olduğu için EF Core'a bildiriyoruz,
            // aksi halde EF Core'un ürettiği UPDATE sorgusundaki OUTPUT cümlesi
            // SQL Server tarafından reddediliyor.
            modelBuilder.Entity<Talep>()
                .ToTable(tb => tb.HasTrigger("TR_Talepler_DurumDegisti"));
        }
    }
}
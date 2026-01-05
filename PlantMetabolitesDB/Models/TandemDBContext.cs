using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using TandemDB.Models;

namespace PlantMetabolitesDB.Models
{
    public class PlantMetabolitesDBContext: DbContext
    {
        public PlantMetabolitesDBContext()
           : base("ConnectionStr")
        {
            this.Configuration.ProxyCreationEnabled = true;
            this.Configuration.LazyLoadingEnabled = true;
        }
        public DbSet<MassSpectra_DataValues> MassSpectra_DataValuess { get; set; }    
        public DbSet<Master_AdvisioryBoard> Master_AdvisioryBoards { get; set; }
        public DbSet<Master_Annotation> Master_Annotations { get; set; }
        public DbSet<Master_Collaborators> Master_Collaboratorss { get; set; }  
        public DbSet<Master_Country> Master_Countrys { get; set; }
        public DbSet<Master_Database> Master_Databases { get; set; }
        public DbSet<Master_Instrument> Master_Instruments { get; set; }
        public DbSet<Master_LatestUpdates> Master_LatestUpdatess { get; set; }
        public DbSet<Master_OrganizationType> Master_OrganizationTypes { get; set; }
        public DbSet<Master_Ticker> Master_Tickers { get; set; }
        public DbSet<Master_FAQ> Master_FAQs { get; set; }
        public DbSet<Master_User> Master_Users { get; set; }
        public DbSet<Master_Role> Master_Roles { get; set; }
        public DbSet<Master_Compound> Master_Compounds { get; set; }
        public DbSet<Master_MS2MassSpectra> Master_MS2MassSpectras { get; set; }
        public DbSet<Master_MS3MassSpectra> Master_MS3MassSpectras { get; set; }
        public DbSet<Master_AductMassSpectra> Master_AductMassSpectras { get; set; }
        public DbSet<Master_Database_Instrument> Master_Database_Instruments { get; set; }
        public DbSet<PlantMetabolites> PlantMetabolites { get; set; }

        public DbSet<M02_VernacularName> M02_VernacularName { get; set; }
        public DbSet<M03_BiologicalActivity> M03_BiologicalActivity { get; set; }
        public DbSet<M04_Distribution> M04_Distribution { get; set; }
        public DbSet<M05_EthnobotanicalInfo> M05_EthnobotanicalInfo { get; set; }
        public DbSet<M06_CompuondClass> M06_CompuondClass { get; set; }
        public DbSet<M07_Constituents> M07_Constituents { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Master_Database>()
                .HasMany(p => p.PlantMetabolites)
                .WithRequired(v => v.Master_Databases)
                .HasForeignKey(v => v.PlantFamilyKey);

            modelBuilder.Entity<PlantMetabolites>()
                .HasMany(p => p.M02_VernacularName)
                .WithRequired(v => v.PlantMetabolites)
                .HasForeignKey(v => v.M01ID);

            modelBuilder.Entity<PlantMetabolites>()
                .HasMany(p => p.M03_BiologicalActivity)
                .WithRequired(b => b.PlantMetabolites)
                .HasForeignKey(b => b.M01ID);

            modelBuilder.Entity<PlantMetabolites>()
                .HasMany(p => p.M04_Distribution)
                .WithRequired(d => d.PlantMetabolites)
                .HasForeignKey(d => d.M01ID);

            modelBuilder.Entity<PlantMetabolites>()
                .HasMany(p => p.M05_EthnobotanicalInfo)
                .WithRequired(e => e.PlantMetabolites)
                .HasForeignKey(e => e.M01ID);

            modelBuilder.Entity<PlantMetabolites>()
                .HasMany(p => p.M06_CompuondClass)
                .WithRequired(c => c.PlantMetabolites)
                .HasForeignKey(c => c.M01ID);

            modelBuilder.Entity<PlantMetabolites>()
                .HasMany(p => p.M07_Constituents)
                .WithRequired(c => c.PlantMetabolites)
                .HasForeignKey(c => c.M01ID);
        }


    }
}
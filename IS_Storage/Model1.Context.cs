﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IS_Storage
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class asonov_KPEntities : DbContext
    {
        public asonov_KPEntities()
            : base("name=asonov_KPEntities")
        {
        }

        public static asonov_KPEntities _context;
        public static asonov_KPEntities GetStockEntity()
        {
            if (_context == null) _context = new asonov_KPEntities();
            return _context;
        }
        public static asonov_KPEntities GetStockEntityD()
        {
            return new asonov_KPEntities();
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Client> Client { get; set; }
        public virtual DbSet<Condition> Condition { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<Place> Place { get; set; }
        public virtual DbSet<PlaceCond> PlaceCond { get; set; }
        public virtual DbSet<ProdCond> ProdCond { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<reqType> reqType { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Transaction> Transaction { get; set; }
        public virtual DbSet<TransType> TransType { get; set; }
        public virtual DbSet<UnitType> UnitType { get; set; }
        public virtual DbSet<userRequest> userRequest { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
    }
}

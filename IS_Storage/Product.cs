//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.ProdCond = new HashSet<ProdCond>();
        }
    
        public int IDProduct { get; set; }
        public string Name { get; set; }
        public string Article { get; set; }
        public int ID_Transction { get; set; }
        public Nullable<System.DateTime> Exp_Date { get; set; }
        public int ID_Place { get; set; }
        public int Amount { get; set; }
        public int UnitID { get; set; }
    
        public virtual Place Place { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProdCond> ProdCond { get; set; }
        public virtual Transaction Transaction { get; set; }
        public virtual UnitType UnitType { get; set; }
    }
}
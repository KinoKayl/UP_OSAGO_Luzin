//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UP_OSAGO_Luzin
{
    using System;
    using System.Collections.Generic;
    
    public partial class Cars
    {
        public int CarID { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public Nullable<int> YearOfManufacture { get; set; }
        public string LicensePlate { get; set; }
        public string STSNumber { get; set; }
        public string PTSNumber { get; set; }
        public Nullable<int> DriverID { get; set; }
    
        public virtual Drivers Drivers { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace MomenMedmSys.Core.Entities
{
    /// <summary>
    /// Supplier/Vendor entity - centralized supplier management for procurement
    /// </summary>
    public class Supplier : BaseEntity
    {
        public string SupplierCode { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Fax { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string TaxNumber { get; set; } = string.Empty;

        // Business Details
        public string ProductCategories { get; set; } = string.Empty; // Comma-separated
        public int Rating { get; set; } // 1-5 stars
        public bool IsApproved { get; set; } = true;
        public int LeadTimeDays { get; set; } // Average delivery time
        public string PaymentTerms { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;

        // Performance
        public int TotalOrders { get; set; }
        public int OnTimeDeliveries { get; set; }
        public decimal OnTimePercentage => TotalOrders > 0 ? (decimal)OnTimeDeliveries / TotalOrders * 100 : 0;

        // Navigation
        public ICollection<MedicalDevice> SuppliedDevices { get; set; } = new List<MedicalDevice>();
        public ICollection<SparePart> SuppliedParts { get; set; } = new List<SparePart>();
        public ICollection<ServiceContract> Contracts { get; set; } = new List<ServiceContract>();
        public ICollection<ProcurementRequest> ProcurementRequests { get; set; } = new List<ProcurementRequest>();
    }
}

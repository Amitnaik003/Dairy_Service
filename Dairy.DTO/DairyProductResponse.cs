namespace Dairy.DTO
{
    // The restricted view for normal users (only allowed columns)
    public class DairyProductResponse
    {
        public string ProductId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public double FatContent { get; set; }
        public bool IsFresh { get; set; }
    }

    // The full view for Admins (all columns including stock and temperature)
    public class DairyProductAdminResponse
    {
        public string ProductId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public double FatContent { get; set; }
        public string TemperatureRequired { get; set; } = string.Empty;
        public bool IsFresh { get; set; }
        public int StockQuantity { get; set; }
    }
    
    // Request DTO for creating/updating products
    public class DairyProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public double FatContentPercentage { get; set; }
        public string StorageTemperatureRange { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
    }
}

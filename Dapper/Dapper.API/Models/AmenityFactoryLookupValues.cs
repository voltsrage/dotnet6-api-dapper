namespace Dapper.API.Models
{
    /// <summary>
    /// Lookup values used to search dictionary in AmenityFactory
    /// </summary>
    public static class AmenityFactoryLookupValues
    {
        // BaseAmenity properties
        public const string Name = nameof(Name);
        public const string Description = nameof(Description);
        public const string PriceModifier = nameof(PriceModifier);
        public const string IsStandard = nameof(IsStandard);

        // WIFIAmenity properties
        public const string NetworkName = nameof(NetworkName);
        public const string Password = nameof(Password);
        public const string SpeedMbps = nameof(SpeedMbps);

        // MiniBarAmenity properties
        public const string IsComplimentary = nameof(IsComplimentary);
        public const string Items = nameof(Items);

        // RoomServiceAmenity properties
        public const string HoursAvailable = nameof(HoursAvailable);
        public const string Is24Hours = nameof(Is24Hours);

        // Decorator properties
        // PremiumDecorator properties
        public const string PremiumFeature = nameof(PremiumFeature);
        public const string AdditionalCost = nameof(AdditionalCost);

        // SeasonalDecorator properties
        public const string Season = nameof(Season);
        public const string StartDate = nameof(StartDate);
        public const string EndDate = nameof(EndDate);
        public const string SeasonalPriceAdjustment = nameof(SeasonalPriceAdjustment);
    }
}

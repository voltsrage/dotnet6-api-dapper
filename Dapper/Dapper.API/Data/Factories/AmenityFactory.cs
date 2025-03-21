using Dapper.API.Data.Factories.Interfaces;
using Dapper.API.Entities.Amentities;
using Dapper.API.Enums.StandardEnums;
using Dapper.API.Models;

namespace Dapper.API.Data.Factories
{
    public class AmenityFactory : IAmenityFactory
    {
        public BaseAmenity CreateAmenity(AmenityTypeEnum amenityType, Dictionary<string, object> properties)
        {
            // Extract common properties
            if (!properties.TryGetValue(AmenityFactoryLookupValues.Name, out object nameObj) || !(nameObj is string name))
                throw new ArgumentException("Name is required for all amenities.");

            if (!properties.TryGetValue(AmenityFactoryLookupValues.Description, out object descObj) || !(descObj is string description))
                throw new ArgumentException("Description is required for all amenities.");

            if (!properties.TryGetValue(AmenityFactoryLookupValues.PriceModifier, out object priceObj) || !(priceObj is decimal priceModifier))
                throw new ArgumentException("PriceModifier is required for all amenities.");

            if (!properties.TryGetValue(AmenityFactoryLookupValues.IsStandard, out object stdObj) || !(stdObj is bool isStandard))
                throw new ArgumentException("IsStandard flag is required for all amenities.");


            // Create the specific type of amenity
            switch (amenityType)
            {
                case AmenityTypeEnum.Wifi:
                    if (!properties.TryGetValue(AmenityFactoryLookupValues.NetworkName, out object netObj) || !(netObj is string networkName))
                        throw new ArgumentException("NetworkName is required for WiFi amenities.");

                    if (!properties.TryGetValue(AmenityFactoryLookupValues.Password, out object passObj) || !(passObj is string password))
                        throw new ArgumentException("Password is required for WiFi amenities.");

                    if (!properties.TryGetValue(AmenityFactoryLookupValues.SpeedMbps, out object speedObj) || !(speedObj is int speedMbps))
                        throw new ArgumentException("SpeedMbps is required for WiFi amenities.");

                    return new WIFIAmenity(name, description, priceModifier, isStandard,
                                          networkName, password, speedMbps);

                case AmenityTypeEnum.Minibar:
                    if (!properties.TryGetValue(AmenityFactoryLookupValues.IsComplimentary, out object compObj) || !(compObj is bool isComplimentary))
                        throw new ArgumentException("IsComplimentary flag is required for MiniBar amenities.");

                    if (!properties.TryGetValue(AmenityFactoryLookupValues.Items, out object itemsObj) || !(itemsObj is List<string> items))
                        throw new ArgumentException("Items list is required for MiniBar amenities.");

                    return new MiniBarAmenity(name, description, priceModifier, isStandard,
                                            isComplimentary, items);

                case AmenityTypeEnum.RoomService:
                    if (!properties.TryGetValue(AmenityFactoryLookupValues.HoursAvailable, out object hoursObj) || !(hoursObj is int hoursAvailable))
                        throw new ArgumentException("HoursAvailable is required for RoomService amenities.");

                    if (!properties.TryGetValue(AmenityFactoryLookupValues.Is24Hours, out object hrs24Obj) || !(hrs24Obj is bool is24Hours))
                        throw new ArgumentException("Is24Hours flag is required for RoomService amenities.");

                    return new RoomServiceAmenity(name, description, priceModifier, isStandard,
                                                hoursAvailable, is24Hours);

                default:
                    throw new ArgumentException($"Unknown amenity type: {amenityType}");
            }
        }

        public IAmenity CreateDecorator(DecoratorTypeEnum decoratorType, IAmenity baseAmenity, Dictionary<string, object> properties)
        {
            switch (decoratorType) 
            {
                case DecoratorTypeEnum.Premium:
                    if(!properties.TryGetValue(AmenityFactoryLookupValues.PremiumFeature, out object featureObj) || !(featureObj is string premiumFeature))
                        throw new ArgumentException("PremiumFeature is required for Premium decorators.");

                    if (!properties.TryGetValue(AmenityFactoryLookupValues.AdditionalCost, out object costObj) || !(costObj is decimal additionalCost))
                        throw new ArgumentException("AdditionalCost is required for Premium decorators.");

                    return new PremiumAmenityDecorator(baseAmenity, premiumFeature, additionalCost);

                case DecoratorTypeEnum.Seasonal:
                    if (!properties.TryGetValue(AmenityFactoryLookupValues.Season, out object seasonObj) || !(seasonObj is string season))
                        throw new ArgumentException("Season is required for Seasonal decorators.");

                    if (!properties.TryGetValue(AmenityFactoryLookupValues.StartDate, out object startObj) || !(startObj is DateTime startDate))
                        throw new ArgumentException("StartDate is required for Seasonal decorators.");

                    if (!properties.TryGetValue(AmenityFactoryLookupValues.EndDate, out object endObj) || !(endObj is DateTime endDate))
                        throw new ArgumentException("EndDate is required for Seasonal decorators.");

                    if (!properties.TryGetValue(AmenityFactoryLookupValues.SeasonalPriceAdjustment, out object adjObj) || !(adjObj is decimal adjustment))
                        throw new ArgumentException("SeasonalPriceAdjustment is required for Seasonal decorators.");

                    return new SeasonalAmenityDecorator(baseAmenity, season, startDate, endDate, adjustment);

                default:
                    throw new ArgumentException($"Unknown decorator type: {decoratorType}");

            }
        }
    }
}

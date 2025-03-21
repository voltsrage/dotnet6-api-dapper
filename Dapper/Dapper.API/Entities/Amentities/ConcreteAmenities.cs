namespace Dapper.API.Entities.Amentities
{
    /// <summary>
    /// WIFI amenity
    /// </summary>
    public class WIFIAmenity : BaseAmenity
    {
        /// <summary>
        /// Name of the network
        /// </summary>
        public string NetworkName { get; private set; }

        /// <summary>
        /// Password for the network
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Speed of the network in Mbps
        /// </summary>
        public int SpeedMbps { get; private set; }

        /// <summary>
        /// Constructor for the WIFIAmenity class
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="priceModifier"></param>
        /// <param name="isStandard"></param>
        /// <param name="networkName"></param>
        /// <param name="password"></param>
        /// <param name="speedMbps"></param>
        public WIFIAmenity(string name, string description, decimal priceModifier, bool isStandard,
                          string networkName, string password, int speedMbps)
            : base(name, description, priceModifier, isStandard)
        {
            NetworkName = networkName;
            Password = password;
            SpeedMbps = speedMbps;
        }

        /// <summary>
        /// Updates the details of the network
        /// </summary>
        /// <param name="networkName"></param>
        /// <param name="password"></param>
        /// <param name="speedMbps"></param>
        public void UpdateNetworkDetails(string networkName, string password, int speedMbps)
        {
            NetworkName = networkName;
            Password = password;
            SpeedMbps = speedMbps;
        }
    }

    /// <summary>
    /// Mini bar amenity
    /// </summary>
    public class MiniBarAmenity : BaseAmenity
    {
        /// <summary>
        /// Is the mini bar complimentary
        /// </summary>
        public bool IsComplimentary { get; private set; }

        /// <summary>
        /// List of items in the mini bar
        /// </summary>
        public List<string> Items { get; private set; } = new List<string>();

        /// <summary>
        /// Constructor for the MiniBarAmenity class
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="priceModifier"></param>
        /// <param name="isStandard"></param>
        /// <param name="isComplimentary"></param>
        /// <param name="items"></param>
        public MiniBarAmenity(string name, string description, decimal priceModifier, bool isStandard,
                              bool isComplimentary, List<string> items)
            : base(name, description, priceModifier, isStandard)
        {
            IsComplimentary = isComplimentary;
            Items = items ?? new List<string>();
        }

        /// <summary>
        /// Updates the details of the mini bar
        /// </summary>
        /// <param name="isComplimentary"></param>
        /// <param name="items"></param>
        public void UpdateMiniBarDetails(bool isComplimentary, List<string> items)
        {
            IsComplimentary = isComplimentary;
            Items = items ?? new List<string>();
        }
    }

    /// <summary>
    /// Room service amenity
    /// </summary>
    public class RoomServiceAmenity : BaseAmenity
    {
        /// <summary>
        /// Hours available for room service
        /// </summary>
        public int HoursAvailable { get; private set; }

        /// <summary>
        /// Is the room service available 24 hours
        /// </summary>
        public bool Is24Hours { get; private set; }

        /// <summary>
        /// Constructor for the RoomServiceAmenity class
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="priceModifier"></param>
        /// <param name="isStandard"></param>
        /// <param name="hoursAvailable"></param>
        /// <param name="is24Hours"></param>
        public RoomServiceAmenity(string name, string description, decimal priceModifier, bool isStandard,
                                  int hoursAvailable, bool is24Hours)
            : base(name, description, priceModifier, isStandard)
        {
            HoursAvailable = hoursAvailable;
            Is24Hours = is24Hours;
        }

        /// <summary>
        /// Updates the availability of room service
        /// </summary>
        /// <param name="hoursAvailable"></param>
        /// <param name="is24Hours"></param>
        public void UpdateAvailability(int hoursAvailable, bool is24Hours)
        {
            HoursAvailable = hoursAvailable;
            Is24Hours = is24Hours;
        }
    }
}

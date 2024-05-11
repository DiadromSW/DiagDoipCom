namespace Logging
{
    /// <summary>
    /// Tuple representing a vin and a status whether or not it is logging
    /// </summary>
    public struct ActiveLogging
    {
        public ActiveLogging(string vin, bool status)
        {
            Vin = vin;
            Active = status;
        }

        /// <summary>
        /// Vehicle Identification number which is logging or have been logging
        /// </summary>
        public string Vin { get; set; }
        
        /// <summary>
        /// The state of logging
        /// </summary>
        public bool Active { get; set; }
    }
}

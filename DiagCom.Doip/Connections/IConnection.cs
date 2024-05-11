namespace DiagCom.Doip.Connections
{
    public interface IConnection
    {
        public string Vin { get; set; }

        void Disconnect();
        void Connect();

        IDoipNetworkLayer DoipNetworkLayer { get; }
        bool IsActive();

        Iso14229Logger Iso14229Logger { get; }
    }
}

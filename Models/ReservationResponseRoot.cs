namespace SteamDeckStatus.Models
{
    public class ReservationResponseRoot
    {
        public bool bAlreadyReserved { get; set; }
        public int oReservationStatus { get; set; }
        public bool bAllowedToReserve { get; set; }
        public int rtReserveTime { get; set; }
        public int nReservationPackage { get; set; }
        public int unReserveAppID { get; set; }
        public int success { get; set; }
        public string? strReservationMessage { get; set; }
    }
}

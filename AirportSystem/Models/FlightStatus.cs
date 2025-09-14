namespace AirportSystem.Models
{
    /// <summary>
    /// Нислэгийн статусыг илэрхийлэх enum.
    /// Нислэгийн одоогийн төлөвийг тодорхойлно.
    /// </summary>
    public enum FlightStatus
    {
        /// <summary>
        /// Check-in хийх үе шат.
        /// Зорчигчид check-in хийх боломжтой.
        /// </summary>
        CheckingIn,

        /// <summary>
        /// Нислэгт суух үе шат.
        /// Зорчигчид нислэгт суух боломжтой.
        /// </summary>
        Boarding,

        /// <summary>
        /// Нислэг хөөрсөн.
        /// Нислэг амжилттай хөөрсөн.
        /// </summary>
        Departed,

        /// <summary>
        /// Нислэг хойшлогдсон.
        /// Нислэгийн цаг хойшлогдсон.
        /// </summary>
        Delayed,

        /// <summary>
        /// Нислэг цуцлагдсан.
        /// Нислэг цуцлагдсан.
        /// </summary>
        Cancelled
    }
}

namespace MarsRoverAPI.Models
{
    public class PhotoInformation
    {
        public int Id { get; set; }

        public int Sol { get; set; }

        public CameraInformation Camera { get; set; }

        public string EarthDate { get; set; }

        public RoverInformation Rover { get; set; }
    }
}

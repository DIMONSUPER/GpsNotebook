using SQLite;
using Xamarin.Essentials;
using Xamarin.Forms.GoogleMaps;

namespace GpsNotebook.Models
{
    public class PinModel : IEntityBase
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int UserId { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public bool IsFavourite { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [Ignore]
        public Position Position
        {
            get { return new Position(Latitude, Longitude); }
            set
            {
                Latitude = value.Latitude;
                Longitude = value.Longitude;
            }
        }
    }
}

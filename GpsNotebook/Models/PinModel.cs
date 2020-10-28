using SQLite;
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
        public string Latitude { get; set; }
        public string Longitude { get; set; }

        [Ignore]
        public Position Position
        {
            get { return new Position(double.Parse(Latitude),double.Parse(Longitude)); }
            set
            {
                Latitude = value.Latitude.ToString();
                Longitude = value.Longitude.ToString();
            }
        }
    }
}

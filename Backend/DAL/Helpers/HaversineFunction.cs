using System.Data.SQLite;
using System.Text;

namespace DAL.Helpers;

public class HaversineFunction : SQLiteFunction
{
    public override object Invoke(object[] args)
    {
        double lat1 = Convert.ToDouble(args[0]);
        double lon1 = Convert.ToDouble(args[1]);
        double lat2 = Convert.ToDouble(args[2]);
        double lon2 = Convert.ToDouble(args[3]);

        double EarthRadiusInKilometers = 6371.0;

        double lat1Rad = DegreesToRadians(lat1);
        double lon1Rad = DegreesToRadians(lon1);
        double lat2Rad = DegreesToRadians(lat2);
        double lon2Rad = DegreesToRadians(lon2);

        double diffLat = lat2Rad - lat1Rad;
        double diffLon = lon2Rad - lon1Rad;

        double a = Math.Sin(diffLat / 2) * Math.Sin(diffLat / 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                   Math.Sin(diffLon / 2) * Math.Sin(diffLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusInKilometers * c;
    }

    private double DegreesToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }
}
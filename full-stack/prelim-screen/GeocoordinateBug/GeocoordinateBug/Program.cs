// See https://aka.ms/new-console-template for more information
Console.Write("Lat:");
string Latitude = Console.ReadLine();
Console.Write("Long:");
string Longitude = Console.ReadLine();
Console.Write ($"You entered {GeocoordinateBug.Helper.LatLong(Latitude)}, {GeocoordinateBug.Helper.LatLong(Longitude)}");


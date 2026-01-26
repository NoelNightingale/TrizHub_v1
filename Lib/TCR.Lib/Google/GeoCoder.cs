#region Usings

using System;

#endregion

namespace TCR.Lib.Google
{
    public class GeoCoder
    {
        public static void GoogleGeoCode(string address, out string countryCode, out string province, out string city,
            out double? lat, out double? lng)
        {
            countryCode = "";
            province = "";
            city = "";

            lat = null;
            lng = null;

            var url = "http://maps.googleapis.com/maps/api/geocode/json?sensor=true&address=";

            dynamic googleResults = new Uri(url + address).GetDynamicJsonObject();


            foreach (var place in googleResults.results)
            {
                if (place != null && place.geometry != null && place.geometry.location != null)
                {
                    lat = place.geometry.location.lat;
                    lng = place.geometry.location.lng;
                }

                foreach (var addrComponent in place.address_components)
                {
                    foreach (var addrType in addrComponent.types)
                    {
                        switch (addrType as string)
                        {
                            case "administrative_area_level_1":
                                if (!string.IsNullOrWhiteSpace(addrComponent.long_name as string) &&
                                    string.IsNullOrWhiteSpace(province))
                                    province = addrComponent.long_name;
                                break;
                            case "country":
                                if (!string.IsNullOrWhiteSpace(addrComponent.short_name as string) &&
                                    string.IsNullOrWhiteSpace(countryCode))
                                    countryCode = addrComponent.short_name;
                                break;
                            case "locality":
                                if (!string.IsNullOrWhiteSpace(addrComponent.long_name as string) &&
                                    string.IsNullOrWhiteSpace(city))
                                    city = addrComponent.long_name;
                                break;

                            default:
                                break;
                        }

                        if (!string.IsNullOrWhiteSpace(province) && !string.IsNullOrWhiteSpace(countryCode) &&
                            !string.IsNullOrWhiteSpace(city))
                            break;
                    }
                }

                if (!string.IsNullOrWhiteSpace(province) && !string.IsNullOrWhiteSpace(countryCode) &&
                    !string.IsNullOrWhiteSpace(city))
                    break;
            }
        }
    }
}
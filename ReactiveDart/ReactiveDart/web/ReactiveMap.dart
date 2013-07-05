part of RxDart;


/*
 * Class managing the openlayers map
 */
class OLMap {
 
  //Conscructor
  //Managing the subscriptions
  OLMap() {
   // var context = js.context;
   // js.context.InitializeOL();
    
    geoStream.listen((ip) {
      this.AddToMap(ip);
    });
  }
  
  //Add a point to the map
  void AddToMap(GeoIp ip) {
    js.context.addToMap(ip.long, ip.lat);
  }
}



/*
 * Data class with a constructor
 */
class GeoIp {
  int geoIpId;
  String ip;
  double long;
  double lat;
  String description;
  
  GeoIp(int ipid, String ip, double lon, double lat, String desc) {
    geoIpId = ipid;
    this.ip = ip;
    long = lon;
    this.lat = lat;
    description = desc;
  }
}
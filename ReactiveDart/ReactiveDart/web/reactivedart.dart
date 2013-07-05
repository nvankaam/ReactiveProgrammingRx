library RxDart;
import 'package:js/js.dart' as js;
import 'dart:html';
import 'dart:async';

part 'ReactiveMap.dart';
part 'ReactiveConsole.dart';


Stream<ConsoleMessage> consoleStream;
Stream<GeoIp> geoStream;

RxConsole console;

void main() {
  
  //Initialize the stream for consolemessages
  StreamController<ConsoleMessage> controller = new StreamController<ConsoleMessage>();
  int counter = 0;
  void newMessage(String c, String m) {
    controller.add(new ConsoleMessage(c, m));
  }
  var callback = new js.Callback.many(newMessage);
  js.context.consoleMessage = callback;
  consoleStream = controller.stream;
  console = new RxConsole();
  
  
  //Initialize the stream for geoIps
  //TODO: Figure out a way to construct the geoip without tons of casting
  //Very hard to debug casts tough, as the debugger is not capable of finding the source of this inner function
  StreamController<GeoIp> geoController = new StreamController<GeoIp>();
  void addNewIp (var ipid, var ip, var lon, var lat, var desc)  {
    var geo = new GeoIp(int.parse(ipid.toString()), ip.toString(), double.parse(lon.toString()), double.parse(lat.toString()), desc.toString());
    geoController.add(geo);
  }
  
  var geoCallback = new js.Callback.many(addNewIp);
  js.context.newGeoIp = geoCallback;
  geoStream = geoController.stream;
  var mapController = new OLMap();
  
}



void newMessage() {
  
}


void reverseText(MouseEvent event) {
  var text = query("#sample_text_id").text;
  var buffer = new StringBuffer();
  for (int i = text.length - 1; i >= 0; i--) {
    buffer.write(text[i]);
  }
  query("#sample_text_id").text = buffer.toString();
}

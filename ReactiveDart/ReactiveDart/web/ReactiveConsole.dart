part of RxDart;


//Class managing the console on the screen
class RxConsole {
  var id = "#console";
  var filterId = "#consoleFilter";
  var searchId = "#consoleSearch";
  var nr = 1;
  bool shouldRender = false;
  
  var consoleFilter = "";
  var searchFilter = "";
  
  List<ConsoleMessage> messages;
  
  //Handler for new consolemessages from signalR
  void newMessage(ConsoleMessage message) {
    messages.insert(0, message);
    if(messages.length > 5000)
      messages.removeLast();
    registerRender();
  }

  
  //Call this method to force a load
  //TODO: Do this on a nice reactive way, without the timer
  void registerRender() {
    shouldRender = true;
  }
  
  //Renders the console
  void render() {
    StringBuffer buffer = new StringBuffer();
    int nr = 1;
    messages.where(
      (ConsoleMessage message) => message.category.toLowerCase().contains(consoleFilter.toLowerCase()) && message.message.toLowerCase().contains(searchFilter.toLowerCase())
    ).take(20).forEach(
      (ConsoleMessage message) => buffer.write(""+(nr++).toString()+" ("+message.category+"): "+message.message+"<br />")   
    );
    query(id).innerHtml = buffer.toString();
  }
  
  //Constructor of the console, initializes all hooks on the datastream
  RxConsole() {
    messages = new List<ConsoleMessage>();
    consoleStream.listen(
      (data) => console.newMessage(data)
    );
    
    
    //To prevent lagg i had to set a refreshrate
    //TODO: This can also be solved in a nice reactive way with for example buffer
    var duration = const Duration(milliseconds: 100);
    new Timer.periodic(duration, (timer) {
      if(shouldRender) {
        shouldRender = false;
        render();
      }
    
    });
    
    //OnKeyUp for the filter
    //Nice that everything is a Stream
    query(searchId).onKeyUp.map(
        (Event event) => (query(searchId) as InputElement).value
     )
    .listen(
      (String newValue) {
          this.searchFilter = newValue;
          this.registerRender();
      }
    );
    
    //OnKeyUp for the filter
    query(filterId).onKeyUp.map(
        (Event event) => (query(filterId) as InputElement).value
     )
    .listen(
      (String newValue) {
          this.consoleFilter = newValue;
          this.registerRender();
      }
    );
   }
}

//Little dataclass with a constructor
class ConsoleMessage {
  String message;
  String category;
  
  ConsoleMessage(String c, String m) {
    message = m;
    category = c;
  }
}

# Arbeiten mit Zehnerpotenzen
## Situation
Der Nummernkreis beginnt bei 1000000
Der Benutzer bestimmt wann neue Nummern angelegt werden.
Im ini file ist 999 hinterlegt, dass heißt, das jeder neue Zyklus neue Nummern bis *999 anlegt.

### Stelle sicher das die höchste Potenz im Zyklus kleiner als die Höchste Potenz im Nummernkreis ist
```csharp
uint = 1000000;
string highestPower = uint
```

# How to: Make Thread-Safe Calls to Windows Forms Controls

https://docs.microsoft.com/de-de/dotnet/framework/winforms/controls/how-to-make-thread-safe-calls-to-windows-forms-controls#making-thread-safe-calls-to-windows-forms-controls

# make foreach asynch and wait for everything to finish

http://gigi.nullneuron.net/gigilabs/avoid-await-in-foreach/

Task.WaitAll Method 

````
public async Task RunAsync()
{
    var tasks = new List<Task>();
 
    foreach (var x in new[] { 1, 2, 3 })
    {
        var task = DoSomethingAsync(x);
        tasks.Add(task);
    }
 
    await Task.WhenAll();
}
````

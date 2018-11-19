# Arbeiten mit Zehnerpotenzen
## Situation
Der Nummernkreis beginnt bei 1000000
Der Benutzer bestimmt wann neue Nummern angelegt werden.
Im ini file ist 999 hinterlegt, dass heißt, das jeder neue Zyklus neue Nummern bis *999 anlegt.

### Stelle sicher das die höchste Potenz im Zyklus kleiner als die Höchste Potenz im Nummernkreis ist
```csharp
uint = 1000000;
string highestPower = uint
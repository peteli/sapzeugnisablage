# working with PDFsharp

## Q: Is there a way to embed custom metadata strings into a PDF created with PdfSharp?
For example, I want to embed

CustomProperty1="String1"
CustomProperty2="String2"

I would then need to read those properties at a later time.

Thanks in advance
Hi guys,

This is my first post in here.
I want to attach my question to this topic.

I know how to create new Custom Properties fields in PDF (below):
````
PdfDocument document = PdfReader.Open(OutputFile);
document.Info.Elements.Add(new KeyValuePair<String, PdfItem>("/CustProp01", new PdfString("TEST01")));
document.Info.Elements.Add(new KeyValuePair<String, PdfItem>("/CustProp02", new PdfString("TEST02")));
document.Info.Elements.Add(new KeyValuePair<String, PdfItem>("/CustProp03", new PdfString("TEST03")));
document.Info.Elements.Add(new KeyValuePair<String, PdfItem>("/CustProp04", new PdfString("")));
document.Info.Elements.Add(new KeyValuePair<String, PdfItem>("/CustProp05", new PdfString("")));
document.Save(OutputFile);
````
Please, note that while I am adding new Custom Properties, I can add also their values - and they are visible later after opening a PDF file.

But I don't know how to update existing Custom Properties??
No matter what I am doing - they don't want to go in.
I tried to use below code (SetValue) - but it doesn't work:
```
PdfDocument document = PdfReader.Open(*newOutputFile*);
document.Info.Elements.SetValue("/CustProp01", new PdfString("TEST01")));
document.Info.Elements.SetValue("/CustProp02", new PdfString("TEST02")));
document.Info.Elements.SetValue("/CustProp03", new PdfString("TEST03")));
document.Info.Elements.SetValue("/CustProp04", new PdfString(myvariable01)));
document.Info.Elements.SetValue("/CustProp05", new PdfString(myvariable02)));
document.Save(*OutputFile*);
```
No matter if I use user text or calculated variable - it doesn't update the file.
Am I using wrong options (command) to update existing Custom Properties?

Guys, please help as I have stuck with this for last two days and I can't find any info how to update existing custom properties using PDFSharp.

Best Regards,
Bartek

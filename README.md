OFXSharp
===

C# to parse OFX documents\statements with the following enhancements:
* Support for the Investment message set

### How to use

```
var parser = new OFXDocumentParser();
var ofxDocument = parser.Import(new FileStream(@"c:\ofxdoc.ofx", FileMode.Open));
```

OFXSharp
===

C# to parse OFX documents\statements with the following enhancements:
* Support for the Investment message set

[Forked from jhollingworth/OFXSharp](https://github.com/jhollingworth/OFXSharp).

### How to use

```
var parser = new OFXDocumentParser();
var ofxDocument = parser.Import(new FileStream(@"c:\ofxdoc.ofx", FileMode.Open));
```

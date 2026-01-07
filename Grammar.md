Pattern       ::= Round+

Round         ::= Statement+

Statement     ::= ColorDirective | Sequence | FastenOff

ColorDirective ::= "@" Identifier

Sequence      ::= Element ("," Element)*

Element       ::= Stitch | Group

Group         ::= "(" Sequence ")" "x"? Number

Stitch        ::= Number? StitchType

StitchType    ::= "sc" | "hdc" | "dc" | "trc" | "inc" | "dec" | "slst"

FastenOff     ::= "FO"

Number        ::= integer >= 1

Identifier    ::= letter (letter | digit)*

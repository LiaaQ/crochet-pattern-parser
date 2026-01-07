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

## Examples

**Single color round**
- 3 sc, inc, 4 sc

**Multi color round**
- @black 3 sc, @white 2 sc, 3 inc

**Repetition groups**
- 3 sc, (1 sc, 1 inc) 6
- (2 sc, 1 inc) x 6

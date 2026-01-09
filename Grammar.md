Pattern ::= Round+

Round ::= Element ("," Element)* ;?

Element ::= Stitch | Group | Color | FastenOff

Group ::= "(" Stitch ("," Stitch)* ")" Number

Stitch ::= Number? StitchType

StitchType ::= "sc" | "hdc" | "dc" | "trc" | "inc" | "dec" | "slst"

Color ::= "@" letter (letter | digit)*

FastenOff ::= "FO"

Number ::= integer >= 1

## Examples

**Single color round**
- 3 sc, inc, 4 sc

**Multi color round**
- @black 3 sc, @white 2 sc, 3 inc

**Repetition groups**
- 3 sc, (1 sc, 1 inc) 6
- (2 sc, 1 inc) x 6

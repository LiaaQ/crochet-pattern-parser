# MVP – Crochet Pattern Language

This document defines the **strict minimum viable product** for this project.
Anything not listed here is explicitly **out of scope** for the MVP.

---

## Core Goal

Design and implement a **domain-specific language (DSL)** for crochet patterns that can be:
- parsed,
- semantically interpreted,
- and validated based on stitch counts and structure.

The MVP focuses on formal correctness and validation, not physical crochet behavior or social features.

---

## Supported Language Features

### Stitches
The language supports the following stitch keywords in the US terminology:

- sc   (single crochet)
- hdc  (half double crochet)
- dc   (double crochet)
- trc  (triple crochet)
- inc  (increase)
- dec  (decrease)
- slst (slip stitch)

Stitch count effects:
- sc, hdc, dc, trc → +1 stitch
- inc → +2 stitches
- dec → +1 stitch
- slst → +0 stitches

---

### Groups and Repetition
- Parenthesized stitch sequences may be repeated using `xN`
- Groups expand into flat stitch sequences during semantic processing

Example: (2 sc, inc) x 3

---

### Color Directives
- Colors are set using `@colorName`
- A color directive applies to all following stitches until overridden
- Color changes are allowed anywhere, including mid-round
- Colors do not affect stitch count or validation

Example: @white 3 sc @black 3 sc


---

### Fasten Off (`FO`)
- `FO` represents fastening off the current working yarn
- It produces no stitches and does not affect validation
- It may appear multiple times within a pattern
- `FO` does not end the pattern or define structure

`FO` exists for semantic clarity only.

---

## Rounds
- A pattern consists of one or more rounds
- Each round consists of one or more statements
- Validation is performed **per round**
- No implicit joining or round-closing logic exists in the MVP

---

## Validation Rules

The system validates:
- syntactic correctness
- group structure
- valid stitch keywords
- repetition counts
- expected vs actual stitch count per round

The system does **not** validate:
- stitch placement
- joins
- yarn usage
- tension
- physical geometry

---

## Output
After parsing and semantic processing, the system produces:
- a flat, structured representation of stitches and their counts
- optional metadata (color, fasten-off markers)

This output is used for validation, debugging, and persistence, not visualization.

---

## Application Scope

### Included
- Parser and semantic interpreter
- Stitch count validation
- Minimal web interface:
  - textarea for pattern input
  - validation result display
- User authentication (managed provider)
- Per-user saved patterns
- Create and load patterns owned by the authenticated user


### Explicitly Excluded
- Pattern sharing between users
- Public patterns
- Roles or permissions
- Password recovery or email verification
- Yarn or size calculations
- Charts or diagrams
- Mobile UI
- Pattern assembly or parts modeling

---

## Post-MVP Work (Not Implemented)
These ideas are intentionally deferred:
- row vs. round distinction
- more complex patterns (eg. body parts crocheted in during rounds)
- adding pictures

---

## MVP Completion Criteria

The MVP is considered complete when:
- valid patterns parse and validate correctly
- invalid patterns produce clear errors
- stitch counts are accurate per round
- documentation matches implemented behavior
- authenticated users can save and reload their own patterns
- users cannot access patterns belonging to other users



# UMOnline

Core Engine + Server + Client + Card Parser for playing games of Unmatched.

## Projects

### UMCore

Core library of Unmatched. 

### UMServer

ASP.NET server for connecting and playing games of Unmatched.

### UMClient

A game client implemented in Godot 4 for connecting to the server (or hosting offline games).

### UMCardParser

A parsing library that turns text into .lua scripts.

## Core

### Cards

#### Card Template

A .lua file which describes the behaviour of a card. Contains a function called `_Create`, which creates the card.
Returns info with: card type (Attack, Defence, Versatile, Scheme), card power (if any), card name (?), card text and boost.


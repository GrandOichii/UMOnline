# UMOnline - Unmatched online

UMOnline allows you to play matches of Unmatched against one or more online players or bots. This project includes a full rules engine, a parser editor application, a Godot-based client application and an ASP.NET server application.

## Projects

### UMCore

A C# class library which descrives the rules of the game.

### UMServer

An ASP.NET application for hosting online matches.

To launch, run:
```bash
dotnet run
```

### UMClient

A client application written in Godot 4 and C#. To run, open the project in the Godot editor and click _Run project_.

### um_parser

An editor for the game text parser, written in Godot 4 and Rust. To run, first build the project using `cargo build`, then open the project in the Godot editor and click _Run project_.

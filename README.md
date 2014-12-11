# Conflight

Conflight is a simple JSON variant, with a deserializer for C# that will populate arbitrary objects with the data from a file.

I'm writing this thing for my own purposes and adding features as I come across them. This is not meant to be a new standard, or something that even sees wide use. It's a personal project for fun and personal use. If you use it, please don't call me when your house catches on fire or when your spouse leaves you because of it. I'm willing to accept pull requests and feature requests, though!

## What does it look like?

Think JSON, but don't bother putting quotes are regular old text--unless it's got characters that might mean something special in JSON.

    {Title: Introduction, Message: "Conflight lets you skip out on some annoying facts about JSON, like having to put quotes around everything."}
    
That *does* look kind of ugly. Let's make it prettier.

    {
      Title: Prettier Printing
      Message: "Ah, this is a bit easier to read."
      Do I have to remember to put commas: false
      A List: [0, 1, 4, 10]
      A Prettier List: 
      [
        Dog
        Cat
        Barbeque Sauce
        The Lunar Lander
      ] 
    }

Newlines act as a delimiter for lists of items and mappings! Commas are only needed in inline lists, but you can include commas where just newlines would do if you'd like. This won't generate empty list elements or null -> null mappings or anything weird like that.

## How do I use this?

Currently, I have only written a deserializer that takes Conflight strings and C# data types and populates instances of those data types with whatever matching elements it can find in the Conflight text.

To build an object from Conflight text:

    TypeName varName = ObjectLoader.LoadInstance<TypeName>(conflightString);
    
You can also load lists of objects of a certain type:

    List<TypeName> listName = ObjectLoader.LoadInstanceList<TypeName>(conflightString);
    
Loading objects will set properties (*not* fields) with the same name in both Conflight and your C# type to the value you provide.

The currently supported member types are

* int
* double
* bool
* Enums
* Lists
* Arrays
* Dictionaries with int, enum, or string keys.
* Arbitrary objects that have default (parameterless) constructors.


## Limitations

* I have no idea how slow this might be on bigger files.
* No serializer... yet!
* Can't have characters that are both non-word and non-space in bare text (unquoted strings).
* No support for empty lists, dictionaries, and objects. But that's ok, just don't include them and they'll be populated with empty or default values by default constructors in the objects you're building!
* If your type doesn't have a default constructor, this won't work! Maybe I'll come up with some way to work around this, or maybe you simply shouldn't use Conflight on types that need constructor parameters for other reasons. I'm not sure how opinionated Conflight should be yet.

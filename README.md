# Patch Types
#### What is it
Allows to create a proxy type of a given DTO that implements IPatch interface and allows to check for dirty (or set) properies. It also provides a JsonConverter that handles IPatch types.
It's a part of Detached.Mappers library.

## What does it solve
Unlike JavaScript, C# does not provide the 'undefined' value for properties that were not set during the deserialization of a 
request or file.
This library creates proxy types that internally keep tracking of the dirty state of each property.

### How it works
A manual call to PatchTypeFactory.Create() can be made to create/retrieve a proxy type for a given type.

```csharp
// create a patch type for Entity, implementing IPatch
Entity entity = PatchTypeFactory.Create<Entity>();
// set any property value 
entity.Name = "newName";
// access to IPatch members
IPatch entityChanges = (IPatch)entity;
// check for the property status
Assert.True(entityChanges.IsSet("Name"));
```

System.Text.Json serializer can be configured to deserialize DTOs to patch types automatically.



´´´csharp
JsonSerializerOptions jsonOptions = new JsonSerializerOptions();
// configure the serializer
jsonOptions.Converters.Add(new PatchJsonConverterFactory());
jsonOptions.IgnoreReadOnlyProperties = true;

string json = @"
    {
        'Id': 1,
        'Name': 'test name'
    }".Replace("'", "\"");
            
// deserialize some entity
Entity entity = JsonSerializer.Deserialize<Entity>(json, jsonOptions);

// check for undefined properties
IPatch changeTracking = (IPatch)entity;

Assert.True(changeTracking.IsSet("Name"));
Assert.True(changeTracking.IsSet("Id"));
Assert.False(changeTracking.IsSet("Date"));
´´´

More info and examples will be added later.
Check unit tests for more samples!

### Any help on debugging, or adding new features is very welcome!
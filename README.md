![Detached Banner](banner.png?raw=true)

# Patch Types
### What is it
Allows to create a proxy type for a given DTO that implements IPatch interface and allows to check for dirty (or set) properies. It also provides a JsonConverter that handles IPatch types.
It's a part of [Detached.Mappers](https://github.com/leonardoporro/Detached-Mapper) library.

### What does it solve
Unlike JavaScript, C# does not provide the 'undefined' value to check what properties were set during the deserialization of a 
request or file.
This library creates proxy types that internally keep tracking of the dirty state of each property.
Detached.Mappers uses IPatch types to select which properties to copy when working with ORMs.

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

```csharp
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
```

#### Note: As in any other proxy tool, properties must be marked as virtual in order to override them with the proper dirty tracking code.

## Configure
PatchJsonConverterFactory takes a IPachTypeInfoProvider as a parameter. This class has the responsability of determine if the type should be 
patched or not.
Default is DefaultPatchTypeInfoProvider, that will patch everything but primitives in .Primitives property.
AnnotationPatchTypeInfoProvider checks for [UsePach] attribute before allow the patch creation.

## Integrate to MVC
In order to get Patch Types in the bodies of Post methods, in Startup.cs, add the converter to main serializer like this: 
AnnotationPatchTypeInfoProvider is the preferred method, as you may not need all types to be proxied.
You can write your own, just don't forget to check if the type you are patching is not already a patch type, otherwise a stack overflow
will occur.

```charp
services.AddControllers().AddJsonOptions(j =>
{
    j.JsonSerializerOptions.Converters.Add(new PatchJsonConverterFactory(new AnnotationPatchTypeInfoProvider()));
});
```
Then add the [UsePach] attribute to your model:
 
```csharp
 [UsePatch]
    public class SampleModel
    {
        public virtual int Id { get; set; } // don't forget to add virtual, otherwise, patch factory won't be able to override.

        public virtual string Name { get; set; }

        public virtual DateTime? DateTime { get; set; }
    }
```

Write your controller and enjoy!

```csharp
[HttpPost]
public IActionResult PostPatcheableModel([FromBody] SampleModel model)
{
    // at this point, model is a proxy that inherits SampleModel and implements IPach for other libs like Detached.Mappers
    // (or your library!) that need to check property status.

    // just some code to print the status of the properties
    IPatch patch = (IPatch)model;

    // use patch.IsSet(propName) to check the status of the properties. Or install Detached.Mappers.EntityFramework to map directly to EF Core.
}
```

*Check Sample folder for a working example!*

#### Any help on debugging, or adding new features is very welcome!

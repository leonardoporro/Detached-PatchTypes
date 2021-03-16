using Detached.PatchTypes;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace Detached.Mappers.Tests.Patching
{
    public class JsonPatchTests
    {
        [Fact]
        public void deserialize_as_patch()
        {
            JsonSerializerOptions jsonOptions = new JsonSerializerOptions();
            jsonOptions.Converters.Add(new PatchJsonConverterFactory());
            jsonOptions.IgnoreReadOnlyProperties = true;

            string json = @"
                {
                    'Id': 1,
                    'Name': 'test name'
                }".Replace("'", "\"");
            
            Entity entity = JsonSerializer.Deserialize<Entity>(json, jsonOptions);

            IPatch changeTracking = (IPatch)entity;

            Assert.True(changeTracking.IsSet("Name"));
            Assert.True(changeTracking.IsSet("Id"));
            Assert.False(changeTracking.IsSet("Date"));
        }

        [Fact]
        public void deserialize_list_as_patch()
        {
            JsonSerializerOptions jsonOptions = new JsonSerializerOptions();
            jsonOptions.Converters.Add(new PatchJsonConverterFactory());

            string json = @"
                [
                    {
                        'Id': 1,
                        'Name': 'test name'
                    },
                    {
                        'Id': 2,
                        'Name': 'test name 2'
                    }

                ]".Replace("'", "\"");

            List<Entity> entities = JsonSerializer.Deserialize<List<Entity>>(json, jsonOptions);

            Assert.True(((IPatch)entities[0]).IsSet("Name"));
            Assert.True(((IPatch)entities[0]).IsSet("Id"));
            Assert.False(((IPatch)entities[0]).IsSet("Date"));

            Assert.True(((IPatch)entities[1]).IsSet("Name"));
            Assert.True(((IPatch)entities[1]).IsSet("Id"));
            Assert.False(((IPatch)entities[1]).IsSet("Date"));
        }

        [Fact]
        public void deserialize_nested_as_patch()
        {
            JsonSerializerOptions jsonOptions = new JsonSerializerOptions();
            jsonOptions.Converters.Add(new PatchJsonConverterFactory());

            string json = @"
                [
                    {
                        'Id': 1,
                        'Name': 'test root',
                        'Reference': {
                            'Id': 2,
                            'Name': 'child reference'
                        },
                        'Collection': [
                            {
                                'Id': 3,
                                'Name': 'child item'
                            }
                        ]
                    }
                   
                ]".Replace("'", "\"");

            List<Root> root = JsonSerializer.Deserialize<List<Root>>(json, jsonOptions);

            Assert.True(((IPatch)root[0].Reference).IsSet("Name"));
            Assert.True(((IPatch)root[0].Reference).IsSet("Id"));
            Assert.False(((IPatch)root[0].Reference).IsSet("Date"));

            Assert.True(((IPatch)root[0].Collection[0]).IsSet("Name"));
            Assert.True(((IPatch)root[0].Collection[0]).IsSet("Id"));
            Assert.False(((IPatch)root[0].Collection[0]).IsSet("Date"));
        }

        public class Entity
        {
            public virtual int Id { get; set; }

            public virtual string Name { get; set; }

            public virtual DateTime Date { get; set; }
        }

        public class Root
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public Entity Reference { get; set; }

            public List<Entity> Collection { get; set; }
        }
    }
}
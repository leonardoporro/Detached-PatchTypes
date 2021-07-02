using Detached.PatchTypes.Newtonsoft;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Detached.PatchTypes.Tests
{
    public class NewtonsoftJsonPatchTests
    {
        [Fact]
        public void deserialize_as_patch()
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new PatchJsonConverter(new DefaultPatchTypeInfoProvider()));

            string json = @"
                {
                    'Id': 1,
                    'Name': 'test name'
                }".Replace("'", "\"");

            Entity entity = serializer.Deserialize<Entity>(new JsonTextReader(new StringReader(json)));

            IPatch changeTracking = (IPatch)entity;

            Assert.True(changeTracking.IsSet("Name"));
            Assert.True(changeTracking.IsSet("Id"));
            Assert.False(changeTracking.IsSet("Date"));
        }


        [Fact]
        public void deserialize_nested_as_patch()
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new PatchJsonConverter(new DefaultPatchTypeInfoProvider()));

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

            List<Root> root = serializer.Deserialize<List<Root>>(new JsonTextReader(new StringReader(json)));

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

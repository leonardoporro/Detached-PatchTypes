using Detached.PatchTypes.Annotations;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;

namespace Detached.PatchTypes.SampleRestApi.Controllers
{
    [ApiController]
    [Route("sample")]
    public class SampleController : Controller
    {
        /// <summary>
        /// Post a model here and check what properties are defined!
        /// For configuration, check Startup.cs.
        /// And don't forget to check https://github.com/leonardoporro/Detached-Mapper for mapping patcheable models 
        /// to entity framework!
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        public IActionResult PostPatcheableModel([FromBody] SampleModel model)
        {
            // at this point, model is a proxy that inherits SampleModel and implements IPach for other libs like Detached.Mappers
            // (or your library!) that need to check property status.

            // just some code to print the status of the properties
            IPatch patch = (IPatch)model;

            StringBuilder stringBuilder = new StringBuilder();
            foreach (string propName in new[] { nameof(SampleModel.Id), nameof(SampleModel.Name), nameof(SampleModel.DateTime) })
            {
                if (patch.IsSet(propName))
                    stringBuilder.AppendLine($"{propName} is set");
                else
                    stringBuilder.AppendLine($"{propName} is not set");
            }

            return Ok(stringBuilder.ToString());
        }
    }

    [UsePatch]
    public class SampleModel
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual DateTime? DateTime { get; set; }
    }
}
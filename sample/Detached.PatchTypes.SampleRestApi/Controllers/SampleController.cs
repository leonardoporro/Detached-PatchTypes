using Detached.PatchTypes.SampleRestApi.Model;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult PostPatcheableModel([FromBody]IEntity model)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var propInfo in model.GetType().GetProperties())
            {
                if (model.IsSet(propInfo.Name))
                    stringBuilder.AppendLine($"{propInfo.Name} is SET");
                else
                    stringBuilder.AppendLine($"{propInfo.Name} is NOT SET");
            }

            return Ok(stringBuilder.ToString());
        }
    }
}
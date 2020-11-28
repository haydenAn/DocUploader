using UploaderApp.API.Models;
using UploaderApp.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace UploaderApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocInfoController : ControllerBase
    {
        private readonly DocInfoService _docInfoService;

        public DocInfoController(DocInfoService docInfoService)
        {
            _docInfoService = docInfoService;
        }

        [HttpGet]
        public ActionResult<List<DocInfo>> Get() =>
            _docInfoService.Get();

        [HttpGet("{id:length(24)}", Name = "GetDocInfo")]
        public ActionResult<DocInfo> Get(string id)
        {
            var docInfo = _docInfoService.Get(id);

            if (docInfo == null)
            {
                return NotFound();
            }

            return docInfo;
        }

        [HttpPost]
        public ActionResult<DocInfo> Create(DocInfo docInfo)
        {
            _docInfoService.Create(docInfo);

            return CreatedAtRoute("GetDocInfo", new { id = docInfo.Id.ToString() }, docInfo);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, DocInfo docInfoIn)
        {
            var docInfo = _docInfoService.Get(id);

            if (docInfo == null)
            {
                return NotFound();
            }

            _docInfoService.Update(id, docInfoIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var docInfo = _docInfoService.Get(id);

            if (docInfo == null)
            {
                return NotFound();
            }

            _docInfoService.Remove(docInfo.Id);

            return NoContent();
        }
    }
}
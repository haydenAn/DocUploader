using UploaderApp.API.Models;
using UploaderApp.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using UploaderApp.API.Data;
using UploaderApp.API.Helpers;
using System.Threading.Tasks;


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

        //api/docinfo
        [HttpGet]
        [HttpGet("{filter}")]
        public async Task<IActionResult> Get([FromQuery] ReportParams rptParams) {

           var docs = await _docInfoService.Get(rptParams);

           if(docs!= null){
              Response.AddPagination(docs.CurrentPage, docs.PageSize, docs.TotalCount, docs.TotalPages);
           }
           return Ok(docs);
        }

        [HttpGet("{email:length(50)}", Name = "GetDocInfoByEmail")]
        public ActionResult<DocInfo> Get(string email)
        {
            var docInfo = _docInfoService.Get(email);

            if (docInfo == null)
            {
                return NotFound();
            }
            // s1 = $"Sent {doc.DocumentFullName} to {doc.FirstName} {doc.LastName} at {doc.EmailAddress} at company {doc.Company}";
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